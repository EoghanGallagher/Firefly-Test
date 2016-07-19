#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace Galvanic.PipPlugin
{
	class PipOsxBridge : IPipBridge
	{
		public delegate void OsxPluginCallback(string context, string message);

		[AttributeUsage (AttributeTargets.Method)]
		public sealed class MonoPInvokeCallbackAttribute : Attribute 
		{
			public MonoPInvokeCallbackAttribute(Type t) {}
		}

		[MonoPInvokeCallback(typeof(OsxPluginCallback))]
		public static void NativeCallback(string context, string message) 
		{
			QueueMessage(context, message);
		}

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

		private static Queue<PipMessage> ms_messageQueue = new Queue<PipMessage>();

		static void QueueMessage(string methodName, object value)
		{
			PipMessage message = new PipMessage(methodName, value);
			lock (ms_messageQueue)
			{
				ms_messageQueue.Enqueue(message);
			}
		}

		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_initPlugin(OsxPluginCallback callback);
		[DllImport("UnityOsxBridge")]
		private static extern bool osxBridge_isInitialized();
		[DllImport("UnityOsxBridge")]
		private static extern bool osxBridge_isBluetoothActive();
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_resetAll();
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_discoverPips();
		[DllImport("UnityOsxBridge")]
		private static extern bool osxBridge_isDiscovering();
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_cancelDiscovery();
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getNumDiscoveredPips();
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getNumRegisteredPips();
		[DllImport("UnityOsxBridge")]
		private static extern IntPtr osxBridge_getNameForDiscoveredPip(int discoveredIndex);
		[DllImport("UnityOsxBridge")]
		private static extern IntPtr osxBridge_getNameForRegisteredPip(int registeredIndex);
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getPipIDForDiscoveredPip(int discoveredIndex);
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getPipIDForRegisteredPip(int registeredIndex);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_registerPip(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_unregisterPip(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_unregisterAll();
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_connectPip(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_disconnectPip(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_startStreaming(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_stopStreaming(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestPowerOff(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestReset(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestPing(int pipID, bool flashLED);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestVersion(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestGetName(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_requestSetName(int pipID, string name);
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getBatteryLevel(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern int osxBridge_getBatteryEvent(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern IntPtr osxBridge_getAllowedPipNameChars();
		[DllImport("UnityOsxBridge")]
		private static extern IntPtr osxBridge_getName(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern bool osxBridge_isPipActive(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern IntPtr osxBridge_getVersion(int pipID);
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_suspendPips();
		[DllImport("UnityOsxBridge")]
		private static extern void osxBridge_resumePips();
		
		public PipOsxBridge()
		{		
			if ( osxBridge_isInitialized() )
				QueueMessage("ReceiveDiscoveryMessage", "pip_plugin:discovery:initialized:0");
			
			osxBridge_initPlugin(NativeCallback);
		}
		
		public void Update()
		{
			while (ms_messageQueue.Count > 0)
			{
				PipMessage msg;
				lock (ms_messageQueue)
				{
					msg = ms_messageQueue.Dequeue();
				}
				if (msg.methodName.Length > 0)
					Galvanic.PipPlugin.PipManager.Instance.SendMessage(msg.methodName, msg.value);
			}
		}

		public bool BluetoothActive{ get { return osxBridge_isBluetoothActive(); } }
		
		public void ResetManager()
		{
			osxBridge_resetAll();
		}
		
		public void ShutDownManager() 
		{
			osxBridge_resetAll();
		}
		
		public void SuspendPips()
		{
			osxBridge_suspendPips();
		}
		
		public void ResumePips()
		{
			osxBridge_resumePips();
		}
		
		public void DiscoverPips()
		{
			osxBridge_discoverPips();
		}
		
		public void CancelDiscovery()
		{
			osxBridge_cancelDiscovery();
		}
		
		public int NumDiscoveredPips { get { return osxBridge_getNumDiscoveredPips(); } }
		
		public void RegisterPip(Pip pip)
		{
			osxBridge_registerPip((int)pip.PipID);
		}
		
		public void UnregisterPip(Pip pip)
		{
			osxBridge_unregisterPip((int)pip.PipID);
		}
		
		public void UnregisterAllPips()
		{
			osxBridge_unregisterAll();
		}
		
		public int NumRegisteredPips { get { return osxBridge_getNumRegisteredPips(); }  }
		
		public void ConnectPip(Pip pip)
		{
			osxBridge_connectPip((int)pip.PipID);
		}
		
		public void DisconnectPip(Pip pip)
		{
			osxBridge_disconnectPip((int)pip.PipID);
		}
		
		public void RequestVersion(Pip pip)
		{
			osxBridge_requestVersion((int)pip.PipID);
		}
		
		public void RequestGetName(Pip pip)
		{
			osxBridge_requestGetName((int)pip.PipID);
		}
		
		public void RequestPing(Pip pip, bool flashLED)
		{
			osxBridge_requestPing((int)pip.PipID, flashLED);
		}
		
		public void RequestSetName(Pip pip, string name)
		{
			osxBridge_requestSetName((int)pip.PipID, name);
		}
		
		public void RequestReset(Pip pip)
		{
			osxBridge_requestReset((int)pip.PipID);
		}
		
		public void RequestPowerOff(Pip pip)
		{
			osxBridge_requestPowerOff((int)pip.PipID);
		}
		
		public void StartStreaming(Pip pip)
		{
			osxBridge_startStreaming((int)pip.PipID);
		}
		
		public void StopStreaming(Pip pip)
		{
			osxBridge_stopStreaming((int)pip.PipID);
		}
		
		public int GetBatteryLevel(Pip pip)
		{
			return osxBridge_getBatteryLevel((int)pip.PipID);
		}
		
		public PipBatteryStatus GetBatteryStatus(Pip pip)
		{
			return (PipBatteryStatus)osxBridge_getBatteryEvent((int)pip.PipID);
		}
		
		public string GetVersion(Pip pip)
		{
			return Marshal.PtrToStringAnsi(osxBridge_getVersion((int)pip.PipID));
		}
		
		public char[] AllowedPipNameChars
		{
			get
			{
				string allowed = Marshal.PtrToStringAnsi(osxBridge_getAllowedPipNameChars());
				return allowed.ToCharArray();
			}
		}
		
		public bool GetIsActive(Pip pip)
		{
			return osxBridge_isPipActive((int)pip.PipID);
		}
		
		public string GetName(Pip pip)
		{
			return Marshal.PtrToStringAnsi(osxBridge_getName((int)pip.PipID));
		}
		
		public string GetRegisteredPipName(int registeredIndex)
		{
			return Marshal.PtrToStringAnsi(osxBridge_getNameForRegisteredPip(registeredIndex));
		}
		
		public long GetRegisteredPipId(int registeredIndex)
		{
			return osxBridge_getPipIDForRegisteredPip(registeredIndex);
		}
		
		public string GetDiscoveredPipName(int discoveredIndex)
		{
			return Marshal.PtrToStringAnsi(osxBridge_getNameForDiscoveredPip(discoveredIndex));
		}
		
		public long GetDiscoveredPipId(int discoveredIndex)
		{
			return osxBridge_getPipIDForDiscoveredPip(discoveredIndex);
		}
	}
}

#endif
