using UnityEngine;
using UnityEngine.UI;
using CORC;


public class SceneManager : MonoBehaviour
{
    public Button ConnectBtn, CmdButton;
    public Text Status;
    public InputField IP;
    public GameObject Cursor, Arrow;
    public CORCM3 Robot;

    double last_t = 0;

    // Start is called before the first frame update
    void Start()
    {
        ConnectBtn.onClick.AddListener(() => { Connect(); });
        CmdButton.onClick.AddListener(() => { GTNSCommand(); });
        IP.text = "192.168.7.2";
    }

    // Update is called once per frame
    void Update()
    {
        Status.text = "Status:";
        if (Robot.IsInitialised())
        {
            //Update status text box
            Status.text += " Connected\n";
            Status.text += "\tt: " + Robot.State["t"][0].ToString("####.00") + " (" + (Robot.State["t"][0]-last_t).ToString("0.000") + ")\n";
            last_t= Robot.State["t"][0];
            Status.text += "\tX:";
            foreach(double val in Robot.State["X"])
                Status.text += val.ToString("0.000") + " \t";
            Status.text += "\n";
            Status.text += "\tdX:";
            foreach (double val in Robot.State["dX"])
                Status.text += val.ToString("0.00") + " \t";
            Status.text += "\n";
            Status.text += "\tF:";
            foreach (double val in Robot.State["F"])
                Status.text += val.ToString("00.0") + " \t";
            Status.text += "\n";

            //Map cursor position and force interaction vector to current robot values
            float scale = 1000;
            Vector3 Origin = new Vector3(0, 80, -500);
            Cursor.transform.position = new Vector3((float)Robot.State["X"][1], (float)Robot.State["X"][2], -(float)Robot.State["X"][0])*scale+Origin;
            Vector3 force = new Vector3((float)Robot.State["F"][1], (float)Robot.State["F"][2], -(float)Robot.State["F"][0]);
            float force_scale = 10;
            Arrow.transform.localPosition = new Vector3(0, 0, force.magnitude / force_scale);
            Arrow.transform.localScale = new Vector3(0.2f, force.magnitude / force_scale, 0.2f);
            Cursor.transform.LookAt(Cursor.transform.position - force);
        }
        else
        {
            Status.text += " Not Connected\n";
        }
    }


    public void GTNSCommand()
    {
        Robot.SendCmd("GTNS");
    }

    public void Connect()
    {
        if (!Robot.IsInitialised())
        {
            Robot.Init(IP.text);
            if(Robot.IsInitialised())
                ConnectBtn.GetComponentInChildren<Text>().text = "Disconnect";
        }
        else
        {
            Robot.Disconnect();
            ConnectBtn.GetComponentInChildren<Text>().text = "Connect";
        }
    }
    

    private void OnApplicationQuit() 
    {
        if (Robot.IsInitialised())
        {
            Robot.Disconnect();
        }
    }
}
