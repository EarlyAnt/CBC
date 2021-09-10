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
    private DataReceiver dataReceiver;
    [SerializeField]
    private Text leftHealth;
    [SerializeField]
    private Text rightHealth;
    [SerializeField]
    private Text leftCard;
    [SerializeField]
    private Text rightCard;
    private Queue<Action> actions = new Queue<Action>();
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.dataReceiver.ReceiveDataAction = this.OnReceiveData;
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
                    break;
                case NetDataTags.ANIMATION:
                    break;
                case NetDataTags.HEALTH:
                    string healthDataString = JsonUtil.Json2String(netData.Data);
                    HealthData healthData = JsonUtil.String2Json<HealthData>(healthDataString);
                    if (healthData.DataOwner == DataOwners.LEFT)
                        this.leftHealth.text = healthData.Value.ToString();
                    else if (healthData.DataOwner == DataOwners.RIGHT)
                        this.rightHealth.text = healthData.Value.ToString();
                    break;
                case NetDataTags.CARD:
                    string cardDataString = JsonUtil.Json2String(netData.Data);
                    CardData cardData = JsonUtil.String2Json<CardData>(cardDataString);
                    if (cardData.DataOwner == DataOwners.LEFT)
                        this.leftCard.text = cardData.Value.ToString();
                    else if (cardData.DataOwner == DataOwners.RIGHT)
                        this.rightCard.text = cardData.Value.ToString();
                    break;
            }
        });
    }
}
