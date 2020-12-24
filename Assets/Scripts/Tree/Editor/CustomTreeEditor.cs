#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
//using System.Windows.Forms;

struct treePoint
{
	public List<treePoint> childs;
	public int index;
}

[CustomEditor(typeof(CustomTree))]
public class CustomTreeEditor : Editor
{
	public GUISkin skin;
	private CustomTree me;
	private Vector2 layerScrollValue;
	Material mat;
	Rect layerRect;
	bool colliderFoldout;
	bool typeFoldout;
	bool autoCreateMeshSettingsFoldout;
	bool MaterialListFoldout;
	bool LodMultFoldout;
	Material pAllMaterial;
	Material pAllCutMaterial;
	int cutBranchIndex;
	float cutBranchProg;

	void Awake()
	{
		me = (target as CustomTree);
	}
	void OnEnable()
	{

		skin = Resources.Load<GUISkin>("treeGuiSkin");
		Shader shader = Shader.Find("Hidden/Internal-Colored");
		mat = new Material(shader);
		me = (target as CustomTree);
	}
	private void OnDisable()
	{
		DestroyImmediate(mat);
	}

	void DisplayLayers(Rect offset)
	{
		List<Vector2> positions = new List<Vector2>(new Vector2[me.layers.Count]);
		List<int> done = new List<int>();// for storing already done stuff
		List<int> done1 = new List<int>();//for storing already done stuff on same y-pos
		//int mainIndex = 0;
		for (int k = 0; k < me.layers.Count; k++)
		{
			if (me.layers[k].parent == k)
			{
				//mainIndex = k;
				break;
			}
		}
		for (int i = 0; i < me.layers.Count; i++)
		{
			int j = i;
			int heirachyPos = 0;
			while (me.layers[j].parent != j)
			{
				j = me.layers[j].parent;
				heirachyPos++;
			}
			while (done.Count < heirachyPos + 1)
			{
				done.Add(0);
				done1.Add(0);
			}
			done[heirachyPos]++;
		}
		for (int i = 0; i < me.layers.Count; i++)
		{
			int j = i;
			int heirachyPos = 0;
			while (me.layers[j].parent != j)
			{
				j = me.layers[j].parent;
				heirachyPos++;
			}
			positions[i] = new Vector2(done1[heirachyPos] * 50, heirachyPos * 50);
			done1[heirachyPos]++;
			
		}
		GUI.Box(layerRect, "", skin.box);
		layerScrollValue = GUI.BeginScrollView(layerRect, layerScrollValue, new Rect(5, 5, offset.width - 10, me.layers.Count * 50 + 50));
		//find layer buttons' positions
		//int layerY = 1;

		

		//draw layers
		for (int i = 0; i < me.layers.Count; i++)
		{

			if (me.tlIndex == i)
			{
				GUI.color = Color.gray;
			}
			
			if (GUI.Button(new Rect(12.5f + positions[i].x, 15 - positions[i].y + me.layers.Count * 50, 50, 25), "Layer " + i))
			{
				me.tlIndex = i;
			}

			GUI.color = Color.white;
			if (me.layers[i].parent != i)
			{
				Handles.BeginGUI();
				Handles.matrix = GUI.matrix;
				Vector3 pos1 = new Vector3(37.5f + positions[i].x, 15 + 25f - positions[i].y + me.layers.Count * 50, 0);
				Vector3 pos2 = new Vector3(37.5f + positions[me.layers[i].parent].x, 15 - positions[me.layers[i].parent].y + me.layers.Count * 50, 0);
				Handles.DrawLine(pos1, pos2);
				Handles.DrawWireDisc(pos1, Vector3.forward, 2f);
				Handles.DrawWireDisc(pos2, Vector3.forward, 2f);
				Handles.EndGUI();
			}
			done1.Add(i);
			//			GUILayout.EndArea ();
		}
		GUI.EndScrollView();
	}

