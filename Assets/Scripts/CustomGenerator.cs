using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using System.Threading.Tasks;

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

        currentTrack = umaRandomer.GenerateRandomCharacter(bornPos, Quaternion.Euler(0, 180, 0), "Generate Avator");
        //RemoveRigidBody(currentTrack);
    }

    public GameObject GetNewUMA(){
        GameObject temp = umaRandomer.GenerateRandomCharacter(bornPos, Quaternion.Euler(0, 180, 0), "Generate Avator");
        //RemoveRigidBody(temp);
        return temp;
    }

    // async void RemoveRigidBody(GameObject avatarObj){
    //     Debug.Break();
	// 	await Task.Delay(100);
	// 	Debug.Log("Try to remove:" + avatarObj.GetComponent<Rigidbody>());
	// 	Destroy(avatarObj.GetComponent<Rigidbody>());
	// 	Destroy(avatarObj.GetComponent<Collider>());
	// }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            StartGenerate();
        }
    }
}
