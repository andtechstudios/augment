using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

    [InitializeOnLoad]
    public static class AutoRefresher
    {

        static AutoRefresher()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
            EditorApplication.quitting += Teardown;

            Initialize();
        }

		static void Initialize()
        {
            if (!SessionState.GetBool("autoRefreshInitialized", false))
            {
                Setup();

                if (useDeferredRefresh)
                {

                    AssetDatabase.Refresh();
                    var paths = AssetDatabase.FindAssets("t:script")
                        .Select(x => AssetDatabase.GUIDToAssetPath(x))
                        .Where(x => x.Contains("Editor"));
                    var path = paths.FirstOrDefault();
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.ImportAsset(path);
                    }

                    SessionState.SetBool("autoRefreshInitialized", true);
                }
            }

            if (useDeferredRefresh)
			{
                Debug.Log("Deferred refresh is enabled");
            }
        }

        private static bool prefsLoaded = false;
        private static bool useDeferredRefresh = false;

        [PreferenceItem("Andtech/Augment")]
        private static void CustomPreferencesGUI()
        {
            if (!prefsLoaded)
            {
                Setup();
            }

            //EditorGUILayout.LabelField("Version: x.xx");
            useDeferredRefresh = EditorGUILayout.Toggle(new GUIContent("Use Deferred Refresh", "When enabled, the asset database will not be refreshed automatically.\n\nThis has no effect if the \"Auto Refresh\" preference is on."), useDeferredRefresh);

            if (GUI.changed)
            {
                EditorPrefs.SetBool(nameof(useDeferredRefresh), useDeferredRefresh);

                if (useDeferredRefresh)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        static void Setup()
        {
            useDeferredRefresh = EditorPrefs.GetBool(nameof(useDeferredRefresh), false);
            prefsLoaded = true;

            if (useDeferredRefresh)
            {
                Enable();
            }
            else
			{
                Disable();
			}
        }

        static void Teardown()
        {
            Disable();
        }

        static void Enable()
        {
            AssetDatabase.DisallowAutoRefresh();
        }

        static void Disable()
        {
            AssetDatabase.AllowAutoRefresh();
        }

        static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
