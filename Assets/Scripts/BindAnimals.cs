using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindAnimals : MonoBehaviour
{
    public GameObject animal;

    public Transform ModelBody;
    public Transform ModelHandRight;
    public Transform ModelHandLeft;
    public Transform ModelLegRight;
    public Transform ModelLegLeft;
    

    public Transform AnimalBody;
    public Transform AnimalHandRight;
    public Transform AnimalHandLeft;
    public Transform AnimalLegRight;
    public Transform AnimalLegLeft;
    public Transform AnimalLegRight2;
    public Transform AnimalLegLeft2;
    
    
    List<Quaternion> originModelRotate;
    List<Quaternion> originPrefabRotate;
    public List<Vector3> originLocalPosition;

    public List<GameObject> savedLocalPosition;
    

    public List<string> prefabElement = new List<string>(){
        "Body",
        "Hand1", //Right
        "Hand2",
        "Leg1", //Right
        "Leg2",
        "Leg11",
        "Leg22"
    };

    void Start()
    {
        ModelBody = transform.FindDeepChildCustom("joint_TorsoA");
        ModelHandRight = transform.FindDeepChildCustom("joint_ShoulderRT");
        ModelHandLeft = transform.FindDeepChildCustom("joint_ShoulderLT");
        ModelLegRight = transform.FindDeepChildCustom("joint_HipRT");
        ModelLegLeft = transform.FindDeepChildCustom("joint_HipLT");

        originModelRotate = new List<Quaternion>();
        originModelRotate.Add(ModelBody.localRotation);
        originModelRotate.Add(ModelHandRight.localRotation);
        originModelRotate.Add(ModelHandLeft.localRotation);
        originModelRotate.Add(ModelLegRight.localRotation);
        originModelRotate.Add(ModelLegLeft.localRotation);

        GameObject prefab = animal;
        //GameObject prefab = ZoomCenter.instance.AnimalTypes[2];
        animal = Instantiate(prefab, transform);
        animal.transform.Translate(0, 0.25f, 0);

        AnimalBody = animal.transform.Find("Body");
        AnimalHandRight = animal.transform.Find("Hand1");
        AnimalHandLeft = animal.transform.Find("Hand2");
        AnimalLegRight = animal.transform.Find("Leg1");
        AnimalLegLeft = animal.transform.Find("Leg2");
        AnimalLegRight2 = animal.transform.Find("Leg11");
        AnimalLegLeft2 = animal.transform.Find("Leg22");

        originPrefabRotate = new List<Quaternion>();
        originPrefabRotate.Add(AnimalBody.localRotation);
        originPrefabRotate.Add(AnimalHandRight.localRotation);
        originPrefabRotate.Add(AnimalHandLeft.localRotation);
        originPrefabRotate.Add(AnimalLegRight.localRotation);
        originPrefabRotate.Add(AnimalLegLeft.localRotation);

        if(AnimalLegRight2 != null)
            originPrefabRotate.Add(AnimalLegRight2.localRotation);
        if(AnimalLegLeft2 != null)
            originPrefabRotate.Add(AnimalLegLeft2.localRotation);
        
        AnimalBody.SetParent(ModelBody);
        AnimalHandRight.SetParent(ModelHandLeft);
        AnimalHandLeft.SetParent(ModelHandRight);
        AnimalLegRight.SetParent(ModelLegLeft);
        AnimalLegLeft.SetParent(ModelLegRight);
        if(AnimalLegRight2 != null)
            AnimalLegRight2.SetParent(ModelLegLeft);
        if(AnimalLegLeft2 != null)
            AnimalLegLeft2.SetParent(ModelLegRight);

        savedLocalPosition = new List<GameObject>();
        savedLocalPosition.Add(CreateRefObject(AnimalBody, ModelBody));
        savedLocalPosition.Add(CreateRefObject(AnimalHandRight, ModelHandLeft));
        savedLocalPosition.Add(CreateRefObject(AnimalHandLeft, ModelHandRight));
        savedLocalPosition.Add(CreateRefObject(AnimalLegRight, ModelLegLeft));
        savedLocalPosition.Add(CreateRefObject(AnimalLegLeft, ModelLegRight));
        if(AnimalLegRight2 != null)
            savedLocalPosition.Add(CreateRefObject(AnimalLegRight2, ModelLegLeft));
        if(AnimalLegLeft2 != null)
            savedLocalPosition.Add(CreateRefObject(AnimalLegLeft2, ModelLegRight));


        originLocalPosition = new List<Vector3>();
        originLocalPosition.Add(AnimalBody.localPosition);
        originLocalPosition.Add(AnimalHandRight.localPosition);
        originLocalPosition.Add(AnimalHandLeft.localPosition);
        originLocalPosition.Add(AnimalLegRight.localPosition);
        originLocalPosition.Add(AnimalLegLeft.localPosition);

        if(AnimalLegRight2 != null)
            originLocalPosition.Add(AnimalLegRight2.localPosition);
        if(AnimalLegLeft2 != null)
            originLocalPosition.Add(AnimalLegLeft2.localPosition);
        
        
    }

    GameObject CreateRefObject(Transform source, Transform target){
        GameObject temp = new GameObject(source.name + "_Ref");
        temp.transform.position = source.transform.position;
        temp.transform.SetParent(target.parent);
        return temp;
    }

    // Update is called once per frame
    void Update()
    {
        //return;
        AnimalBody.position = savedLocalPosition[0].transform.position;
        AnimalHandRight.position = savedLocalPosition[1].transform.position;
        AnimalHandLeft.position = savedLocalPosition[2].transform.position;
        AnimalLegRight.position = savedLocalPosition[3].transform.position;
        AnimalLegLeft.position = savedLocalPosition[4].transform.position;
        if(AnimalLegRight2 != null)
            AnimalLegRight2.position = savedLocalPosition[5].transform.position;
        if(AnimalLegLeft2 != null)
            AnimalLegLeft2.position = savedLocalPosition[6].transform.position;

        //Debug.Log(">> " + transform.position);


        //AnimalBody.localRotation = originPrefabRotate[0] * (ModelBody.localRotation * Quaternion.Inverse(originModelRotate[0]));
        //AnimalHandLeft.localRotation = originPrefabRotate[1] * (ModelHandLeft.localRotation * Quaternion.Inverse(originModelRotate[1]));
        //AnimalHandRight.localRotation = originPrefabRotate[2] * (ModelHandRight.localRotation * Quaternion.Inverse(originModelRotate[2]));
        //AnimalLegLeft.localRotation = originPrefabRotate[3] * (ModelLegLeft.localRotation * Quaternion.Inverse(originModelRotate[3]));
        //AnimalLegRight.localRotation = originPrefabRotate[4] * (ModelLegRight.localRotation * Quaternion.Inverse(originModelRotate[4]));

        //if(AnimalLegLeft2 != null)
        //    AnimalLegLeft2.localRotation = originPrefabRotate[5] * (ModelLegLeft.localRotation * Quaternion.Inverse(originModelRotate[3]));
        //if(AnimalLegRight2 != null)
        //    AnimalLegRight2.localRotation = originPrefabRotate[6] * (ModelLegRight.localRotation * Quaternion.Inverse(originModelRotate[4]));

        //AnimalBody.localRotation =  Quaternion.Euler(originPrefabRotate[0].eulerAngles + (ModelBody.localRotation.eulerAngles - originModelRotate[0].eulerAngles));
        //AnimalHandLeft.localRotation =  Quaternion.Euler(originPrefabRotate[1].eulerAngles + (ModelHandLeft.localRotation.eulerAngles - originModelRotate[1].eulerAngles));
        //AnimalHandRight.localRotation =  Quaternion.Euler(originPrefabRotate[2].eulerAngles + (ModelHandRight.localRotation.eulerAngles - originModelRotate[2].eulerAngles));
        //AnimalLegLeft.localRotation =  Quaternion.Euler(originPrefabRotate[3].eulerAngles + (ModelLegLeft.localRotation.eulerAngles - originModelRotate[3].eulerAngles));
        //AnimalLegRight.localRotation =  Quaternion.Euler(originPrefabRotate[4].eulerAngles + (ModelLegRight.localRotation.eulerAngles - originModelRotate[4].eulerAngles));

        //if(AnimalLegLeft2 != null)
        //    AnimalLegLeft2.localRotation =  Quaternion.Euler(originPrefabRotate[5].eulerAngles + (ModelLegLeft.localRotation.eulerAngles - originModelRotate[3].eulerAngles));
        //if(AnimalLegRight2 != null)
        //    AnimalLegRight2.localRotation =  Quaternion.Euler(originPrefabRotate[6].eulerAngles + (ModelLegRight.localRotation.eulerAngles - originModelRotate[4].eulerAngles));
        
        //AnimalBody.localRotation = ModelBody.localRotation;
        //AnimalHandRight.localRotation = ModelHandLeft.localRotation;
        //AnimalHandLeft.localRotation = ModelHandRight.localRotation;
        //AnimalLegRight.localRotation = ModelLegLeft.localRotation;
        //AnimalLegLeft.localRotation = ModelLegRight.localRotation;
        //if(AnimalLegRight2 != null)
        //    AnimalLegRight2.localRotation = ModelLegLeft.localRotation;
        //if(AnimalLegLeft2 != null)
        //    AnimalLegLeft2.localRotation = ModelLegRight.localRotation;
    
    }
}


public static class ExtentTransformCustom {
    public static Transform FindDeepChildCustom(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;

            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}