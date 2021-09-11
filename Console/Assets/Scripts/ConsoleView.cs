using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 展示端窗口
/// </summary>
public class ConsoleView : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private DataSender dataSender;
    [SerializeField]
    private string dataOwner;
    [SerializeField]
    private InputField health;
    [SerializeField]
    private Text card;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
    }
    private void Update()
    {
    }
    private void OnDestroy()
    {

    }
    /************************************************自 定 义 方 法************************************************/
    public void PlayAudioEffect(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.AUDIO,
            Data = new AudioEffect() { EffectFile = effectName }
        }));
    }

    public void PlayAnimationEffect(string effectName)
    {
        if (string.IsNullOrEmpty(effectName))
            return;

        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.ANIMATION,
            Data = new AnimationEffect() { EffectFile = effectName }
        }));
    }

    public void PlusHealth(int value)
    {
        int healthValue = int.Parse(this.health.text) + value;
        this.health.text = healthValue.ToString();
        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.HEALTH,
            Data = new HealthData() { DataOwner = this.dataOwner, Value = healthValue }
        }));
    }

    public void MinusHealth(int value)
    {
        int healthValue = int.Parse(this.health.text);
        healthValue = healthValue >= value ? (healthValue - value) : 0;
        this.health.text = healthValue.ToString();
        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.HEALTH,
            Data = new HealthData() { DataOwner = this.dataOwner, Value = healthValue }
        }));
    }

    public void PlusCard(int value)
    {
        int cardValue = int.Parse(this.card.text) + value;
        this.card.text = cardValue.ToString();
        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.CARD,
            Data = new CardData() { DataOwner = this.dataOwner, Value = cardValue }
        }));
    }

    public void MinusCard(int value)
    {
        int cardValue = int.Parse(this.card.text);
        cardValue = cardValue >= value ? (cardValue - value) : 0;
        this.card.text = cardValue.ToString();
        this.dataSender.SendData(PackageHelper.Package(new NetData()
        {
            Tag = NetDataTags.CARD,
            Data = new CardData() { DataOwner = this.dataOwner, Value = cardValue }
        }));
    }
}
