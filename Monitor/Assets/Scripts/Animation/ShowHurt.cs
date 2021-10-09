using DG.Tweening;
using UnityEngine;

/// <summary>
/// 雪花控制器
/// </summary>
public class ShowHurt : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField, Range(0f, 90f)]
    private float angle = 15f;
    [SerializeField, Range(0f, 6f)]
    private float duration = 0.2f;
    private Vector3 leftAngle { get { return new Vector3(0, 0, -angle); } }
    private Vector3 rightAngle { get { return new Vector3(0, 0, angle); } }
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
    }
    /************************************************自 定 义 方 法************************************************/
    public void Play()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.transform.DOLocalRotate(this.leftAngle, this.duration / 2));
        sequence.Append(this.transform.DOLocalRotate(this.rightAngle, this.duration));
        sequence.Append(this.transform.DOLocalRotate(this.leftAngle, this.duration));
        sequence.Append(this.transform.DOLocalRotate(this.rightAngle, this.duration));
        sequence.Append(this.transform.DOLocalRotate(Vector3.zero, this.duration / 2));
    }
}
