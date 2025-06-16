using System;
using System.IO;
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
            GUILayout.Label("Create a new ASMDEF files for YABBI", EditorStyles.boldLabel);

            if (GUILayout.Button("Create ASMDEF"))
                CreateYabbiASMDEFs();

            if (GUILayout.Button("Edit ASMDEF"))
                EditYabbiASMDEFs();

            if (GUILayout.Button("Check plugin ASMDEF"))
                CheckASMDEF();

            GUILayout.Label("Create a new ASMDEF files for YANDEX", EditorStyles.boldLabel);
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
            string libraryPath = Path.Combine(Application.dataPath, "..", "Library\\PackageCache");

            string[] directories = Directory.GetDirectories(libraryPath, "com.kindzadzagames.advertisementplugin*@*", SearchOption.TopDirectoryOnly);

            if(directories.Length == 0)
            {
                Debug.Log($"Directory not found.");
            }
            else
            {
                string path = Path.Combine(directories[0], "Runtime\\KDDG.Advertisement.asmdef");

                if (File.Exists(path))
                {
                    string jsonContent = File.ReadAllText(path);
                    AssemblyDefinition asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                    Debug.Log($"File path: {path}.");
                }
                else
                {
                    Debug.Log($"The file was not found on the way - {path}.");
                }
            }

            /*string packagesPath = Application.dataPath + "\\..\\Packages\\com.kindzadzagames.advertisementplugin\\Runtime\\KDDG.Advertisement.asmdef";

            if (File.Exists(packagesPath))
            {
                string jsonContent = File.ReadAllText(packagesPath);
                AssemblyDefinition asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                Debug.Log("ѕуть к файлу: " + packagesPath);
            }
            else
            {
                Debug.Log($"‘айл не найден по пути - {packagesPath}.");
            }*/
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
