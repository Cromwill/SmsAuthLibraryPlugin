using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KinDzaDzaGames.AdvertisementPlugin.Editor
{
    public class ASMDEFGenerator : EditorWindow
    {
        [MenuItem("Advertisement plugin/ASMDEF Generator")]
        public static void ShowWindow()
        {
            GetWindow<ASMDEFGenerator>("ASMDEF Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create new ASMDEF files for YABBI", EditorStyles.boldLabel);

            if (GUILayout.Button("Create YABBI ASMDEFs"))
                CreateYabbiASMDEFs();

            if (GUILayout.Button("Edit YABBI ASMDEFs"))
                EditYabbiASMDEFs();

            if (GUILayout.Button("Check advertisement plugin ASMDEF"))
                CheckASMDEF();

            if (GUILayout.Button("Add YABBI define"))
                TryAddYabbiDefine();
        }

        private void CreateYabbiASMDEFs()
        {
            CreateASMDEF(ASMDEFSettings.YabbiAds.SspnetSDK, ASMDEFSettings.YabbiAds.FilePathSspnetSDK);
            CreateASMDEF(ASMDEFSettings.YabbiAds.SspnetSDKEditor, ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor);
            CreateASMDEF(ASMDEFSettings.YabbiAds.YabbiSDK, ASMDEFSettings.YabbiAds.FilePathYabbiSDK);
            CreateASMDEF(ASMDEFSettings.YabbiAds.YabbiSDKEditor, ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor);
        }

        private void CreateASMDEF(string assemblyName, string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                AssemblyDefinition asmdefObject = new AssemblyDefinition();
                asmdefObject.name = assemblyName;
                asmdefObject.autoReferenced = true;
                asmdefObject.references = new string[] { };
                asmdefObject.excludePlatforms = new string[] { };

                string jsonContent = JsonUtility.ToJson(asmdefObject, true);

                File.WriteAllText(filePath, jsonContent);
                AssetDatabase.Refresh();

                Debug.Log($"ASMDEF file {assemblyName} created at: {filePath}.");
            }
            else
            {
                Debug.Log($"ASMDEF file {assemblyName} already created at path: {filePath}.");
            }
        }

        private void EditYabbiASMDEFs()
        {
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathSspnetSDK, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor) }, new string[] { });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor, new string[] { }, new string[] { ASMDEFSettings.ExcludePlatforms.Android, ASMDEFSettings.ExcludePlatforms.iOS });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathYabbiSDK, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDK), GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor), GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor) }, new string[] { });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor) }, new string[] { ASMDEFSettings.ExcludePlatforms.Android, ASMDEFSettings.ExcludePlatforms.iOS });
        }

        private void EditASMDEF(string filePath, string[] references, string[] excludePlatforms)
        {
            string jsonContent = File.ReadAllText(filePath);
            AssemblyDefinition asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

            asmdefObject.references = references;
            asmdefObject.excludePlatforms = excludePlatforms;

            string updatedJson = JsonUtility.ToJson(asmdefObject, true);

            File.WriteAllText(filePath, updatedJson);
            AssetDatabase.Refresh();

            Debug.Log($"ASMDEF file {asmdefObject.name} updated at: {filePath}.");
        }

        private string GetAsmdefGuid(string filePath) => ASMDEFSettings.GUID + AssetDatabase.AssetPathToGUID(filePath);

        private void CheckASMDEF()
        {
            bool needSave = false;
            AssemblyDefinition asmdefObject = null;
            string packagesPath = string.Empty;

            string libraryPath = Path.Combine(Application.dataPath, "..", "Library\\PackageCache");

            string[] directories = Directory.GetDirectories(libraryPath, "com.kindzadzagames.advertisementplugin*@*", SearchOption.TopDirectoryOnly);

            if(directories.Length == 0)
            {
                Debug.Log($"Directory not found in PackageCache, try find in Packages.");

                packagesPath = Application.dataPath + "\\..\\Packages\\com.kindzadzagames.advertisementplugin\\Runtime\\KDDG.Advertisement.asmdef";
            }
            else
            {
                Debug.Log($"Directory found in PackageCache.");

                packagesPath = Path.Combine(directories[0], "Runtime\\KDDG.Advertisement.asmdef");
            }

            if (File.Exists(packagesPath))
            {
                string jsonContent = File.ReadAllText(packagesPath);
                asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                TryAddASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathSspnetSDK, ref needSave);
                TryAddASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathYabbiSDK, ref needSave);

                Debug.Log($"File path: {packagesPath}.");
            }
            else
            {
                Debug.Log($"The file was not found on the way - {packagesPath}.");
            }

            if (needSave)
            {
                string updatedJson = JsonUtility.ToJson(asmdefObject, true);

                File.WriteAllText(packagesPath, updatedJson);
                AssetDatabase.Refresh();
            }
        }

        private void TryAddASMDEF(ref string[] references, string path, ref bool needSave)
        {
            if (CheckContainsASMDEF(references, path) == false)
            {
                AddASMDEF(ref references, GetAsmdefGuid(path));
                needSave = true;

                Debug.Log($"ASMDEF added {GetAsmdefGuid(path)} to references.");
            }
            else
            {
                Debug.Log($"ASMDEF has {GetAsmdefGuid(path)} in references.");
            }
        }

        private bool CheckContainsASMDEF(string[] references, string path) => references.Contains(GetAsmdefGuid(path));

        private void AddASMDEF(ref string[] references, string GUID)
        {
            string[] newArray = new string[references.Length + 1];

            for (int i = 0; i < references.Length; i++)
                newArray[i] = references[i];

            newArray[newArray.Length - 1] = GUID;
            references = newArray;
        }

        private void TryAddYabbiDefine()
        {
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

            if (currentSymbols.Contains(ASMDEFSettings.YabbiAds.YabbiDefine) == false)
            {
                string newSymbols = currentSymbols + ";" + ASMDEFSettings.YabbiAds.YabbiDefine;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), newSymbols);
                Debug.Log($"Added define symbol: {ASMDEFSettings.YabbiAds.YabbiDefine}.");
            }
            else
            {
                Debug.Log($"Define symbol already exists: {ASMDEFSettings.YabbiAds.YabbiDefine}.");
            }
        }
    }

    [Serializable]
    public class AssemblyDefinition
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode = false;
        public bool overrideReferences = false;
        public string[] precompiledReferences;
        public bool autoReferenced = true;
        public string[] defineConstraints;
        public string[] versionDefines;
        public bool noEngineReferences = false;
    }
}
