using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏开始动画
/// </summary>
public class CutCardAnimation : BasetAnimation
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
        }, 0.65f);
    }
}
