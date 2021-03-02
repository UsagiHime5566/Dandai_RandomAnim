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
    public UserConnectedEvent OnUserConnected;
    public Action<string> OnServerLogs;

    [Header("Auto Work")]
    public bool runInStart = false;

    // Private works
    Socket serverSocket; //服務器端socket  
    Socket [] clientSockets; //客戶端socket  
    IPEndPoint ipEnd; //偵聽端口  
    string [] token;
    Thread [] connectThread; //連接線程
    Action ActionQueue;

    // Use this for initialization
    void Start()
    {
        token = new string[]{ EndToken };
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

        ClearThreads();
        ClearClients();
        for (int i = 0; i < maxUsers; i++)
        {
            //開啟一個線程連接，必須的，否則主線程卡死
            int targetIndex = i;
            connectThread[targetIndex] = new Thread(() => ServerWork(targetIndex));
            connectThread[targetIndex].Start();
        }
        
        DebugLog($"Start Server at :{serverPort} , with {maxUsers} thread.");
    }

    void ServerWork(int index)
    {
        //連接
        Socket currSocket = SocketConnet(index);
        //進入接收循環  
        while (true)
        {
            byte[] recvData = new byte[recvBufferSize];
            int recvLen = 0;
            string recvStr;
            try
            {
                //獲取收到的數據的長度  
                recvLen = currSocket.Receive(recvData);
            }
            catch (System.Net.Sockets.SocketException)
            {
                SocketConnet(index);
                continue;
            }
            //如果收到的數據長度為0，則重連並進入下一個循環  
            if (recvLen == 0)
            {
                SocketConnet(index);
                continue;
            }
            //輸出接收到的數據  
            recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);

            //N[/TCP]
            //Debug.Log($"TCP >> Raw msg : {recvStr}");

            //Recieve Data Will Be   245,135,90[/TCP]   , str 不會包含[/TCP]
            string[] clearString = recvStr.Split(token, StringSplitOptions.None);  // => N , [/TCP]

            if (clearString.Length > 1)
            {
                DebugLog($"TCP >> Recieved : {clearString[0]}");

                ActionQueue += delegate {
                    OnSignalReceived.Invoke(clearString[0]);
                };
            } // end Length

        }  // end While
    }

    Socket SocketConnet(int index)
    {
        if (clientSockets[index] != null)
            clientSockets[index].Close();

        //控制台輸出偵聽狀態
        DebugLog($"Waiting for a client ({index})");
        //一旦接受連接，創建一個客戶端  
        clientSockets[index] = serverSocket.Accept();
        //獲取客戶端的IP和端口  
        IPEndPoint ipEndClient = (IPEndPoint)clientSockets[index].RemoteEndPoint;
        //輸出客戶端的IP和端口  
        DebugLog($"Thread ({index}) Connect with " + ipEndClient.Address.ToString() + ":" + ipEndClient.Port.ToString());

        //連接成功則發送數據  
        //sendStr="Welcome to my server";
        //SocketSend(sendStr);

        ActionQueue += delegate {
            OnUserConnected?.Invoke(GetCurrentClientsNum());
        };

        return clientSockets[index];
    }

    int GetCurrentClientsNum(){
        int onlineUsers = 0;
        foreach (var clientSocket in clientSockets)
        {
            if(clientSocket == null)
                continue;

            if(clientSocket.Connected)
                onlineUsers += 1;
        }

        return onlineUsers;
    }

    //Data to Glass can use UTF8
    public void SocketSend(string sendStr)
    {
        int totalSend = 0;
        foreach (var clientSocket in clientSockets)
        {
            if (clientSocket == null)
                continue;
            if (clientSocket.Connected == false)
                continue;
            try {
                var toSend = sendStr + EndToken;
                //清空發送緩存  
                var sendData = new byte[1024];
                //數據類型轉換  
                sendData = Encoding.UTF8.GetBytes(toSend);
                //發送  
                clientSocket.Send(sendData, sendData.Length, SocketFlags.None);

                DebugLog ($"TCP >> Send: {toSend}");

                totalSend += 1;
            }
            catch(System.Exception e){
                DebugLogError(e.Message.ToString());
            }
        }

        DebugLog ($"TCP >> Total Send {totalSend} clients. ({System.DateTime.Now})");
    }

    void ClearThreads(){
        if(connectThread != null){
            foreach (var item in connectThread)
            {
                if(item != null){
                    item.Interrupt();
                    item.Abort();
                }
                
            }
        }
        connectThread = new Thread[maxUsers];
    }

    void ClearClients(){
        if(clientSockets != null){
            foreach (var item in clientSockets)
            {
                if(item != null){
                    item.Close();
                }
            }
        }
        clientSockets = new Socket[maxUsers];
    }

    void CloseSocket()
    {
        //先關閉客戶端  
        ClearClients();
        
        //再關閉線程  
        ClearThreads();
        //最後關閉服務器
        if (serverSocket != null)
        {
            serverSocket.Close();
            DebugLog("diconnect.");
        }
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void DebugLog(string msg){
        Debug.Log(msg);
        OnServerLogs?.Invoke(msg);
    }

    void DebugLogError(string msg){
        Debug.LogError(msg);
        OnServerLogs?.Invoke(msg);
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

    [Serializable] public class SocketSignalEvent : UnityEvent<string> {}

    [Serializable] public class UserConnectedEvent : UnityEvent<int> {}
}
