using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪花动画
/// </summary>
public class SnowAnimation : BasetAnimation
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private List<Transform> rootPoints;
    [SerializeField]
    private RectTransform snowPrefab;
    [SerializeField, Range(1f, 5f)]
    private float lifeTime = 3f;
    private float time = 0;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        foreach (Transform point in this.rootPoints)
        {
            GameObject.Instantiate(this.snowPrefab, point.position, point.rotation, point.transform);
        }
    }
    private void Update()
    {
        this.time += Time.deltaTime;
        if (this.time >= this.lifeTime)
            GameObject.Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
    }
    /************************************************自 定 义 方 法************************************************/

}
