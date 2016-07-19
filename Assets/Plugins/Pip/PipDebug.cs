using System;
using System.Diagnostics;

namespace Galvanic.PipPlugin
{
    public class PipDebug
    {
        static bool m_enabled = false;

        [Conditional("UNITY_EDITOR")]
        public static void Enable(bool enable)
        {
            m_enabled = enable;
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            if ( m_enabled )
                UnityEngine.Debug.Log(message);
        } 
    }
}
