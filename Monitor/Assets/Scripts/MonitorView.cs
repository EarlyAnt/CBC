using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 展示端窗口
/// </summary>
public class MonitorView : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    #region 页面UI组件
    [SerializeField]
    private Text timer;
    [SerializeField]
    private DataReceiver messageReceiver;
    [SerializeField]
    private AnimationPlayer animationPlayer;
    [SerializeField]
    private PlayerPanel leftPlayerPanel;
    [SerializeField]
    private PlayerPanel rightPlayerPanel;
    [SerializeField]
    private StatusPanel leftStatusPanel;
    [SerializeField]
    private StatusPanel rightStatusPanel;
    [SerializeField, Range(0, 60)]
    private int gameDuration = 20;
    #endregion
    #region 其他变量
    private int leftSeconds = 0;
    private GameEvents gameEvent = GameEvents.End;
    private Queue<System.Action> actions = new Queue<System.Action>();
    #endregion
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.messageReceiver.ReceiveDataAction = this.OnReceiveData;
    }
    private void Update()
    {
        while (this.actions != null && this.actions.Count > 0)
        {
            System.Action action = this.actions.Dequeue();
            action.Invoke();
        }
    }
    private void OnDestroy()
    {
    }
    /************************************************自 定 义 方 法************************************************/
    //当接收到数据时
    private void OnReceiveData(string dataString)
    {
        this.actions.Enqueue(() =>
        {
            NetData netData = PackageHelper.DataToNetData(dataString);
            if (netData == null) { return; }

            switch (netData.Tag)
            {
                case NetDataTags.AUDIO:
                    this.HandleAudio(netData);
                    break;
                case NetDataTags.ANIMATION:
                    this.HandleAnimation(netData);
                    break;
                case NetDataTags.EVENT:
                    this.HandleGameEvent(netData);
                    break;
                case NetDataTags.HURT:
                    this.HandleHurt(netData);
                    break;
                case NetDataTags.WEAK:
                    this.HandleWeak(netData);
                    break;
                case NetDataTags.AID:
                    this.HandleAid(netData);
                    break;
                case NetDataTags.EFFECT:
                    this.HandleEffect(netData);
                    break;
                case NetDataTags.CARD:
                    this.HandleCardCount(netData);
                    break;
                case NetDataTags.HEALTH:
                    this.HandleHealth(netData);
                    break;
                case NetDataTags.AVATAR:
                    this.HandleAvatarName(netData);
                    break;
            }
        });
    }
    //获取游戏数据对象
    private T GetGameData<T>(object jsonData)
    {
        string dataString = JsonUtil.Json2String(jsonData);
        T gameData = JsonUtil.String2Json<T>(dataString);
        return gameData;
    }
    //刷新计时器
    private void RefreshTimer()
    {
        if (this.gameEvent == GameEvents.Start)
        {
            if (this.leftSeconds > 0)
            {
                this.leftSeconds -= 1;
                int minute = this.leftSeconds / 60;
                int second = this.leftSeconds - (minute > 0 ? minute * 60 : 0);
                this.timer.text = string.Format("{0:d2}:{1:d2}", minute, second);

                if (this.leftSeconds <= 30)
                    this.animationPlayer.Play("countdown");
            }
            else
            {
                this.CancelInvoke("RefreshTimer");
            }
        }
    }
    //处理游戏事件
    private void HandleGameEvent(NetData netData)
    {
        EventData eventData = this.GetGameData<EventData>(netData.Data);
        switch (eventData.GameEvent)
        {
            case GameEvents.Start:
                if (this.gameEvent == GameEvents.End)
                {
                    PlayerInfo playerInfo = JsonUtil.String2Json<PlayerInfo>(eventData.Parameter.ToString());
                    if (playerInfo != null)
                    {
                        Sprite leftAvatar = null, rightAvatar = null;
                        if (!string.IsNullOrEmpty(playerInfo.LeftAvatar))
                        {
                            this.StartCoroutine(ResourceUtils.Instance.LoadTexture(playerInfo.LeftAvatar, (avatar) =>
                            {
                                leftAvatar = avatar;
                            }, (failureInfo) =>
                            {
                                Debug.LogErrorFormat("<><MonitorView.OnReceiveData>Error: {0}", failureInfo.Message);
                            }));
                        }

                        if (!string.IsNullOrEmpty(playerInfo.RightAvatar))
                        {
                            this.StartCoroutine(ResourceUtils.Instance.LoadTexture(playerInfo.RightAvatar, (avatar) =>
                            {
                                rightAvatar = avatar;
                            }, (failureInfo) =>
                            {
                                Debug.LogErrorFormat("<><MonitorView.OnReceiveData>Error: {0}", failureInfo.Message);
                            }));
                        }

                        this.DelayInvoke(() =>
                        {
                            this.EndGame();
                            this.StartGame(playerInfo.LeftName, playerInfo.RightName, leftAvatar, rightAvatar);

                            this.leftPlayerPanel.SetName(playerInfo.LeftName);
                            this.rightPlayerPanel.SetName(playerInfo.RightName);
                            this.leftPlayerPanel.SetAvatar(leftAvatar);
                            this.rightPlayerPanel.SetAvatar(rightAvatar);
                            this.gameEvent = eventData.GameEvent;
                        }, 1f);
                    }
                }
                else
                {
                    this.gameEvent = eventData.GameEvent;
                }
                break;
            case GameEvents.Pause:
                this.gameEvent = eventData.GameEvent;
                break;
            case GameEvents.End:
                this.gameEvent = eventData.GameEvent;
                this.EndGame();
                break;
        }
    }
    //开始游戏
    private void StartGame(string leftPlayerName, string rightPlayerName, Sprite leftPlayerAvatar, Sprite rightPlayerAvatar)
    {
        this.leftSeconds = this.gameDuration * 60;

        GameStartAnimation animation = this.animationPlayer.Play("gamestart",
            () => this.InvokeRepeating("RefreshTimer", 0, 1)) as GameStartAnimation;
        if (animation != null)
        {
            animation.SetPlayerData(leftPlayerName, rightPlayerName, leftPlayerAvatar, rightPlayerAvatar);
            animation.Play();
        }
    }
    //结束游戏
    private void EndGame()
    {
        this.CancelInvoke("RefreshTimer");
        this.leftSeconds = 0;
        this.timer.text = "00:00";

        this.leftPlayerPanel.SetAvatar(null);
        this.leftPlayerPanel.SetName("蓝方玩家");
        this.leftPlayerPanel.SetCardCount(40);
        this.leftPlayerPanel.SetHurtCount(0);
        this.leftStatusPanel.SetHealth(0);
        this.leftStatusPanel.SetStatus(StatusPanel.Items.Weak, false);
        this.leftStatusPanel.SetStatus(StatusPanel.Items.Aid, false);
        this.leftStatusPanel.SetStatus(StatusPanel.Items.Effect, false);

        this.rightPlayerPanel.SetAvatar(null);
        this.rightPlayerPanel.SetName("红方玩家");
        this.rightPlayerPanel.SetCardCount(40);
        this.rightPlayerPanel.SetHurtCount(0);
        this.rightStatusPanel.SetHealth(0);
        this.rightStatusPanel.SetStatus(StatusPanel.Items.Weak, false);
        this.rightStatusPanel.SetStatus(StatusPanel.Items.Aid, false);
        this.rightStatusPanel.SetStatus(StatusPanel.Items.Effect, false);
    }
    //处理音效
    private void HandleAudio(NetData netData)
    {
        AudioEffect audioEffect = this.GetGameData<AudioEffect>(netData.Data);
        this.animationPlayer.Play(audioEffect.EffectFile);
    }
    //处理动画
    private void HandleAnimation(NetData netData)
    {
        AnimationEffect animationEffect = this.GetGameData<AnimationEffect>(netData.Data);
        this.animationPlayer.Play(animationEffect.EffectFile);
    }
    //处理伤害值
    private void HandleHurt(NetData netData)
    {
        HurtData hurtData = this.GetGameData<HurtData>(netData.Data);
        if (hurtData.DataOwner == DataOwners.LEFT)
        {
            System.Action setHurtAction = () => this.leftPlayerPanel.SetHurtCount(hurtData.Value);
            if (this.leftPlayerPanel.HurtCount < hurtData.Value)
            {
                this.animationPlayer.Play("cutcardleft", () =>
                {
                    setHurtAction();
                    if (hurtData.Value == 4)
                        this.animationPlayer.Play("harm4left");
                });
            }
            else
            {
                setHurtAction();
            }
        }
        else if (hurtData.DataOwner == DataOwners.RIGHT)
        {
            System.Action setHurtAction = () => this.rightPlayerPanel.SetHurtCount(hurtData.Value);
            if (this.rightPlayerPanel.HurtCount < hurtData.Value)
            {
                this.animationPlayer.Play("cutcardright", () =>
                {
                    setHurtAction();
                    if (hurtData.Value == 4)
                        this.animationPlayer.Play("harm4right");
                });
            }
            else
            {
                setHurtAction();
            }
        }
    }
    //处理虚弱状态
    private void HandleWeak(NetData netData)
    {
        WeakData weakData = this.GetGameData<WeakData>(netData.Data);
        if (weakData.DataOwner == DataOwners.LEFT)
            this.leftStatusPanel.SetStatus(StatusPanel.Items.Weak, weakData.Light);
        else if (weakData.DataOwner == DataOwners.RIGHT)
            this.rightStatusPanel.SetStatus(StatusPanel.Items.Weak, weakData.Light);
    }
    //处理援助状态
    private void HandleAid(NetData netData)
    {
        AidData aidData = this.GetGameData<AidData>(netData.Data);
        if (aidData.DataOwner == DataOwners.LEFT)
            this.leftStatusPanel.SetStatus(StatusPanel.Items.Aid, aidData.Light);
        else if (aidData.DataOwner == DataOwners.RIGHT)
            this.rightStatusPanel.SetStatus(StatusPanel.Items.Aid, aidData.Light);
    }
    //处理效果状态
    private void HandleEffect(NetData netData)
    {
        EffectsData effectsData = this.GetGameData<EffectsData>(netData.Data);
        if (effectsData.DataOwner == DataOwners.LEFT)
            this.leftStatusPanel.SetStatus(StatusPanel.Items.Effect, effectsData.Light);
        else if (effectsData.DataOwner == DataOwners.RIGHT)
            this.rightStatusPanel.SetStatus(StatusPanel.Items.Effect, effectsData.Light);
    }
    //处理卡片数量
    private void HandleCardCount(NetData netData)
    {
        CardData cardData = this.GetGameData<CardData>(netData.Data);
        if (cardData.DataOwner == DataOwners.LEFT)
        {
            this.leftPlayerPanel.SetCardCount(cardData.Value);
            if (cardData.Value <= 5)
                this.animationPlayer.Play("cardtipsleft");
        }
        else if (cardData.DataOwner == DataOwners.RIGHT)
        {
            this.rightPlayerPanel.SetCardCount(cardData.Value);
            if (cardData.Value <= 5)
                this.animationPlayer.Play("cardtipsright");
        }
    }
    //处理战斗力
    private void HandleHealth(NetData netData)
    {
        HealthData healthData = this.GetGameData<HealthData>(netData.Data);
        if (healthData.DataOwner == DataOwners.LEFT)
            this.leftStatusPanel.SetHealth(healthData.Value);
        else if (healthData.DataOwner == DataOwners.RIGHT)
            this.rightStatusPanel.SetHealth(healthData.Value);
    }
    //处理头像
    private void HandleAvatarName(NetData netData)
    {
        AvatarNameData avatarNameData = this.GetGameData<AvatarNameData>(netData.Data);
        if (avatarNameData.DataOwner == DataOwners.LEFT)
            this.leftPlayerPanel.SetName(avatarNameData.Name);
        else if (avatarNameData.DataOwner == DataOwners.RIGHT)
            this.rightPlayerPanel.SetName(avatarNameData.Name);

        if (!string.IsNullOrEmpty(avatarNameData.AvatarUrl))
        {
            this.StartCoroutine(ResourceUtils.Instance.LoadTexture(avatarNameData.AvatarUrl, (avatar) =>
            {
                if (avatarNameData.DataOwner == DataOwners.LEFT)
                    this.leftPlayerPanel.SetAvatar(avatar);
                else if (avatarNameData.DataOwner == DataOwners.RIGHT)
                    this.rightPlayerPanel.SetAvatar(avatar);
            }, (failureInfo) =>
            {
                Debug.LogErrorFormat("<><MonitorView.OnReceiveData>Error: {0}", failureInfo.Message);
            }));
        }
    }
}

