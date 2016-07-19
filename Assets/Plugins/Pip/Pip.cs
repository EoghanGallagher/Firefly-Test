using System;

namespace Galvanic.PipPlugin
{
    public enum PipResult
    {
        Succeeded,
        Failed,
        PairingFailed
    }

    public enum PipStatus
    {
        Disconnected,
        Disconnecting,
        Connected,
        Connecting,
        Streaming,
        Error
    }

    public enum PipStressTrend
    {
        Relaxing,
        Stressing,
        Steady,
        None
    }

    public enum PipBatteryStatus
    {
        BatteryLow = 0,
        ChargerAttached = 1,
        ChargerDetached = 2,
        BatteryOK = 3,
        BatteryCritical = 4,
        NoStatus = 5,
    }

    public class PipEventArgs : EventArgs
    {
        public PipEventArgs(PipResult result)
        {
            Result = result;
        }

        public PipResult Result { get; private set; }
    }

    public delegate void PipEvent(Pip pip, PipEventArgs e);

    public class Pip
    {
        public event PipEvent ConnectCompleted;
        public event PipEvent Disconnected;
        public event PipEvent VersionAvailable;
        public event PipEvent BatteryLevelAvailable;
        public event PipEvent BatteryStatusChanged;
        public event PipEvent PingCompleted;
        public event PipEvent NameAvailable;
        public event PipEvent SetNameCompleted;
        public event PipEvent AnalyzerOutputAvailable;
        public event PipEvent ActivationChanged;

        public int DiscoveredIndex { get; private set; }
        public int RegisteredIndex { get; private set; }
        public long PipID { get; internal set; }
        public string Name { get; internal set; }
        public Boolean Registered { get; internal set; }
        public Boolean Discovered { get; internal set; }
        public int BatteryLevel { get; internal set; }
        public PipBatteryStatus BatteryStatus { get; internal set; }
        public string Version { get; internal set; }
        public string StatusString { get; internal set; }
        public PipStatus Status { get; internal set; }
        public bool Active { get; internal set; }
        public PipStressTrend StressTrend { get; internal set; }
        public float DisconnectionTime { get; internal set; }

        public Pip(string name, long pipID)
        {
            if (name == null)
                Name = "";
            else
                Name = name;
            PipID = pipID;
            StatusString = name;
            Status = PipStatus.Disconnected;
            Active = false;
            StressTrend = PipStressTrend.None;
            DisconnectionTime = -1.0f;
            Registered = false;
            Discovered = false;
        }

        public void Connect()
        {
            PipBridge.Instance.ConnectPip(this);
        }

        public void Disconnect()
        {
            PipBridge.Instance.DisconnectPip(this);
        }

        public void RequestGetName()
        {
            PipBridge.Instance.RequestGetName(this);
        }

        public void RequestSetName(string name)
        {
            PipBridge.Instance.RequestSetName(this, name);
        }

        public void RequestPing(bool flashLED)
        {
            PipBridge.Instance.RequestPing(this, flashLED);
        }

        public void RequestReset()
        {
            PipBridge.Instance.RequestReset(this);
        }

        public void RequestVersion()
        {
            PipBridge.Instance.RequestVersion(this);
        }

        public void RequestPowerOff()
        {
            PipBridge.Instance.RequestPowerOff(this);
        }

        public void StartStreaming()
        {
            PipBridge.Instance.StartStreaming(this);
        }

        public void StopStreaming()
        {
            PipBridge.Instance.StopStreaming(this);
        }



        internal void RaiseEventConnectCompleted(PipResult result)
        {
            if (ConnectCompleted != null)
                ConnectCompleted(this, new PipEventArgs(result));
        }

        internal void RaiseEventDisconnected(PipResult result)
        {
            if (Disconnected != null)
                Disconnected(this, new PipEventArgs(result));
        }

        internal void RaiseEventVersionAvailable(PipResult result)
        {
            if (VersionAvailable != null)
                VersionAvailable(this, new PipEventArgs(result));
        }

        internal void RaiseEventBatteryStatusChanged(PipResult result)
        {
            if (BatteryStatusChanged != null)
                BatteryStatusChanged(this, new PipEventArgs(result));
        }
        internal void RaiseEventBatteryLevelAvailable(PipResult result)
        {
            if (BatteryLevelAvailable != null)
                BatteryLevelAvailable(this, new PipEventArgs(result));

        }
        internal void RaiseEventPingCompleted(PipResult result)
        {
            if (PingCompleted != null)
                PingCompleted(this, new PipEventArgs(result));
        }

        internal void RaiseEventAnalyzerOutputAvailable(PipResult result)
        {
            if (AnalyzerOutputAvailable != null)
                AnalyzerOutputAvailable(this, new PipEventArgs(result));
        }

        internal void RaiseEventSetNameCompleted(PipResult result)
        {
            if (SetNameCompleted != null)
                SetNameCompleted(this, new PipEventArgs(result));
        }

        internal void RaiseEventNameAvailable(PipResult result)
        {
            if (NameAvailable != null)
                NameAvailable(this, new PipEventArgs(result));
        }

        internal void RaiseEventActivationChanged(PipResult result)
        {
            if (ActivationChanged != null)
                ActivationChanged(this, new PipEventArgs(result));
        }
    }
}
