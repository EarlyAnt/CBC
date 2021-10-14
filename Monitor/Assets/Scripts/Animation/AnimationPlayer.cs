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
        animationInfos.Add("gamestart", new AnimationInfo() { Animation = "Prefabs/GameStart" });
        animationInfos.Add("cutcardleft", new AnimationInfo() { Animation = "Prefabs/CutCardLeft" });
        animationInfos.Add("cutcardright", new AnimationInfo() { Animation = "Prefabs/CutCardRight" });
        animationInfos.Add("countdown", new AnimationInfo() { Animation = "Prefabs/CountDown" });
        animationInfos.Add("harm4left", new AnimationInfo() { Animation = "Prefabs/Harm4Left" });
        animationInfos.Add("harm4right", new AnimationInfo() { Animation = "Prefabs/Harm4Right" });
        animationInfos.Add("cardtipsleft", new AnimationInfo() { Animation = "Prefabs/CardTipsLeft" });
        animationInfos.Add("cardtipsright", new AnimationInfo() { Animation = "Prefabs/CardTipsRight" });
        animationInfos.Add("cr", new AnimationInfo() { Audio = "Audios/rare_card", Animation = "Prefabs/RareCardS" });
        animationInfos.Add("ur", new AnimationInfo() { Audio = "Audios/rare_card", Animation = "Prefabs/RareCardSS" });
        animationInfos.Add("guzhang", new AnimationInfo() { Audio = "Audios/guzhang2" });
        animationInfos.Add("huanhu", new AnimationInfo() { Audio = "Audios/huanhu" });
    }
    /************************************************自 定 义 方 法************************************************/
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

        if (!string.IsNullOrEmpty(animationInfo.Animation))
        {
            Object animationObject = Resources.Load(animationInfo.Animation);
            GameObject gameObject = GameObject.Instantiate(animationObject) as GameObject;
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

    public bool IsPlaying(string animationName)
    {
        if (!this.animationInfos.ContainsKey(animationName))
            return false;

        string prefabName = this.animationInfos[animationName].Animation;
        if (string.IsNullOrEmpty(prefabName))
            return false;

        prefabName = prefabName.Replace("Prefabs/", "");
        BaseAnimation[] animations = GameObject.FindObjectsOfType<BaseAnimation>();
        bool existed = animations != null && animations.Length > 0 && animations.ToList().Exists(t => t.name.Contains(prefabName));
        return existed;
    }
}

public class AnimationInfo
{
    public string Audio { get; set; }
    public string Animation { get; set; }
}
