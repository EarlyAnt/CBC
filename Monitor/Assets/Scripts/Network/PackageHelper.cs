using EarlyAnt.Framework.Util;
using System;
using System.Collections.Generic;


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
/// 游戏事件枚举
/// </summary>
public enum GameEvents
{
    Start = 0,
    Pause = 1,
    End = 2
}

/// <summary>
/// 网络数据类型
/// </summary>
public static class NetDataTags
{
    public const string HEARTBEAT = "heartbeat";
    public const string EVENT = "event";
    public const string AUDIO = "audio";
    public const string ANIMATION = "animation";
    public const string AVATAR = "avatar";
    public const string HEALTH = "health";
    public const string HURT = "hurt";
    public const string CARD = "card";
    public const string WEAK = "weak";
    public const string AID = "aid";
    public const string EFFECT = "effect";
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
/// 心跳数据
/// </summary>
public class Heartbeat
{
    public GameEvents GameEvent { get; set; }
    public int LeftSeconds { get; set; }

    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}, {2}", this.GetType().Name, this.GameEvent, this.LeftSeconds);
    }
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
/// 事件数据
/// </summary>
public class EventData : CommandData
{
    public string GameEventString { get; set; }
    public object Parameter { get; set; }

    public GameEvents GameEvent
    {
        get
        {
            GameEvents gameEvent = GameEvents.End;
            if (!string.IsNullOrEmpty(this.GameEventString))
            {
                switch (this.GameEventString.ToUpper())
                {
                    case "GAMEEVENT.START":
                        gameEvent = GameEvents.Start;
                        break;
                    case "GAMEEVENT.PAUSE":
                        gameEvent = GameEvents.Pause;
                        break;
                    case "GAMEEVENT.END":
                        gameEvent = GameEvents.End;
                        break;
                }
            }
            return gameEvent;
        }
    }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.GameEventString);
    }
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
/// 英雄伤害区
/// </summary>
public class HurtData : CommandData
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
/// 衰弱
/// </summary>
public class WeakData : CommandData
{
    public bool Light { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.Light);
    }
}

/// <summary>
/// 援助
/// </summary>
public class AidData : CommandData
{
    public bool Light { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.Light);
    }
}

/// <summary>
/// 附属效果
/// </summary>
public class EffectsData : CommandData
{
    public bool Light { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}", this.GetType().Name, this.Light);
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
/// 头像名字数据
/// </summary>
public class AvatarNameData : CommandData
{
    public String Name { get; set; }
    public string AvatarUrl { get; set; }
    /// <summary>
    /// 数据转成字符串形式
    /// </summary>
    /// <returns>返回自描述信息</returns>
    public override string ToString()
    {
        return string.Format("{0}: {1}, {2}", this.GetType().Name, this.Name, this.AvatarUrl);
    }
}

/// <summary>
/// 玩家信息
/// </summary>
public class PlayerInfo
{
    public string LeftName { get; set; }
    public string RightName { get; set; }
    public string LeftAvatar { get; set; }
    public string RightAvatar { get; set; }
}
#endregion

