using Spine.Unity;
using UnityEngine;

/// <summary>
///  普通动画
/// </summary>
public class GeneralAnimation : BaseAnimation
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private SkeletonGraphic spine;
    [SerializeField]
    private string animationClipName;
    [SerializeField, Range(1f, 5f)]
    private float animationDuration = 3f;
    [SerializeField, Range(0f, 2f)]
    private float audioFadeOut = 0.5f;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.Initialize();
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
    private void Initialize()
    {
        if (this.spine != null && this.spine.AnimationState != null)
            this.spine.AnimationState.ClearTracks();
        this.audioPlayer.volume = 0;
    }

    public void Play()
    {
        this.spine.AnimationState.SetAnimation(0, this.animationClipName, false).Complete += (trackEntry) =>
        {
            this.OnComplete();
            GameObject.Destroy(this.gameObject);
        };

        this.DelayInvoke(() =>
        {
            this.audioPlayer.volume = 1;
            this.audioPlayer.PlayOneShot(this.audioClip);
        }, this.audioFadeOut);
    }
}
