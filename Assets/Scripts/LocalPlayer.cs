using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LocalPlayer : MonoBehaviour
{
    public ServerPlayer serverPlayer;
    public VideoPlayer videoPlayer;
    void Start()
    {
        serverPlayer.OnServerStartPlay += PlayLocalPlay;
    }

    void PlayLocalPlay(){
        videoPlayer.time = 0;
        videoPlayer.Play();
    }
}
