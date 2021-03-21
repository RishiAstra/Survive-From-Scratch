#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]
[ExecuteInEditMode]
public class meterrain : MonoBehaviour
{
	public int sx, sy, sz;
	//	[SerializeField]
	[HideInInspector ()]public float speed;
	[HideInInspector ()]public float rad;
	[HideInInspector ()]public float maxSize;

	private MeshFilter mf;
	public MeshCollider mc;
	private Mesh mesh;
	private List<Vector3> v = new List<Vector3> ();
	private List<Vector2> u = new List<Vector2> ();
	private List<int> t = new List<int> ();
	private List<Vector3> n = new List<Vector3> ();
	private int maxTriSamePoint = 15;
	enum tools{expand, paint};
	// Use this for initialization
	void Start ()
	{
		mf = GetComponent<MeshFilter> ();
		mc = GetComponent<MeshCollider> ();

		mesh = mc.sharedMesh;
//		if(Application.isEditor){
//			mesh = new Mesh ();
//		}

//		quad (Vector3.zero, 1, Vector3.zero);
		updateMesh ();
	}

	//	void OnEnable(){
	//		EditorApplication.update += MeUpdate;
	//	}
	//
	//	void OnDisable(){
	//		EditorApplication.update -= MeUpdate;
	//	}

	public void startMesh ()
	{
		v.Clear ();
		u.Clear ();
		t.Clear ();
		n.Clear ();
		for (int x = 0; x < sx; x++) {
			for (int z = 0; z < sz; z++) {
				if (x == 0) {
					quad (new Vector3 (0, 0.5f, z + 0.5f), 1, new Vector3 (90, 270, 0));
				}
				if (x == sx - 1) {
					quad (new Vector3 (x + 1, 0.5f, z + 0.5f), 1, new Vector3 (90, 90, 0));
				}
				if (z == 0) {
					quad (new Vector3 (x + 0.5f, 0.5f, 0), 1, new Vector3 (90, 180, 0));
				}
				if (z == sz - 1) {
					quad (new Vector3 (x + 0.5f, 0.5f, z + 1), 1, new Vector3 (90, 0, 0));
				}

				quad (new Vector3 (x + 0.5f, 1, z + 0.5f), 1, new Vector3 (0, 0, 0));
				quad (new Vector3 (x + 0.5f, 0, z + 0.5f), 1, new Vector3 (180, 0, 0));
			}
		}
		redoNormals ();
		updateMesh ();

	}

	void shiftTri(int index){
		for(int i = index;i<t.Count;i++){
			t [i] -= 1;
		}
	}

