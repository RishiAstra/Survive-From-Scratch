using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUCol : MonoBehaviour {
	public ComputeShader cs;
	public float bounds;
	public float rad;
	[Range(0, 5)]
	public float velMult;
	[Range(1, 100)]
	public int updateSpeed;
	[Range(0, 20)]
	public float gravity;
	[Range(0, 1)]
	public float bouncy;
	[Range(0, 1)]
	public float colThreshold;

	private Vector3[] vel;
	private Vector3[] pos;
	ComputeBuffer posb;
	ComputeBuffer velb;
	ComputeBuffer col;
	//ComputeBuffer hit;
	private int frameCount;
	private List<Transform> t = new List<Transform>();
	private bool[] b = new bool[4];
	private bool mustUpdate;
	private float pTime;
	// Use this for initialization
	void Start () {
		pTime = Time.time;
		//find all spheres
		t.AddRange(GameObject.FindObjectsOfType<Transform>());
		for (int i = 0; i < t.Count; i++)
		{
			if(t[i].gameObject.name != "Col")
			{
				t.RemoveAt(i);
				i--;
			}
		}

		//inisianalize velocity and position
		vel = new Vector3[t.Count];
		pos = new Vector3[t.Count];
		for (int i = 0;i<t.Count;i++)
		{
			vel[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * velMult;// t[i].position;
			pos[i] = t[i].position;
		}
		
		//initionalize buffers
		posb = new ComputeBuffer(pos.Length, sizeof(float) * 3);
		velb = new ComputeBuffer(vel.Length, sizeof(float) * 3);
		col = new ComputeBuffer(1, 4);
		//hit = new ComputeBuffer(pos.Length, 1);
		posb.SetData(pos);
		velb.SetData(vel);
		col.SetData(new bool[1]);
		cs.SetInt("length", pos.Length);
		cs.SetFloat("bounds", bounds);
		cs.SetFloat("rad", rad);
		cs.SetFloat("speed", Time.fixedDeltaTime);
		cs.SetFloat("bouncy", bouncy);
		cs.SetFloat("gravity", gravity);
		cs.SetFloat("colThreshold", colThreshold);
		cs.SetBuffer(0, "pos", posb);
		cs.SetBuffer(0, "vel", velb);
		cs.SetBuffer(0, "col", col);
		Destroy(GameObject.Find("Main Camera (1)"));
		Destroy(GameObject.Find("Main Camera (2)"));
	}
	private int fc;
	// Update is called once per frame
	void FixedUpdate () {
		



		//print(pos[0].ToString());
		//posb.SetData(pos);
		//velb.SetData(vel);



		cs.Dispatch(0, Mathf.CeilToInt(t.Count / 1024f), 1, 1);


		//posb.Release();
		//velb.Release();

		//for (int i = 0; i < t.Count; i++)
		//{
		//	pos[i] = t[i].position;
		//}
		//UpdatePos();
	}

	private void OnPreRender()
	{
		fc++;
		frameCount++;
		b = new bool[1];
		col.GetData(b);
		mustUpdate = mustUpdate || b[0];
		//print(b[0]);
		if (frameCount % updateSpeed == 0 && mustUpdate)
		{
			fc = 0;
			UpdateCol();
			mustUpdate = false;
		}


		for (int i = 0; i < t.Count; i++)
		{
			t[i].position += vel[i]-Vector3.up*gravity * (Time.time - pTime);
		}
		pTime = Time.time;
	}

	void UpdateCol()
	{
		posb.GetData(pos);
		velb.GetData(vel);
		for(int i = 0; i < pos.Length; i++)
		{
			t[i].position = pos[i];
		}
		b[0] = false;
		col.SetData(b);
		
	}
	void UpdatePos()
	{
		for (int i = 0; i < t.Count; i++)
		{
			//if (i == 0) print(t[i].position-pos[i]);
			t[i].position = Vector3.Lerp(t[i].position, pos[i], 0.15f);
			//t[i].position += vel[i] * Time.fixedDeltaTime;
			//pos[i] = t[i].position;
		}
	}


	private void OnApplicationQuit()
	{
		posb.Dispose();
		velb.Dispose();
		col.Dispose();
	}
}
