using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.ShortcutManagement;
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

		[MenuItem("Edit/Andtech/Run Maximized #_F5")]
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

		[MenuItem("File/Show Project in Explorer %#E", priority = 199)]
		public static void ShowInExplorer()
		{
			string path = Directory.GetParent(Application.dataPath).FullName;
			EditorUtility.RevealInFinder(path);
		}

		[MenuItem("Edit/Launch...", priority = 185)]
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

		[MenuItem("File/Force Save %#&S", false, 180)]
		public static void ForceSave()
		{
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Shortcuts/Close Window Tab &W")]
		static void CloseTab()
		{
			EditorWindow focusedWindow = EditorWindow.focusedWindow;
			if (focusedWindow != null)
			{
				focusedWindow.Close();
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
