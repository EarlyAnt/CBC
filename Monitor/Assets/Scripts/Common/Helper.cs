using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// 数值工具类
/// </summary>
public static class ValueHelperExt
{
    #region GetDirection
    /// <summary>
    /// 获取方向值
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>返回1，0，-1</returns>
    public static int GetDirection(float value)
    {
        if (value == 0)
        {
            return 0;
        }
        else
        {
            return (int)(value / Mathf.Abs(value));
        }
    }
    /// <summary>
    /// 获取方向值
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>返回1，0，-1</returns>
    public static int GetDirection(int value)
    {
        if (value == 0)
        {
            return 0;
        }
        else
        {
            return (value / Mathf.Abs(value));
        }
    }
    #endregion
    #region SplitString
    /// <summary>
    /// 分割字符串
    /// </summary>
    /// <param name="str">原字符串</param>
    /// <param name="separator">分隔符</param>
    /// <returns>分割后的字符串数组为null，或者长度为0，或者长度为1但数组的第1个元素为空，返回null；否则返回字符串数组</returns>
    public static string[] SplitString(string str, char separator)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        else
        {
            string[] result = str.Split(new char[] { separator });
            if (result == null || result.Length == 0 ||
                (result.Length == 1 && string.IsNullOrEmpty(result[0])))
            {
                return null;
            }
            else
            {
                return result;
            }
        }
    }
    #endregion
    #region FormatValue
    /// <summary>
    /// 获取浮点数的整数部分(不考虑进位，即仅舍不入)
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <returns>返回浮点数的整数部分</returns>
    public static int GetInteger(float value)
    {
        string valueString = value.ToString("F1");
        valueString = valueString.Substring(0, valueString.IndexOf("."));
        return int.Parse(valueString);
    }
    #endregion
}
/// <summary>
/// 数学计算工具类
/// </summary>
public static class MathExt
{
    /// <summary>
    /// 角度转换成弧度
    /// </summary>
    /// <param name="angle">角度值</param>
    /// <returns>返回弧度值</returns>
    public static float AToR(float angle)
    {
        return angle * Mathf.PI / 180;
    }
    /// <summary>
    /// 浮点型数据四舍五入
    /// </summary>
    /// <param name="value">浮点型数据</param>
    /// <param name="digits">小数位数</param>
    /// <returns></returns>
    public static float Round(float value, int digits)
    {
        return (float)Math.Round((decimal)value, digits);
    }
    /// <summary>
    /// 返回浮点型数据指定位置的左半部分(不进行四舍五入)
    /// </summary>
    /// <param name="value">浮点型数据</param>
    /// <returns>如返回2.5123的前3位，2.5；或-2.5456的前3位，2.5</returns>
    public static float GetFloatLeft(float value, int digits)
    {
        string valueString = value.ToString();
        if (valueString.Length > digits)
        {
            if (value >= 0)
            {
                return float.Parse(valueString.Substring(0, digits));
            }
            else
            {
                return float.Parse(valueString.Substring(0, digits + 1));
            }
        }
        else
        {
            return value;
        }
    }
    /// <summary>
    /// 返回浮点型数据的指定位置的左半部分(不进行四舍五入)
    /// </summary>
    /// <param name="value">浮点型数据</param>
    /// <returns>如返回2.5123前3位后面的部分，0.0123；或-2.5456前3位后面的部分，0.0456</returns>
    public static float GetFloatRight(float value, int digits)
    {
        string valueString = value.ToString();
        if (valueString.Length > digits)
        {
            if (value >= 0)
            {
                return float.Parse(valueString.Substring(digits, valueString.Length - digits)) / Mathf.Pow(10, (valueString.Length - digits + 1));
            }
            else
            {
                return float.Parse(valueString.Substring(digits + 1, valueString.Length - digits - 1)) / Mathf.Pow(10, (valueString.Length - digits));
            }
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 获取角度与0度的真实偏移值
    /// </summary>
    /// <param name="value">当前角度值</param>
    /// <returns></returns>
    public static float FormatAngle(float value)
    {
        value = Mathf.Abs(value) % 360;
        if (value > 180)
        {
            value = 360 - value;
        }
        return value;
    }
}
/// <summary>
/// 进程工具
/// </summary>
public static class ProcessHelper
{
    /// <summary>
    /// 终止进程(单个进程)
    /// </summary>
    /// <param name="processName">进程名称</param>
    public static void Kill(string processName)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName.Equals(processName))
                process.Kill();
        }
    }
    /// <summary>
    /// 终止进程(一个或多个进程)
    /// </summary>
    /// <param name="processes">进程名称序列</param>
    public static void Kill(params string[] processes)
    {
        foreach (string name in processes)
        {
            Kill(name);
        }
    }
    /// <summary>
    /// 启动程序
    /// </summary>
    /// <param name="programName">程序名称</param>
    public static void Start(string programName)
    {
        if (File.Exists(programName))
        {
            UnityEngine.Debug.Log(programName);
            Process process = new Process();
            process.StartInfo.FileName = programName;
            process.Start();
        }
    }
    /// <summary>
    /// 启动程序
    /// </summary>
    /// <param name="programName">程序名称</param>
    /// <param name="style">窗口状态类型(普通，最大化，最小化，隐藏)</param>
    public static void Start(string programName, ProcessWindowStyle style)
    {
        if (File.Exists(programName))
        {
            UnityEngine.Debug.Log(programName);
            Process process = new Process();
            process.StartInfo.FileName = programName;
            process.StartInfo.WindowStyle = style;
            process.Start();
        }
    }
}
/// <summary>
/// Unity工具
/// </summary>
public static class UnityHelper
{    
    /// <summary>
    /// 获取声音播放器
    /// </summary>
    /// <param name="channelName">声道的名字</param>
    /// <returns>返回声音播放器或者null</returns>
    public static AudioSource GetAudioPlayer(string channelName)
    {
        //GameObject player = GameObject.Find("PlayerFactory");
        //if (player != null)
        //{
        //    return player.GetComponent<PlayerFactory>().GetPlayer(channelName);
        //}
        //else
        //{
        //    return null;
        //}
        return null;
    }
    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="player">播放器</param>
    /// <param name="audioClip">声音片段</param>
    public static void PlayAudioClip(AudioSource player, AudioClip audioClip)
    {
        if (player != null && audioClip != null)
        {
            player.PlayOneShot(audioClip);
        }
    }
    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="player">播放器</param>
    /// <param name="audioClip">声音片段</param>
    /// <param name="loop">是否循环</param>
    public static void PlayAudio(AudioSource player, AudioClip audioClip, bool loop)
    {
        PlayAudio(player, audioClip, loop, false);
    }
    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="player">播放器</param>
    /// <param name="audioClip">声音片段</param>
    /// <param name="loop">是否循环</param>
    /// <param name="interrupt">是否中断上一个声音片段</param>
    public static void PlayAudio(AudioSource player, AudioClip audioClip, bool loop, bool interrupt)
    {
        if (player != null && audioClip != null && (!player.isPlaying || interrupt))
        {
            player.clip = audioClip;
            player.loop = loop;
            player.Play();
        }
    }
    /// <summary>
    /// 停止播放声音
    /// </summary>
    /// <param name="player">播放器</param>
    public static void StopAudio(AudioSource player)
    {
        if (player != null)
        {
            player.Stop();
        }
    }
    /// <summary>
    /// 停止播放声音
    /// </summary>
    /// <param name="channelName">声道的名字</param>
    public static void StopAudio(string channelName)
    {
        //GameObject playerFactory = GameObject.Find("PlayerFactory");
        //if (playerFactory != null)
        //{
        //    AudioSource player = playerFactory.GetComponent<PlayerFactory>().GetPlayer(channelName);
        //    StopAudio(player);
        //}
    }
}
/// <summary>
/// 数据工具
/// </summary>
public static class ByteHelper
{
    /// <summary>
    /// 十进制数据转为十六进制(字节表示)
    /// </summary>
    /// <param name="number">十进制数</param>
    /// <param name="length">字节长度</param>
    /// <returns>返回十六进制数的字节数组</returns>
    public static byte[] NumberToBytes(int number, int length)
    {
        byte[] result = new byte[length];
        string numberString = number.ToString("X" + length * 2);
        for (int i = 0; i < length; i++)
        {
            result[i] = (byte)Int32.Parse(numberString.Substring(i * 2, 2),
                                          System.Globalization.NumberStyles.HexNumber);
        }
        return result;
    }
    /// <summary>
    ///  十进制数据转为十六进制(字节表示)
    /// </summary>
    /// <param name="number">十进制数</param>
    /// <param name="length">字节长度</param>
    /// <returns>返回十六进制数的字节数组</returns>
    public static byte[] NumberToBytes(double number, int length)
    {
        int numberInt = int.Parse(number.ToString("F0"));
        byte[] result = NumberToBytes(numberInt, length);
        return result;
    }
}
public static class SetWindowFull
{
    [DllImport("WindowDummyFullScreen")]
    public static extern void DummyWindow();
    [DllImport("WindowDummyFullScreen")]
    public static extern void ShowTaskBar();

