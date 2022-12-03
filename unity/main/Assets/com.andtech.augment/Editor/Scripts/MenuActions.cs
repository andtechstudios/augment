using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

	public static class MenuActions
	{
		private static EditorWindow GameWindow => EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
		private static bool autoMaximize;
		private static bool wasMaximized;

		static MenuActions()
		{
			EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
		}

		[MenuItem("Edit/Andtech/Close Tab\\Window %#W")]
		static void CloseTab()
		{
			EditorWindow focusedWindow = EditorWindow.focusedWindow;
			if (focusedWindow)
			{
				focusedWindow.Close();
			}
		}

		[MenuItem("Edit/Andtech/Run _F4")]
		public static void Play()
		{
			EditorApplication.ExecuteMenuItem("Edit/Play");
		}

		[MenuItem("Edit/Andtech/Pause _F5")]
		public static void Pause()
		{
			EditorApplication.ExecuteMenuItem("Edit/Pause");
		}

		[MenuItem("Edit/Andtech/Step _F6")]
		public static void Step()
		{
			EditorApplication.ExecuteMenuItem("Edit/Step");
		}

		[MenuItem("Edit/Andtech/Run Maximized _#F5")]
		public static void PlayMaximized()
		{
			if (!EditorApplication.isPlaying)
			{
				autoMaximize = true;
				wasMaximized = GameWindow.maximized;

				GameWindow.maximized = true;
			}
			EditorApplication.ExecuteMenuItem("Edit/Play");
		}

		[MenuItem("File/Force Save %#S", false, 180)]
		public static void ForceSave()
		{
			AssetDatabase.SaveAssets();
		}

		[MenuItem("File/Force Reload Scripts %#R", false, 181)]
		public static void ForceReload()
		{
			EditorUtility.RequestScriptReload();
		}

		[MenuItem("Edit/Andtech/Maximize (Force) %#UP")]
		public static void MaximizeForce()
		{
			GameWindow.maximized = true;
		}

		[MenuItem("Edit/Andtech/Unmaximize (Force) %#DOWN")]
		public static void UnmaximizeForce()
		{
			GameWindow.maximized = false;
		}

		[MenuItem("File/Run Build...", priority = 215)]
		public static void Launch()
		{
			var buildDir = Path.Combine(Environment.CurrentDirectory, "builds");
			string extension;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				extension = ".exe";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				extension = ".app";
			}
			else
			{
				throw new InvalidOperationException("The current platform is not supported.");
			}

			var binaries = Directory.EnumerateFiles(buildDir, $"*{extension}", SearchOption.TopDirectoryOnly);
			if (binaries.Any())
			{
				var binary = binaries.First();
				Process.Start(binary);
			}
			else
			{
				UnityEngine.Debug.LogError($"No binaries found in {buildDir}");
			}
		}

		private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
		{
			if (obj == PlayModeStateChange.EnteredEditMode)
			{
				if (autoMaximize && !wasMaximized)
				{
					GameWindow.maximized = false;
				}

				autoMaximize = false;
			}
		}
	}
}
