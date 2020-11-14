using CORC;

namespace CORC
{
    public class CORCM3 : CORCRobot
    {
        public override void Init(string ip = "192.168.7.2", int port = 2048)
        {
            if (Client.IsConnected())
                Client.Disconnect();
            Client.Connect(ip, port);

            //Define state values to receive (in pre-defined order: should match CORC implementation)
            State = new FixedDictionary
            {
                ["X"] = new double[3],
                ["dX"] = new double[3],
                ["F"] = new double[3],
                ["t"] = new double[1]
            };
            State.Init(new string[] { "X", "dX", "F", "t" });
            Initialised = true;
        }
    }
}