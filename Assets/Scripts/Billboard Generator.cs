using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BillboardGenerator : EditorWindow
{
    static string savePath { get { return Path.Combine(Application.dataPath, "Billboards/"); } }

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    GameObject target;
    Texture2D tex;
    RenderTexture rt;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Billboard Generator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BillboardGenerator window = (BillboardGenerator)EditorWindow.GetWindow(typeof(BillboardGenerator));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Target", EditorStyles.boldLabel);
        target = (GameObject)EditorGUILayout.ObjectField(target, typeof(GameObject), true);

        if(target != null)
		{
            if (GUILayout.Button("Generate"))
            {
                GenerateBillboard();
            }

            if(tex != null)
			{

                float maxw = 1000;
                float w = tex.width;
                float h = tex.height;
                if (tex.width > maxw)
                {
                    h *= maxw / w;
                    w = maxw;
                }

                GUILayout.Label(new GUIContent(tex), GUILayout.Width(w), GUILayout.Height(h));
            }

        }        

        //GUILayout.Box(tex);//, GUILayout.Width(w), GUILayout.Height(h));//, GUILayout.Width(512));

        //GUILayout.Label("Base Settings", EditorStyles.boldLabel);


        //myString = EditorGUILayout.TextField("Text Field", myString);

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }

	private void GenerateBillboard()
	{

        Bounds b = new Bounds(target.transform.position, Vector3.zero);
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        int[] originalLayers = new int[renderers.Length];

		for (int i = 0; i < renderers.Length; i++)
        {
			Renderer r = renderers[i];
            originalLayers[i] = r.gameObject.layer;
            r.gameObject.layer = 31;
			b.Encapsulate(r.bounds);
        }

        


        GameObject g = new GameObject("BILLBOARD_CAMERA");
        Camera c = g.AddComponent<Camera>();
        c.enabled = false;
        //SetLayerRecursively(target, 31);//set layer
        c.cullingMask = 1 << 31;
        c.orthographic = true;
        c.orthographicSize = b.extents.magnitude;
        c.useOcclusionCulling = false;
        c.clearFlags = CameraClearFlags.Color;
        c.backgroundColor = Color.clear;

        int yc = 16;
        int xc = 8;
        float yinc = 360f / yc;
        float xinc = 180f / xc;
        const int textureSize = 128;
        RenderTexture temp = RenderTexture.active;
        rt = new RenderTexture(textureSize, textureSize, 16);
        c.targetTexture = rt;

        tex = new Texture2D(rt.width * yc, rt.height * xc);


        for (int ix = 0; ix < xc; ix++)
		{
            for (int iy = 0; iy < yc; iy++)
            {

                float x = ix * xinc - 90f;
                float y = iy * yinc;

                Quaternion rot = Quaternion.Euler(0, y, 0) * Quaternion.Euler(x, 0, 0);
                Vector3 dir = rot * Vector3.forward;

                g.transform.position = b.center - dir * 100;
                g.transform.rotation = rot;//.forward = dir;
                RenderTexture.active = rt;

                c.Render();
                RenderTexture.active = rt;

				//Graphics.CopyTexture(rt, 0, 0, 0, 0, textureSize, textureSize, tex, 0, 0, iy * textureSize, ix * textureSize);
				tex.ReadPixels(new Rect(0, 0, textureSize, textureSize), textureSize * iy, textureSize * ix);
			}
        }

		tex.Apply();

        Directory.CreateDirectory(savePath);
        File.WriteAllBytes(savePath + target.name + "_billboard.png", ImageConversion.EncodeToPNG(tex));
        AssetDatabase.Refresh();

		RenderTexture.active = temp;

		DestroyImmediate(g);

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            r.gameObject.layer = originalLayers[i];
        }

        Debug.Log("Generated Billboard texture for \"" + target.name + "\"");

	}

    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}