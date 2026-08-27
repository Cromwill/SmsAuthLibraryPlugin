using UnityEditor;
using UnityEngine;

public class BuildBundles
{
    [MenuItem("Tools/Build AssetBundles")]
    static void Build()
    {
        string outputPath = "Assets/AssetBundles";

        if (!System.IO.Directory.Exists(outputPath))
            System.IO.Directory.CreateDirectory(outputPath);

        BuildTarget target = BuildTarget.Android;
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, target);

        Debug.Log("AssetBundles builded in " + outputPath);
    }
}
