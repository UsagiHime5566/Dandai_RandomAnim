using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(SignalServer))]
public class SignalServerHelper : MonoBehaviour
{
    public string fileName = "Server.txt";
    
    
    //Root dictionary of application , without '/'
    string exePath, filePath;
    SignalServer signalServer;

    void Awake(){
        signalServer = GetComponent<SignalServer>();
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
        string _serverPort = reader.ReadLine();
        string _maxUsers = reader.ReadLine();
        string _buffSize = reader.ReadLine();
        string _EndToken = reader.ReadLine();
        reader.Close();

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
    }
}