	// Use this for initialization
	public override void OnInspectorGUI()
	{
		if (me.layers.Count == 0)
		{
			me.layers.Add(new TreeLayer(me.layers.Count, new List<int>(), 1, 0.5f, 5, 1, 0));
		}
		Rect offset = EditorGUILayout.BeginHorizontal();
		EditorGUILayout.EndHorizontal();
		offset.y += 10;
		layerRect = GUILayoutUtility.GetRect(0, 10000, 200, 200);
		DisplayLayers(offset);
		if (GUILayout.Button("New Layer", GUILayout.Height(25)))
		{
			TreeLayer temp = new TreeLayer(me.layers.Count, new List<int>(), 1, 0.5f, 5, 1, 0, 1, me.layers[me.tlIndex].thickness);
			temp.parent = me.tlIndex;
			me.tlIndex = me.layers.Count;
			me.layers.Add(temp);
		}
		if (GUILayout.Button("Remove Layer", GUILayout.Height(10)))
		{
			me.layers.RemoveAt(me.tlIndex);
			me.tlIndex--;
		}
		GUILayout.Label(me.allBranches.Count + " Branches");
		DrawDefaultInspector();

		TreeLayer t = me.layers[me.tlIndex];

		t.type = (TreeLayerType)EditorGUILayout.EnumPopup(t.type);
		typeFoldout = EditorGUILayout.Foldout(typeFoldout, "Type Options");
		if (typeFoldout)
		{
			EditorGUI.indentLevel++;

			if (t.type == TreeLayerType.Alternate)
			{

				EditorGUILayout.BeginHorizontal();
				t.ao.minAngle1 = EditorGUILayout.FloatField(t.ao.minAngle1, GUILayout.Width(50));
				t.ao.maxAngle1 = EditorGUILayout.FloatField(t.ao.maxAngle1, GUILayout.Width(50));
				EditorGUILayout.MinMaxSlider(
					new GUIContent("Angle", "The angle range allowed for pairs of branches. If 90Deg is enabled, then this is the offset from 90 degrees allowed"), ref t.ao.minAngle1, ref t.ao.maxAngle1, -360, 360);
				EditorGUILayout.EndHorizontal();
				t.ao.mirror = EditorGUILayout.Toggle(
					new GUIContent("Mirror", "Should the branches mirror across an angle instead of being opposite each other?"), t.ao.mirror);
				t.ao.deg90 = EditorGUILayout.Toggle(
					new GUIContent("90 Deg", "Should alternate pairs be 90 degrees rotated from each other?"), t.ao.deg90);
				if (t.ao.mirror)
				{
					t.ao.mirrorAngle = 90 + EditorGUILayout.Slider(new GUIContent("Mirror Angle", "The angle to mirror off of"), t.ao.mirrorAngle - 90, -360, 360);
				}
				//EditorGUILayout.MinMaxSlider("Angle 2", ref t.ao.minAngle2, ref t.ao.maxAngle2, -360, 360);
				//EditorGUILayout.MinMaxSlider(ref t.ao.minAngle2, ref t.ao.maxAngle2, -360, 360);
			}
			else if (t.type == TreeLayerType.Opposite)
			{
				EditorGUILayout.BeginHorizontal();
				t.oo.minAngle1 = EditorGUILayout.FloatField(t.oo.minAngle1, GUILayout.Width(50));
				t.oo.maxAngle1 = EditorGUILayout.FloatField(t.oo.maxAngle1, GUILayout.Width(50));
				EditorGUILayout.MinMaxSlider("Angle", ref t.oo.minAngle1, ref t.oo.maxAngle1, -360, 360);
				EditorGUILayout.EndHorizontal();
				//EditorGUILayout.MinMaxSlider("Angle", ref t.oo.minAngle1, ref t.oo.maxAngle1, -360, 360);
				t.oo.mirror = EditorGUILayout.Toggle("Mirror", t.oo.mirror);
				if (t.oo.mirror)
				{
					t.oo.mirrorAngle = EditorGUILayout.Slider("Mirror Angle", t.oo.mirrorAngle, -180, 180);
				}

			}
			else if (t.type == TreeLayerType.Whorled)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Amount", GUILayout.Width(50));
				t.wo.amountMult = EditorGUILayout.Slider(t.wo.amountMult, 1, 50);
				t.wo.amount = EditorGUILayout.CurveField(t.wo.amount, Color.green, new Rect(0, 0, 1, 1));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.MinMaxSlider(new GUIContent("Amount Random"), ref t.wo.amountRandomMin, ref t.wo.amountRandomMax, -10, 10);
				t.wo.amountRandomMin = EditorGUILayout.FloatField(t.wo.amountRandomMin);
				t.wo.amountRandomMax = EditorGUILayout.FloatField(t.wo.amountRandomMax);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.MinMaxSlider(new GUIContent("Angle"), ref t.wo.minAngle1, ref t.wo.maxAngle1, -180, 180);
				t.wo.minAngle1 = EditorGUILayout.FloatField(t.wo.minAngle1);
				t.wo.maxAngle1 = EditorGUILayout.FloatField(t.wo.maxAngle1);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.MinMaxSlider(new GUIContent("Height"), ref t.wo.minHeight, ref t.wo.maxHeight, -10, 10);
				t.wo.minHeight = EditorGUILayout.FloatField(t.wo.minHeight);
				t.wo.maxHeight = EditorGUILayout.FloatField(t.wo.maxHeight);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUI.indentLevel--;

		}

		#region materials

		for (int j = 0; j < me.layers.Count; j++)
		{
			int times = 0;
			TreeLayer tt = me.layers[j];
			while (tt.mat.Count > me.numberOfLods)
			{
				tt.mat.RemoveAt(tt.mat.Count - 1);
				if (times > 50) break;
				times++;
			}
			while (tt.cutMat.Count > me.numberOfLods)
			{
				tt.cutMat.RemoveAt(tt.cutMat.Count - 1);
				if (times > 50) break;
				times++;
			}
			while (tt.mat.Count < me.numberOfLods)
			{
				tt.mat.Add((tt.allMaterialSame || tt.mat.Count == 0) ? tt.allMaterial : tt.mat[tt.mat.Count - 1]);
				if (times > 50) break;
				times++;
			}
			while (tt.cutMat.Count < me.numberOfLods)
			{
				tt.cutMat.Add((tt.allMaterialSame || tt.cutMat.Count == 0) ? tt.allCutMaterial : tt.mat[tt.cutMat.Count - 1]);
				if (times > 50) break;
				times++;
			}
			if (times > 50)
			{
				UnityEngine.Debug.LogError("err");
			}
		}

		t.allMaterialSame = EditorGUILayout.Toggle(new GUIContent("All Material Same", "All LODs have same Material"), t.allMaterialSame);
		EditorGUI.indentLevel++;
		if (t.allMaterialSame)
		{
			t.allMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Material", "The Material to use for this treelayer"), t.allMaterial, typeof(Material), false);
			if (t.allMaterial != pAllMaterial)
			{
				for (int i = 0; i < me.numberOfLods; i++)
				{
					t.mat[i] = t.allMaterial;
				}
				pAllMaterial = t.allMaterial;
			}
			t.allCutMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Cut Material", "The Cut Material to use for this treelayer"), t.allCutMaterial, typeof(Material), false);
			if (t.allCutMaterial != pAllCutMaterial)
			{
				for (int i = 0; i < me.numberOfLods; i++)
				{
					t.cutMat[i] = t.allCutMaterial;
				}
				pAllCutMaterial = t.allCutMaterial;
			}
		}
		else
		{
			MaterialListFoldout = EditorGUILayout.Foldout(MaterialListFoldout, new GUIContent("Materials", "Materials for diffrent LODs"));
			if (MaterialListFoldout)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < me.numberOfLods; i++)
				{
					//TODO display all materials, cutmaterials in 2 lists so they can be edited.
					//TODO materials.count and cutmaterials.count = number of LODs.
				}
				EditorGUI.indentLevel--;
			}


		}
		EditorGUI.indentLevel--;

		#endregion

		#region LOD
		LodMultFoldout = EditorGUILayout.Foldout(LodMultFoldout, "LOD multipliers");
		if (LodMultFoldout)
		{
			//auto resize this list
			int times1 = 0;//infinite loop protection (just in case somehow)
			while (me.lodMults.Count < me.numberOfLods && times1 < 50)
			{
				me.lodMults.Add(1 - (me.lodMults.Count / (me.numberOfLods + 1f)));
				times1++;
			}
			while (me.lodMults.Count > me.numberOfLods && times1 < 50)
			{
				me.lodMults.RemoveAt(me.lodMults.Count - 1);
				times1++;
			}
			//display list for editing
			EditorGUI.indentLevel++;
			for (int i = 0; i < me.numberOfLods; i++)
			{
				me.lodMults[i] = EditorGUILayout.FloatField(new GUIContent(i + ": "), me.lodMults[i]);
			}
			EditorGUI.indentLevel--;
		}
		#endregion

		#region mesh
		t.autoCreateMesh = EditorGUILayout.Toggle(
			new GUIContent("Auto Create Mesh", "Automatically create a mesh, given settings"), t.autoCreateMesh);

		autoCreateMeshSettingsFoldout = EditorGUILayout.Foldout(autoCreateMeshSettingsFoldout,
			new GUIContent("Mesh Settings", "The settings for automatically making mesh"));


		if (autoCreateMeshSettingsFoldout) {
			EditorGUI.indentLevel++;
			t.uvYScale = EditorGUILayout.FloatField(
				new GUIContent("Uv Y Scale", "Scale the uv in y to make it look less stretched for thin branches"), t.uvYScale);

			t.segLength = EditorGUILayout.FloatField(
				new GUIContent("Seg. Length", "The lengh of segments. More = better looking, less = less vertex count"), t.segLength);

			t.subdivs = EditorGUILayout.IntField(
				new GUIContent("Subdivs", "More = better looking, less = less vertex count"), t.subdivs);
			t.segmantMesh = (Mesh)EditorGUILayout.ObjectField(
				new GUIContent("Segmant Mesh", "The segmant mesh to use for this trelayer"), t.segmantMesh, typeof(Mesh), false);
			if (GUILayout.Button("Generate Seg. Mesh") && !t.isLeaf)
			{
				t.segmantMesh = me.GetMeshFromPoints(0.5f, 0.5f, 0, t.subdivs, t.uvYScale);
			}
			EditorGUI.indentLevel--;

		}
		#endregion

		#region variables

		colliderFoldout = EditorGUILayout.Foldout(colliderFoldout, new GUIContent("Collider Options"));
		if (colliderFoldout)
		{
			EditorGUI.indentLevel++;
			t.colliderEnable = EditorGUILayout.Toggle(new GUIContent("Enable"), t.colliderEnable);
			t.colliderIsTrigger = EditorGUILayout.Toggle(new GUIContent("Is Trigger"), t.colliderIsTrigger);
			EditorGUI.indentLevel--;


		}

		t.randomZRotation = EditorGUILayout.Toggle(new GUIContent("Random Z rot."), t.randomZRotation);
		t.startHigh = EditorGUILayout.Toggle(new GUIContent("Start High"), t.startHigh);
		t.branchRangeGlobal = EditorGUILayout.Toggle(new GUIContent("Range Global", "Is the range (below) local or global?"), t.branchRangeGlobal);
		EditorGUILayout.MinMaxSlider(new GUIContent("Range", "The range (0-1) which branches can appear"), ref t.startBranches, ref t.endBranches, 0, 1);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Dist Btw.", "Distance between branches"), GUILayout.Width(50));
		t.sizeMult = EditorGUILayout.Slider(t.sizeMult, 0.001f, 5);
		t.size = EditorGUILayout.CurveField(t.size, Color.green, new Rect(0, 0, 1, 1));
		EditorGUILayout.EndHorizontal();

		t.distProgCurve = EditorGUILayout.CurveField(new GUIContent("Dist Btw. By prog"), t.distProgCurve, Color.green, new Rect(0, 0, 1, 1));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Length", "The length of the tree branches"), GUILayout.Width(50));
		t.lengthMult = EditorGUILayout.Slider(t.lengthMult, 0.05f, 50);
		t.length = EditorGUILayout.CurveField(t.length, Color.green, new Rect(0, 0, 1, 1));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Prog Remap", "Remap Prog, currently only used for leafs"), GUILayout.Width(50));
		t.progCurve = EditorGUILayout.CurveField(t.progCurve, Color.green, new Rect(0, 0, 1, 1));
		EditorGUILayout.EndHorizontal();

		t.lengthRandomness = EditorGUILayout.Slider(new GUIContent("Length Randomness", "The randomness between branch lengths"), t.lengthRandomness, 0, 30);
		t.randomness = EditorGUILayout.Slider(new GUIContent("Randomness", "The crincleness of branches"), t.randomness, 0, 5);
		t.noiseScale = EditorGUILayout.Slider(new GUIContent("Noise Scale", ""), t.noiseScale, 0.001f, 1);
		t.thickness = EditorGUILayout.Slider(new GUIContent("Branch Thickness", "The thickness of the branches"), t.thickness, 0.001f, 5);
		t.angle = EditorGUILayout.Slider(new GUIContent("Angle", "Angle relevant to parent branches. 0 = perpendicular"), t.angle, -90, 90);
		//if (t.isLeaf) t.angle2 = EditorGUILayout.Slider(new GUIContent("Angle 2", "Angle relevant to parent branches. 0 = perpendicular"), t.angle2, -90, 90);
		t.angleRandomness = EditorGUILayout.Slider("Angle Randomness", t.angleRandomness, 0, 90);
		t.isLeaf = EditorGUILayout.Toggle(new GUIContent("Is Leaf", "Check if layer is meant for leaves"), t.isLeaf);

		me.layers[me.tlIndex] = t;
		cutBranchIndex = EditorGUILayout.IntField(new GUIContent("Cut Branch Index", "The index to cut the branch at"), cutBranchIndex);
		cutBranchProg = EditorGUILayout.Slider(new GUIContent("Cut Branch Index", "The index to cut the branch at"), cutBranchProg, 0, 1);
		#endregion

		#region buttons

		if (GUILayout.Button("Cut Branch"))
		{
			me.CutBranch(cutBranchIndex, cutBranchProg);
		}

		if (GUILayout.Button("Set Data Location"))
		{
			me.pathToSaveData = EditorUtility.OpenFolderPanel("Data Location", "", "");
		}

		if (GUILayout.Button("Update"))
		{
			//Stopwatch ss = new Stopwatch();
			//ss.Start();
			me.GenerateAll(false);
			//UnityEngine.Debug.Log("Updated tree, took " + (ss.ElapsedMilliseconds / 1000f).ToString() + "s");
		}

		if (GUILayout.Button("Generate"))
		{
			//Stopwatch ss = new Stopwatch();
			//ss.Start();
			me.GenerateAll();
			//UnityEngine.Debug.Log("Generated tree, took " + (ss.ElapsedMilliseconds/1000f).ToString() + "s");
		}

		#endregion
	}
}
#endif