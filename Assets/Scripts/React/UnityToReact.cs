using System.Runtime.InteropServices;

namespace Solcery.React
{
    public sealed class UnityToReact : MonoBehaviourSingleton<UnityToReact>
    {
        [DllImport("__Internal")] private static extern void OnUnityLoaded(string message);
        [DllImport("__Internal")] private static extern void OnGameOverPopupButtonClicked();
        [DllImport("__Internal")] private static extern void OpenLinkInNewTab(string link);
        [DllImport("__Internal")] private static extern void SendCommand(string command);
        
        public void CallOnUnityLoaded()
        {
            UnityEngine.Debug.Log("CallOnUnityLoaded");
            
#if (UNITY_WEBGL && !UNITY_EDITOR)
            OnUnityLoaded ("message");
#endif
        }

        public void CallOnGameOverPopupButtonClicked()
        {
            UnityEngine.Debug.Log("CallOnGameOverPopupButtonClicked");

#if (UNITY_WEBGL && !UNITY_EDITOR)
            OnGameOverPopupButtonClicked ();
#endif
        }

        public void CallOpenLinkInNewTab(string link)
        {
            UnityEngine.Debug.Log($"CallOpenLinkInNewTab {link}");
            
#if (UNITY_WEBGL && !UNITY_EDITOR)
            OpenLinkInNewTab (link);
#endif
        }

        public void CallSendCommand(string command)
        {
            UnityEngine.Debug.Log($"CallSendCommand: {command}");

#if (UNITY_WEBGL && !UNITY_EDITOR)
            SendCommand(command);
#endif
        }
    }
}