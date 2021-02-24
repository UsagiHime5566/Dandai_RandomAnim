using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(KinectParam))]
public class KinectParamHelper : MonoBehaviour
{
    KinectParam param;
    public Slider SLD_MovePara;
    public Text TXT_MoveValueText;
    public Button BTN_Quit;

    void Awake(){
        param = GetComponent<KinectParam>();
    }

    void Start()
    {
        SLD_MovePara.SetSavedDataFloat("KinectUnitShift", 1.0f, x => {
            param.unitMoveShift = x;
            TXT_MoveValueText.text = x.ToString("0.00");
        });

        BTN_Quit.onClick.AddListener(QuitApp);
    }

    public void QuitApp(){
        Application.Quit();
    }
}
