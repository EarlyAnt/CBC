using System;
using System.Collections.Generic;
using System.Net.Sockets;

#region 公共枚举
/// <summary>
/// 物体属性
/// </summary>
public enum Elements : int
{
    /// <summary>
    /// 位置(传递localPosition属性的值)
    /// </summary>
    Position = 0,
    /// <summary>
    /// 角度(传递eulerAngle属性的值)
    /// </summary>
    Rotation = 1,
    /// <summary>
    /// 位置和角度
    /// </summary>
    PosAndRot = 2,
    /// <summary>
    /// 颜色(传递renderer.material.color属性的值)
    /// </summary>
    Color = 3
}
#endregion
#region SuperValue相关
#region SuperValue
/// <summary>
/// 超级值(对常用的布尔型、整形和浮点型数值进行封装)
/// </summary>
public abstract class SuperValue
{
    /// <summary>
    /// 输出自描述信息
    /// </summary>        
    /// <param name="type">类型</param>
    /// <param name="description">描述</param>
    /// <returns>返回自描述信息</returns>
    protected string ToString(Type type, string description)
    {
        return string.Format("{0} Sel1f-description: {1}。", type.FullName, description);
    }
    /// <summary>
    /// 输出自描述信息
    /// </summary>        
    /// <param name="type">类型</param>
    /// <param name="description">描述</param>
    /// <param name="parameters">参数</param>
    /// <returns>返回自描述信息</returns>
    protected string ToString(Type type, string description, string parameters)
    {
        return string.Format("{0} Sel1f-description: {1} [{2}]。", type.FullName, description, parameters);
    }
}
#endregion
#region BValue
/// <summary>
/// 布尔值(开关量)
/// </summary>
[Serializable]
public sealed class BValue : SuperValue
{
    #region 私有变量
    private bool enable = false;
    private bool flashTrue = false;
    private bool flashFalse = false;
    private bool lockTrue = false;
    private bool lockFalse = false;
    #endregion
    #region 公共属性
    /// <summary>
    /// 真实值
    /// </summary>
    public bool Value { get; set; }
    /// <summary>
    /// 空值
    /// </summary>
    public static BValue Empty
    {
        get
        {
            return new BValue(false, false);
        }
    }
    #endregion

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value"></param>
    /// <param name="enable"></param>
    public BValue(bool value, bool enable)
    {
        this.Value = value;
        this.flashTrue = value;
        this.flashFalse = !value;
        this.enable = enable;
    }
    /// <summary>
    /// 获取瞬间为真的状态
    /// </summary>
    /// <returns>值为真的瞬间返回True，否则返回False</returns>
    public bool FlashTrue()
    {
        if (this.enable && this.flashTrue)
        {
            this.flashTrue = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 获取瞬间为假的状态
    /// </summary>
    /// <returns>值为假的瞬间返回True，否则返回False</returns>
    public bool FlashFalse()
    {
        if (this.enable && this.flashFalse)
        {
            this.flashFalse = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 获取瞬间为真的状态
    /// </summary>
    /// <returns>值为真的瞬间返回True，否则返回False</returns>
    public bool BecomeTrue()
    {
        if (this.enable && !lockTrue && this.Value)
        {
            this.lockTrue = true;
            this.lockFalse = false;
            return true;
        }
        else
        {
            if (!this.Value)
            {
                this.lockTrue = false;
            }
            return false;
        }
    }
    /// <summary>
    /// 获取瞬间为假的状态
    /// </summary>
    /// <returns>值为假的瞬间返回True，否则返回False</returns>
    public bool BecomeFalse()
    {
        if (this.enable && !lockFalse && !this.Value)
        {
            this.lockFalse = true;
            this.lockTrue = false;
            return true;
        }
        else
        {
            if (this.Value)
            {
                this.lockFalse = false;
            }
            return false;
        }
    }
    /// <summary>
    /// 输出自描述信息
    /// </summary>
    public override string ToString()
    {
        string toString = this.ToString(this.GetType(), "布尔值", string.Format("Value({0}), FlashTrue({1}), FlashFalse({2})",
                        this.Value, this.flashTrue, this.flashFalse));
        //if (this.flashTrue || this.flashFalse) Debug.Log(toString);
        return toString;
    }
    /// <summary>
    /// 普通布尔值转换成复合布尔值
    /// </summary>
    /// <param name="bValue">复合布尔值</param>
    /// <param name="boolValue">普通布尔值</param>
    /// <returns></returns>
    public static BValue Transfer(BValue bValue, bool boolValue)
    {
        return new BValue(boolValue, true) { lockTrue = bValue.lockTrue, lockFalse = bValue.lockFalse };
    }
    /// <summary>
    /// 运算符重载
    /// </summary>
    /// <param name="boolValue">普通布尔值</param>
    /// <returns>返回复合布尔值</returns>
    public static implicit operator BValue(bool boolValue)
    {
        return new BValue(boolValue, true);
    }
    /// <summary>
    /// 运算符重载
    /// </summary>
    /// <param name="bValue">复合布尔值</param>
    /// <returns>返回普通布尔值</returns>
    public static implicit operator bool(BValue bValue)
    {
        return bValue.Value;
    }
}
#endregion
#region NValue<T>
/// <summary>
/// 数值(模拟量)
/// </summary>
[Serializable]
public abstract class NValue<T> : SuperValue
{
    #region 私有变量
    public Dictionary<T, bool> LockEqual { get; protected set; }
    public Dictionary<T, bool> LockChange { get; protected set; }
    public Dictionary<T, bool> LockUpper { get; protected set; }
    public Dictionary<T, bool> LockLower { get; protected set; }
    #endregion
    #region 公共属性
    /// <summary>
    /// 真实值
    /// </summary>
    public T Value { get; protected set; }
    /// <summary>
    /// 上一个真实值
    /// </summary>
    public T LastValue { get; protected set; }
    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    protected NValue()
    {
        this.LockEqual = new Dictionary<T, bool>();
        this.LockChange = new Dictionary<T, bool>();
        this.LockUpper = new Dictionary<T, bool>();
        this.LockLower = new Dictionary<T, bool>();
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value"></param>
    public NValue(T value)
        : this()
    {
        this.Value = value;
        this.LastValue = value;
        this.SetFlashStatus(this.LockEqual, value, true);
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="lastValue">上一个值</param>
    /// <param name="lockEqual">等于某个值的瞬间</param>
    /// <param name="lockChange">不等于某个值的瞬间</param>
    /// <param name="lockUpper">大于某个值的瞬间</param>
    /// <param name="lockLower">小于某个值的瞬间</param>
    public NValue(T value, T lastValue, Dictionary<T, bool> lockEqual, Dictionary<T, bool> lockChange,
                           Dictionary<T, bool> lockUpper, Dictionary<T, bool> lockLower)
        : this()
    {
        this.Value = value;
        this.LastValue = lastValue;
        this.LockEqual = lockEqual;
        this.LockChange = lockChange;
        this.LockUpper = lockUpper;
        this.LockLower = lockLower;
    }
    /// <summary>
    /// 获取等于某个值的瞬间
    /// </summary>
    /// <param name="t">被比较的值</param>
    /// <returns>等于某个值的瞬间返回True，否则返回False</returns>
    public abstract bool FlashEqual(T t);
    /// <summary>
    /// 获取不等于某个值的瞬间
    /// </summary>
    /// <param name="t">被比较的值</param>
    /// <returns>不等于某个值的瞬间返回True，否则返回False</returns>
    public abstract bool FlashChange(T t);
    /// <summary>
    /// 获取大于某个值的瞬间
    /// </summary>
    /// <param name="t">被比较的值</param>
    /// <returns>大于某个值的瞬间返回True，否则返回False</returns>
    public abstract bool FlashUpper(T t);
    /// <summary>
    /// 获取小于某个值的瞬间
    /// </summary>
    /// <param name="t">被比较的值</param>
    /// <returns>小于某个值的瞬间返回True，否则返回False</returns>
    public abstract bool FlashLower(T t);
    /// <summary>
    /// 获取某个值的状态
    /// </summary>
    /// <param name="flashStatus">状态字典</param>
    /// <param name="value">数值</param>
    /// <param name="defaultStatus">默认状态</param>
    /// <returns>返回某个值的状态</returns>
    public bool GetFlashStatus(Dictionary<T, bool> flashStatus, T value, bool defaultStatus)
    {
        if (flashStatus.ContainsKey(value))
        {
            return flashStatus[value];
        }
        else
        {
            return defaultStatus;
        }
    }
    /// <summary>
    /// 设置某个值的状态
    /// </summary>
    /// <param name="flashStatus">状态字典</param>
    /// <param name="value">数值</param>
    /// <param name="status">状态</param>
    /// <returns></returns>
    public void SetFlashStatus(Dictionary<T, bool> flashStatus, T value, bool status)
    {
        if (flashStatus.ContainsKey(value))
        {
            flashStatus[value] = status;
        }
        else
        {
            flashStatus.Add(value, status);
        }
    }
    /// <summary>
    /// 输出自描述信息
    /// </summary>
    public override string ToString()
    {
        string toString = this.ToString(this.GetType(), "数值", string.Format("Value({0}), ", this.Value));
        //Debug.Log(toString);
        return toString;
    }
}
#endregion
#region IValue
/// <summary>
/// 整数值
/// </summary>
[Serializable]
public sealed class IValue : NValue<int>
{
    /// <summary>
    /// 空值
    /// </summary>
    public static IValue Empty
    {
        get { return new IValue(0); }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="intValue">int型的值</param>
    public IValue(int intValue)
        : base(intValue)
    {
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="lastValue">上一个值</param>
    /// <param name="lockEqual">等于某个值的瞬间</param>
    /// <param name="lockChange">不等于某个值的瞬间</param>
    /// <param name="lockUpper">大于某个值的瞬间</param>
    /// <param name="lockLower">小于某个值的瞬间</param>
    public IValue(int value, int lastValue, Dictionary<int, bool> lockEqual, Dictionary<int, bool> lockChange,
                                            Dictionary<int, bool> lockUpper, Dictionary<int, bool> lockLower)
        : base(value, lastValue, lockEqual, lockChange, lockUpper, lockLower)
    {
    }
    /// <summary>
    /// 获取等于某个值的瞬间
    /// </summary>
    /// <param name="intValue">被比较的值</param>
    /// <returns>等于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashEqual(int intValue)
    {
        if (!this.GetFlashStatus(this.LockEqual, intValue, false) && this.Value == intValue)
        {
            this.SetFlashStatus(this.LockEqual, intValue, true);
            this.SetFlashStatus(this.LockChange, intValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashEqual Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, this.lockChange));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockEqual, intValue, false) && this.Value != intValue)
            {
                this.SetFlashStatus(this.LockEqual, intValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取不等于某个值的瞬间
    /// </summary>
    /// <param name="intValue">被比较的值</param>
    /// <returns>不等于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashChange(int intValue)
    {
        if (!this.GetFlashStatus(this.LockChange, intValue, true) && this.Value != intValue)
        {
            this.SetFlashStatus(this.LockChange, intValue, true);
            this.SetFlashStatus(this.LockEqual, intValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashChange Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, intValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockChange, intValue, true) && this.Value == intValue)
            {
                this.SetFlashStatus(this.LockChange, intValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取大于某个值的瞬间
    /// </summary>
    /// <param name="intValue">被比较的值</param>
    /// <returns>大于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashUpper(int intValue)
    {
        if (!this.GetFlashStatus(this.LockUpper, intValue, false) && this.Value > intValue)
        {
            this.SetFlashStatus(this.LockUpper, intValue, true);
            this.SetFlashStatus(this.LockLower, intValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashUpper Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, intValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockUpper, intValue, false) && this.Value <= intValue)
            {
                this.SetFlashStatus(this.LockUpper, intValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取小于某个值的瞬间
    /// </summary>
    /// <param name="intValue">被比较的值</param>
    /// <returns>小于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashLower(int intValue)
    {
        if (!this.GetFlashStatus(this.LockLower, intValue, false) && this.Value < intValue)
        {
            this.SetFlashStatus(this.LockLower, intValue, true);
            this.SetFlashStatus(this.LockUpper, intValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashLower Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, intValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockLower, intValue, false) && this.Value >= intValue)
            {
                this.SetFlashStatus(this.LockLower, intValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 运算符重载
    /// </summary>
    /// <param name="iValue">复合整数值</param>
    /// <returns>返回普通整数值</returns>
    public static implicit operator int(IValue iValue)
    {
        return iValue.Value;
    }
}
public static class IValueExtend
{
    /// <summary>
    /// 普通浮点值转换成复合浮点值
    /// </summary>
    /// <param name="iValue">复合浮点值</param>
    /// <param name="intValue">普通浮点值</param>
    /// <returns>返回复合浮点值</returns>
    public static IValue Transfer(this IValue iValue, int intValue)
    {
        return new IValue(intValue, iValue.Value, iValue.LockEqual, iValue.LockChange, iValue.LockUpper, iValue.LockLower);
    }
}
#endregion
#region FValue
/// <summary>
/// 浮点值
/// </summary>
[Serializable]
public sealed class FValue : NValue<float>
{
    /// <summary>
    /// 空值
    /// </summary>
    public static FValue Empty
    {
        get { return new FValue(0); }
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="floatValue">float型的值</param>
    public FValue(float floatValue)
        : base(floatValue)
    {
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="lastValue">上一个值</param>
    /// <param name="lockEqual">等于某个值的瞬间</param>
    /// <param name="lockChange">不等于某个值的瞬间</param>
    /// <param name="lockUpper">大于某个值的瞬间</param>
    /// <param name="lockLower">小于某个值的瞬间</param>
    public FValue(float value, float lastValue, Dictionary<float, bool> lockEqual, Dictionary<float, bool> lockChange,
                                                Dictionary<float, bool> lockUpper, Dictionary<float, bool> lockLower)
        : base(value, lastValue, lockEqual, lockChange, lockUpper, lockLower)
    {
    }
    /// <summary>
    /// 获取等于某个值的瞬间
    /// </summary>
    /// <param name="floatValue">被比较的值</param>
    /// <returns>等于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashEqual(float floatValue)
    {
        if (!this.GetFlashStatus(this.LockEqual, floatValue, false) && this.Value == floatValue)
        {
            this.SetFlashStatus(this.LockEqual, floatValue, true);
            this.SetFlashStatus(this.LockChange, floatValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashEqual Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, this.lockChange));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockEqual, floatValue, false) && this.Value != floatValue)
            {
                this.SetFlashStatus(this.LockEqual, floatValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取不等于某个值的瞬间
    /// </summary>
    /// <param name="floatValue">被比较的值</param>
    /// <returns>不等于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashChange(float floatValue)
    {
        if (!this.GetFlashStatus(this.LockChange, floatValue, true) && this.Value != floatValue)
        {
            this.SetFlashStatus(this.LockChange, floatValue, true);
            this.SetFlashStatus(this.LockEqual, floatValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashChange Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, floatValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockChange, floatValue, true) && this.Value == floatValue)
            {
                this.SetFlashStatus(this.LockChange, floatValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取大于某个值的瞬间
    /// </summary>
    /// <param name="floatValue">被比较的值</param>
    /// <returns>大于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashUpper(float floatValue)
    {
        if (!this.GetFlashStatus(this.LockUpper, floatValue, false) && this.Value > floatValue)
        {
            this.SetFlashStatus(this.LockUpper, floatValue, true);
            this.SetFlashStatus(this.LockLower, floatValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashUpper Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, floatValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockUpper, floatValue, false) && this.Value <= floatValue)
            {
                this.SetFlashStatus(this.LockUpper, floatValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 获取小于某个值的瞬间
    /// </summary>
    /// <param name="floatValue">被比较的值</param>
    /// <returns>小于某个值的瞬间返回True，否则返回False</returns>
    public override bool FlashLower(float floatValue)
    {
        if (!this.GetFlashStatus(this.LockLower, floatValue, false) && this.Value < floatValue)
        {
            this.SetFlashStatus(this.LockLower, floatValue, true);
            this.SetFlashStatus(this.LockUpper, floatValue, false);
            //Debug.Log(string.Format("{0} Self({1}) FlashLower Value({2})", DateTime.Now.ToyyyyMMddHHmmssfff(), this.Value, floatValue));
            return true;
        }
        else
        {
            if (this.GetFlashStatus(this.LockLower, floatValue, false) && this.Value >= floatValue)
            {
                this.SetFlashStatus(this.LockLower, floatValue, false);
            }
            return false;
        }
    }
    /// <summary>
    /// 运算符重载
    /// </summary>
    /// <param name="fValue">复合浮点值</param>
    /// <returns>返回普通浮点值</returns>
    public static implicit operator float(FValue fValue)
    {
        return fValue.Value;
    }
}
public static class FValueExtend
{
    /// <summary>
    /// 普通浮点值转换成复合浮点值
    /// </summary>
    /// <param name="fValue">复合浮点值</param>
    /// <param name="floatValue">普通浮点值</param>
    /// <returns>返回复合浮点值</returns>
    public static FValue Transfer(this FValue fValue, float floatValue)
    {
        return new FValue(floatValue, fValue.Value, fValue.LockEqual, fValue.LockChange, fValue.LockUpper, fValue.LockLower);
    }
}
#endregion
#endregion
#region 网络通信相关
/// <summary>
/// 异步通信数据
/// </summary>
public class StateObject
{
    /// <summary>
    /// 缓冲区大小
    /// </summary>
    public const int BUFFER_SIZE = 10240;
    /// <summary>
    /// 套接字通道
    /// </summary>
    public Socket Socket { get; set; }
    /// <summary>
    /// 字节数组
    /// </summary>
    public byte[] Buffer { get; set; }
    /// <summary>
    /// 构造函数
    /// </summary>
    public StateObject()
    {
        this.Buffer = new byte[BUFFER_SIZE];
    }
}
#endregion