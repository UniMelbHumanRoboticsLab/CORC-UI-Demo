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

    //static float x = 0;

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
            Status.text += "\tF:" + Robot.State["F"][0].ToString("00.0") + "\t" + Robot.State["F"][1].ToString("00.0") + "\t" + Robot.State["F"][2].ToString("00.0") + "\n";

            float scale = 1000;
            Vector3 Origin = new Vector3(0, 0, -500);
            Cursor.transform.position = new Vector3((float)Robot.State["X"][1], (float)Robot.State["X"][2], -(float)Robot.State["X"][0])*scale+Origin;
            Vector3 force = new Vector3((float)Robot.State["F"][1], (float)Robot.State["F"][2], -(float)Robot.State["F"][0]);
            float force_scale = 10;
            Arrow.transform.localPosition = new Vector3(0, 0, force.magnitude / force_scale);
            Arrow.transform.localScale = new Vector3(0.2f, force.magnitude / force_scale, 0.2f);
            Cursor.transform.LookAt(Cursor.transform.position + force);
        }
        else
        {
            Status.text += "\tNot Connected\n";

            /*float scale = 1000;
            Vector3 Origin = new Vector3(0f, 0f, 0f);
            x += 0.000001f;
            Cursor.transform.position += new Vector3(x, 0, 0) * scale + Origin;
            Vector3 force = new Vector3(100/40, 100/40, 0);
            Arrow.transform.localPosition = new Vector3(0, 0, force.magnitude);
            Arrow.transform.localScale = new Vector3(0.2f, force.magnitude, 0.2f);
            Cursor.transform.LookAt(Cursor.transform.position + force);*/
        }
    }


    public void GTNSCommand()
    {
        Robot.SendCmd("GTNS");
    }
    

    private void OnApplicationQuit() 
    {
        if (Robot.IsInitialised())
        {
            Robot.Disconnect();
        }
    }
}
