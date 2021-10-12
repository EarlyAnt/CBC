using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class ResourceUtils
{
    public static ResourceUtils Instance
    {
        get
        {
            if (instance == null)
                instance = new ResourceUtils();
            return instance;
        }
    }
    public bool IgnoreFileNotExisted { get; set; }//是否忽略要下载的文件在服务器上不存在(忽略[true]则不中断下载，否则[false]中断下载)
    public string SERVER_ROOT_PATH { get { return "kayou/"; } }
    private string serverPath = "https://rex-qn.gululu-a.com/kayou";//服务器地址
    private string localPath = string.Format("file:///{0}", Application.persistentDataPath);//本地路径
    private string[] cacheFolders = new string[] { "Audio", "Texture", "Sprite", "Model" };//本地资源文件相关的路径
    private Dictionary<string, AssetBundle> assetBundleBuffer = new Dictionary<string, AssetBundle>();
    private static ResourceUtils instance;

    /// <summary>
    /// 设置服务器地址
    /// </summary>
    /// <param name="serverPath">文件服务器下载地址前缀</param>
    public void SetServerPath(string serverPath)
    {
        Debug.LogFormat("<><ResourceUtils.SetServerPath>1 - old server path: {0}", this.serverPath);
        if (!string.IsNullOrEmpty(serverPath) && serverPath.StartsWith("https://"))
        {
            this.serverPath = serverPath + SERVER_ROOT_PATH;
            if (this.serverPath.EndsWith("/"))
                this.serverPath = this.serverPath.Remove(this.serverPath.Length - 1, 1);//去掉末尾的斜线
        }
        Debug.LogFormat("<><ResourceUtils.SetServerPath>2 - new server path: {0}, cur server path: {1}", serverPath, this.serverPath);
    }
    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="fileName">配置文件中的相对路径(需要包括后缀名)</param>
    /// <returns>文件存在返回true，否则返回false</returns>
    public bool FileExisted(string fileName)
    {
        string fileFullName = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
        return File.Exists(fileFullName);
    }
    /// <summary>
    /// 从网络或缓存中获取资源文件(音频、图片、模型等)
    /// </summary>
    /// <param name="filePath">资源文件相对路径</param>
    /// <param name="success">成功时的回调</param>
    /// <param name="failure">失败时的回调</param>
    /// <param name="forcedDownload">是否强制下载(即使本地存在文件也下载并覆盖)</param>
    /// <returns></returns>
    public IEnumerator LoadAsset(string filePath, Action<object> success = null, Action<FailureInfo> failure = null, bool forcedDownload = false)
    {
        if (string.IsNullOrEmpty(filePath))
        {//音频文件路径异常，退出下载
            Debug.LogError("<><ResourceUtils.LoadAsset>filePath is null");
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = "filePath is null" });
            yield break;
        }

        if (FileTypes.IsAudio(filePath))
            yield return this.LoadAudio(filePath, (audioClip) => { if (success != null) success(audioClip); }, failure, forcedDownload);
        else if (FileTypes.IsTexture(filePath))
            yield return this.LoadTexture(filePath, (texture) => { if (success != null) success(texture); }, failure, forcedDownload);
        else if (FileTypes.IsAssetBundle(filePath))
            yield return this.LoadAssetBundle(filePath, (assetBundle) => { if (success != null) success(assetBundle); }, failure, forcedDownload);
        else if (FileTypes.IsManifest(filePath))
            yield return this.LoadManifest(filePath, (manifest) => { if (success != null) success(manifest); }, failure, forcedDownload);
    }
    /// <summary>
    /// 从网络或缓存中获取音频
    /// </summary>
    /// <param name="audioPath">音频相对路径</param>    
    /// <param name="success">成功时的回调</param>
    /// <param name="failure">失败时的回调</param>
    /// <param name="forcedDownload">是否强制下载(即使本地存在文件也下载并覆盖)</param>
    /// <returns></returns>
    public IEnumerator LoadAudio(string audioPath, Action<AudioClip> success = null, Action<FailureInfo> failure = null, bool forcedDownload = false)
    {
        if (string.IsNullOrEmpty(audioPath))
        {//音频文件路径异常，退出下载
            Debug.LogError("<><ResourceUtils.LoadAudio>audioPath is null");
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = "audioPath is null" });
            yield break;
        }

        string fileName = string.Format("{0}/{1}", Application.persistentDataPath, audioPath);
        //Debug.LogFormat("<><ResourceUtils.LoadAudio>local file name: {0}", fileName);

        if (!this.CheckFolderExist(Path.GetDirectoryName(fileName)))
        {//逐级检查检查本地是否存在指定的目录，没有则创建
            Debug.LogErrorFormat("<><ResourceUtils.LoadAudio>Folder Not Exist: {0}", Path.GetDirectoryName(fileName));
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = string.Format("Folder Not Exist: {0}", Path.GetDirectoryName(fileName)) });
            yield break;
        }

        bool fileExisted = File.Exists(fileName) && !forcedDownload;//文件不存在或指定强制下载，都视为文件不存在
        fileName = string.Format("{0}/{1}", fileExisted ? this.localPath : this.serverPath, audioPath);//根据本地是否存在指定文件，拼接文件获取路径(本地或网络)
        //Debug.LogFormat("<><ResourceUtils.LoadAudio>{0} file name: {1}", fileExisted ? "cache" : "server", fileName);

        WWW www = new WWW(fileName);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {//下载过程中遇到错误，退出下载
            Debug.LogErrorFormat("<><ResourceUtils.LoadAudio>Download error: {0}({1})", www.error, fileName);
            string errorText = www.error;
            www.Dispose();
            www = null;
            if (failure != null) failure(new FailureInfo() { Interrupt = !this.IgnoreFileNotExisted, Message = string.Format("Download error: {0}({1})", errorText, fileName) });
            yield break;
        }
        else
        {
            if (!fileExisted)
            {//如果文件原本不存在，则写文件到本地指定目录
                fileName = string.Format("{0}/{1}", Application.persistentDataPath, audioPath);
                //Debug.LogFormat("<><ResourceUtils.LoadAudio>write file name: {0}", fileName);
                File.WriteAllBytes(fileName, www.bytes);
            }

            if (success != null)
            {
                AudioClip audioClip = www.GetAudioClipCompressed();
                if (audioClip != null)
                {
                    //Debug.LogFormat("<><ResourceUtils.LoadAudio>Path: {0}, Size: {1}", audioPath, audioClip.length);
                    success(audioClip);
                }
            }
            www.Dispose();
            www = null;
        }
    }
    /// <summary>
    /// 从网络或缓存中获取图片
    /// </summary>
    /// <param name="imagePath">图片相对路径</param>    
    /// <param name="success">成功时的回调</param>
    /// <param name="failure">失败时的回调</param>
    /// <param name="forcedDownload">是否强制下载(即使本地存在文件也下载并覆盖)</param>
    /// <returns></returns>
    public IEnumerator LoadTexture(string imagePath, Action<Sprite> success = null, Action<FailureInfo> failure = null, bool forcedDownload = false)
    {
        if (string.IsNullOrEmpty(imagePath))
        {//图片文件路径异常，退出下载
            Debug.LogError("<><ResourceUtils.LoadTexture>imagePath is null");
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = "imagePath is null" });
            yield break;
        }

        string fileName = string.Format("{0}/{1}", Application.persistentDataPath, imagePath);
        //Debug.LogFormat("<><ResourceUtils.LoadTexture>local file name: {0}", fileName);

        if (!this.CheckFolderExist(Path.GetDirectoryName(fileName)))
        {//逐级检查检查本地是否存在指定的目录，没有则创建
            Debug.LogErrorFormat("<><ResourceUtils.LoadTexture>Folder Not Exist: {0}", Path.GetDirectoryName(fileName));
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = string.Format("Folder Not Exist: {0}", Path.GetDirectoryName(fileName)) });
            yield break;
        }

        bool fileExisted = File.Exists(fileName) && !forcedDownload;//文件不存在或指定强制下载，都视为文件不存在
        fileName = string.Format("{0}/{1}", fileExisted ? this.localPath : this.serverPath, imagePath);//根据本地是否存在指定文件，拼接文件获取路径(本地或网络)
        //Debug.LogFormat("<><ResourceUtils.LoadTexture>{0} file name: {1}", fileExisted ? "cache" : "server", fileName);

        WWW www = new WWW(fileName);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {//下载过程中遇到错误，退出下载
            Debug.LogErrorFormat("<><ResourceUtils.LoadTexture>Download error: {0}({1})", www.error, fileName);
            string errorText = www.error;
            www.Dispose();
            www = null;
            if (failure != null) failure(new FailureInfo() { Interrupt = !this.IgnoreFileNotExisted, Message = string.Format("Download error: {0}({1})", errorText, fileName) });
            yield break;
        }
        else
        {
            if (!fileExisted)
            {//如果文件原本不存在，则写文件到本地指定目录
                fileName = string.Format("{0}/{1}", Application.persistentDataPath, imagePath);
                Debug.LogFormat("<><ResourceUtils.LoadTexture>write file name: {0}", fileName);
                File.WriteAllBytes(fileName, www.bytes);
            }

            Texture2D texture = www.texture;
            if (texture != null && success != null)
            {
                string spriteName = Path.GetFileName(imagePath);
                if (!string.IsNullOrEmpty(spriteName))
                    spriteName = spriteName.Replace(FileTypes.TEXTURE, "");
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                sprite.name = spriteName;
                //Debug.LogFormat("<><ResourceUtils.LoadTexture>Path: {0}, Rect: {1}", imagePath, sprite.rect);
                success(sprite);
            }
            www.Dispose();
            www = null;
        }
    }
    /// <summary>
    /// 从网络或缓存中获取AssetBundle包
    /// </summary>
    /// <param name="assetBundlePath">AssetBundle包相对路径</param>    
    /// <param name="success">成功时的回调</param>
    /// <param name="failure">失败时的回调</param>
    /// <param name="forcedDownload">是否强制下载(即使本地存在文件也下载并覆盖)</param>
    /// <returns></returns>
    public IEnumerator LoadAssetBundle(string assetBundlePath, Action<AssetBundle> success = null, Action<FailureInfo> failure = null, bool forcedDownload = false)
    {
        if (string.IsNullOrEmpty(assetBundlePath))
        {//AB包文件路径异常，退出下载
            Debug.LogError("<><ResourceUtils.LoadAssetBundle>assetBundlePath is null");
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = "assetBundlePath is null" });
            yield break;
        }

        if (this.CheckAssetBundleBuffer(assetBundlePath) && success != null)
        {
            success(this.assetBundleBuffer[assetBundlePath]);
            Debug.LogFormat("<><ResourceUtils.LoadAssetBundle>AssetBundle '{0}' existed", assetBundlePath);
            yield break;
        }

        string fileName = string.Format("{0}/{1}", Application.persistentDataPath, assetBundlePath);
        Debug.LogFormat("<><ResourceUtils.LoadAssetBundle>local file name: {0}", fileName);

        if (!this.CheckFolderExist(Path.GetDirectoryName(fileName)))
        {//逐级检查检查本地是否存在指定的目录，没有则创建
            Debug.LogErrorFormat("<><ResourceUtils.LoadAssetBundle>Folder Not Exist: {0}", Path.GetDirectoryName(fileName));
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = string.Format("Folder Not Exist: {0}", Path.GetDirectoryName(fileName)) });
            yield break;
        }

        bool fileExisted = File.Exists(fileName) && !forcedDownload;//文件不存在或指定强制下载，都视为文件不存在
        if (!fileExisted)
        {//如果AssetBundle包文件不存在则从服务器上下载，否则什么都不做
            fileName = string.Format("{0}/{1}", this.serverPath, assetBundlePath);//拼接文件网络获取路径
            Debug.LogFormat("<><ResourceUtils.LoadAssetBundle>{0} file name: {1}", fileExisted ? "cache" : "server", fileName);

            WWW www = new WWW(fileName);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {//下载过程中遇到错误，退出下载
                Debug.LogErrorFormat("<><ResourceUtils.LoadAssetBundle>Download error: {0}({1})", www.error, fileName);
                string errorText = www.error;
                www.Dispose();
                www = null;
                if (failure != null) failure(new FailureInfo() { Interrupt = !this.IgnoreFileNotExisted, Message = string.Format("Download error: {0}({1})", errorText, fileName) });
                yield break;
            }
            else
            {
                if (!fileExisted)
                {//如果文件原本不存在，则写文件到本地指定目录
                    fileName = string.Format("{0}/{1}", Application.persistentDataPath, assetBundlePath);
                    Debug.LogFormat("<><ResourceUtils.LoadAssetBundle>write file name: {0}", fileName);
                    File.WriteAllBytes(fileName, www.bytes);
                }

                if (www.assetBundle != null && success != null)
                {
                    if (!this.CheckAssetBundleBuffer(assetBundlePath)) this.assetBundleBuffer.Add(assetBundlePath, www.assetBundle);
                    Debug.LogFormat("<><ResourceUtils.LoadAssetBundle>Path: {0}, AssetBundle: {1}", assetBundlePath, www.assetBundle.name);
                    success(www.assetBundle);
                }
                else if (www.assetBundle == null && failure != null)
                {
                    failure(new FailureInfo() { Message = "<><ResourceUtils.LoadAssetBundle>Error: www.assetBundle is null", Interrupt = false });
                }
                www.Dispose();
                www = null;
            }
        }
    }
    /// <summary>
    /// 从网络或缓存中获取Manifest文件
    /// </summary>
    /// <param name="assetBundlePath">AssetBundle包相对路径</param>    
    /// <param name="success">成功时的回调</param>
    /// <param name="failure">失败时的回调</param>
    /// <param name="forcedDownload">是否强制下载(即使本地存在文件也下载并覆盖)</param>
    /// <returns></returns>
    public IEnumerator LoadManifest(string assetBundlePath, Action<AssetBundleManifest> success = null, Action<FailureInfo> failure = null, bool forcedDownload = false)
    {
        if (string.IsNullOrEmpty(assetBundlePath))
        {//AB包文件路径异常，退出下载
            Debug.LogError("<><ResourceUtils.LoadManifest>assetBundlePath is null");
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = "assetBundlePath is null" });
            yield break;
        }

        string fileName = string.Format("{0}/{1}", Application.persistentDataPath, assetBundlePath);
        Debug.LogFormat("<><ResourceUtils.LoadManifest>local file name: {0}", fileName);

        if (!this.CheckFolderExist(Path.GetDirectoryName(fileName)))
        {//逐级检查检查本地是否存在指定的目录，没有则创建
            Debug.LogErrorFormat("<><ResourceUtils.LoadManifest>Folder Not Exist: {0}", Path.GetDirectoryName(fileName));
            if (failure != null) failure(new FailureInfo() { Interrupt = false, Message = string.Format("Folder Not Exist: {0}", Path.GetDirectoryName(fileName)) });
            yield break;
        }

        bool fileExisted = File.Exists(fileName) && !forcedDownload;//文件不存在或指定强制下载，都视为文件不存在
        if (!fileExisted)
        {//如果manifest文件不存在则下载，否则啥也不做，继续AB包的下载
            fileName = string.Format("{0}/{1}", this.serverPath, assetBundlePath);//拼接文件网络获取路径
            Debug.LogFormat("<><ResourceUtils.LoadManifest>{0} file name: {1}", fileExisted ? "cache" : "server", fileName);

            WWW www = new WWW(fileName);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {//如果文件原本不存在，则写文件到本地指定目录
                fileName = string.Format("{0}/{1}", Application.persistentDataPath, assetBundlePath);
                Debug.LogFormat("<><ResourceUtils.LoadManifest>write file name: {0}", fileName);
                File.WriteAllBytes(fileName, www.bytes);
            }
            www.Dispose();
            www = null;
        }
    }
    /// <summary>
    /// 删除本地文件
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    /// <param name="failure">删除成功的回调</param>
    /// <param name="success">删除失败的回调</param>
    public void DeleteFile(string filePath, Action<string> success = null, Action<string> failure = null)
    {
        try
        {
            string fileFullName = string.Format("{0}/{1}", Application.persistentDataPath, filePath);
            Debug.LogFormat("<><ResourceUtils.DeleteFile>File full name: {0}", fileFullName);
            if (File.Exists(fileFullName))
            {//如果文件存在，则删除文件
                File.Delete(fileFullName);
                Debug.Log("<><ResourceUtils.DeleteFile>File deleted");
            }
            if (success != null) success("File deleted");
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><ResourceUtils.DeleteFile>Delete error: {0}", ex.Message);
            if (failure != null) failure(string.Format("Delete error: {0}", ex.Message));
        }
    }
    /// <summary>
    /// 删除本地目录
    /// </summary>
    /// <param name="folderPath">目录相对路径</param>
    /// <param name="failure">删除成功的回调</param>
    /// <param name="success">删除失败的回调</param>
    public void DeleteFolder(string folderPath, Action<string> success = null, Action<string> failure = null)
    {
        try
        {
            string folderFullPath = string.Format("{0}/{1}", Application.persistentDataPath, folderPath);
            Debug.LogFormat("<><ResourceUtils.DeleteFolder>Folder full name: {0}", folderFullPath);

            if (string.IsNullOrEmpty(folderPath))
            {//如果目录异常，则退出删除操作
                if (failure != null)
                    failure("Delete error: param[folderPath] is null or empty, it is invalid");

                return;
            }

            if (folderPath.ToUpper() == "ALL")
            {//删除本地左右存放资源文件的目录
                foreach (string folder in this.cacheFolders)
                {
                    folderFullPath = string.Format("{0}/{1}", Application.persistentDataPath, folder);
                    if (Directory.Exists(folderFullPath)) Directory.Delete(folderFullPath, true);
                }
                Debug.Log("<><ResourceUtils.DeleteFolder>All cache folder deleted");
                if (success != null) success("All cache folder deleted");
            }
            else
            {//删除指定的路径
                if (Directory.Exists(folderFullPath))
                {
                    Directory.Delete(folderFullPath, true);
                    Debug.Log("<><ResourceUtils.DeleteFolder>Folder deleted");
                }
                if (success != null) success("Folder deleted");
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><ResourceUtils.DeleteFolder>Delete error: {0}", ex.Message);
            if (failure != null) failure(string.Format("Delete error: {0}", ex.Message));
        }
    }
    /// <summary>
    /// 删除指定模块的资源文件(图片和音频)
    /// </summary>
    /// <param name="moduleName">模块名称</param>
    /// <param name="success">删除成功的回调</param>
    /// <param name="failure">删除失败的回调</param>
    public void DeleteModule(string moduleName, Action<string> success = null, Action<string> failure = null)
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            if (failure != null)
            {
                if (failure != null)
                    failure("Delete error: param[moduleName] is null or empty, it is invalid");
            }
            return;
        }

        try
        {
            Debug.LogFormat("<><ResourceUtils.DeleteModule>ModuleName: {0}", moduleName);
            if (moduleName.ToUpper() == "NEWS")
                this.DeleteOldFiles();

            this.DeleteFolder(string.Format("Audio/{0}", moduleName), (audioSuccess) =>
            {
                this.DeleteFolder(string.Format("Texture/{0}", moduleName), (textureSuccess) =>
                {
                    if (success != null)
                        success("Module deleted");
                }, failure);
            }, failure);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><ResourceUtils.DeleteModule>Delete error: {0}", ex.Message);
            if (failure != null) failure(string.Format("Delete error: {0}", ex.Message));
        }
    }
    /// <summary>
    /// 从缓存中移除指定的AssetBundle
    /// </summary>
    /// <param name="assetBundlePath">指定的AssetBundle的名字或路径</param>
    public void UnloadAssetBundle(string assetBundlePath)
    {
        if (this.assetBundleBuffer != null && this.assetBundleBuffer.ContainsKey(assetBundlePath))
        {
            if (this.assetBundleBuffer[assetBundlePath] != null)
                this.assetBundleBuffer[assetBundlePath].Unload(true);
            this.assetBundleBuffer.Remove(assetBundlePath);
        }
    }
    /// <summary>
    /// 检查文件夹是否存在并逐级创建文件夹
    /// </summary>
    /// <param name="folder">文件夹</param>
    /// <returns></returns>
    private bool CheckFolderExist(string folder)
    {
        if (!Directory.Exists(folder))
        {
            DirectoryInfo directoryInfo = Directory.GetParent(folder);
            if (!directoryInfo.Exists)
                this.CheckFolderExist(directoryInfo.FullName);

            Directory.CreateDirectory(folder);
            Debug.LogFormat("<><ResourceUtils.CheckFolderExist>Folder: {0}", folder);
            return true;
        }
        else return true;
    }
    /// <summary>
    /// 删除原新闻页目录下的文件
    /// </summary>
    private void DeleteOldFiles()
    {
        //删除所有的音频
        string audioPath = string.Format("{0}/Audio", Application.persistentDataPath);
        if (Directory.Exists(audioPath))
        {
            Debug.LogFormat("<><ResourceUtils.DeleteOldFiles>Audio path existed: {0}", audioPath);
            string[] fileInfos = Directory.GetFiles(audioPath, "*.mp3", SearchOption.TopDirectoryOnly);
            Debug.LogFormat("<><ResourceUtils.DeleteOldFiles>File count: {0}", fileInfos != null ? fileInfos.Length : -1);
            if (fileInfos != null && fileInfos.Length > 0)
            {
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    File.Delete(fileInfos[i]);
                    Debug.LogFormat("<><ResourceUtils.DeleteOldFiles>Delete file: {0}", fileInfos[i]);
                }
            }
        }
        //删除所有的图片
        string spritePath = string.Format("{0}/Sprite", Application.persistentDataPath);
        if (Directory.Exists(spritePath))
        {
            Debug.LogFormat("<><ResourceUtils.DeleteOldFiles>Sprite path existed: {0}", spritePath);
            Directory.Delete(spritePath, true);
        }
    }
    /// <summary>
    /// 检查AssetBundle缓存
    /// </summary>
    /// <param name="assetBundlePath">assetBundle名字或路径</param>
    /// <returns>缓存中存在指定的AssetBundle，且不为null返回true，否则返回false</returns>
    private bool CheckAssetBundleBuffer(string assetBundlePath)
    {
        if (this.assetBundleBuffer != null && this.assetBundleBuffer.ContainsKey(assetBundlePath))
        {
            if (this.assetBundleBuffer[assetBundlePath] != null)
            {
                return true;
            }
            else
            {
                this.assetBundleBuffer.Remove(assetBundlePath);
                return false;
            }
        }
        else return false;
    }
}

