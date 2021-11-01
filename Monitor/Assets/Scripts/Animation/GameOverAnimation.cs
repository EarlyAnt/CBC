using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏结束动画
/// </summary>
public class GameOverAnimation : BaseAnimation
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private GameObject rootObject;
    [SerializeField]
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private PlayerBox leftPlayer;
    [SerializeField]
    private PlayerBox rightPlayer;
    [SerializeField]
    private SkeletonGraphic intoSpine;
    [SerializeField]
    private SkeletonGraphic idleSpine;
    [SerializeField]
    private string intoAnimation;
    [SerializeField]
    private string idleAnimation;
    [SerializeField, Range(0f, 2f)]
    private float delayShowPlayer = 0.3f;
    [SerializeField, Range(1f, 5f)]
    private float animationDuration = 3f;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
    }
    /************************************************自 定 义 方 法************************************************/
    public void SetPlayerData(string leftPlayerName, string rightPlayerName, Sprite leftPlayerAvatar, Sprite rightPlayerAvatar)
    {
        this.leftPlayer.SetName(leftPlayerName);
        this.rightPlayer.SetName(rightPlayerName);
        this.leftPlayer.SetAvatar(leftPlayerAvatar ? leftPlayerAvatar : Resources.Load<Sprite>("Images/profile"));
        this.rightPlayer.SetAvatar(rightPlayerAvatar ? rightPlayerAvatar : Resources.Load<Sprite>("Images/profile"));
    }

    public void Play()
    {
        this.rootObject.SetActive(true);
        this.intoSpine.gameObject.SetActive(true);

        this.intoSpine.AnimationState.SetAnimation(0, this.intoAnimation, false).Complete += (trackEntry) =>
        {
            this.idleSpine.gameObject.SetActive(true);
            this.idleSpine.AnimationState.SetAnimation(0, this.idleAnimation, true);
        };
        this.audioPlayer.volume = 1;
        this.audioPlayer.PlayOneShot(this.audioClip);
        this.DelayInvoke(() =>
        {
            this.leftPlayer.MoveToDestination();
            this.rightPlayer.MoveToDestination();
        }, this.delayShowPlayer);

        //this.DelayInvoke(() =>
        //{
        //    this.audioPlayer.DOFade(0f, 0.5f).onComplete = () =>
        //    {
        //        this.OnComplete();
        //        GameObject.Destroy(this.gameObject);
        //    };
        //}, this.animationDuration - 0.75f);
    }
}