	// Update is called once per frame
	public void grow (Vector3 pos)
	{
		if (mesh == null) {
			mesh = mc.sharedMesh;
		}
		bool[] doneThese = new bool[v.Count];
//			print ("step 1 complete");
//			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//Camera.current.ScreenPointToRay (Event.current.mousePosition);
//			RaycastHit hit;
//			if(Physics.Raycast(ray, out hit)){
//				if (hit.collider == mc) {
		for (int i = 0; i < v.Count; i++) {

			if (!doneThese [i] && Vector3.Distance (v [i] + transform.position, pos) < rad) {
				List<int> tv = new List<int> ();
				Vector3 direct = new Vector3 ();
				int si = i;
				//					tv.Add(v);
				for (int j = 0; j < 8; j++) {
					int io = ioV3 (v [i], v, si);// v.IndexOf (v [i], si);
					if (io != -1) {
						if (doneThese [io]) {
							Debug.LogError ("Not working!");
							return;
						}

						Vector3 t = v [io];
						tv.Add (io);
						if (io % 3 == 0) {
							direct += Vector3.Cross (v [io + 1] - v [io], v [io + 2] - v [io]);//1-0, 3-0
						}
						if (io % 3 == 1) {
							direct += Vector3.Cross (v [io] - v [io - 1], v [io + 1] - v [io - 1]);//1-0, 3-0
						}
						if (io % 3 == 2) {
							direct += Vector3.Cross (v [io - 1] - v [io - 2], v [io] - v [io - 2]);//1-0, 3-0
						}
//						if (io % 4 == 3) {
//							direct += Vector3.Cross (v [io - 2] - v [io - 3], v [io] - v [io - 3]);//1-0, 3-0
//						}
						si = io + 1;
						doneThese [io] = true;
					}
				}
				direct.Normalize ();
				for (int j = 0; j < tv.Count; j++) {
					v [tv [j]] += direct * speed;
					n [tv [j]] = direct;
				}
			}

		}
		int vcount = v.Count;
		for (int i = 0; i < vcount; i += 3) {
			float d1 = Vector3.Distance (v [i], v [i + 1]);
			float d2 = Vector3.Distance (v [i + 1], v [i + 2]);
			float d3 = Vector3.Distance (v [i + 2], v [i]);
			int v1 = -1;
			int v2 = -1;
			int v4 = -1;
			int v5 = -1;
			int v3 = -1;
			int v6 = -1;
//			int p = -1;
			if (d1 > maxSize) {
//				p = 0;
				v1 = i + 0;
				v2 = i + 1;
				v3 = i + 2;
			}else if (d2 > maxSize) {
//				p = 1;
				v1 = i + 1;
				v2 = i + 2;
				v3 = i + 0;
			}else if (d3 > maxSize) {
//				p = 2;
				v1 = i + 2;
				v2 = i + 0;
				v3 = i + 1;
			}
			if (v1 != -1 && v2 != -1) {//0 0 0 0 0
				int si = 0;
				for (int j = 0; j < maxTriSamePoint; j++) {
					int io = ioV3 (v [v1], v, si);
					if (io != -1 && io != v1) {
						if (v [io] == v [v1]) {
							
							int p1 = -1;
							int p2 = -1;
							if (io%3==0) {
								p1 = io + 1;
								p2 = io + 2;
							}else if (io%3==1) {
								p1 = io + 1;
								p2 = io - 1;
							}else if (io%3==2) {
								p1 = io - 1;
								p2 = io - 2;
							}
							if (v [p1] == v [v2]) {
								v5 = p1;
								v4 = io;
								print ("p1");
							}
							if (v [p2] == v [v2]) {
								v5 = p2;
								v4 = io;
								print ("p2");
							}
							if (v [p1] == v [v2] && v [p2] == v [v2]) {
								print ("Whaaa??");
							}
//							print (string.Format("io {0}, v4 {1}, v5 {2}", io, v4, v5));
						}
					}
					si = io + 1;
				}
				if (v5 == -1) {
					print ("some kind of error. counld'nt find v5");
				}
				if (v4 != -1 && v5 != -1) {
					for (int j = 0; j < v.Count; j++) {//could cause error
						if (j == v4) {
							if (j % 3 == 0) {
								if (j + 1 == v5) {
									v6 = j + 2;
								} else {
									v6 = j + 1;
								}
							}
							if (j % 3 == 1) {
								if (j - 1 == v5) {
									v6 = j + 1;
								} else {
									v6 = j - 1;
								}
							}
							if (j % 3 == 2) {
								if (j - 1 == v5) {
									v6 = j - 2;
								} else {
									v6 = j - 1;
								}
							}
						}
					}
//					v1 = t.IndexOf (v1);
//					v2 = t.IndexOf (v2);
//					v3 = t.IndexOf (v3);
//					v4 = t.IndexOf (v4);
				}
				if (v1 != -1 && v2 != -1 && v4 != -1 && v5 != -1 && v3 != -1 && v6 != -1) {
					print ("v1: " + v1 + ", v2: " + v2 + ", v3: " + v3 + ", v4: " + v4 + ", v5: " + v5 + ", v6: " + v6);
					//NEEDS FIXING, IT NOT REALLY WORK
					int vc = v.Count;
					v.Add (v[v1]);
					v.Add (v[v3]);
					v.Add ((v[v1] + v[v2])/2);
					t.Add (vc + 1);
					t.Add (vc + 0);
					t.Add (vc + 2);
					u.Add (new Vector2(0, 0));
					u.Add (new Vector2(1, 0));
					u.Add (new Vector2(0, 1));
					n.Add (v[vc + 1].normalized);
					n.Add (v[vc + 0].normalized);
					n.Add (v[vc + 2].normalized);

					v.Add (v[v2]);
					v.Add (v[v3]);
					v.Add ((v[v1] + v[v2])/2);
					t.Add (vc + 3);
					t.Add (vc + 4);
					t.Add (vc + 5);
					u.Add (new Vector2(1, 1));
					u.Add (new Vector2(1, 0));
					u.Add (new Vector2(0, 1));
					n.Add (v[vc + 3].normalized);
					n.Add (v[vc + 4].normalized);
					n.Add (v[vc + 5].normalized);


					v.Add (v[v4]);
					v.Add (v[v6]);
					v.Add ((v[v4] + v[v5])/2);
					t.Add (vc + 6);
					t.Add (vc + 7);
					t.Add (vc + 8);
					u.Add (new Vector2(0, 0));
					u.Add (new Vector2(1, 0));
					u.Add (new Vector2(0, 1));
					n.Add (v[vc + 6].normalized);
					n.Add (v[vc + 7].normalized);
					n.Add (v[vc + 8].normalized);

					v.Add (v[v5]);
					v.Add (v[v6]);
					v.Add ((v[v4] + v[v5])/2);
					t.Add (vc + 10);
					t.Add (vc + 9);
					t.Add (vc + 11);
					u.Add (new Vector2(1, 1));
					u.Add (new Vector2(1, 0));
					u.Add (new Vector2(0, 1));
					n.Add (v[vc + 10].normalized);
					n.Add (v[vc + 9].normalized);
					n.Add (v[vc + 11].normalized);

					v.RemoveAt (v1);
					t.RemoveAt (v1);
					u.RemoveAt (v1);
					n.RemoveAt (v1);
					for(int ii = v1;ii<t.Count;ii++){
						t[ii]--;
						if (v2 > v1) {
							v2--;
						}
						if (v3 > v1) {
							v3--;
						}
						if (v4 > v1) {
							v4--;
						}
						if (v5 > v1) {
							v5--;
						}
						if (v6 > v1) {
							v6--;
						}
					}
					v.RemoveAt (v2);
					t.RemoveAt (v2);
					u.RemoveAt (v2);
					n.RemoveAt (v2);
					for(int ii = v2;ii<t.Count;ii++){
						t[ii]--;
						if (v3 > v2) {
							v3--;
						}
						if (v4 > v2) {
							v4--;
						}
						if (v5 > v2) {
							v5--;
						}
						if (v6 > v2) {
							v6--;
						}
					}
					v.RemoveAt (v3);
					t.RemoveAt (v3);
					u.RemoveAt (v3);
					n.RemoveAt (v3);
					for(int ii = v3;ii<t.Count;ii++){
						t[ii]--;
						if (v4 > v3) {
							v4--;
						}
						if (v5 > v3) {
							v5--;
						}
						if (v6 > v3) {
							v6--;
						}
					}
					v.RemoveAt (v4);
					t.RemoveAt (v4);
					u.RemoveAt (v4);
					n.RemoveAt (v4);
					for(int ii = v4;ii<t.Count;ii++){
						t[ii]--;
						if (v5 > v4) {
							v5--;
						}
						if (v6 > v4) {
							v6--;
						}
					}
					v.RemoveAt (v5);
					t.RemoveAt (v5);
					u.RemoveAt (v5);
					n.RemoveAt (v5);
					for(int ii = v5;ii<t.Count;ii++){
						t[ii]--;
						if (v6 > v5) {
							v6--;
						}
					}
					v.RemoveAt (v6);
					t.RemoveAt (v6);
					u.RemoveAt (v6);
					n.RemoveAt (v6);
					for(int ii = v6;ii<t.Count;ii++){
						t[ii]--;
					}
//
//					shiftTri (v1);
//					shiftTri (v2);
//					shiftTri (v3);
//					shiftTri (v4);
//					shiftTri (v5);
//					shiftTri (v6);
//
//					n[v1] = null;
//					n[v2] = null;
//					n[v3] = null;
//					n[v4] = null;
//					n[v5] = null;
//					n[v6] = null;
//
//					n[v1] = null;
//					n[v2] = null;
//					n[v3] = null;
//					n[v4] = null;
//					n[v5] = null;
//					n[v6] = null;




					redoNormals ();

				} else {
					print ("error v1: " + v1 + "v2: " + v2 + "v3: " + v3 + "v4: " + v4 + "v5: " + v5 + "v6: " + v6);
				}
			}
		}
		updateMesh ();
//		}
	}