/// <summary>
/// 玩家信息面板
/// </summary>
[System.Serializable]
public class PlayerPanel
{
    [SerializeField]
    private Image avatarBox;//头像
    [SerializeField]
    private Text nameBox;//名字
    [SerializeField]
    private Text cardCountBox;//卡牌数
    [SerializeField]
    private List<Icon> hurtCountList;//伤害
    public int HurtCount//伤害数
    {
        get
        {
            return hurtCountList != null && hurtCountList.Count > 0 ?
                   hurtCountList.FindAll(t => t.Light).Count : 0;
        }
    }

    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="avatar"></param>
    public void SetAvatar(Sprite avatar)
    {
        this.avatarBox.sprite = avatar != null ? avatar : Resources.Load<Sprite>("Images/profile");
    }
    /// <summary>
    /// 设置姓名
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        this.nameBox.text = name;
    }
    /// <summary>
    /// 设置卡牌数量
    /// </summary>
    /// <param name="cardCount"></param>
    public void SetCardCount(int cardCount)
    {
        this.cardCountBox.text = cardCount.ToString();
    }
    /// <summary>
    /// 设置伤害
    /// </summary>
    /// <param name="hurtCount"></param>
    public void SetHurtCount(int hurtCount)
    {
        int currentHurtCount = this.HurtCount;

        for (int i = 0; i < this.hurtCountList.Count; i++)
        {
            this.hurtCountList[i].SetStatus(i < hurtCount, false);
        }

        if (currentHurtCount < this.HurtCount && hurtCount > 0)
            this.hurtCountList[hurtCount - 1].SetStatus(true, true);
    }
}

