using CORC;

namespace CORC
{
    public class CORCX2 : CORCRobot
    {
        public override void Init(string ip = "192.168.7.2", int port = 2048)
        {
            if (Client.IsConnected())
                Client.Disconnect();
            Client.Connect(ip, port);

            //Define state values to receive (in pre-defined order: should match CORC implementation)
            State = new FixedDictionary
            {
                ["q"] = new double[4],
                ["dq"] = new double[4],
                ["tau"] = new double[4],
                ["t"] = new double[1],
            };
            State.Init(new string[] { "q", "dq", "tau", "t" });
            Initialised = true;
        }
    }
}