//Only Project setting API to .Net 4.X Can Use Serial Port
#if NET_4_6
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ArduinoInteractive))]
public class SerialHelper : MonoBehaviour
{
    [HimeLib.HelpBox] public string tip = "10秒後 自動啟動Arduino";
    public Dropdown DD_Speed;
    public Dropdown DD_Serial;
    public Button BTN_Restart;
    public Text TXT_State;

    [Header("模擬測試")]
    public string messageToRecieve;

    [EasyButtons.Button("Emu Recieve")]
    void EmuRecieved(){
        arduinoInteractive.OnRecieveData?.Invoke(messageToRecieve);
    }

    ArduinoInteractive arduinoInteractive;
    string[] baudRate = {"300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "28800", "31250", "38400", "57600", "115200"};

    void Awake(){
        arduinoInteractive = GetComponent<ArduinoInteractive>();
    }

    IEnumerator Start()
    {
        DD_Speed.SetLabels(baudRate);
        DD_Speed.SetSavedData("COMSpeed", x => {
            int rate = 9600;
            if(!int.TryParse(DD_Speed.options[x].text, out rate))
                return;

            arduinoInteractive.baudRate = rate;
        });

        string[] ports = SerialPort.GetPortNames(); 
        DD_Serial.SetLabels(ports);
        DD_Serial.SetSavedData("ReadCOM", x => {
            arduinoInteractive.comName = DD_Serial.options[x].text;
        });

        BTN_Restart.onClick.AddListener(delegate {
            RestartArduino();
        });

        yield return new WaitForSeconds(10);

        arduinoInteractive.StartSerial();
        BTN_Restart.interactable = true;
        StartCoroutine(CheckStatus());
    }

    async void RestartArduino(){
        arduinoInteractive.CloseArduino();
        await Task.Delay(5000);
        if(this == null) return;
        arduinoInteractive.StartSerial();
    }

    WaitForSeconds wait = new WaitForSeconds(1.0f);
    IEnumerator CheckStatus(){
        while(true){
            yield return wait;

            if(arduinoInteractive.ArduinoPortState){
                TXT_State.text = "Online";
                TXT_State.color = Color.cyan;
            } else {
                TXT_State.text = "Offline";
                TXT_State.color = Color.red;
            }
        }
    }
}

#endif