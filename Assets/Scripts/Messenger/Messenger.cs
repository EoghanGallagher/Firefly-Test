/*
 * Advanced C# messenger by Ilya Suzdalnitski. V1.0
 *
 * Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 *
 * Features:
    * Prevents a MissingReferenceException because of a reference to a destroyed message handler.
    * Option to log all messages
    * Extensive error detection, preventing silent bugs
 *
 * Usage examples:
    1. Messenger.AddListener<GameObject>("prop collected", PropCollected);
       Messenger.Broadcast<GameObject>("prop collected", prop);
    2. Messenger.AddListener<float>("speed changed", SpeedChanged);
       Messenger.Broadcast<float>("speed changed", 0.5f);
 *
 * Messenger cleans up its evenTable automatically upon loading of a new level.
 *
 * Don't forget that the messages that should survive the cleanup, should be marked with Messenger.MarkAsPermanent(string)
 *
 */


//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define REQUIRE_LISTENER

using System;
using System.Collections.Generic;
using UnityEngine;

static internal class Messenger {
    #region Internal variables

    public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
	
	private const string _gameObjectStringFormat = "GameObjectID_{0}_{1}";

    #endregion
    #region Helper methods
    // Call this static method to print out a list of registered events   
    public static void PrintEventTable()
    {
        Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");
       
        foreach (KeyValuePair<string, Delegate> pair in eventTable) {
            Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
        }
       
        Debug.Log("\n");
    }
    #endregion
   
    #region Message logging and exception throwing
    public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded) {
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
        Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif
       
        if (!eventTable.ContainsKey(eventType)) {
            eventTable.Add(eventType, null );
        }

        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
#if LOG_ALL_MESSAGES
        Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif
       
        if (eventTable.ContainsKey(eventType)) {
            Delegate d = eventTable[eventType];

            if (d == null) {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            } else if (d.GetType() != listenerBeingRemoved.GetType()) {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        } else {
            throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    public static void OnListenerRemoved(string eventType) {
        if (eventTable[eventType] == null) {
            eventTable.Remove(eventType);
        }
    }

    public static void OnBroadcasting(string eventType) {
#if REQUIRE_LISTENER
        if (!eventTable.ContainsKey(eventType)) {
            throw new BroadcastException(string.Format("Broadcasting message \"{0}\" but no listener found.", eventType));
        }
#endif
    }

    public static BroadcastException CreateBroadcastSignatureException(string eventType) {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    public class BroadcastException : Exception {
        public BroadcastException(string msg)
            : base(msg) {
        }
    }

    public class ListenerException : Exception {
        public ListenerException(string msg)
            : base(msg) {
        }
    }
    #endregion
   
    #region AddListener
    // No parameters
    public static void AddListener(string eventType, Callback handler) {
        OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback)eventTable[eventType] + handler;
    }
   
    // Single parameter
    public static void AddListener<T>(string eventType, Callback<T> handler) {
        OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
    }
   
    // Two parameters
    public static void AddListener<T, U>(string eventType, Callback<T, U> handler) {
        OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
    }
   
    // Three parameters
    public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler) {
        OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
    }
    #endregion
	
	#region AddListener with GameObject
    // No parameters
    public static void AddListener(GameObject gameObject, string eventType, Callback handler) {
        eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
		Messenger.AddListener(eventType, handler);
    }
   
    // Single parameter
    public static void AddListener<T>(GameObject gameObject, string eventType, Callback<T> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.AddListener<T>(eventType, handler);
    }
   
    // Two parameters
    public static void AddListener<T, U>(GameObject gameObject, string eventType, Callback<T, U> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.AddListener<T, U>(eventType, handler);
    }
   
    // Three parameters
    public static void AddListener<T, U, V>(GameObject gameObject, string eventType, Callback<T, U, V> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.AddListener<T, U, V>(eventType, handler);
    }
    #endregion
   
    #region RemoveListener
    // No parameters
    public static void RemoveListener(string eventType, Callback handler) {
        OnListenerRemoving(eventType, handler);  
        eventTable[eventType] = (Callback)eventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
   
    // Single parameter
    public static void RemoveListener<T>(string eventType, Callback<T> handler) {
        OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
   
    // Two parameters
    public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler) {
        OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
   
    // Three parameters
    public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler) {
        OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
    #endregion
	
	#region RemoveListener with GameObject
    // No parameters
    public static void RemoveListener(GameObject gameObject, string eventType, Callback handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.RemoveListener(eventType, handler);
    }
   
    // Single parameter
    public static void RemoveListener<T>(GameObject gameObject, string eventType, Callback<T> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.RemoveListener<T>(eventType, handler);
    }
   
    // Two parameters
    public static void RemoveListener<T, U>(GameObject gameObject, string eventType, Callback<T, U> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.RemoveListener<T, U>(eventType, handler);
    }
   
    // Three parameters
    public static void RemoveListener<T, U, V>(GameObject gameObject, string eventType, Callback<T, U, V> handler) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
        Messenger.RemoveListener<T, U, V>(eventType, handler);
    }
    #endregion
   
    #region Broadcast
    // Broadcast with no parameters
    public static void Broadcast(string eventType) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
       
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Callback callback = d as Callback;

            if (callback != null) {
                callback();
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
	
	// Broadcast with GameObject and no parameters
	public static void Broadcast(GameObject gameObject, string eventType) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
		Messenger.Broadcast(eventType);
    }
   
    // Broadcast with single parameter
    public static void Broadcast<T>(string eventType, T arg1) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
       
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Callback<T> callback = d as Callback<T>;
           
            if (callback != null) {
                callback(arg1);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
	
	// Broadcast with GameObject and single parameter
	public static void Broadcast<T>(GameObject gameObject, string eventType, T arg1) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
		Messenger.Broadcast<T>(eventType, arg1);
    }
   
    // Broadcast with two parameters
    public static void Broadcast<T, U>(string eventType, T arg1, U arg2) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
       
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Callback<T, U> callback = d as Callback<T, U>;
           
            if (callback != null) {
                callback(arg1, arg2);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
	
	// Broadcast with GameObject and two parameters
	public static void Broadcast<T, U>(GameObject gameObject, string eventType, T arg1, U arg2) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
		Messenger.Broadcast<T, U>(eventType, arg1, arg2);
    }
   
    // Broadcast with three parameters
    public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
       
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Callback<T, U, V> callback = d as Callback<T, U, V>;

            if (callback != null) {
                callback(arg1, arg2, arg3);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
	
	//  Broadcast with GameObject and three parameters
	public static void Broadcast<T, U, V>(GameObject gameObject, string eventType, T arg1, U arg2, V arg3) {
		eventType = string.Format(_gameObjectStringFormat, gameObject.GetInstanceID().ToString(), eventType);
		Messenger.Broadcast<T, U, V>(eventType, arg1, arg2, arg3);
    }
    #endregion
}