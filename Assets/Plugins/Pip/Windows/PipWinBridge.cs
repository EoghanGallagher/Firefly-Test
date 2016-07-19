#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

namespace Galvanic.PipPluginWindows
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Galvanic.PipSdk;
    using System.Diagnostics;


    public class PipWinBridge : Galvanic.PipPlugin.IPipBridge
    {
        Galvanic.PipSdk.PipManager m_pipMan;
        struct PipMessage
        {
            public PipMessage(string methodName, object value)
            {
                this.methodName = methodName;
                this.value = value;
            }

            public string methodName;
            public object value;
        }

        private Queue<PipMessage> m_messageQueue = new Queue<PipMessage>();

        static class PipConnectionError
        {
            public const int Success = 0;
            public const int ConnectionTimeout = 1;
            public const int PairingFailed = 2;
            public const int UnspecifiedError = 3;
        };

        public PipWinBridge()
        {
            m_pipMan = PipManager.Instance;
            m_pipMan.DiscoveryComplete += new PipManagerEvent(DiscoveryComplete);
            m_pipMan.PipDiscovered += new PipManagerEvent(PipDiscovered);
            m_pipMan.Suspending += new PipManagerEvent(PipManagerSuspending);
            m_pipMan.Resuming += new PipManagerEvent(PipManagerResuming);
            m_pipMan.Resumed += new PipManagerEvent(PipManagerResumed);
            QueueMessage("ReceiveDiscoveryMessage", "pip_plugin:discovery:initialized:" + (int)PipPlugin.PipResult.Succeeded);
        }

        public void ResetManager()
        {
            m_pipMan.Reset();
        }

        public void ShutDownManager()
        {
            m_pipMan.ShutDown();
        }

        void QueueMessage(string methodName, object value)
        {
            PipMessage message = new PipMessage(methodName, value);
            lock (m_messageQueue)
            {
                m_messageQueue.Enqueue(message);
            }
        }

        public void Update()
        {
            while (m_messageQueue.Count > 0)
            {
                PipMessage msg;
                lock (m_messageQueue)
                {
                    msg = m_messageQueue.Dequeue();
                }
                if (msg.methodName.Length > 0)
                    Galvanic.PipPlugin.PipManager.Instance.SendMessage(msg.methodName, msg.value);
            }
        }

        public bool BluetoothActive
        {
            get
            {
                bool active = PipManager.IsBluetoothAdapterEnabled();
                PipPlugin.PipDebug.Log("PipWinBridge.BluetoothActive: " + active.ToString());
                return active;
            }
        }

        public void SuspendPips()
        {
            PipPlugin.PipDebug.Log("PipWinBridge.SuspendPips");
            m_pipMan.SuspendPips();
        }

        public void ResumePips()
        {
            PipPlugin.PipDebug.Log("PipWinBridge.ResumePips");
            m_pipMan.ResumePips();
        }

        public void DiscoverPips()
        {
            PipPlugin.PipDebug.Log("PipWinBridge.DiscoverPips");
            m_pipMan.DiscoverPips(0);
        }

        public void CancelDiscovery()
        {
            PipPlugin.PipDebug.Log("PipWinBridge.CancelDiscovery");
            m_pipMan.CancelDiscovery();
        }

        public int NumDiscoveredPips
        {
            get { return m_pipMan.NumDiscoveries; }
        }

        public int NumRegisteredPips
        {
            get { return m_pipMan.NumRegistered; }
        }

        public string GetRegisteredPipName(int registeredIndex)
        {
            PipInfo info = m_pipMan.GetRegisteredPip(registeredIndex);
            return info.Name;
        }

        public long GetRegisteredPipId(int registeredIndex)
        {
            PipInfo info = m_pipMan.GetRegisteredPip(registeredIndex);
            return info.Id;
        }

        public string GetDiscoveredPipName(int discoveredIndex)
        {
            PipInfo info = m_pipMan.GetDiscoveredPip(discoveredIndex);
            return info.Name;
        }

        public long GetDiscoveredPipId(int discoveredIndex)
        {
            PipInfo info = m_pipMan.GetDiscoveredPip(discoveredIndex);
            return info.Id;
        }

        public void RegisterPip(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RegisterPip, pipID=" + pip.PipID.ToString());
            foreach (PipInfo info in m_pipMan.DiscoveredPips)
            {
                if (info.Id == pip.PipID)
                {
                    PipPlugin.PipDebug.Log("PipWinBridge: Registering Pip");
                    m_pipMan.RegisterPip(info);
                    break;
                }
            }
        }

        public void UnregisterPip(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.UnregisterPip, pipID=" + pip.PipID.ToString());

            foreach (PipInfo info in m_pipMan.RegisteredPips)
            {
                if (info.Id == pip.PipID)
                {
                    m_pipMan.UnregisterPip(info);
                    break;
                }
            }
        }

        public void UnregisterAllPips()
        {
            PipPlugin.PipDebug.Log("PipWinBridge.UnregisterAllPips");
            m_pipMan.UnregisterAllPips();
        }

        public char [] AllowedPipNameChars
        {
            get 
            {
                return Pip.AllowedNameChars.ToCharArray();
            }
        }

        public void ConnectPip(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.ConnectPip, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);

            if (pip != null)
            {
                p.Connected += new PipEvent(ConnectCompleted);
                p.Disconnected += new PipEvent(DisconnectCompleted);
                p.AnalyzerOutputAvailable += new PipEvent(AnalyzerOutputAvailable);
                p.VersionAvailable += new PipEvent(VersionAvailable);
                p.BatteryLevelAvailable += new PipEvent(BatteryLevelAvailable);
                p.SetPinCompleted += new PipEvent(SetPinCompleted);
                p.BatteryStatusAvailable += new PipEvent(BatteryStatusAvailable);
                p.SetNameCompleted += new PipEvent(SetNameCompleted);
                p.SetPinCompleted += new PipEvent(SetPinCompleted);
                p.NameAvailable += new PipEvent(NameAvailable);
                p.ActivationChanged += new PipEvent(ActivationChanged);
                p.RequestConnect();
            }
            else
            {
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
            }
        }

        public void DisconnectPip(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.DisconnectPip, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);

            if (p != null)
                p.RequestDisconnect();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void RequestVersion(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RequestVersion, pipID=" + pip.PipID.ToString());
            PipSdk.Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestVersion();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public string GetVersion(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.GetVersion, pipID=" + pip.PipID.ToString());
            string version = "";
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                version = p.Version;
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");

            return version;
        }

        public void RequestGetName(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RequesGettName, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestName();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void RequestPing(PipPlugin.Pip pip, bool flashLED)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RequestPing, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestPing(flashLED);
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void RequestReset(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RequestReset, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestReset();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void RequestSetName(PipPlugin.Pip pip, string name)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.RequestSetName, pipID=" + pip.PipID.ToString());

            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestSetName(name);
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public string GetName(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.GetName, pipID=" + pip.PipID.ToString());
            string name = "";
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                name = p.Name;
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");

            return name;
        }


        public int GetBatteryLevel(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.GetBatteryLevel, pipID=" + pip.PipID.ToString());
            int level = 0;
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                level = p.BatteryLevel;
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");

            return level;
        }

        public PipPlugin.PipBatteryStatus GetBatteryStatus(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.GetBatteryStatus, pipID=" + pip.PipID.ToString());
            PipPlugin.PipBatteryStatus status = PipPlugin.PipBatteryStatus.NoStatus;
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                status = (PipPlugin.PipBatteryStatus)p.BatteryStatus;
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");

            return status;
        }

        public bool GetIsActive(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.GetIsActive, pipID=" + pip.PipID.ToString());
            bool active = false;
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                active = p.IsActive;
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");

            return active;
        }

        public void RequestPowerOff(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.PowerOff, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestPowerOff();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void StartStreaming(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.StartStreaming, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestStartStream();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void StopStreaming(PipPlugin.Pip pip)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.StopStreaming, pipID=" + pip.PipID.ToString());
            Pip p = m_pipMan.GetPip(pip.PipID);
            if (p != null)
                p.RequestStopStream();
            else
                PipPlugin.PipDebug.Log("PipManager.GetPip returned null");
        }

        public void PipManagerResumed(object sender, PipManagerEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge: PipManagerResumed, status=" + e.Status.ToString());
            QueueMessage("ReceiveDiscoveryMessage", "pip_plugin:discovery::resumed:" + e.Status.ToString());
        }

        private void PipManagerResuming(object sender, PipManagerEventArgs e)
        {
            // TODO
        }

        private void PipManagerSuspending(object sender, PipManagerEventArgs e)
        {
            // TODO
        }

        private void PipDiscovered(object sender, PipManagerEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge: PipDiscovered, status=" + e.Status.ToString());
            QueueMessage("ReceiveDiscoveryMessage", "pip_plugin:discovery:devicefound:" + e.Status.ToString());
        }

        void DiscoveryComplete(object sender, PipManagerEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.DiscoveryComplete, status=" + e.Status.ToString());
            QueueMessage("ReceiveDiscoveryMessage", "pip_plugin:discovery:complete:" + e.Status.ToString());
        }

        void ConnectCompleted(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.ConnectCompleted, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveConnectionMessage", "pip_plugin:connection:connected:" + e.Status.ToString() + ":" +  e.Id.ToString());
        }

        void DisconnectCompleted(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.DisconnectCompleted, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveConnectionMessage", "pip_plugin:connection:disconnected:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void Disconnected(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.Disconnected, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveConnectionMessage", "pip_plugin:connection:disconnected:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void VersionAvailable(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.VersionAvailable, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:version:" + e.Status.ToString() +":" + e.Id.ToString());
        }

        void BatteryLevelAvailable(object sender, PipEventArgs e)
        {
            // Battery level is updated frequently. Only uncomment the line below if you are specifically interested in
            // debugging issues relating to battery level.
            // PipPlugin.PipDebug.Log("PipWinBridge.BatteryLevelAvailable, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:batterylevel:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void BatteryStatusAvailable(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.BatteryStatusAvailable, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:batteryevent:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void SetNameCompleted(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.SetNameCompleted, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:setname:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void NameAvailable(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.NameAvailable, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:getname:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void ActivationChanged(object sender, PipEventArgs e)
        {
            PipPlugin.PipDebug.Log("PipWinBridge.ActivationChanged, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());
            QueueMessage("ReceiveUpdateMessage", "pip_plugin:update:activation:" + e.Status.ToString() + ":" + e.Id.ToString());
        }

        void SetPinCompleted(object sender, PipEventArgs e)
        {
        }

        void AnalyzerOutputAvailable(object sender, PipEventArgs e)
        {
            // The line below will generate a lot of output - only uncomment if there is a specific need to debug analyzer output
            // PipPlugin.PipDebug.Log("PipWinBridge.AnalyzerOutputAvailable, pipID=" + e.Id.ToString() + ", status=" + e.Status.ToString());

            Pip pip = m_pipMan.GetPip(e.Id);
            int trend = -1;
            if (pip.IsActive)
            {
                AnalyzerOutput[] op = pip.AnalyzerOutput;
                trend = (int)op[StandardSignalAnalyzer.OpIndexStressTrend].Value;
            }
            QueueMessage("ReceiveDataMessage", "pip_plugin:data:" + e.Status.ToString() + ":" + e.Id.ToString() + ":" + trend.ToString());
        }
    }
}

#endif // if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN