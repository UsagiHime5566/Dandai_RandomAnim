using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public int removeTime = 5;
    public float removeThreshold = 0.0001f;

    Transform anChild;

    int stackIndex = 0;

    Vector3 lastPoint = Vector3.zero;


    async void Start()
    {
        await Task.Delay(1000);

        if(this == null)
            return;
        
        anChild = this.gameObject.transform.GetChild(0);

        if(anChild != null){
            StartCoroutine(CheckRemove());
        }
    }

    WaitForSeconds wait = new WaitForSeconds(1);
    IEnumerator CheckRemove(){
        while(true){
            
            float distance = Vector3.Distance(lastPoint, anChild.position);
            if(distance < removeThreshold){
                stackIndex += 1;
                Debug.Log($"distance min {distance}");
            } else {
                stackIndex = 0;
            }

            if(stackIndex >= removeTime){
                Destroy(gameObject);
            }

            lastPoint = anChild.position;

            yield return wait;
        }
    }
}
