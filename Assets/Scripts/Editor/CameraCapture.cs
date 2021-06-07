using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraCapture : MonoBehaviour
{
    public static int fileCounter = 0;

    public bool capture = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        capture = false;
    }

	private void Update()
	{
        if (capture)
        {
            Camera cam = GetComponent<Camera>();

            if (cam != null)
            {

                RenderTexture currentRT = RenderTexture.active;
                RenderTexture.active = cam.targetTexture;

                cam.Render();

                Texture2D Image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false);
                //Image.sr
                Image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
                Image.Apply();
                RenderTexture.active = currentRT;

                var Bytes = Image.EncodeToPNG();
                DestroyImmediate(Image);
                string parent = Application.dataPath + "/Captures/";
                string path = "";
                fileCounter = 0;
                do
                {
                    path = parent + fileCounter + ".png";
                    fileCounter++;
                }
                while (File.Exists(path));
                Directory.CreateDirectory(parent);
                File.WriteAllBytes(path, Bytes);
                AssetDatabase.Refresh();
                capture = false;
            }
        }
    }
}
