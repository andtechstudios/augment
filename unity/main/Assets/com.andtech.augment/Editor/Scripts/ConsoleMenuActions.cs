using System;
using System.Reflection;
using UnityEditor;

namespace Andtech.Augment
{

	public static class ConsoleMenuActions
	{
		private static readonly Type consoleWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ConsoleWindow");

		[MenuItem("Edit/Andtech/Clear Console %#L")]
		public static void ClearConsole()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
			Type type = assembly.GetType("UnityEditor.LogEntries");
			MethodInfo method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
		}
	}
}
