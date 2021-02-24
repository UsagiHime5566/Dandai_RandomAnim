using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectParam : MonoBehaviour
{
    public static KinectParam instance;
    public float unitMoveShift = 1.0f;

    void Awake(){
        instance = this;
    }
}
