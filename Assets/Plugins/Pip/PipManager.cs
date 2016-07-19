namespace Galvanic.PipPlugin
{
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Assertions;
    
    public class PipManagerEventArgs : EventArgs
    {
        public PipManagerEventArgs(Pip pip, PipResult result)
        {
            Pip = pip;
            Result = result;
        }

        public Pip Pip { get; private set; }
        public PipResult Result { get; private set; }
    }

    public delegate void PipManagerEvent(object sender, PipManagerEventArgs e);

    public class PipManager : MonoBehaviour
    {
        public event PipManagerEvent PipManagerReady;
        public event PipManagerEvent PipManagerResumed;
        public event PipManagerEvent PipDiscovered;
        public event PipManagerEvent PipDiscoveryComplete;

        private static PipManager ms_instance;
        private static char[] ms_allowedPipNameChars;

        public static PipManager Instance
        {
            get { return ms_instance; }
        }

        private readonly List<Pip> m_pips = new List<Pip>();
        public int NumPips { get { return m_pips.Count; } }

        private bool m_canSuspendResumePips = true;
        public bool CanSuspendResumePips { set { m_canSuspendResumePips = value; } }
        public int NumConnectedPips
        {
            get
            {
                int connected = 0;
                for (int i = 0; i < NumPips; i++)
                {
                    if ((m_pips[i].Status == PipStatus.Connected) ||
                        (m_pips[i].Status == PipStatus.Streaming))
                    {
                        connected++;
                    }
                }

                return connected;
            }
        }

        public bool BluetoothIsActive
        {
            get { return PipBridge.Instance.BluetoothActive; }
        }

        public bool IsPipConnected(Pip pip)
        {
            List<Pip> connPips = ConnectedPips;
            for (int i = 0; i < connPips.Count; i++)
            {
                if (connPips[i].PipID == pip.PipID)
                    return true;
            }

            return false;
        }

        public int NumDiscoveredPips
        {
            get { return PipBridge.Instance.NumDiscoveredPips; }
        }

        public int NumRegisteredPips
        {
            get { return PipBridge.Instance.NumRegisteredPips; }
        }

        public string GetRegisteredPipName(int registeredIndex)
        {
            string name = "";
            if (registeredIndex >= 0 && registeredIndex < NumRegisteredPips)
                name = PipBridge.Instance.GetRegisteredPipName(registeredIndex);
            return name;
        }

        public long GetRegisteredPipID(int registeredIndex)
        {
            long pipID = 0;
            if (registeredIndex >= 0 && registeredIndex < NumRegisteredPips)
                pipID = PipBridge.Instance.GetRegisteredPipId(registeredIndex);

            return pipID;
        }

        public string GetDiscoveredPipName(int discoveredIndex)
        {
            return PipBridge.Instance.GetDiscoveredPipName(discoveredIndex);
        }

        public long GetDiscoveredPipID(int discoveredIndex)
        {
            return PipBridge.Instance.GetDiscoveredPipId(discoveredIndex);
        }

        public void RegisterPip(Pip pip)
        {
            PipDebug.Log("PipManager.RegisterPip, pipID=" + pip.PipID.ToString());
            PipBridge.Instance.RegisterPip(pip);
        }

        public void UnregisterPip(Pip pip)
        {
            PipDebug.Log("PipManager.UnregisterPip, pipID=" + pip.PipID.ToString());

            for (int i = 0; i < NumPips; i++)
            {
                if (m_pips[i].PipID == pip.PipID)
                {
                    m_pips[i].Registered = false;
                    break;
                }
            }

            PipBridge.Instance.UnregisterPip(pip);
        }

        public List<Pip> ConnectedPips
        {
            get
            {
                List<Pip> connPips = new List<Pip>();
                for (int i = 0; i < NumPips; i++)
                {
                    if ((m_pips[i].Status == PipStatus.Connected) ||
                        (m_pips[i].Status == PipStatus.Streaming))
                    {
                        connPips.Add(m_pips[i]);
                    }
                }

                return connPips;
            }
        }

        public List<Pip> DiscoveredPips
        {
            get
            {
                List<Pip> discoveredPips = new List<Pip>();
                for (int i = 0; i < NumPips; i++)
                {
                    discoveredPips.Add(m_pips[i]);
                }

                return discoveredPips;
            }
        }

        public void ClearPipList()
        {
            m_pips.Clear();
        }

        public int GetConnectedDeviceIndex(int connectedIndex)
        {
            int count = -1;
            for (int i = 0; i < NumPips; ++i)
            {
                if ((m_pips[i].Status == PipStatus.Connected) ||
                    (m_pips[i].Status == PipStatus.Streaming))
                {
                    count++;
                }
                if (connectedIndex == count)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsPipRegistered(long pipID)
        {
            for (int i = 0; i < NumPips; i++)
            {
                if (m_pips[i].PipID == pipID)
                {
                    return m_pips[i].Registered;
                }
            }

            return false;
        }

        private void Awake()
        {
            if (ms_instance != null &&
                ms_instance != this)
            {
                // If we are not the first instance in existance (e.g. if another scene is loaded
                // with this script also there), then remove this instance
                Destroy(this.gameObject);
                return;
            }
            name = "PipManager";
            ms_instance = this;

            // Don't destroy on the next scene load
            DontDestroyOnLoad(this.gameObject);
        }

        void OnApplicationQuit()
        {
            PipBridge.Instance.ShutDownManager();
        }

        // Use this for initialization
        private void Start()
        {
            ms_allowedPipNameChars = PipBridge.Instance.AllowedPipNameChars;
        }
        
        void OnApplicationPause(bool pause)
        {
            


			if (Application.isEditor) 
                return;

            if (pause)
                if (m_canSuspendResumePips)
                    PipBridge.Instance.SuspendPips();
            else
                if (m_canSuspendResumePips)
                    PipBridge.Instance.ResumePips();
        }

        public void DiscoverPips()
        {
            PipDebug.Log("PipManager.DiscoverPips");
            PipBridge.Instance.DiscoverPips();
        }

        public void CancelPipDiscovery()
        {
            PipDebug.Log("PipManager.CancelPipDiscovery");
            PipBridge.Instance.CancelDiscovery();
        }

        public bool IsValidPipName(string name)
        {
            bool isValid = true;
            if (name.IndexOfAny(ms_allowedPipNameChars) == -1)
                isValid = false;

            return isValid;
        }

        public void ResetManager()
        {
            PipBridge.Instance.ResetManager();
        }

        public Pip GetPipWithID(long pipID)
        {
            Pip matchedPip = null;
            foreach (Pip pip in m_pips)
            {
                if (pip.PipID == pipID)
                {
                    matchedPip = pip;
                    break;
                }
            }

            return matchedPip;
        }

        public void ReceiveDiscoveryMessage(string message)
        {
            PipDebug.Log("PipManager.ReceiveDiscoveryMessage, message=" + message);
            string[] data = message.Split(':');

            PipResult result = PipResult.Succeeded;
            int status = int.Parse(data[3]);
            if ( status != 0 )
                result = PipResult.Failed;

            if (data[0] == "pip_plugin")
            {
                if (data[1] == "discovery")
                {
                    if (data[2] == "complete")
                    {
                        if ( PipDiscoveryComplete != null )
                            PipDiscoveryComplete(this, new PipManagerEventArgs(null, PipResult.Succeeded));

                    }
                    else if (data[2] == "devicefound")
                    {
                        PipDebug.Log("Pip discovered");
                        
                        bool addPip = true;
                        Pip newDiscPip = null;
                        int numDiscoveries = PipBridge.Instance.NumDiscoveredPips;
                        string pipName = PipBridge.Instance.GetDiscoveredPipName(numDiscoveries - 1);
                        long pipID = PipBridge.Instance.GetDiscoveredPipId(numDiscoveries - 1);
                        
                        foreach (Pip pip in m_pips)
                        {
                            if (pip.PipID == pipID)
                            {
                                addPip = false;
                                newDiscPip = pip;
                                break;
                            }
                        }

                        if (addPip)
                        {
                            newDiscPip = new Pip(pipName, pipID);
                            m_pips.Add(newDiscPip);
                        }
                        else
                        {
                            newDiscPip.Name = pipName;
                        }

						if ( PipDiscovered != null )
                            PipDiscovered(this, new PipManagerEventArgs(newDiscPip, result));
                    }
                    else if (data[2] == "initialized")
                    {
                        int pipCount = NumRegisteredPips;
                        for (int i = 0; i < pipCount; i++)
                        {
                            string name = PipBridge.Instance.GetRegisteredPipName(i);
                            long pipID = PipBridge.Instance.GetRegisteredPipId(i);
                            Pip newRegPip = new Pip(name, pipID);
                            newRegPip.Registered = true;
                            m_pips.Add(newRegPip);
                        }

                        if ( PipManagerReady != null )
                            PipManagerReady(this, new PipManagerEventArgs(null, result));
                    }
                    else if (data[2] == "resumed")
                    {
                        
						Debug.Log ( "RESUMED CALLED" );

						if (PipManagerResumed != null)
                            PipManagerResumed(this, new PipManagerEventArgs(null, result));
                    }
                }
            }
        }

        public void ReceiveConnectionMessage(string message)
        {
            string[] data = message.Split(':');
            int status = int.Parse(data[3]);
            long pipID = long.Parse(data[4]);
            Pip pip = GetPipWithID(pipID);
            
            if (data[0] == "pip_plugin")
            {
                if (data[1] == "connection")
                {
                    if ( data[2] == "connected" )
                    {
                        PipResult result = PipResult.Failed;
                        switch ( status )
                        {
                            case 0:
                            {
                                result = PipResult.Succeeded;
                                pip.Status = PipStatus.Connected;
                                pip.DisconnectionTime = -1;
                                pip.RequestGetName();
    					        pip.StartStreaming();
                                break;
                            }                       
                            case 2:
                            {
                                result = PipResult.PairingFailed;
                                break;
                            }
                            default:
                            {
                                result = PipResult.Failed;
                                break;
                            }
                        }

                        pip.RaiseEventConnectCompleted(result);
                    }
                    else if (data[2] == "disconnected")
                    {
                        pip.Status = PipStatus.Disconnected;
                        pip.DisconnectionTime = -1.0f;
                        pip.RaiseEventDisconnected(PipResult.Succeeded);
                    }
                }
            }
        }

        public void ReceiveDataMessage(string message)
        {
            string[] data = message.Split(':');
            Assert.IsTrue(data.Length == 5);

            if (data[0] == "pip_plugin")
            {
                if (data[1] == "data")
                {
                    long pipID = long.Parse(data[3]);
                    Pip pip = GetPipWithID(pipID);
                    int status = int.Parse(data[2]);
                    PipResult result = (status == 0 ? PipResult.Succeeded : PipResult.Failed);
                    if (pip.Status == PipStatus.Connected)
                        pip.Status = PipStatus.Streaming;
 
                    switch ( data[4] )
                    { 
                        case "1": // relaxing
                            pip.StressTrend = PipStressTrend.Relaxing;
                            break;
                        case "2": // stressing
                            pip.StressTrend = PipStressTrend.Stressing;
                            break;
                        case "3": // Steady
                            pip.StressTrend = PipStressTrend.Steady;
                            break;
                        default:
                            pip.StressTrend = PipStressTrend.None;
                            break;
                    }

                    pip.RaiseEventAnalyzerOutputAvailable(result);
                }
            }
        }

        public void ReceiveUpdateMessage(string message)
        {
            string[] data = message.Split(':');
            Assert.IsTrue(data.Length == 5);

            if (data[0] == "pip_plugin")
            {
                if (data[1] == "update")
                {
                    int status = int.Parse(data[3]);
                    PipResult result = status == 0 ? PipResult.Succeeded : PipResult.Failed;
                    long pipID = long.Parse(data[4]);
                    Pip pip = GetPipWithID(pipID);
                    string updateType = data[2];

                    switch (updateType)
                    {
                        case "batterylevel":
                            pip.BatteryLevel = PipBridge.Instance.GetBatteryLevel(pip);
                            pip.RaiseEventBatteryLevelAvailable(result);
                            break;
                        case "batteryevent":
                            pip.BatteryStatus = PipBridge.Instance.GetBatteryStatus(pip);
                            pip.RaiseEventBatteryStatusChanged(result);
                            break;
                        case "version":
                            pip.Version = PipBridge.Instance.GetVersion(pip);
                            pip.RaiseEventVersionAvailable(result);
                            break;
                        case "getname":
                            pip.Name = PipBridge.Instance.GetName(pip);
                            pip.RaiseEventNameAvailable(result);
                            break;
                        case "setname":
                            pip.RaiseEventSetNameCompleted(result);
                            break;
                        case "ping":
                            pip.RaiseEventPingCompleted(result);
                            break;
                        case "activation":
                            pip.Active = PipBridge.Instance.GetIsActive(pip);
                            pip.RaiseEventActivationChanged(result);
                            break;
                    }
                }
            }
        }

        private void Update()
        {
            PipBridge.Instance.Update();

            for (int i = 0; i < NumPips; i++)
            {
                if (m_pips[i].DisconnectionTime > -1.0f && (Time.time - m_pips[i].DisconnectionTime) > 10.0f)
                {
                    m_pips[i].Status = PipStatus.Disconnected;
                    m_pips[i].DisconnectionTime = -1.0f;
                }
            }
        }
    }
}

 