using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace AdsAppView.Utility
{
    public static class FileUtils
    {
        public static string ConstructCacheFilePath(string filePath)
        {
            string name = Path.GetFileName(filePath);
            return Path.Combine(Application.persistentDataPath, name);
        }

        public static bool TryLoadFile(string filePath, out byte[] bytes)
        {
            bytes = null;
            if (File.Exists(filePath))
            {
                bytes = File.ReadAllBytes(filePath);

#if UNITY_EDITOR
                Debug.Log($"#FileUtils# Cache texture loaded from path: {filePath}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"#FileUtils# Path {filePath} doesn't exist");
#endif
            }

            return bytes != null;
        }

        public static async Task<byte[]> TryLoadFileAsync(string filePath)
        {
            byte[] bytes = await RunOffMainThread(() => File.Exists(filePath) ? File.ReadAllBytes(filePath) : null);

#if UNITY_EDITOR
            if (bytes != null)
                Debug.Log($"#FileUtils# Cache texture loaded from path: {filePath}");
            else
                Debug.Log($"#FileUtils# Path {filePath} doesn't exist");
#endif
            return bytes;
        }

        public static async Task<bool> FileExistsAsync(string filePath)
        {
            return await RunOffMainThread(() => File.Exists(filePath));
        }

        public static async Task TrySaveFile(string filePath, byte[] bytes)
        {
            try
            {
                await File.WriteAllBytesAsync(filePath, bytes);
#if UNITY_EDITOR
                Debug.Log($"#FileUtils# File saved to path: {filePath}");
#endif
            }
            catch (IOException exception)
            {
                Debug.LogError("#FileUtils# Fail to save file: " + exception.Message);
            }
        }

        public static bool TryLoadTexture(string filePath, out Texture2D texture)
        {
            texture = null;

            if (TryLoadFile(filePath, out byte[] bytes))
            {
                texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
            }

            return texture != null;
        }

        public static void TrySaveTexture(string filePath, Texture2D texture)
        {
            TrySaveFile(filePath, texture.EncodeToPNG());
        }

        public static Sprite LoadSprite(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            if (texture.LoadImage(bytes) == false)
            {
                UnityEngine.Object.Destroy(texture);
                return null;
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
            return sprite;
        }

        private static Task<T> RunOffMainThread<T>(Func<T> action)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Task.FromResult(action());
#else
            return Task.Run(action);
#endif
        }
    }
}