	void quad (Vector3 pos, float scale, Vector3 rotation)
	{
		int vc = v.Count;
		v.Add (RotatePointAroundPivot (pos + new Vector3 (-0.5f, 0, -0.5f) * scale, pos, rotation));//0, 0
		v.Add (RotatePointAroundPivot (pos + new Vector3 (-0.5f, 0, 0.5f) * scale, pos, rotation));// 0, 1
		v.Add (RotatePointAroundPivot (pos + new Vector3 (0.5f, 0, 0.5f) * scale, pos, rotation));//  1, 1
		v.Add (RotatePointAroundPivot (pos + new Vector3 (0.5f, 0, -0.5f) * scale, pos, rotation));// 1, 0

		v.Add (RotatePointAroundPivot (pos + new Vector3 (-0.5f, 0, -0.5f) * scale, pos, rotation));//0, 0
		v.Add (RotatePointAroundPivot (pos + new Vector3 (0.5f, 0, 0.5f) * scale, pos, rotation));//  1, 1
		u.Add (new Vector2 (0, 0));
		u.Add (new Vector2 (0, 1));
		u.Add (new Vector2 (1, 1));
		u.Add (new Vector2 (1, 0));

		u.Add (new Vector2 (0, 0));
		u.Add (new Vector2 (1, 1));
		t.Add (vc + 0);
		t.Add (vc + 1);
		t.Add (vc + 2);
		t.Add (vc + 3);
		t.Add (vc + 4);
		t.Add (vc + 5);
		n.Add (Vector3.zero);
		n.Add (Vector3.zero);
		n.Add (Vector3.zero);
		n.Add (Vector3.zero);

		n.Add (Vector3.zero);
		n.Add (Vector3.zero);
	}

