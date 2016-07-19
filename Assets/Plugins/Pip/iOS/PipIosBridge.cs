#if UNITY_IPHONE

using System;
using System.Runtime.InteropServices;

namespace Galvanic.PipPlugin
{
    class PipIosBridge : IPipBridge
    {
        [DllImport("__Internal")]
        private static extern void iosBridge_initPlugin();
        [DllImport("__Internal")]
        private static extern bool iosBridge_isBluetoothActive();
        [DllImport("__Internal")]
        private static extern void iosBridge_resetAll();
        [DllImport("__Internal")]
        private static extern void iosBridge_discoverPips();
        [DllImport("__Internal")]
        private static extern bool iosBridge_isDiscovering();
        [DllImport("__Internal")]
        private static extern void iosBridge_cancelDiscovery();
        [DllImport("__Internal")]
        private static extern int iosBridge_getNumDiscoveredPips();
        [DllImport("__Internal")]
        private static extern int iosBridge_getNumRegisteredPips();
        [DllImport("__Internal")]
        private static extern IntPtr iosBridge_getNameForDiscoveredPip(int discoveredIndex);
        [DllImport("__Internal")]
        private static extern IntPtr iosBridge_getNameForRegisteredPip(int registeredIndex);
        [DllImport("__Internal")]
        private static extern int iosBridge_getPipIDForDiscoveredPip(int discoveredIndex);
        [DllImport("__Internal")]
        private static extern int iosBridge_getPipIDForRegisteredPip(int registeredIndex);
        [DllImport("__Internal")]
        private static extern void iosBridge_registerPip(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_unregisterPip(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_unregisterAll();
        [DllImport("__Internal")]
        private static extern void iosBridge_connectPip(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_disconnectPip(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_startStreaming(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_stopStreaming(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestPowerOff(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestReset(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestPing(int pipID, bool flashLED);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestVersion(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestGetName(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_requestSetName(int pipID, string name);
        [DllImport("__Internal")]
        private static extern int iosBridge_getBatteryLevel(int pipID);
        [DllImport("__Internal")]
        private static extern int iosBridge_getBatteryEvent(int pipID);
        [DllImport("__Internal")]
        private static extern IntPtr iosBridge_getAllowedPipNameChars();
        [DllImport("__Internal")]
        private static extern IntPtr iosBridge_getName(int pipID);
        [DllImport("__Internal")]
        private static extern bool iosBridge_isPipActive(int pipID);
        [DllImport("__Internal")]
        private static extern IntPtr iosBridge_getVersion(int pipID);
        [DllImport("__Internal")]
        private static extern void iosBridge_suspendPips();
        [DllImport("__Internal")]
        private static extern void iosBridge_resumePips();

        public PipIosBridge()
        {
		    iosBridge_initPlugin();
        }

        public void Update() {}

		public bool BluetoothActive{ get { return iosBridge_isBluetoothActive(); } }

		public void ResetManager()
        {
            iosBridge_resetAll();
        }

		public void ShutDownManager() 
		{
		}

		public void SuspendPips()
        {
            iosBridge_suspendPips();
        }

		public void ResumePips()
        {
            iosBridge_resumePips();
        }

		public void DiscoverPips()
        {
            iosBridge_discoverPips();
        }

		public void CancelDiscovery()
        {
            iosBridge_cancelDiscovery();
        }

		public int NumDiscoveredPips { get { return iosBridge_getNumDiscoveredPips(); } }

		public void RegisterPip(Pip pip)
        {
            iosBridge_registerPip((int)pip.PipID);
        }

		public void UnregisterPip(Pip pip)
        {
            iosBridge_unregisterPip((int)pip.PipID);
        }

		public void UnregisterAllPips()
        {
            iosBridge_unregisterAll();
        }

		public int NumRegisteredPips { get { return iosBridge_getNumRegisteredPips(); }  }

		public void ConnectPip(Pip pip)
        {
            iosBridge_connectPip((int)pip.PipID);
        }

		public void DisconnectPip(Pip pip)
        {
            iosBridge_disconnectPip((int)pip.PipID);
        }

		public void RequestVersion(Pip pip)
        {
            iosBridge_requestVersion((int)pip.PipID);
        }

		public void RequestGetName(Pip pip)
        {
            iosBridge_requestGetName((int)pip.PipID);
        }

		public void RequestPing(Pip pip, bool flashLED)
        {
            iosBridge_requestPing((int)pip.PipID, flashLED);
        }
        
		public void RequestSetName(Pip pip, string name)
        {
            iosBridge_requestSetName((int)pip.PipID, name);
        }

		public void RequestReset(Pip pip)
        {
            iosBridge_requestReset((int)pip.PipID);
        }

		public void RequestPowerOff(Pip pip)
        {
            iosBridge_requestPowerOff((int)pip.PipID);
        }

		public void StartStreaming(Pip pip)
        {
            iosBridge_startStreaming((int)pip.PipID);
        }

		public void StopStreaming(Pip pip)
        {
            iosBridge_stopStreaming((int)pip.PipID);
        }

		public int GetBatteryLevel(Pip pip)
        {
            return iosBridge_getBatteryLevel((int)pip.PipID);
        }

		public PipBatteryStatus GetBatteryStatus(Pip pip)
        {
            return (PipBatteryStatus)iosBridge_getBatteryEvent((int)pip.PipID);
        }

		public string GetVersion(Pip pip)
        {
            return Marshal.PtrToStringAnsi(iosBridge_getVersion((int)pip.PipID));
        }

		public char[] AllowedPipNameChars
        {
            get
            {
                string allowed = Marshal.PtrToStringAnsi(iosBridge_getAllowedPipNameChars());
                return allowed.ToCharArray();
            }
        }

		public bool GetIsActive(Pip pip)
        {
            return iosBridge_isPipActive((int)pip.PipID);
        }

		public string GetName(Pip pip)
        {
            return Marshal.PtrToStringAnsi(iosBridge_getName((int)pip.PipID));
        }

		public string GetRegisteredPipName(int registeredIndex)
        {
            return Marshal.PtrToStringAnsi(iosBridge_getNameForRegisteredPip(registeredIndex));
        }

		public long GetRegisteredPipId(int registeredIndex)
        {
            return iosBridge_getPipIDForRegisteredPip(registeredIndex);
        }

		public string GetDiscoveredPipName(int discoveredIndex)
        {
            return Marshal.PtrToStringAnsi(iosBridge_getNameForDiscoveredPip(discoveredIndex));
        }

		public long GetDiscoveredPipId(int discoveredIndex)
        {
            return iosBridge_getPipIDForDiscoveredPip(discoveredIndex);
        }
    }
}

#endif
