using System;
using System.Collections.Generic;
using UnityEngine;

namespace CORC
{
    //Dictionnary with predefined type, fixed non-modifiable structure and defined order
    //(C# has no generic OrderedDictionnary nor nonmodifiable ones)
    public class FixedDictionary : Dictionary<string, double[]>
    {
        private int TotalLength = 0;
        public string[] ItemsOrder;
        private bool Locked = false;

        //Initialise the order of the items and total length in doubles of the dictionnary
        public void Init(string[] itemsorder)
        {
            //Compute total length of arrays elements and fill ItermsOrder list
            ItemsOrder = new string[itemsorder.Length];
            TotalLength = 0;
            for (int i= 0; i < itemsorder.Length; i++)
            {
                ItemsOrder[i] = itemsorder[i];
                TotalLength += this[itemsorder[i]].Length;
            }

            //Lock dict
            Locked = true;
        }

        public int GetTotalLength()
        {
            return TotalLength;
        }

        //Fill entire dictionnary double values in the order specified in ItemsOrder
        public bool FillAll(double[] values)
        {
            if (values.Length != TotalLength)
            {
                Debug.Log("Non matching number of values: incoherent states?");
                return false;
            }
            else
            {
                int k = 0;
                foreach (string key in ItemsOrder)
                {
                    for(int i=0; i< this[key].Length; i++)
                    {
                        this[key][i] = values[k];
                        k++;
                    }
                }
                return true;
            }
        }

        //Prevent change to the dictionnary structure
        public new void Add(string key, double[] value)
        {
            //Not allowed
            throw new NotSupportedException();
        }

        //Prevent change to the dictionnary structure
        public new bool Remove(string key)
        {
            //Not allowed
            throw new NotSupportedException();
        }
    }


    public abstract class CORCRobot: MonoBehaviour
    {
        public FLNLClient Client = new FLNLClient();
        public FixedDictionary State;
        protected bool Initialised = false;

        // Start is called before the first frame update
        public void Start()
        {
            Initialised = false;
        }

        // Update is called once per frame
        public void Update()
        {
            if (Initialised)
            {
                //Update state if values received
                if (Client.IsReceivedValues())
                {
                    State.FillAll(Client.GetReceivedValues());
                }
            }
        }

        public bool IsInitialised()
        {
            return Initialised;
        }

        //Connect to CORC and register states values to be updated every loop
        public abstract void Init(string ip = "192.168.7.2", int port = 2048);

        public void Disconnect()
        {
            Client.Disconnect();
        }

        //Send a command (up to 4 characters) and associated parameters (up to 30)
        public void SendCmd(string cmd, double[] parameters = null)
        {
            Client.SendCmd(cmd.ToCharArray(), parameters);
        }
    }
}