	void updateMesh ()
	{
		if (mesh == null) {
			mesh = mc.sharedMesh;
		}
		mesh.Clear ();
		mesh.vertices = v.ToArray ();
		mesh.uv = u.ToArray ();
		mesh.triangles = t.ToArray ();
		mesh.normals = n.ToArray ();
		mesh.RecalculateBounds ();
//		mesh.RecalculateNormals ();
		mf.sharedMesh = mesh;
		mc.sharedMesh = mesh;
		mc.convex = true;
		mc.convex = false;
	}

	Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler (angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	int ioV3 (Vector3 findThis, List<Vector3> list, int starthere)
	{
		int index = -1;
		if (starthere >= list.Count) {
			return -1;
		}
		for (int i = starthere; i < list.Count; i++) {
			if (Vector3.Distance (list [i], findThis) < 0.001f) {
				return i;
			}
		}
		return index;
	}

	void redoNormals ()
	{
		if (mesh == null) {
			mesh = mc.sharedMesh;
		}
		bool[] doneThese = new bool[v.Count];
		//			print ("step 1 complete");
		//			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//Camera.current.ScreenPointToRay (Event.current.mousePosition);
		//			RaycastHit hit;
		//			if(Physics.Raycast(ray, out hit)){
		//				if (hit.collider == mc) {
		for (int i = 0; i < v.Count; i++) {

			if (!doneThese [i]) {
				List<int> tv = new List<int> ();
				Vector3 direct = new Vector3 ();
				int si = i;
				//					tv.Add(v);
				for (int j = 0; j < maxTriSamePoint; j++) {
					if (si >= v.Count) {
						break;
					}
					int io = ioV3 (v [i], v, si);// v.IndexOf (v [i], si);
					if (io != -1) {
						if (doneThese [io]) {
							Debug.LogError ("Not working!");
							return;
						}

						Vector3 t = v [io];
						tv.Add (io);
						if (io % 3 == 0) {
							direct += Vector3.Cross (v [io + 1] - v [io], v [io + 2] - v [io]);//1-0, 3-0
						}
						if (io % 3 == 1) {
							direct += Vector3.Cross (v [io] - v [io - 1], v [io + 1] - v [io - 1]);//1-0, 3-0
						}
						if (io % 3 == 2) {
							direct += Vector3.Cross (v [io - 1] - v [io - 2], v [io] - v [io - 2]);//1-0, 3-0
						}
						//						if (io % 4 == 3) {
						//							direct += Vector3.Cross (v [io - 2] - v [io - 3], v [io] - v [io - 3]);//1-0, 3-0
						//						}
						si = io + 1;
						doneThese [io] = true;
					}
				}
				direct.Normalize ();
				for (int j = 0; j < tv.Count; j++) {
//					v [tv [j]] += direct * speed;
					n [tv [j]] = direct;
				}
			}

		}
	}
}
#endif