using UnityEditor;
using UnityEngine;

namespace Solcery.Editor.CI.WebGl
{
    public class BuildWindow : EditorWindow
    {
        private static string BranchName
        {
            get
            {
#if UNITY_EDITOR
                return EditorPrefs.GetString("Branch", "main");
#else
                return false;
#endif
            }
            set
            {
#if UNITY_EDITOR
                EditorPrefs.SetString("Branch", value);
#endif
            }
        }

        [MenuItem("Build/WebGl/BuildWindow")]
        public static void ShowWindow()
        {
            GetWindow<BuildWindow>(false, "Builds", true);
        }
        
        void OnGUI()
        {
            if (GUILayout.Button("Develop"))
            {
                BuildWebGl.BuildDevelop();
            } 
            
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            BranchName = EditorGUILayout.TextField(BranchName);
            if (GUILayout.Button("Develop with cms"))
            {
                BuildWebGl.BuildDevelopWithCms(BranchName);
            } 
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            if (GUILayout.Button("Develop with local simulation"))
            {
                BuildWebGl.BuildDevelopWithLocalSimulation();
            } 
            
            GUILayout.Space(10);
            if (GUILayout.Button("Release"))
            {
                BuildWebGl.BuildRelease();
            } 
        }
    }
}
