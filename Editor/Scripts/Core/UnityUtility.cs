using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Andtech.Augment
{

    public static class UnityUtility
    {

		public static string GetProjectPath() => Path.GetDirectoryName(Application.dataPath);

        public static bool IsUnityProjectPath(string path)
        {
            var projectSettingsDir = Path.Join(path, "ProjectSettings");
            var projectSettingsAssetPath = Path.Join(projectSettingsDir, "ProjectSettings.asset");

            return File.Exists(projectSettingsAssetPath);
		}

		public static string GetProductName(string projectPath)
		{
			var projectSettingsDir = Path.Join(projectPath, "ProjectSettings");
			var projectSettingsAssetPath = Path.Join(projectSettingsDir, "ProjectSettings.asset");

			var content = File.ReadAllText(projectSettingsAssetPath);
			var match = Regex.Match(content, @"\bproductName:\s(?<value>.*)$", RegexOptions.Multiline);

			return match.Groups["value"].Value;
		}
	}
}
