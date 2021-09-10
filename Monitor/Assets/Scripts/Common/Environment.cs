using System;
using System.IO;

/// <summary>
/// 运行环境
/// </summary>
public static class Environment
{
    /// <summary>
    /// Plugins文件夹的位置
    /// </summary>
    public static string PluginsFolder
    {
        get { return GetUnityStartupPath() + "/Plugins"; }
    }
    /// <summary>
    /// 项目根目录位置(编译前为Asset文件夹所在的目录，编译后为XX_Data文件夹所在的目录)
    /// </summary>
    public static string RootFolder
    {
        get { return Directory.GetParent(GetUnityStartupPath()).ToString(); }
    }
    /// <summary>
    /// 项目根目录的上一级目录的位置
    /// </summary>
    public static string ParentFolder
    {
        get { return Directory.GetParent(Directory.GetParent(GetUnityStartupPath()).ToString()).ToString(); }
    }
    /// <summary>
    /// 项目根目录中的Files文件夹
    /// </summary>
    public static string ExtendFileFolder
    {
        get { return RootFolder + "\\Files"; }
    }
    /// <summary>
    /// Unity3D启动路径
    /// </summary>
    private static string unityStartupPath = "";
    /// <summary>
    /// 设置Unity3D启动路径
    /// </summary>
    /// <param name="path">路径</param>
    public static void SetUnityStartupPath(string path)
    {
        unityStartupPath = path;
    }
    /// <summary>
    /// 获取Unity3D启动路径
    /// </summary>
    /// <returns>返回Unity3D启动路径</returns>
    public static string GetUnityStartupPath()
    {
        return string.IsNullOrEmpty(unityStartupPath) ? AppDomain.CurrentDomain.BaseDirectory : unityStartupPath;
    }
}
