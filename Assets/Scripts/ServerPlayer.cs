using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SignalServer))]
public class ServerPlayer : MonoBehaviour
{
    [Header("Player Signals")]
    public string RecvSignalToReplay = "End";
    public string SendSignalToPlay = "Play";
    public int delayToReplay = 3000;

    [Header("Runtime Messages")]
    public int totalPlayers = 1;


    public System.Action OnServerStartPlay;

    // private works
    SignalServer signalServer;
    int curPlayers = 0;

    void Awake(){
        signalServer = GetComponent<SignalServer>();
    }

    void Start()
    {
        signalServer.OnSignalReceived.AddListener(SingalRecieved);
        signalServer.OnUserConnected.AddListener(UserConnected);
    }

    void SingalRecieved(string msg){
        if(msg == RecvSignalToReplay){
            curPlayers += 1;
        }

        if(curPlayers >= totalPlayers){
            Debug.Log("All Video Finished. Wait for replay...");

            curPlayers = 0;
            DelayReplay();
        }
    }

    void UserConnected(int onlineUsers){
        if(onlineUsers >= totalPlayers){
            signalServer.SocketSend(SendSignalToPlay);
            OnServerStartPlay?.Invoke();
        }
    }

    async void DelayReplay()
    {
        await Task.Delay(delayToReplay);
        signalServer.SocketSend(SendSignalToPlay);
        OnServerStartPlay?.Invoke();
    }
}
