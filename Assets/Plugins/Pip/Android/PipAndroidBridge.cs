#if UNITY_ANDROID

using System;
using UnityEngine;

namespace Galvanic.PipPlugin
{
    class PipAndroidBridge : IPipBridge
    {
        static AndroidJavaObject m_bridge;

        public PipAndroidBridge()
        {
            using( var pluginClass = new AndroidJavaClass("com.galvanic.pipandroidbridge.PipUnityPlugin"))
            {
                m_bridge = pluginClass.CallStatic<AndroidJavaObject>("instance");
            }
        }

        public void Update() { }

        public char [] AllowedPipNameChars 
        { 
            get 
            {
                return m_bridge.Call<string>("getAllowedPipNameChars").ToCharArray();
            } 
        }

        public bool BluetoothActive
        { 
            get { return (m_bridge.Call<int>("isBluetoothActive") == 1); }
        }

        public void ResetManager()
        {
            m_bridge.Call("resetAll");
        }

        public void ShutDownManager() {}

        public void SuspendPips()
        {
            m_bridge.Call("suspendPips");
        }

        public void ResumePips()
        {
            m_bridge.Call("resumePips");
        }

        public void DiscoverPips()
        {
            m_bridge.Call("discoverPips");
        }

        public void CancelDiscovery()
        {
            m_bridge.Call("cancelDiscovery");
        }

        public int NumDiscoveredPips { get { return m_bridge.Call<int>("getNumDiscoveredPips"); } }

        public void RegisterPip(Pip pip)
        {
            m_bridge.Call("registerPip", (int)pip.PipID);
        }

        public void UnregisterPip(Pip pip)
        {
            m_bridge.Call("unregisterPip", (int)pip.PipID);
        }

        public void UnregisterAllPips()
        {
            m_bridge.Call("unregisterAllPips");
        }

        public int NumRegisteredPips { get { return m_bridge.Call<int>("getNumRegisteredPips"); } }

        public void ConnectPip(Pip pip)
        {
            m_bridge.Call("connectPip", (int)pip.PipID);
        }

        public void DisconnectPip(Pip pip)
        {
            m_bridge.Call("disconnectPip", (int)pip.PipID);
        }

        public void RequestVersion(Pip pip)
        {
            m_bridge.Call("requestVersion", (int)pip.PipID);
        }

        public void RequestGetName(Pip pip)
        {
            m_bridge.Call("requestGetName", (int)pip.PipID);
        }

        public void RequestSetName(Pip pip, string name)
        {
            m_bridge.Call("requestSetName", (int)pip.PipID, name);
        }

        public void RequestPing(Pip pip, bool flashLED)
        {
            m_bridge.Call("requestPing", (int)pip.PipID, (flashLED == false ? 0 : 1));
        }

        public void RequestReset(Pip pip)
        {
            m_bridge.Call("requestReset", (int)pip.PipID);
        }

        public void RequestPowerOff(Pip pip)
        {
            m_bridge.Call("requestPowerOff", (int)pip.PipID);
        }

        public void StartStreaming(Pip pip)
        {
            m_bridge.Call("startStreaming", (int)pip.PipID);
        }

        public void StopStreaming(Pip pip)
        {
            m_bridge.Call("stopStreaming", (int)pip.PipID);
        }

        public int GetBatteryLevel(Pip pip)
        {
            return m_bridge.Call<int>("getBatteryLevel", (int)pip.PipID);
        }

        public PipBatteryStatus GetBatteryStatus(Pip pip)
        {
            return (PipBatteryStatus)m_bridge.Call<int>("getBatteryEvent", (int)pip.PipID);
        }

        public string GetVersion(Pip pip)
        {
            return m_bridge.Call<string>("getVersion", (int)pip.PipID);
        }

        public string GetName(Pip pip)
        {
            return m_bridge.Call<string>("getName", (int)pip.PipID);
        }

        public string GetRegisteredPipName(int registeredIndex)
        {
            return m_bridge.Call<string>("getNameForRegisteredPip", registeredIndex);
        }

        public long GetRegisteredPipId(int registeredIndex)
        {
            return m_bridge.Call<int>("getPipIDForRegisteredPip", registeredIndex);
        }

        public string GetDiscoveredPipName(int discoveredIndex)
        {
            return m_bridge.Call<string>("getNameForDiscoveredPip", discoveredIndex);
        }

        public long GetDiscoveredPipId(int discoveredIndex)
        {
            return m_bridge.Call<int>("getPipIDForDiscoveredPip", discoveredIndex);
        }

        public bool GetIsActive(Pip pip)
        {
            return m_bridge.Call<Boolean>("getIsActive", (int)pip.PipID);
        }
    }
}

#endif
