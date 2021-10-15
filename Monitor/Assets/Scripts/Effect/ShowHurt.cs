using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 伤害点数标记闪烁控制器
/// </summary>
public class ShowHurt : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private Image imageBox;
    [SerializeField, Range(0f, 6f)]
    private float duration = 0.2f;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        if (this.imageBox == null)
            this.imageBox = this.GetComponent<Image>();
    }
    /************************************************自 定 义 方 法************************************************/
    public void Play()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.imageBox.DOFade(1f, this.duration));
        sequence.Append(this.imageBox.DOFade(0f, this.duration));
        sequence.Append(this.imageBox.DOFade(1f, this.duration));
        sequence.Append(this.imageBox.DOFade(0f, this.duration));
        sequence.Append(this.imageBox.DOFade(1f, this.duration));
        sequence.Append(this.imageBox.DOFade(0f, this.duration));
        sequence.Append(this.imageBox.DOFade(1f, this.duration));
    }
}