    [DllImport("WindowDummyFullScreen")]
    public static extern void HideTaskBar();
}
public static class SetWindowNum
{

    [DllImport("WindowDummyFullScreen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void DummyWindow(Int32 x, Int32 y, Int32 w, Int32 h);
    [DllImport("WindowDummyFullScreen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void ShowTaskBar();
    [DllImport("WindowDummyFullScreen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void HideTaskBar();
}
public static class WindowsSetDLL
{

    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    [DllImport("user32.dll")]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr Handle);
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr HWND, int MSG);
    //[DllImport("ChangeDisplaySet")]
    //public static extern bool ChangeWindowsStyle(string ExeName, int ChangeType, int Screen_Width, int Screen_Height);
    ////调用方法为 ChangeWindowsStyle(项目名ProductName中的字符串,改变的类型整型,窗口宽度整型,窗口高度整型);
    ////窗口类型可用值为1,2,3.
    ////1假全屏,允许其他Exe窗口置前显示
    ////2带窗口边框,但是允许拖拽调整窗口尺寸.
    ////3非全屏,无边框.

    ////更改样式失败返回假,成功返回真

    ////例如：ChangeWindowsStyle("MyTestApp",3,800,600);

    ////该DLL中还包含一个设置当前系统分辨率的方法.
    ////public static function ChangeDisplaySet(DisWidth : int; DisHeight : int; DisSave : DisSave) : boolean{};
    ////前两个参数代表要设置系统分辨率的宽高,后面参数可选值为1和4,为1时永久更改屏幕分辨率.为4时为临时更改分辨率.
    [DllImport("user32.dll", EntryPoint = "GetWindowText")]
    public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

}
