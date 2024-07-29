using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;

namespace LucasWidget.ListView
{
    public class SpriteLoader
    {
        public static IEnumerator Load(string url, UnityAction<Sprite> success, UnityAction<string> fail = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                fail?.Invoke($"Null url");
                yield break;
            }

            string fileName = "";
            string pngSplit = ".png";
            string jpgSplit = ".jpg";
            if (url.Contains(pngSplit))
                fileName = url.Split(pngSplit)[0].Split("/").Last() + pngSplit;
            else if (url.Contains(jpgSplit))
                fileName = url.Split(jpgSplit)[0].Split("/").Last() + jpgSplit;

            if (string.IsNullOrEmpty(fileName))
            {
                fail?.Invoke($"Invalid file");
                yield break;
            }

            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(path))
            {
                byte[] textureBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(textureBytes);
                texture.Apply();
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                success?.Invoke(sprite);
                yield break;
            }

            UnityWebRequest uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerTexture();
            yield return uwr.SendWebRequest();

            if (uwr.isDone && uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                success?.Invoke(sprite);
                SaveTexture(texture, path);
                yield break;
            }

            fail?.Invoke($"Fail to request thumb: {uwr.result}");
        }

        public static void SaveTexture(Texture2D mainTexture, string filePath)
        {
            byte[] textureBytes = mainTexture.EncodeToPNG();
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            fileStream.Write(textureBytes, 0, textureBytes.Length);
            fileStream.Close();
            Debug.Log($"Saved in: {filePath}");
        }
    }
}
