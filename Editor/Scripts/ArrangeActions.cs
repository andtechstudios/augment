using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

	public static class ArrangeActions
	{

		[MenuItem("Edit/Andtech/Order/Move Back One %[")]
		public static void MoveBackOne()
		{
			var list = Selection.gameObjects
				.OrderBy(x => x.transform.GetSiblingIndex())
				.ToList();

			if (list.Count > 0)
			{
				int rootIndex = list[0].transform.GetSiblingIndex();
				rootIndex = Mathf.Max(0, rootIndex - 1);

				for (int i = 0; i < list.Count; i++)
				{
					list[i].transform.SetSiblingIndex(rootIndex + i);
				}
			}
		}

		[MenuItem("Edit/Andtech/Order/Move Forward One %]")]
		public static void MoveForwardOne()
		{
			var list = Selection.gameObjects
				.OrderBy(x => x.transform.GetSiblingIndex())
				.ToList();

			if (list.Count > 0)
			{
				var n = list.Count;
				int rootIndex = list[n - 1].transform.GetSiblingIndex() - (n - 1) + 1;

				int blocker = int.MaxValue;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					var gameObject = list[i];
					var initialIndex = gameObject.transform.GetSiblingIndex();
					
					if (initialIndex + 1 == blocker)
					{
						blocker = initialIndex;
					}
					else
					{
						list[i].transform.SetSiblingIndex(rootIndex + i);
						var finalIndex = list[i].transform.GetSiblingIndex();

						if (initialIndex == finalIndex)
						{
							blocker = finalIndex;
						}
					}

					blocker = gameObject.transform.GetSiblingIndex();
				}
			}
		}

		[MenuItem("Edit/Andtech/Order/Move To Back #%[")]
		public static void MoveToBack()
		{
			var list = Selection.gameObjects
				.OrderBy(x => x.transform.GetSiblingIndex())
				.Reverse()
				.ToList();

			for (int i = 0; i < list.Count; i++)
			{
				var gameObject = list[i];
				gameObject.transform.SetAsFirstSibling();
			}
		}

		[MenuItem("Edit/Andtech/Order/Move To Front #%]")]
		public static void MoveToFront()
		{
			var list = Selection.gameObjects
				.OrderBy(x => x.transform.GetSiblingIndex())
				.ToList();

			for (int i = 0; i < list.Count; i++)
			{
				var gameObject = list[i];
				gameObject.transform.SetAsLastSibling();
			}
		}
	}

}
