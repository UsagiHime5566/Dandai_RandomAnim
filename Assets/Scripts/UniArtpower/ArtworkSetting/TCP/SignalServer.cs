using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class SignalServer : HimeLib.SingletonMono<SignalServer>
{
    public int serverPort = 25568;
    public int maxUsers = 10;
    public int recvBufferSize = 1024;
    public string EndToken = "[/TCP]";

    [HimeLib.HelpBox] public string tip = "所有的訊息接編碼為UTF-8";
    public SocketSignalEvent OnSignalReceived;

    [Header("Auto Work")]
    public bool runInStart = false;

    // Private works
    Socket serverSocket; //服務器端socket  
    Socket clientSocket; //客戶端socket  
    IPEndPoint ipEnd; //偵聽端口  
    string recvStr; //接收的字符串
    string sendStr; //發送的字符串
    byte[] recvData = new byte[1024]; //接收的數據，必須為字節  
    byte[] sendData = new byte[1024]; //發送的數據，必須為字節  
    int recvLen; //接收的數據長度
    string [] token;
    Thread connectThread; //連接線程
    Action ActionQueue;

    // Use this for initialization
    async void Start()
    {
        token = new string[]{ EndToken };

        await Task.Delay(1000);

        if(this == null) return;

        if(!runInStart) return;

        InitSocket();
    }

    void Update(){
        if(ActionQueue != null){
            ActionQueue?.Invoke();
            ActionQueue = null;
        }
    }

    public void InitSocket()
    {
        //定義偵聽端口,偵聽任何IP  
        ipEnd = new IPEndPoint(IPAddress.Any, serverPort);
        //定義套接字類型,在主線程中定義
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(ipEnd);
        //開始偵聽,最大10個連接  
        serverSocket.Listen(maxUsers);

        //開啟一個線程連接，必須的，否則主線程卡死  
        connectThread = new Thread(ServerWork);
        connectThread.Start();

        Debug.Log("Start Server at :" + serverPort);
    }

    void ServerWork()
    {
        //連接
        SocketConnet();
        //進入接收循環  
        while (true)
        {
            //對data清零  
            recvData = new byte[recvBufferSize];
            try
            {
                //獲取收到的數據的長度  
                recvLen = clientSocket.Receive(recvData);
            }
            catch (System.Net.Sockets.SocketException)
            {
                SocketConnet();
                continue;
            }
            //如果收到的數據長度為0，則重連並進入下一個循環  
            if (recvLen == 0)
            {
                SocketConnet();
                continue;
            }
            //輸出接收到的數據  
            recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);

            //N[/TCP]
            //Debug.Log(recvStr);

            //Recieve Data Will Be   245,135,90[/TCP]   , str 不會包含[/TCP]
            string[] clearString = recvStr.Split(token, StringSplitOptions.None);  // => N , [/TCP]

            if (clearString.Length > 1)
            {
                Debug.Log($"TCP >> Recieved : {clearString[0]}");

                ActionQueue += delegate {
                    OnSignalReceived.Invoke(clearString[0]);
                };
            } // end Length

        }  // end While
    }

    void SocketConnet()
    {
        if (clientSocket != null)
            clientSocket.Close();
        //控制台輸出偵聽狀態
        print("Waiting for a client");
        //一旦接受連接，創建一個客戶端  
        clientSocket = serverSocket.Accept();
        //獲取客戶端的IP和端口  
        IPEndPoint ipEndClient = (IPEndPoint)clientSocket.RemoteEndPoint;
        //輸出客戶端的IP和端口  
        Debug.Log("Connect with " + ipEndClient.Address.ToString() + ":" + ipEndClient.Port.ToString());

        //連接成功則發送數據  
        //sendStr="Welcome to my server";
        //SocketSend(sendStr);  
    }

    //Data to Glass can use UTF8
    public void SocketSend(string sendStr)
    {
        if (clientSocket == null)
            return;
        if (clientSocket.Connected == false)
            return;
        try {
            sendStr = sendStr + EndToken;
            //清空發送緩存  
            sendData = new byte[1024];
            //數據類型轉換  
            sendData = Encoding.UTF8.GetBytes(sendStr);
            //發送  
            clientSocket.Send(sendData, sendData.Length, SocketFlags.None);

            Debug.Log ($"TCP >> Send: {sendStr}");
        }
        catch(System.Exception e){
            Debug.LogError(e.Message.ToString());
        }
    }

    void CloseSocket()
    {
        //先關閉客戶端  
        if (clientSocket != null)
            clientSocket.Close();
        //再關閉線程  
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉服務器
        if (serverSocket != null)
        {
            serverSocket.Close();
            print("diconnect.");
        }
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    [Header("Signal Emulor")]
    public string signalForRecieved = "";
    public string signalForSend = "";
    [EasyButtons.Button] void EmuSignalRecieve(){
        OnSignalReceived?.Invoke(signalForRecieved);
    }
    [EasyButtons.Button] void EmuSignalSend(){
        SocketSend(signalForSend);
    }

    [Serializable]
    public class SocketSignalEvent : UnityEvent<string>
    {}
}
