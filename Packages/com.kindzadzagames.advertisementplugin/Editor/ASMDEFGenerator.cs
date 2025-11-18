using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KinDzaDzaGames.AdvertisementPlugin.EditorScripts
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
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            GUILayout.Label("Create new ASMDEF files for YABBI", centeredStyle);

            if (GUILayout.Button("Create YABBI ASMDEFs"))
                CreateYabbiASMDEFs();

            if (GUILayout.Button("Edit YABBI ASMDEFs"))
                EditYabbiASMDEFs();

            if (GUILayout.Button("Try add YABBI ASMDEFs to ADS plugin"))
                TryAddASMDEFs(AdvertisementSDK.YabbiSDK);

            if (GUILayout.Button("Try remove YABBI ASMDEFs from ADS plugin"))
                TryRemoveASMDEFs(AdvertisementSDK.YabbiSDK);

            if (GUILayout.Button("Add YABBI define"))
                TryAddAdDefine(ASMDEFSettings.YabbiAds.YabbiDefine, ASMDEFSettings.YandexAds.YandexDefine);

            if (GUILayout.Button("Remove YABBI define"))
                TryRemoveAdDefine(ASMDEFSettings.YabbiAds.YabbiDefine);

            GUILayout.Space(50);

            GUILayout.Label("Create new ASMDEF files for YANDEX", centeredStyle);

            if (GUILayout.Button("Create YANDEX ASMDEFs"))
                CreateYandexASMDEFs();

            if (GUILayout.Button("Edit YANDEX ASMDEFs"))
                EditYandexASMDEFs();

            if (GUILayout.Button("Try add YANDEX ASMDEF to ADS plugin"))
                TryAddASMDEFs(AdvertisementSDK.YandexSDK);

            if (GUILayout.Button("Try remove YANDEX ASMDEF from ADS plugin"))
                TryRemoveASMDEFs(AdvertisementSDK.YandexSDK);

            if (GUILayout.Button("Add YANDEX define"))
                TryAddAdDefine(ASMDEFSettings.YandexAds.YandexDefine, ASMDEFSettings.YabbiAds.YabbiDefine);

            if (GUILayout.Button("Remove YANDEX define"))
                TryRemoveAdDefine(ASMDEFSettings.YandexAds.YandexDefine);

            GUILayout.Space(50);

            GUILayout.Label("Add ATT ASMDEF to AD plugin, ONLY YANDEX in iOS!", centeredStyle);

            if (GUILayout.Button("Try add iOS ASMDEF to ADS plugin"))
                TryAddIosASMDEF();

            if (GUILayout.Button("Try remove iOS ASMDEF from ADS plugin"))
                TryRemoveIosASMDEF();
        }

        private void CreateYabbiASMDEFs()
        {
            CreateASMDEF(ASMDEFSettings.YabbiAds.SspnetSDK, ASMDEFSettings.YabbiAds.FilePathSspnetSDK);
            CreateASMDEF(ASMDEFSettings.YabbiAds.SspnetSDKEditor, ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor);
            CreateASMDEF(ASMDEFSettings.YabbiAds.YabbiSDK, ASMDEFSettings.YabbiAds.FilePathYabbiSDK);
            CreateASMDEF(ASMDEFSettings.YabbiAds.YabbiSDKEditor, ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor);
        }

        private void CreateYandexASMDEFs()
        {
            CreateASMDEF(ASMDEFSettings.YandexAds.YandexSDK, ASMDEFSettings.YandexAds.FilePathYandexSDK);
            CreateASMDEF(ASMDEFSettings.YandexAds.YandexSDKEditor, ASMDEFSettings.YandexAds.FilePathYandexSDKEditor);
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
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathSspnetSDK, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor) }, new string[] { }, new string[] { });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor, new string[] { }, new string[] { ASMDEFSettings.ExcludePlatforms.Android, ASMDEFSettings.ExcludePlatforms.iOS }, new string[] { });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathYabbiSDK, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDK), GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor), GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor) }, new string[] { }, new string[] { });
            EditASMDEF(ASMDEFSettings.YabbiAds.FilePathYabbiSDKEditor, new string[] { GetAsmdefGuid(ASMDEFSettings.YabbiAds.FilePathSspnetSDKEditor) }, new string[] { ASMDEFSettings.ExcludePlatforms.Android, ASMDEFSettings.ExcludePlatforms.iOS }, new string[] { });
        }

        private void EditYandexASMDEFs()
        {
            EditASMDEF(ASMDEFSettings.YandexAds.FilePathYandexSDK, new string[] { }, new string[] { }, new string[] { });
            EditASMDEF(ASMDEFSettings.YandexAds.FilePathYandexSDKEditor, new string[] { GetAsmdefGuid(ASMDEFSettings.YandexAds.FilePathYandexSDK) }, new string[] { }, new string[] { ASMDEFSettings.ExcludePlatforms.Editor });
        }

        private void EditASMDEF(string filePath, string[] references, string[] excludePlatforms, string[] includePlatforms)
        {
            string jsonContent = File.ReadAllText(filePath);
            AssemblyDefinition asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

            asmdefObject.references = references;
            asmdefObject.includePlatforms = includePlatforms;
            asmdefObject.excludePlatforms = excludePlatforms;

            string updatedJson = JsonUtility.ToJson(asmdefObject, true);

            File.WriteAllText(filePath, updatedJson);
            AssetDatabase.Refresh();

            Debug.Log($"ASMDEF file {asmdefObject.name} updated at: {filePath}.");
        }

        private string GetAsmdefGuid(string filePath) => ASMDEFSettings.GUID + AssetDatabase.AssetPathToGUID(filePath);

        private void TryAddASMDEFs(AdvertisementSDK advertisementSDK)
        {
            bool needSave = false;
            AssemblyDefinition asmdefObject = null;
            string packagesPath = GetPackagePath();
            
            if (File.Exists(packagesPath))
            {
                string jsonContent = File.ReadAllText(packagesPath);
                asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                if (advertisementSDK == AdvertisementSDK.YabbiSDK)
                {
                    TryAddASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathSspnetSDK, ref needSave);
                    TryAddASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathYabbiSDK, ref needSave);
                }
                else
                {
                    TryAddASMDEF(ref asmdefObject.references, ASMDEFSettings.YandexAds.FilePathYandexSDK, ref needSave);
                }
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

        private void TryRemoveASMDEFs(AdvertisementSDK advertisementSDK)
        {
            bool needSave = false;
            AssemblyDefinition asmdefObject = null;
            string packagesPath = GetPackagePath();

            if (File.Exists(packagesPath))
            {
                string jsonContent = File.ReadAllText(packagesPath);
                asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                if (advertisementSDK == AdvertisementSDK.YabbiSDK)
                {
                    TryRemoveASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathSspnetSDK, ref needSave);
                    TryRemoveASMDEF(ref asmdefObject.references, ASMDEFSettings.YabbiAds.FilePathYabbiSDK, ref needSave);
                }
                else
                {
                    TryRemoveASMDEF(ref asmdefObject.references, ASMDEFSettings.YandexAds.FilePathYandexSDK, ref needSave);
                }
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

        private void TryRemoveASMDEF(ref string[] references, string path, ref bool needSave)
        {
            if (CheckContainsASMDEF(references, path))
            {
                RemoveASMDEF(ref references, GetAsmdefGuid(path));
                needSave = true;

                Debug.Log($"ASMDEF removed {GetAsmdefGuid(path)} in references.");
            }
            else
            {
                Debug.Log($"In ASMDEF is no such element in the object: {GetAsmdefGuid(path)}.");
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

        private void RemoveASMDEF(ref string[] references, string GUID)
        {
            string[] newArray = references.Where(e => e != GUID).ToArray();
            references = newArray;
        }

        private string GetPackagePath()
        {
            string packagesPath = string.Empty;
            string libraryPath = Path.Combine(Application.dataPath, "..", $"Library{Path.DirectorySeparatorChar}PackageCache");

            string[] directories = Directory.GetDirectories(libraryPath, "com.kindzadzagames.advertisementplugin*@*", SearchOption.TopDirectoryOnly);

            if (directories.Length == 0)
            {
                packagesPath = Application.dataPath + $"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Packages{Path.DirectorySeparatorChar}com.kindzadzagames.advertisementplugin{Path.DirectorySeparatorChar}Runtime{Path.DirectorySeparatorChar}KDDG.Advertisement.asmdef";

                Debug.Log($"AD ASMDEF found in Packages, path: {packagesPath}.");
            }
            else
            {
                packagesPath = Path.Combine(directories[0], $"Runtime{Path.DirectorySeparatorChar}KDDG.Advertisement.asmdef");

                Debug.Log($"AD ASMDEF found in PackageCache, path: {packagesPath}.");
            }

            return packagesPath;
        }

        private void TryAddIosASMDEF()
        {
            bool finded = false;
            string[] allASMdefs = AssetDatabase.FindAssets(ASMDEFSettings.ASMDEFSIGNATURE);

            foreach (string guid in allASMdefs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.Contains(ASMDEFSettings.IosATT.ATTASMDEF))
                {
                    finded = true;
                    AssemblyDefinition asmdefObject = null;
                    string packagesPath = GetPackagePath();

                    if (File.Exists(packagesPath))
                    {
                        string jsonContent = File.ReadAllText(packagesPath);
                        asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                        if (asmdefObject.references.Contains(ASMDEFSettings.GUID + guid) == false)
                        {
                            AddASMDEF(ref asmdefObject.references, ASMDEFSettings.GUID + guid);

                            string updatedJson = JsonUtility.ToJson(asmdefObject, true);
                            File.WriteAllText(packagesPath, updatedJson);
                            AssetDatabase.Refresh();
                        }
                        else
                        {
                            Debug.LogError($"AD ASMDEF has iOS ATT guid: {guid}, in references.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"AD ASMDEF was not found on the way - {packagesPath}.");
                    }

                    break;
                }
            }

            if (finded == false)
                Debug.LogError($"ASMDEF from App Tracking Transparency API was not found, check if the plugin for iOS is installed. If not - install iOS 14 Advertising Support from package manager.");
        }

        private void TryRemoveIosASMDEF()
        {
            bool finded = false;
            string[] allASMdefs = AssetDatabase.FindAssets(ASMDEFSettings.ASMDEFSIGNATURE);

            foreach (string guid in allASMdefs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.Contains(ASMDEFSettings.IosATT.ATTASMDEF))
                {
                    finded = true;
                    AssemblyDefinition asmdefObject = null;
                    string packagesPath = GetPackagePath();

                    if (File.Exists(packagesPath))
                    {
                        string jsonContent = File.ReadAllText(packagesPath);
                        asmdefObject = JsonUtility.FromJson<AssemblyDefinition>(jsonContent);

                        if (asmdefObject.references.Contains(ASMDEFSettings.GUID + guid))
                        {
                            RemoveASMDEF(ref asmdefObject.references, ASMDEFSettings.GUID + guid);

                            string updatedJson = JsonUtility.ToJson(asmdefObject, true);
                            File.WriteAllText(packagesPath, updatedJson);
                            AssetDatabase.Refresh();
                        }
                        else
                        {
                            Debug.LogError($"AD ASMDEF doesn't have iOS ATT guid: {guid}, in references.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"AD ASMDEF was not found on the way - {packagesPath}.");
                    }

                    break;
                }
            }

            if (finded == false)
                Debug.LogError($"ASMDEF from App Tracking Transparency API was not found, check if the plugin for iOS is installed. If not - install iOS 14 Advertising Support from package manager.");
        }

        private void TryAddAdDefine(string sdkLabel, string deletingSdk)
        {
            TryRemoveAdDefine(deletingSdk);

            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

            if (currentSymbols.Contains(sdkLabel) == false)
            {
                string newSymbols = currentSymbols + ";" + sdkLabel;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), newSymbols);
            }
            else
            {
                Debug.Log($"Define symbol already exists: {sdkLabel}.");
            }
        }

        private void TryRemoveAdDefine(string sdkLabel)
        {
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            string newSymbols;

            if (currentSymbols.StartsWith($"{sdkLabel}"))
            {
                newSymbols = currentSymbols.Substring($"{sdkLabel};".Length);
            }
            else if (currentSymbols.Contains(sdkLabel))
            {
                newSymbols = currentSymbols.Replace($";{sdkLabel}", "");
            }
            else
            {
                Debug.Log($"Define not contains: {sdkLabel}.");
                return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), newSymbols);
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

    public enum AdvertisementSDK
    {
        YabbiSDK,
        YandexSDK
    }
}
