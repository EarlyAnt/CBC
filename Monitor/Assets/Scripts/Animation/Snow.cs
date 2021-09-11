using UnityEngine;

/// <summary>
/// 雪花控制器
/// </summary>
public class Snow : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField, Range(0f, 360f)]
    private float angle = 2f;
    [SerializeField, Range(0f, 30f)]
    private float speed = 0.5f;
    [SerializeField, Range(1f, 5f)]
    private float lifeTime = 3f;
    private float time = 0;
    private float originX = 0;
    /************************************************Unity方法与事件***********************************************/
    private void Awake()
    {
        this.originX = this.transform.position.x;
    }
    private void Start()
    {
        this.angle *= Random.Range(0.5f, 2f);
        this.speed *= Random.Range(0.5f, 2f);
        this.originX = this.transform.position.x;
    }
    private void Update()
    {
        this.transform.Translate(this.originX, -this.speed, 0, Space.World);
        this.transform.Rotate(0, 0, this.angle, Space.Self);

        this.time += Time.deltaTime;
        if (this.time >= this.lifeTime)
            GameObject.Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
    /************************************************自 定 义 方 法************************************************/

}
