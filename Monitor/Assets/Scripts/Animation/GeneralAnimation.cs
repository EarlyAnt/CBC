using Spine.Unity;
using UnityEngine;

/// <summary>
///  普通动画
/// </summary>
public class GeneralAnimation : BaseAnimation
{
    /************************************************属性与变量命名************************************************/
    public enum ExitTriggers { Once, Time }//退出机制
    [SerializeField]
    private AudioSource audioPlayer;//音频播放器
    [SerializeField]
    private AudioClip audioClip;//声音片段
    [SerializeField]
    private SkeletonGraphic spine;//Spine动画
    [SerializeField]
    private string animationClipName;//Spine动画片段
    [SerializeField]
    private ExitTriggers exitTrigger;//退出机制
    [SerializeField, Range(1f, 600f)]
    private float animationDuration = 3f;//动画持续时长
    [SerializeField, Range(0f, 2f)]
    private float audioFadeOut = 0.5f;//声音淡出提前时间
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.Initialize();
        if (this.autoPlay)
            this.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.Initialize();
            this.Play();
        }
    }
    /************************************************自 定 义 方 法************************************************/
    //初始化
    private void Initialize()
    {
        if (this.spine != null && this.spine.AnimationState != null)
            this.spine.AnimationState.ClearTracks();

        if (this.audioPlayer != null)
            this.audioPlayer.volume = 0;
    }
    //播放动画
    private void Play()
    {
        if (this.exitTrigger == ExitTriggers.Once)
            this.PlayOnce();
        else if (this.exitTrigger == ExitTriggers.Time)
            this.PlayForTime();
    }
    //播放一次
    private void PlayOnce()
    {
        this.spine.AnimationState.SetAnimation(0, this.animationClipName, false).Complete += (trackEntry) =>
        {
            this.OnComplete();
            GameObject.Destroy(this.gameObject);
        };

        if (this.audioPlayer != null && this.audioClip != null)
        {
            this.audioPlayer.loop = false;
            this.DelayInvoke(() =>
            {
                this.audioPlayer.volume = 1;
                this.audioPlayer.PlayOneShot(this.audioClip);
            }, this.audioFadeOut);

        }
    }
    //播放直到设定时间退出
    private void PlayForTime()
    {
        this.spine.AnimationState.SetAnimation(0, this.animationClipName, true);
        this.DelayInvoke(() =>
        {
            this.OnComplete();
            GameObject.Destroy(this.gameObject);
        }, this.animationDuration);

        if (this.audioPlayer != null && this.audioClip != null)
        {
            this.audioPlayer.loop = true;
            this.DelayInvoke(() =>
            {
                this.audioPlayer.volume = 1;
                this.audioPlayer.PlayOneShot(this.audioClip);
            }, this.audioFadeOut);

        }
    }
}
