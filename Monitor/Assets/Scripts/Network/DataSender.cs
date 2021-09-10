using HLSoft.Framework.Util;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// 远程数据发送器
/// </summary>
public class DataSender : MonoBehaviourExtension
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    public int localPort;           //本机端口号
    [SerializeField]
    public int remotePort;          //远程端口号
    [SerializeField]
    public DataReceiver dataReceiver;           //数据接收器
    private IPEndPoint localEndPoint = null;    //本地地址端口号
    private EndPoint remoteEndPoint = null;     //远程地址端口号
    public bool Initialized { get; private set; }                    //是否已初始化
    public Action InitializedAction;             //初始化委托
    public Action<string> SendDataAction;       //发送数据委托
    public Socket Channel { get; private set; } //套接字通道
    /************************************************Unity方法与事件***********************************************/
    void Start()
    {
        this.Initialize();
    }
    void Update()
    {
    }
    void OnDestroy()
    {
        if (this.Channel != null)
        {
            this.Channel.Close();
        }
    }
    /************************************************自 定 义 方 法************************************************/
    //初始化
    private void Initialize()
    {
        //初始化通道
        if (this.dataReceiver == null)
        {
            this.localEndPoint = new IPEndPoint(IPAddress.Parse(NetHelper.GetLocalIPv4()), this.localPort);
            this.remoteEndPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), this.remotePort);
            this.Channel = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Channel.Bind(this.localEndPoint);
            this.Channel.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.OnInitialized();
        }
        else
        {
            this.DelayInvoke(delegate ()
            {
                this.localPort = this.dataReceiver.localPort;
                this.localEndPoint = new IPEndPoint(IPAddress.Parse(this.dataReceiver.LocalIP), this.remotePort);
                this.remotePort = this.dataReceiver.remotePort;
                this.remoteEndPoint = new IPEndPoint(IPAddress.Parse(this.dataReceiver.RemoteIP), this.localPort);
                this.Channel = this.dataReceiver.Channel;
                this.OnInitialized();
            }, 1f);
        }
    }
    //发送数据
    public void SendData(string data)
    {
        if (this.Channel == null) return;
        try
        {
            byte[] sendData = Encoding.Default.GetBytes(data);
            //print(string.Format("{0}, {1}, {2}", data, sendData[0], this.remoteEndPoint));
            this.Channel.SendTo(sendData, sendData.Length, SocketFlags.None, this.remoteEndPoint);
            this.OnSendData(data);
        }
        catch (Exception ex)
        {
            print(string.Format("发送数据发生错误: {0}", ex.ToString()));
        }
    }
    //当发送数据时
    public virtual void OnSendData(string data)
    {
        if (this.SendDataAction != null)
        {
            this.SendDataAction(data);
        }
    }
    //当初始化完成时
    private void OnInitialized()
    {
        this.Initialized = true;
        if (this.InitializedAction != null)
        {
            this.InitializedAction();
        }
    }
}
