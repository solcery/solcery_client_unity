using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solcery.React
{
    public sealed class ReactToUnity : MonoBehaviourSingleton<ReactToUnity>
    {
        public const string EventOnSetWalletConnected = "EventOnSetWalletConnected";
        public const string EventOnOpenGameOverPopup = "EventOnOpenGameOverPopup";
        public const string EventOnUpdateGameContent = "EventOnUpdateGameContent";
        public const string EventOnUpdateGameContentOverrides = "EventOnUpdateGameContentOverrides";
        public const string EventOnUpdateGameDisplay = "EventOnUpdateGameDisplay";
        public const string EventOnUpdateGameState = "EventOnUpdateGameState";
        
        private static readonly Dictionary<string, List<Action<string>>> Callbacks = new Dictionary<string, List<Action<string>>>();

        public static void CleanupAllCallbacks()
        {
            Callbacks.Clear();
        }

        public static void AddCallback(string eventKey, Action<string> callback)
        {
            if (!Callbacks.ContainsKey(eventKey))
            {
                Callbacks.Add(eventKey, new List<Action<string>>());
            }
            
            Callbacks[eventKey].Add(callback);
        }

        public static void RemoveCallback(string eventKey, Action<string> callback)
        {
            if (Callbacks.TryGetValue(eventKey, out var callbacks))
            {
                callbacks.Remove(callback);
            }
        }

        private static void CallAllCallbackForEventKey(string eventKey, string value)
        {
            if (Callbacks.TryGetValue(eventKey, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback?.Invoke(value);
                }
            }
        }

        public void SetWalletConnected(string data)
        {
            CallAllCallbackForEventKey(EventOnSetWalletConnected, data);
        }

        public void OpenGameOverPopup(string data)
        {
            CallAllCallbackForEventKey(EventOnOpenGameOverPopup, data);
        }

        public void UpdateGameContent(string data)
        {
            Debug.Log("Execute UpdateGameContent");
            CallAllCallbackForEventKey(EventOnUpdateGameContent, data);
        }
        
        public void UpdateGameContentOverrides(string data)
        {
            Debug.Log("Execute UpdateGameContentOverrides");
            CallAllCallbackForEventKey(EventOnUpdateGameContentOverrides, data);
        }

        public void UpdateGameDisplay(string data)
        {
            CallAllCallbackForEventKey(EventOnUpdateGameDisplay, data);
        }

        public void UpdateGameState(string data)
        {
            Debug.Log("Execute UpdateGameState");
            CallAllCallbackForEventKey(EventOnUpdateGameState, data);
        }
    }
}