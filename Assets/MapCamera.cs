using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MapCamera : MonoBehaviour
{
    [Range(1, 25)] public float fps;
    public Camera cam;
    //public Shader unlit;

    private double ps;
    private Stopwatch sw;
    private Transform mainCameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw.Start();
        mainCameraTransform = Camera.main.transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = mainCameraTransform.position;
        pos.y = transform.position.y;
        transform.position = pos;

        double ticks = sw.ElapsedTicks;
        double seconds = ticks / Stopwatch.Frequency;

		double timePerFrame = 1.0 / fps;
		if (seconds - ps > timePerFrame)
		{
			ps += timePerFrame;
            cam.Render();// WithShader(unlit, null);
		}
    }
}
