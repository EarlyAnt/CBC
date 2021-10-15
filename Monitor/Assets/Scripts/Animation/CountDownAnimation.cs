using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自动缩放控制器
/// </summary>
public class CountDownAnimation : BaseAnimation
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private SkeletonGraphic spine;//Spine动画
    [SerializeField]
    private Text timerBox;
    [SerializeField]
    private Vector3 targetScale;
    [SerializeField, Range(0f, 5f)]
    private float scaleDuration = 1f;
    private MonitorView monitorView;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.monitorView = GameObject.FindObjectOfType<MonitorView>();
        if (this.monitorView != null)
            this.monitorView.OnTimer += this.UpdateTimer;

        this.DelayInvoke(() =>
        {
            this.spine.gameObject.SetActive(true);
            this.timerBox.transform.DOScale(this.targetScale, this.scaleDuration);
        }, 1f);

        this.DelayInvoke(() =>
        {
            if (this.monitorView != null)
                this.monitorView.OnTimer -= this.UpdateTimer;

            this.OnComplete();
            GameObject.Destroy(this.gameObject);
        }, 31f);
    }
    private void OnDestroy()
    {
        if (this.monitorView != null)
            this.monitorView.OnTimer -= this.UpdateTimer;
    }
    /************************************************自 定 义 方 法************************************************/
    private void UpdateTimer(int leftSeconds)
    {
        int minute = leftSeconds / 60;
        int second = leftSeconds - (minute > 0 ? minute * 60 : 0);
        this.timerBox.text = string.Format("{0:d2}:{1:d2}", minute, second);
    }
}
