using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏开始动画
/// </summary>
public class GameStartAnimation : MonoBehaviourExtension
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
    private SkeletonGraphic spine;
    [SerializeField, Range(0f, 2f)]
    private float delayShowPlayer = 0.3f;
    [SerializeField, Range(1f, 5f)]
    private float animationDuration = 3f;
    private Vector3 originLeftPosition;
    private Vector3 originRightPosition;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.originLeftPosition = this.leftPlayer.Transform.position;
        this.originRightPosition = this.rightPlayer.Transform.position;

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
        this.leftPlayer.Transform.position = this.originLeftPosition;
        this.rightPlayer.Transform.position = this.originRightPosition;
        this.rootObject.SetActive(false);
    }

    public void SetPlayerData(string leftPlayerAvatar, string leftPlayerName, string rightPlayerAvatar, string rightPlayerName)
    {
        this.leftPlayer.SetAvatar(leftPlayerAvatar);
        this.leftPlayer.SetName(leftPlayerName);
        this.rightPlayer.SetAvatar(rightPlayerAvatar);
        this.rightPlayer.SetName(rightPlayerName);
    }

    public void Play()
    {
        this.rootObject.SetActive(true);
        this.spine.AnimationState.SetAnimation(0, "into", false);
        this.audioPlayer.volume = 1;
        this.audioPlayer.PlayOneShot(this.audioClip);
        this.DelayInvoke(() =>
        {
            this.leftPlayer.MoveToDestination();
            this.rightPlayer.MoveToDestination();
        }, this.delayShowPlayer);
        this.DelayInvoke(() =>
        {
            this.audioPlayer.DOFade(0f, 0.5f).onComplete = () =>
            {
                //this.rootObject.SetActive(false);
                GameObject.Destroy(this.gameObject);
            };
        }, this.animationDuration - 0.75f);
    }
}

[System.Serializable]
public class PlayerBox
{
    [SerializeField]
    private Image avatar;
    [SerializeField]
    private Text name;
    [SerializeField]
    private Transform transform;
    [SerializeField]
    private Transform targetPosition;
    [SerializeField, Range(0f, 2f)]
    private float duration = 0.5f;
    public Transform Transform { get { return this.transform; } }

    public void SetAvatar(string avatarPath)
    {
        if (!string.IsNullOrEmpty(avatarPath))
        {
            this.avatar.sprite = Resources.Load<Sprite>(avatarPath);
        }
    }

    public void SetName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            this.name.text = name;
        }
    }

    public void MoveToDestination()
    {
        this.transform.DOMove(this.targetPosition.position, this.duration);
    }
}
