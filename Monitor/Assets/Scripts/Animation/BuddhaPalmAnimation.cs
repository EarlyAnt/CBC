using DG.Tweening;
using UnityEngine;

/// <summary>
/// 雪花动画
/// </summary>
public class BuddhaPalmAnimation : BaseAnimation
{
    /************************************************属性与变量命名************************************************/
    [SerializeField, Range(1f, 3f)]
    private float rate = 1.5f;
    [SerializeField, Range(0f, 3f)]
    private float duration = 1.5f;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.transform.DOScale(this.rate, this.duration).onComplete += () =>
        {
            GameObject.Destroy(this.gameObject);
        };
    }
    private void OnDestroy()
    {
    }
    /************************************************自 定 义 方 法************************************************/

}
