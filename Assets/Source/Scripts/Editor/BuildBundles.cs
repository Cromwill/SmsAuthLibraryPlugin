using UnityEditor;
using UnityEngine;

public class BuildBundles
{
    private const string Android = nameof(Android);
    private const string iOS = nameof(iOS);

    [MenuItem("Tools/Build AssetBundles/Android Version")]
    static void BuildAndroid() => Build(Android, BuildTarget.Android);

    [MenuItem("Tools/Build AssetBundles/iOS Version")]
    static void BuildiOS() => Build(iOS, BuildTarget.iOS);

    static void Build(string targetFolder, BuildTarget buildTarget)
    {
        string outputPath = $"Assets/AssetBundles/{targetFolder}";

        if (System.IO.Directory.Exists(outputPath) == false)
            System.IO.Directory.CreateDirectory(outputPath);

        BuildTarget target = buildTarget;
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, target);

        Debug.Log($"{targetFolder} AssetBundles builded in: {outputPath}");
    }
}
