using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip bornSound;
    [HideInInspector] public AudioSource compAudio;
    void Awake(){
        instance = this;
        compAudio = GetComponent<AudioSource>();
    }

    public void PlayBornSound(){
        compAudio.PlayOneShot(bornSound);
    }
}
