﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace CORC
{
    /// <summary>
    /// Dictionnary with predefined type, fixed non-modifiable structure and defined order
    /// (C# has no generic OrderedDictionnary nor nonmodifiable ones)
    /// </summary>
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

        /// <summary>
        /// Return the total length (number of double values) registered within the dcitionnary
        /// </summary>
        public int GetTotalLength()
        {
            return TotalLength;
        }

        /// <summary>
        /// Fill the entire dictionnary double values in the order specified in ItemsOrder
        /// </summary>
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

        /// <summary>
        /// Prevent change to the dictionnary structure: this has no effect
        /// </summary>
        public new void Add(string key, double[] value)
        {
            //Not allowed
            throw new NotSupportedException();
        }

        /// <summary>
        /// Prevent change to the dictionnary structure: this has no effect
        /// </summary>
        public new bool Remove(string key)
        {
            //Not allowed
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Abstract class to define a CORC robot object and manage communication with CORC server
    /// </summary>
    public abstract class CORCRobot: MonoBehaviour
    {
        public FLNLClient Client = new FLNLClient();
        public FixedDictionary State;
        protected bool Initialised = false;
        private string LoggingFilename = "";

        /// <summary>
        /// Initialisation
        /// </summary>
        public void Start()
        {
            Initialised = false;
        }
        
        /// <summary>
        /// Set the logging file and write headers to it
        /// </summary>
        public bool SetLoggingFile(string filename)
        {
            LoggingFilename = filename;
            //Create file and write header (and keep it open)
            Client.LogFileStream = new StreamWriter(LoggingFilename);
            foreach (string key in State.ItemsOrder)
            {
                Client.LogFileStream.Write(key + ",");
            }
            Client.LogFileStream.Write("\n");
            return true;
        }
        
        /// <summary>
        /// Start or stop logging to file. Use SetLoggingFile() first to set a logging file.
        /// </summary>
        public bool SetLogging(bool v)
        {
            if(LoggingFilename != "")
            {
                return Client.SetLogging(v);
            }
            
            return false;
        }

        /// <summary>
        /// Retrieve Robot state values from CORC server
        /// </summary>
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

        /// <summary>
        /// Is Robot initialised and communication established?
        /// </summary>
        public bool IsInitialised()
        {
            return Initialised;
        }

        /// <summary>
        /// Connect to CORC and register states values to be updated every loop
        /// </summary>
        public abstract void Init(string ip = "192.168.7.2", int port = 2048);

        /// <summary>
        /// Disconnect from CORC server
        /// </summary>
        public void Disconnect()
        {
            SetLogging(false);
            Client.Disconnect();
            Initialised = false;
        }

        /// <summary>
        /// Send a command (up to 4 characters) and associated double parameters (up to 30) to CORC server
        /// </summary>
        public void SendCmd(string cmd, double[] parameters = null)
        {
            Client.SendCmd(cmd.ToCharArray(), parameters);
        }
        
        /// <summary>
        /// Return cmd if one received (command text only, no params)
        /// </summary>
        public string GetCmd()
        {
            if(Client.IsReceivedCmd()) {
                return Client.GetReceivedCmd().cmd;
            }
            else {
                return "";
            }
        }
    }
}
