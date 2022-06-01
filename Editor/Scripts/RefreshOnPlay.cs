using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

    [InitializeOnLoad]
    public static class RefreshOnPlay
    {

        static RefreshOnPlay()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            Initialize();
        }

        private static bool prefsLoaded = false;
        private static bool refreshOnPlay = false;
        private static bool logRefreshOnPlay = false;

        [PreferenceItem("Andtech/Augment")]
        static void CustomPreferencesGUI()
        {
            if (!prefsLoaded)
            {
                Initialize();
            }

            //EditorGUILayout.LabelField("Version: x.xx");
            refreshOnPlay = EditorGUILayout.Toggle(new GUIContent("Refresh On Play", "When enabled, the asset database will be refreshed when you enter Play Mode."), refreshOnPlay);
            logRefreshOnPlay = EditorGUILayout.Toggle(new GUIContent("Log", "Log whenever Refresh on Play is triggered."), logRefreshOnPlay);

            if (GUI.changed)
            {
                EditorPrefs.SetBool(nameof(refreshOnPlay), refreshOnPlay);
                EditorPrefs.SetBool(nameof(logRefreshOnPlay), logRefreshOnPlay);
            }
        }

        static void Initialize()
        {
            refreshOnPlay = EditorPrefs.GetBool(nameof(refreshOnPlay), false);
            logRefreshOnPlay = EditorPrefs.GetBool(nameof(logRefreshOnPlay), false);
            prefsLoaded = true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (refreshOnPlay)
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    if (logRefreshOnPlay)
					{
                        Debug.Log("Refresh on Play...");
					}
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
