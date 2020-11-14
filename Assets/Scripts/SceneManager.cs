using UnityEngine;
using UnityEngine.UI;
using CORC;


public class SceneManager : MonoBehaviour
{
    public Button ConnectBtn, CmdButton;
    public Text Status;
    public InputField IP;
    public GameObject Cursor;
    public CORCM3 Robot;

    // Start is called before the first frame update
    void Start()
    {
        ConnectBtn.onClick.AddListener(() => { Robot.Init(IP.text); });
        CmdButton.onClick.AddListener(() => { GTNSCommand(); });
        IP.text = "192.168.7.2";
    }

    // Update is called once per frame
    void Update()
    {
        Status.text = "Status:\n";
        if (Robot.IsInitialised())
        {
            Status.text += "\tConnected\n";
            Status.text += "\tt: " + Robot.State["t"][0].ToString("####.00") + "\n";
            Status.text += "\tX:" + Robot.State["X"][0].ToString("0.000") + "\t" + Robot.State["X"][1].ToString("0.000") + "\t" + Robot.State["X"][2].ToString("0.000") + "\n";
            Status.text += "\tdX:" + Robot.State["dX"][0].ToString("0.00") + "\t" + Robot.State["dX"][1].ToString("0.00") + "\t" + Robot.State["dX"][2].ToString("0.00") + "\n";
            Status.text += "\tF:" + Robot.State["F"][0].ToString("00.00") + "\t" + Robot.State["F"][1].ToString("00.00") + "\t" + Robot.State["F"][2].ToString("00.00") + "\n";

            float scale = 1000;
            Vector3 Origin = new Vector3(320, 200, 0);
            Cursor.transform.position = new Vector3((float)Robot.State["X"][0], -(float)Robot.State["X"][2], (float)Robot.State["X"][1])*scale+Origin;
        }
        else
        {
            Status.text += "\tNot Connected\n";
        }
    }

    public void Connect()
    {
    }

    public void GTNSCommand()
    {
        Robot.SendCmd("GTNS");
    }
    

    private void OnApplicationQuit() 
    {
    }
}
