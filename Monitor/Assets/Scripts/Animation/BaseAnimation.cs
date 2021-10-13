﻿using System;
using UnityEngine;

/// <summary>
/// 游戏开始动画
/// </summary>
public abstract class BaseAnimation : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    public Action Complete { get; set; }
    [SerializeField]
    protected bool autoPlay = true;
    /************************************************Unity方法与事件***********************************************/

    /************************************************自 定 义 方 法************************************************/
    protected virtual void OnComplete()
    {
        try
        {
            if (this.Complete != null)
                this.Complete();
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("<><{0}.OnComplete>Error: {1}", this.GetType().Name, ex.Message);
        }
    }
}

