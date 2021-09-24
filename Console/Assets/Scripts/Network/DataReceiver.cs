using HLSoft.Framework.Util;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// 远程数据接收器
/// </summary>
public class DataReceiver : MonoBehaviour
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private int localPort;                                          //本机端口号
    [SerializeField]
    private int remotePort;                                         //远程端口号
    public string LocalIP { get; private set; }                     //本机IP地址
    public string RemoteIP { get; private set; }                    //远程IP地址
    public int LocalPort { get { return this.localPort; } }         //本机端口号
    public int RemotePort { get { return this.remotePort; } }       //远程端口号
    public Action<string> ReceiveDataAction;                        //接收数据委托
    public Socket Channel { get; private set; }                     //套接字通道
    private IPEndPoint localEndPoint = null;                        //本地地址端口号
    private EndPoint remoteEndPoint = null;                         //远程地址端口号
    private StateObject stateObject = null;                         //异步通信数据
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
            this.Channel = null;
        }
    }
    /************************************************自 定 义 方 法************************************************/
    //初始化
    void Initialize()
    {
        //初始化网络连接
        this.LocalIP = NetHelper.GetLocalIPv4();
        this.RemoteIP = "255.255.255.255";
        this.localEndPoint = new IPEndPoint(IPAddress.Parse(this.LocalIP), this.localPort);
        this.remoteEndPoint = new IPEndPoint(IPAddress.Parse(this.RemoteIP), this.remotePort);
        //初始化通道
        this.Channel = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this.Channel.Bind(this.localEndPoint);
        this.Channel.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //初始化数据接收
        this.stateObject = new StateObject();
        this.stateObject.Socket = this.Channel;
        this.Channel.BeginReceiveFrom(stateObject.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None,
                                      ref this.remoteEndPoint, new AsyncCallback(this.ReceiveData), stateObject);
    }
    //异步接收数据
    void ReceiveData(IAsyncResult result)
    {
        try
        {
            StateObject so = (StateObject)result.AsyncState;
            Socket socket = so.Socket;
            int dataLength = socket.EndReceiveFrom(result, ref this.remoteEndPoint);
            //if (((IPEndPoint)this.remoteEndPoint).Address.Equals(this.localEndPoint.Address) ||
            //    ((IPEndPoint)this.remoteEndPoint).Address.Equals(IPAddress.Broadcast)) return;
            string receiveData = Encoding.Default.GetString(((StateObject)result.AsyncState).Buffer, 0, dataLength);
            print(string.Format("{0}->receive data: {1}", DateTime.Now.ToyyyyMMddHHmmssfff(), receiveData));
            this.OnReceiveData(receiveData);
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            print(string.Format("接收数据发生错误: {0}", ex.ToString()));
        }
        finally
        {
            if (this.Channel != null)
            {
                this.Channel.BeginReceiveFrom(this.stateObject.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None,
                                              ref this.remoteEndPoint, new AsyncCallback(this.ReceiveData), this.stateObject);
            }
        }
    }
    //解析定位数据(Demo版程序用)
    void OnReceiveData(string dataString)
    {
        if (this.ReceiveDataAction != null)
        {
            this.ReceiveDataAction(dataString);
        }
    }
}
