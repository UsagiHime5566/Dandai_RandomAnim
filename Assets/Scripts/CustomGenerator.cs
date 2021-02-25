using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

public class CustomGenerator : MonoBehaviour
{
    public UMARandomAvatar umaRandomer;

    public GameObject currentTrack;

    public Vector3 bornPos = Vector3.zero;
    void Start()
    {
        //StartGenerate();
    }

    
    void StartGenerate(){
        if(currentTrack != null)
            Destroy(currentTrack);

        currentTrack = umaRandomer.GenerateRandomCharacter(bornPos, Quaternion.identity, "Generate Avator");
    }

    public GameObject GetNewUMA(){
        return  umaRandomer.GenerateRandomCharacter(bornPos, Quaternion.identity, "Generate Avator");
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            StartGenerate();
        }
    }
}
