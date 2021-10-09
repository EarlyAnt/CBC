using System.Collections.Generic;
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
        animationInfos.Add("cr", new AnimationInfo() { Audio = "Audios/shengli", Animation = "Prefabs/SnowAnimation" });
        animationInfos.Add("ur", new AnimationInfo() { Audio = "Audios/daweitianlong", Animation = "Prefabs/BuddhaPalm" });
        animationInfos.Add("guzhang", new AnimationInfo() { Audio = "Audios/guzhang2" });
        animationInfos.Add("huanhu", new AnimationInfo() { Audio = "Audios/huanhu" });
    }
    /************************************************自 定 义 方 法************************************************/
    public void Play(string animationName, System.Action callback = null)
    {
        if (!this.animationInfos.ContainsKey(animationName))
            return;

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

            if (callback != null)
            {
                BasetAnimation baseAnimation = gameObject.GetComponent<BasetAnimation>();
                if (baseAnimation != null)
                    baseAnimation.Complete = () =>
                    {
                        print("animation complete");
                        callback();
                    };
            }
        }
    }
}

public class AnimationInfo
{
    public string Audio { get; set; }
    public string Animation { get; set; }
}
