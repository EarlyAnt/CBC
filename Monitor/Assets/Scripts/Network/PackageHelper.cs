using HLSoft.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;
//using LitJson;

/// <summary>
/// 数据封包解包工具
/// </summary>
static class PackageHelper
{
    /************************************************属性与变量命名************************************************/
    /// <summary>
    /// 数据集合
    /// </summary>
    public static Dictionary<string, object> DataList { get; private set; }
    /// <summary>
    /// 小数位数
    /// </summary>
    public static int Degree { get; set; }
    /************************************************构  造  函  数************************************************/
    static PackageHelper()
    {
        Degree = 10;
    }
    /************************************************自 定 义 方 法************************************************/
    /// <summary>
    /// 数据封包
    /// </summary>
    /// <param name="netData">灯数据</param>
    /// <returns></returns>
    public static string Package(NetData netData)
    {
        ValidateHelper.ValidateNull(netData);
        string dataString = JsonUtil.Json2String(netData);
        return dataString;
    }
    /// <summary>
    /// 数据解包
    /// </summary>
    /// <param name="netDataString">NetData数据的字符串形式</param>
    /// <returns>返回NetData数据实例</returns>
    public static NetData DataToNetData(string netDataString)
    {
        if (!string.IsNullOrEmpty(netDataString))
        {
            NetData netData = JsonUtil.String2Json<NetData>(netDataString);
            return netData;
        }
        else
        {
            return NetData.Empty;
        }
    }
}

#region 网络数据定义
/// <summary>
/// 网络数据类型
/// </summary>
public static class NetDataTags
{
    public const string AVATAR = "avatar";
    public const string AUDIO = "audio";
    public const string ANIMATION = "animation";
    public const string HEALTH = "health";
    public const string CARD = "card";
}

public static class DataOwners
{
    public const string LEFT = "left";
    public const string RIGHT = "right";
}

/// <summary>
/// 网络数据
/// </summary>
public class NetData
{
    public string Tag { get; set; }
    public object Data { get; set; }
    public static NetData Empty { get { return new NetData(); } }
}

/// <summary>
/// 数据基类
/// </summary>
public abstract class BaseData
{
}

/// <summary>
/// 效果数据基类
/// </summary>
public abstract class EffectData : BaseData
{
    public string EffectFile { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.EffectFile);
    }
}

/// <summary>
/// 音频效果
/// </summary>
public class AudioEffect : EffectData
{
}

/// <summary>
/// 动画效果
/// </summary>
public class AnimationEffect : EffectData
{
}

/// <summary>
/// 命令数据基类
/// </summary>
public class CommandData : BaseData
{
    public string DataOwner { get; set; }
}

/// <summary>
/// 英雄体力(战力)
/// </summary>
public class HealthData : CommandData
{
    public int Value { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.Value);
    }
}

/// <summary>
/// 卡牌库存
/// </summary>
public class CardData : CommandData
{
    public int Value { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.Value);
    }
}

/// <summary>
/// 头像数据
/// </summary>
public class AvatarData : CommandData
{
    public string DataString { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.DataString);
    }
}
#endregion