public sealed class FailureInfo
{
    public bool Interrupt { get; set; }
    public string Message { get; set; }
}

/// <summary>
/// 文件类型
/// </summary>
public static class FileTypes
{
    public const string AUDIO = ".mp3";
    public const string TEXTURE = ".png";
    public const string ASSET_BUNDLE = ".ab";
    public const string MANIFEST = ".manifest";
    /// <summary>
    /// 判断指定的文件是否是AudioClip文件(.mp3)
    /// </summary>
    /// <param name="fileName">指定文件的文件名</param>
    /// <returns></returns>
    public static bool IsAudio(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo != null && fileInfo.Extension == AUDIO;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><FileTypes.IsAudio>Error: {0}", ex.Message);
            return false;
        }
    }
    /// <summary>
    /// 判断指定的文件是否是Texture文件(.png)
    /// </summary>
    /// <param name="fileName">指定文件的文件名</param>
    /// <returns></returns>
    public static bool IsTexture(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo != null && fileInfo.Extension == TEXTURE;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><FileTypes.IsTexture>Error: {0}", ex.Message);
            return false;
        }
    }
    /// <summary>
    /// 判断指定的文件是否是AssetBundle文件(.ab)
    /// </summary>
    /// <param name="fileName">指定文件的文件名</param>
    /// <returns></returns>
    public static bool IsAssetBundle(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo != null && fileInfo.Extension == ASSET_BUNDLE;
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><FileTypes.IsAssetBundle>Error: {0}", ex.Message);
            return false;
        }
    }
    /// <summary>
    /// 判断指定的文件是否是Manifest文件(.manifest)
    /// </summary>
    /// <param name="fileName">指定文件的文件名</param>
    /// <returns></returns>
    public static bool IsManifest(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            FileInfo fileInfo = new FileInfo(fileName);
            return (fileInfo != null && fileInfo.Extension == MANIFEST) || fileInfo.Name.ToLower() == "android";
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><FileTypes.IsManifest>Error: {0}", ex.Message);
            return false;
        }
    }
}