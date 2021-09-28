using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 展示端窗口
/// </summary>
public class MonitorView : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private DataReceiver messageReceiver;
    [SerializeField]
    private DataReceiver rawDataReceiver;
    [SerializeField]
    private RawImage image;
    [SerializeField]
    private Text leftHealth;
    [SerializeField]
    private Text rightHealth;
    [SerializeField]
    private Text leftHurt;
    [SerializeField]
    private Text rightHurt;
    [SerializeField]
    private Text leftCard;
    [SerializeField]
    private Text rightCard;
    [SerializeField]
    private AnimationPlayer animationPlayer;
    private Queue<Action> actions = new Queue<Action>();
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.messageReceiver.ReceiveDataAction = this.OnReceiveData;
        this.rawDataReceiver.ReceiveRawDataAction = this.OnReceiveRawData;
    }
    private void Update()
    {
        while (this.actions != null && this.actions.Count > 0)
        {
            Action action = this.actions.Dequeue();
            action.Invoke();
        }
    }
    private void OnDestroy()
    {

    }
    /************************************************自 定 义 方 法************************************************/
    private void OnReceiveData(string dataString)
    {
        this.actions.Enqueue(() =>
        {
            NetData netData = PackageHelper.DataToNetData(dataString);
            if (netData == null) { return; }

            switch (netData.Tag)
            {
                case NetDataTags.AUDIO:
                    string audioDataString = JsonUtil.Json2String(netData.Data);
                    AudioEffect audioEffect = JsonUtil.String2Json<AudioEffect>(audioDataString);
                    this.animationPlayer.Play(audioEffect.EffectFile);
                    break;
                case NetDataTags.ANIMATION:
                    string animationEffectString = JsonUtil.Json2String(netData.Data);
                    AudioEffect animationEffect = JsonUtil.String2Json<AudioEffect>(animationEffectString);
                    this.animationPlayer.Play(animationEffect.EffectFile);
                    break;
                case NetDataTags.HEALTH:
                    string healthDataString = JsonUtil.Json2String(netData.Data);
                    HealthData healthData = JsonUtil.String2Json<HealthData>(healthDataString);
                    if (healthData.DataOwner == DataOwners.LEFT)
                        this.leftHealth.text = healthData.Value.ToString();
                    else if (healthData.DataOwner == DataOwners.RIGHT)
                        this.rightHealth.text = healthData.Value.ToString();
                    break;
                case NetDataTags.HURT:
                    string hurtDataString = JsonUtil.Json2String(netData.Data);
                    HurtData hurtData = JsonUtil.String2Json<HurtData>(hurtDataString);
                    if (hurtData.DataOwner == DataOwners.LEFT)
                        this.leftHurt.text = hurtData.Value.ToString();
                    else if (hurtData.DataOwner == DataOwners.RIGHT)
                        this.rightHurt.text = hurtData.Value.ToString();
                    break;
                case NetDataTags.CARD:
                    string cardDataString = JsonUtil.Json2String(netData.Data);
                    CardData cardData = JsonUtil.String2Json<CardData>(cardDataString);
                    if (cardData.DataOwner == DataOwners.LEFT)
                        this.leftCard.text = cardData.Value.ToString();
                    else if (cardData.DataOwner == DataOwners.RIGHT)
                        this.rightCard.text = cardData.Value.ToString();
                    break;
                case NetDataTags.AVATAR:
                    string avatarDataString = JsonUtil.Json2String(netData.Data);
                    AvatarData avatarData = JsonUtil.String2Json<AvatarData>(avatarDataString);
                    string[] stringArray = avatarData.DataString.Split(',');
                    List<byte> byteArray = new List<byte>();
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        byteArray.Add(byte.Parse(stringArray[i]));
                    }
                    Texture2D texture = new Texture2D(512, 512);
                    texture.LoadImage(byteArray.ToArray());
                    this.image.texture = texture;
                    break;
            }
        });
    }

    private void OnReceiveRawData(byte[] byteDatas)
    {
        this.actions.Enqueue(() =>
        {
            if (byteDatas != null && byteDatas.Length > 0)
            {
                int width = 1080;
                int height = 1920;
                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(byteDatas);
                this.image.texture = texture;
            }
        });
    }
}
