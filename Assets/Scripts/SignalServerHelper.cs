using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(SignalServer))]
[RequireComponent(typeof(ServerPlayer))]
public class SignalServerHelper : MonoBehaviour
{
    [HimeLib.HelpBox] public string tip = "負責讀入txt, 藉此設定Server 相關設定";
    public string fileName = "Server.txt";
    
    //Root dictionary of application , without '/'
    string exePath, filePath;
    SignalServer signalServer;
    ServerPlayer serverPlayer;

    void Awake(){
        signalServer = GetComponent<SignalServer>();
        serverPlayer = GetComponent<ServerPlayer>();
        exePath = Path.GetDirectoryName(Application.dataPath);
        filePath = exePath + "/" + fileName;
    }

    void Start()
    {
        ReadFileForServer();
    }

    void ReadFileForServer(){
        if(!File.Exists(filePath))
            return;

        StreamReader reader = new StreamReader(filePath);
        string _totalPlayer = reader.ReadLine(); 
        string _serverPort = reader.ReadLine();
        string _maxUsers = reader.ReadLine();
        string _buffSize = reader.ReadLine();
        string _EndToken = reader.ReadLine();
        reader.Close();

        int totalPlayer = 0;
        if(int.TryParse(_totalPlayer, out totalPlayer))
            serverPlayer.totalPlayers = totalPlayer;
        int serverPort = 25566;
        if(int.TryParse(_serverPort, out serverPort))
            signalServer.serverPort = serverPort;
        int maxUsers = 10;
        if(int.TryParse(_maxUsers, out maxUsers))
            signalServer.maxUsers = maxUsers;
        int buffSize = 1024;
        if(int.TryParse(_buffSize, out buffSize))
            signalServer.recvBufferSize = buffSize;

        if(!string.IsNullOrEmpty(_EndToken))
            signalServer.EndToken = _EndToken;

        signalServer.InitSocket();
    }
}
