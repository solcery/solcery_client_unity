using System;
using System.IO;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.Networking;
#endif

namespace Solcery.Utils
{
    public static class StreamingAssetsUtils
    {
#if UNITY_EDITOR
        public static void LoadText(string path, Action<string> result)
        {
            var pathToGameContent = Path.Combine(Application.streamingAssetsPath, path);
            if (File.Exists(pathToGameContent))
            {
                result.Invoke(File.ReadAllText(pathToGameContent));
            }
            else
            {
                result.Invoke(null);
            }
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        private static readonly Dictionary<AsyncOperation, Tuple<UnityWebRequest, Action<string>>> Callbacks = new();
        
        public static void LoadText(string path, Action<string> result)
        {
            var pathToGameContent = Path.Combine(Application.streamingAssetsPath, path);
            Debug.Log($"Resource path {pathToGameContent}");

            var request = UnityWebRequest.Get(pathToGameContent);
            var wr = request.SendWebRequest();
            Callbacks.Add(wr, new Tuple<UnityWebRequest, Action<string>>(request, result));
            wr.completed += OnCompleted;
        }

        private static void OnCompleted(AsyncOperation obj)
        {
            Debug.Log("OnCompleted");
            
            obj.completed -= OnCompleted;

            if (Callbacks.TryGetValue(obj, out var callback))
            {
                Debug.Log($"Result {callback.Item1.downloadHandler.text}");
                callback.Item2.Invoke(callback.Item1.downloadHandler.text);
                callback.Item1.Dispose();
            }
        }
#endif
    }
}