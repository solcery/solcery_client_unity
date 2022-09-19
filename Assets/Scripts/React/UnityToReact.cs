using System.Runtime.InteropServices;

namespace Solcery.React
{
    public sealed class UnityToReact : MonoBehaviourSingleton<UnityToReact>
    {
        [DllImport("__Internal")] private static extern void OnGameStateConfirmed();
        [DllImport("__Internal")] private static extern void OnUnityLoadProgress(string progress);
        [DllImport("__Internal")] private static extern void OnUnityLoaded(string message);
        [DllImport("__Internal")] private static extern void OnGameOverPopupButtonClicked();
        [DllImport("__Internal")] private static extern void OpenLinkInNewTab(string link);
        [DllImport("__Internal")] private static extern void SendCommand(string command);

        public void CallOnGameStateConfirmed()
        {
            UnityEngine.Debug.Log("OnGameStateConfirmed");
            
#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            OnGameStateConfirmed();
#endif
        }
        
        public void CallOnUnityLoadProgress(string progress)
        {
            UnityEngine.Debug.Log($"CallOnUnityLoadProgress {progress}");
            
#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            OnUnityLoadProgress (progress);
#endif
        }
        
        public void CallOnUnityLoaded(string metadata)
        {
            UnityEngine.Debug.Log("CallOnUnityLoaded");
            
#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            OnUnityLoaded (metadata);
#endif
        }

        public void CallOnGameOverPopupButtonClicked()
        {
            UnityEngine.Debug.Log("CallOnGameOverPopupButtonClicked");

#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            OnGameOverPopupButtonClicked ();
#endif
        }

        public void CallOpenLinkInNewTab(string link)
        {
            UnityEngine.Debug.Log($"CallOpenLinkInNewTab {link}");
            
#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            OpenLinkInNewTab (link);
#endif
        }

        public void CallSendCommand(string command)
        {
            UnityEngine.Debug.Log($"CallSendCommand: {command}");

#if (UNITY_WEBGL && !UNITY_EDITOR && !LOCAL_SIMULATION)
            SendCommand(command);
#endif
        }
    }
}