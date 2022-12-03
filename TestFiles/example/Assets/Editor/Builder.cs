using Cyberstar.Builder.Editor;
using UnityEditor;

public static class Builder
{

	[MenuItem("Custom/Build")]
	public static void Build()
	{
		Cyberstar.Builder.Builder.OnPreBuild += BuildProcessor.RebuildPlayerContent;
		Cyberstar.Builder.Builder.Build();
	}
}
