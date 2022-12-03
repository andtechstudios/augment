using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

	public class HierarchyShortcuts : MonoBehaviour
	{

		[MenuItem("GameObject/Utility/Alphabetize", false, 20)]
		public static void Sort()
		{
			IEnumerable<Transform> transforms;
			if (Selection.transforms.Length > 1)
			{
				transforms = Selection.transforms;
			}
			else if (Selection.transforms.Length == 1)
			{
				var root = Selection.activeTransform;
				transforms = root.GetComponentsInChildren<Transform>()
					.Where(x => x != root);
			}
			else
			{
				return;
			}

			Undo.RecordObjects(transforms.Select(x => x.transform).ToArray(), "Sort selection");

			foreach (var transform in transforms.OrderBy(x => x.name, new AlphanumericComparator()))
			{
				transform.SetAsLastSibling();
			}
		}

		[MenuItem("GameObject/Utility/Assign Numbers", false, 21)]
		public static void FormatTree()
		{
			var root = Selection.activeGameObject;
			var children = root.transform.GetComponentsInChildren<Transform>()
				.Where(x => x != root.transform)
				.OrderBy(x => x.gameObject.name)
				.Select(x => x.gameObject)
				.ToArray();

			Undo.RecordObjects(children, "Format children");

			var table = new Dictionary<string, int>();

			foreach (var child in children)
			{
				var baseName = Regex.Replace(child.gameObject.name, @"\s\(.+\)$", string.Empty);

				if (table.TryGetValue(baseName, out var count))
				{
					table.Remove(baseName);
					table.Add(baseName, count + 1);
				}
				else
				{
					table.Add(baseName, count);
				}

				child.gameObject.name = $"{baseName} ({table[baseName]})";
			}
		}
	}
}