/// <summary>
/// 状态信息面板
/// </summary>
[System.Serializable]
public class StatusPanel
{
    public enum Items { Weak, Aid, Effect }//游戏效果状态枚举
    [SerializeField]
    private Icon weakBox;//衰弱
    [SerializeField]
    private Icon aidBox;//援助
    [SerializeField]
    private Icon effectBox;//效果
    [SerializeField]
    private Text healthBox;//战力

    /// <summary>
    /// 设置玩家状态
    /// </summary>
    /// <param name="item">特效项目</param>
    /// <param name="light">是否点亮</param>
    public void SetStatus(Items item, bool light)
    {
        switch (item)
        {
            case Items.Weak:
                this.weakBox.SetStatus(light);
                break;
            case Items.Aid:
                this.aidBox.SetStatus(light);
                break;
            case Items.Effect:
                this.effectBox.SetStatus(light);
                break;
        }
    }
    /// <summary>
    /// 设置战力
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        this.healthBox.text = health.ToString();
    }
}

/// <summary>
/// 图标
/// </summary>
[System.Serializable]
public class Icon
{
    [SerializeField]
    private Image imageBox;//图片框
    [SerializeField]
    private Sprite normalSprite;//常态图片
    [SerializeField]
    private Sprite lightSprite;//高亮图片
    public bool Light { get; private set; }//是否点亮

    /// <summary>
    /// 设置图标状态
    /// </summary>
    /// <param name="light">是否点亮</param>
    public void SetStatus(bool light, bool showAnimation = false)
    {
        this.Light = light;
        this.imageBox.sprite = light ? this.lightSprite : this.normalSprite;
        if (showAnimation)
        {
            ShowHurt showHurt = this.imageBox.GetComponent<ShowHurt>();
            if (showHurt != null) showHurt.Play();
        }
    }
}
