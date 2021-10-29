using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 动画播放控制器
/// </summary>
public class AnimationPlayer : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private Transform animationRoot;
    [SerializeField]
    private AudioSource audioPlayer;
    private Dictionary<string, AnimationInfo> animationInfos;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        animationInfos = new Dictionary<string, AnimationInfo>();
        animationInfos.Add("gamestart", new AnimationInfo() { Path = "Prefabs/", Name = "GameStart" });
        animationInfos.Add("gameoverleft", new AnimationInfo() { Path = "Prefabs/", Name = "GameOverLeft" });
        animationInfos.Add("gameoverright", new AnimationInfo() { Path = "Prefabs/", Name = "GameOverRight" });
        animationInfos.Add("cutcardleft", new AnimationInfo() { Path = "Prefabs/", Name = "CutCardLeft" });
        animationInfos.Add("cutcardright", new AnimationInfo() { Path = "Prefabs/", Name = "CutCardRight" });
        animationInfos.Add("countdown", new AnimationInfo() { Path = "Prefabs/", Name = "CountDown" });
        animationInfos.Add("harm4left", new AnimationInfo() { Path = "Prefabs/", Name = "Harm4Left" });
        animationInfos.Add("harm4right", new AnimationInfo() { Path = "Prefabs/", Name = "Harm4Right" });
        animationInfos.Add("cardtipsleft", new AnimationInfo() { Path = "Prefabs/", Name = "CardTipsLeft" });
        animationInfos.Add("cardtipsright", new AnimationInfo() { Path = "Prefabs/", Name = "CardTipsRight" });
        animationInfos.Add("cr", new AnimationInfo() { Audio = "Audios/rare_card", Path = "Prefabs/", Name = "RareCardS" });
        animationInfos.Add("ur", new AnimationInfo() { Audio = "Audios/rare_card", Path = "Prefabs/", Name = "RareCardSS" });
        animationInfos.Add("guzhang", new AnimationInfo() { Audio = "Audios/guzhang2" });
        animationInfos.Add("huanhu", new AnimationInfo() { Audio = "Audios/huanhu" });
    }
    /************************************************自 定 义 方 法************************************************/
    //播放动画
    public BaseAnimation Play(string animationName, System.Action callback = null, bool single = true)
    {
        BaseAnimation baseAnimation = null;

        if (!this.animationInfos.ContainsKey(animationName))
            return null;

        if (single && this.IsPlaying(animationName))
            return null;

        AnimationInfo animationInfo = this.animationInfos[animationName];
        if (!string.IsNullOrEmpty(animationInfo.Audio))
        {
            Object gameObject = Resources.Load(animationInfo.Audio);
            AudioClip audioClip = Resources.Load<AudioClip>(animationInfo.Audio);
            this.audioPlayer.PlayOneShot(audioClip);
        }

        if (!string.IsNullOrEmpty(animationInfo.Name))
        {
            Object animationObject = Resources.Load(animationInfo.FullName);
            GameObject gameObject = GameObject.Instantiate(animationObject) as GameObject;
            gameObject.name = animationInfo.Name;
            gameObject.transform.parent = this.animationRoot;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            baseAnimation = gameObject.GetComponent<BaseAnimation>();
            if (callback != null && baseAnimation != null)
            {
                baseAnimation.Complete = callback;
            }
        }

        return baseAnimation;
    }
    //判断指定动画是否仍在播放
    public bool IsPlaying(string animationName)
    {
        if (!this.animationInfos.ContainsKey(animationName))
            return false;

        string prefabName = this.animationInfos[animationName].Name;
        if (string.IsNullOrEmpty(prefabName))
            return false;

        BaseAnimation[] animations = GameObject.FindObjectsOfType<BaseAnimation>();
        bool existed = animations != null && animations.Length > 0 && animations.ToList().Exists(t => t.name.Contains(prefabName));
        return existed;
    }
    //停止播放并删除指定动画
    public void Stop(string animationName)
    {
        if (!this.animationInfos.ContainsKey(animationName))
            return;

        string prefabName = this.animationInfos[animationName].Name;
        if (string.IsNullOrEmpty(prefabName))
            return;

        GameObject animationObject = GameObject.Find(prefabName);
        if (animationObject != null)
            GameObject.Destroy(animationObject);
    }
    //删除指定类型的动画
    public void Stop(System.Type type)
    {
        Object[] animationObjects = GameObject.FindObjectsOfType(type);
        if (animationObjects != null && animationObjects.Length > 0)
        {
            foreach (var animationObject in animationObjects)
            {
                GameObject gameObject = GameObject.Find(animationObject.name);
                if (gameObject != null)
                    GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}

public class AnimationInfo
{
    public string Audio { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public string FullName
    {
        get
        {
            string path = string.IsNullOrEmpty(this.Path) ? "" : this.Path;
            string name = string.IsNullOrEmpty(this.Name) ? "" : this.Name;
            return path + name;
        }
    }
}
