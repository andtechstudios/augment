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

	public static class ConsoleMenuActions
	{
		private static bool hasConsole;
		private static readonly Type consoleWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ConsoleWindow");

		[MenuItem("Edit/Andtech/Clear Console %#L")]
		public static void ClearConsole()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
			Type type = assembly.GetType("UnityEditor.LogEntries");
			MethodInfo method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
		}

		[MenuItem("Edit/Andtech/Toggle Console ^`")]
		public static void ToggleConsole()
		{
			if (hasConsole)
			{
				var window = EditorWindow.GetWindow(consoleWindowType, false, "Console");
				window.Close();

				hasConsole = false;
			}
			else
			{
				var window = EditorWindow.GetWindow(consoleWindowType, false, "Console");
				window.Show();

				hasConsole = true;
			}
		}
	}
}
