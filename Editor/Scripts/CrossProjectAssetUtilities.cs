using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

namespace Andtech.Augment
{

	public class CrossProjectAssetUtilities : EditorWindow
	{
		private static string sourceGuid;
		private static string destinationRoot;
		
		static string candidateGuid;
		static string candidateDestinationRoot;

		[MenuItem("Assets/Merge/Pick Folder")]
		static void PickMerge()
		{
			sourceGuid = candidateGuid;

			Debug.Log($"Folder selected for merging. Now, choose 'Merge/Drop Folder'");
		}

		[MenuItem("Assets/Merge/Pick Folder", isValidateFunction: true)]
		static bool PickMerge_Validate()
		{
			switch (Selection.assetGUIDs.Length)
			{
				case 1:
					var guid = Selection.assetGUIDs[0];
					var destinationPath = AssetDatabase.GUIDToAssetPath(guid);
					if (!AssetDatabase.IsValidFolder(destinationPath))
					{
						return false;
					}
					candidateGuid = guid;
					return true;
				default:
					return false;
			}
		}

		[MenuItem("Assets/Merge/Drop Folder")]
		static void DropMerge()
		{
			destinationRoot = candidateDestinationRoot;
			DoMerge();

			sourceGuid = null;
		}

		[MenuItem("Assets/Merge/Drop Folder", isValidateFunction: true)]
		static bool DropMerge_Validate()
		{
			if (string.IsNullOrEmpty(sourceGuid))
			{
				return false;
			}

			string destinationPath;
			switch (Selection.assetGUIDs.Length)
			{
				case 0:
					destinationPath = string.Join(UnityUtility.GetProjectPath(), "Assets");
					break;
				case 1:
					destinationPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
					if (!AssetDatabase.IsValidFolder(destinationPath))
					{
						return false;
					}
					break;
				default:
					return false;
			}

			candidateDestinationRoot = destinationPath;

			return true;
		}

		static void DoMerge()
		{
			var sourceRoot = AssetDatabase.GUIDToAssetPath(sourceGuid);
			var root = Path.GetDirectoryName(sourceRoot);

			var relativePath = Path.GetRelativePath(root, sourceRoot);
			var destinationPath = Path.Join(destinationRoot, relativePath);
			CreateFolder(destinationPath);

			foreach (var directory in Directory.GetDirectories(sourceRoot))
			{
				TransferFolder(directory);
			}

			if (IsEmpty(sourceRoot))
			{
				AssetDatabase.DeleteAsset(sourceRoot);
			}

			AssetDatabase.Refresh();

			void TransferFolder(string path)
			{
				var relativePath = Path.GetRelativePath(root, path);
				var destinationPath = Path.Join(destinationRoot, relativePath);
				CreateFolder(destinationPath);

				foreach (var directory in Directory.EnumerateDirectories(path))
				{
					TransferFolder(directory);
				}

				foreach (var file in Directory.EnumerateFiles(path).Where(x => Path.GetExtension(x) != ".meta"))
				{
					TransferFile(file);
				}

				if (IsEmpty(path))
				{
					AssetDatabase.DeleteAsset(path);
				}
			}

			void TransferFile(string path)
			{
				var relativePath = Path.GetRelativePath(root, path);
				var destinationPath = Path.Join(destinationRoot, relativePath);
				var destinationDirectory = Path.GetDirectoryName(destinationPath);

				Debug.Log($"Merging '{path}' with '{destinationPath}'...");
				var status = AssetDatabase.MoveAsset(path, destinationPath);
				if (!string.IsNullOrEmpty(status))
				{
					Debug.LogError(status);
				}
			}
		}

		static bool IsEmpty(string path)
		{
			if (Directory.EnumerateFiles(path).Where(x => Path.GetExtension(x) != ".meta").Any())
			{
				return false;
			}

			if (Directory.EnumerateDirectories(path).Any())
			{
				return false;
			}

			return true;
		}

		static void CreateFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
			}
		}
	}
}

