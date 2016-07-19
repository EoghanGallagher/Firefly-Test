using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galvanic.PipPlugin
{
    internal interface IPipBridge
    {
        void Update();
        char [] AllowedPipNameChars { get; }
        bool BluetoothActive { get; }
        void ResetManager();
        void ShutDownManager();
        void SuspendPips();
        void ResumePips();
        void DiscoverPips();
        void CancelDiscovery();
        int NumDiscoveredPips { get; }
        void RegisterPip(Pip pip);
        void UnregisterPip(Pip pip);
        void UnregisterAllPips();
        int NumRegisteredPips { get;  }
        void ConnectPip(Pip pip);
        void DisconnectPip(Pip pip);
        void RequestVersion(Pip pip);
        void RequestGetName(Pip pip);
        void RequestPing(Pip pip, bool flashLED);
        void RequestSetName(Pip pip, string name);
        void RequestReset(Pip pip);
        void RequestPowerOff(Pip pip);
        void StartStreaming(Pip pip);
        void StopStreaming(Pip pip);
        int GetBatteryLevel(Pip pip);
        PipBatteryStatus GetBatteryStatus(Pip pip);
        string GetVersion(Pip pip);
        string GetName(Pip pip);
        string GetRegisteredPipName(int registeredIndex);
        long GetRegisteredPipId(int registeredIndex);
        string GetDiscoveredPipName(int discoveredIndex);
        long GetDiscoveredPipId(int discoveredIndex);
        bool GetIsActive(Pip pip);
    }

    class PipBridge
    {
        static IPipBridge ms_instance;

        internal static IPipBridge Instance
        {
            get
            {
                if ( ms_instance == null )
                {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
					ms_instance = (IPipBridge)new PipOsxBridge();
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
					ms_instance = (IPipBridge)new PipPluginWindows.PipWinBridge();
#elif UNITY_IPHONE
					ms_instance = (IPipBridge)new PipIosBridge();
#elif UNITY_ANDROID
					ms_instance = (IPipBridge)new PipAndroidBridge();
#endif
                }

                return ms_instance;
            }
        }
    }
}
