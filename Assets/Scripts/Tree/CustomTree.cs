/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using MyStuff;
using System.Linq;
//FAILED--TODO use ECS
//FIXED--TODO LOD system no errors, ERROR FOUND, materials are null, so it auto skips the meshes, see bellow to fix --||||||-----------but it doesn't make and meshes or gameobjects (probably setmeshes's fault)
//FIXED--TODO see CustomTreeEditor.cs
//TODO: make old lod gameobject destoyed as soon as new one is made. Otherwise either there is no rendered tree for a frame or 2 trees are rendered.
//SetMeshes second bool parameter should make it delete children as it makes meshes for new.
[ExecuteInEditMode()]
public class CustomTree : MonoBehaviour
{
	public List<TreeLayer> layers;
	[Range(0, 5)]
	public float angleNoiseScale = 0.5f;
	[Range(1, 5)]
	public int numberOfLods = 1;
	public Mesh defaultMesh;
	//	public Mesh end;
	//	[HideInInspector()]public AnimationCurve size;
	//	[HideInInspector()]public float sizeMult;
	public int tlIndex = 0;
	//	public string pathForData = "/Stuff/TreeDatas/";
	//	public Object treedata;
	//private MeshFilter mf;
	private Mesh tempTreeLayerMesh;
	private Mesh tempTreeEndMesh;
	private List<List<Mesh>> meshesToShow;
	private List<List<Material>> mats;
	private List<List<Mesh>> tempMesh;//TODO: Im working on a LOD system, thats why its 2d list instead of just list.
	private List<List<Mesh>> tempMeshE;
	private float LastAlternateAngle;
	private int subdivs;
	private bool makingBranches;
	private int lod;
	//private bool loadedData = false;
	[HideInInspector()] public string pathToSaveData;
	[HideInInspector()] public List<float> lodMults;
	//[HideInInspector()]
	[HideInInspector()] public List<Branch> allBranches = new List<Branch>();
	System.Diagnostics.Stopwatch sw;
	//List<int> allBranchesIndex = new List<int>();//TODO allow removing of branches


	private void Start()
	{

		sw = new System.Diagnostics.Stopwatch();

		for (int i = 0; i < layers.Count; i++)
		{
			if (layers[i].isLeaf) continue;
			layers[i].segmantMesh = GetMeshFromPoints(0.5f, 0.5f, 0, layers[i].subdivs, layers[i].uvYScale);
		}

		if (!Application.isPlaying) return;
		//GameObject gg = new GameObject("test");
		//gg.AddComponent<MeshFilter>();
		//gg.AddComponent<MeshRenderer>();
		//gg.GetComponent<MeshFilter>().mesh = GetMeshFromPoints(1, 1, 0f);
		//if(layers[layers.Count - 2].branches.Count > 0)
		//{
		//	//CutBranch(layers[layers.Count-2].branches[0].index, 0);
		//}


	}


	// Update is called once per frame
	void Update()
	{
		//if(Time.frameCount == 100)
		//{
		//	if (layers[layers.Count - 2].branches.Count > 0)
		//	{
		//		CutBranch(layers[layers.Count - 2].branches[0], 0.46f);
		//	}
		//}

	}

	#region generate tree


	//generate branches for a entire tree layer
	TreeLayer generateLayerBranches(TreeLayer t)
	{
		int tIndex = layers.IndexOf(t);
		t.branches = new List<int>();
		TreeLayer tParent = layers[t.parent];
		if (t.parent == layers.IndexOf(t))
		{
			float countDown;
			float prog = t.startBranches;
			while (prog < t.endBranches)
			{

				float l = t.length.Evaluate(prog) *
				t.lengthMult + UnityEngine.Random.Range(-t.lengthRandomness, t.lengthRandomness);

				Vector3 tempAngle = new Vector3(
					t.angle + UnityEngine.Random.Range(-t.angleRandomness, t.angleRandomness),
					 UnityEngine.Random.Range(0, 360),
					0
				);
				Branch b1 = new Branch(l, l, Vector3.zero, tempAngle, Vector3.one, 1, 0, tIndex, allBranches.Count, 0);
				t.branches.Add(allBranches.Count);
				allBranches.Add(b1);
				countDown = (t.size.Evaluate(prog) * t.sizeMult);
				prog += countDown;
			}
		}
		else
		{
			for (int i = 0; i < tParent.branches.Count; i++)
			{
				float countDown = 0;
				float prog = t.startBranches;
				if (t.branchRangeGlobal)
				{
					prog = t.startBranches - ((t.lengthMult - allBranches[tParent.branches[i]].length) / t.lengthMult);
					if (prog < 0) prog = 0;
				}
				if (t.startHigh)
				{
					countDown = (t.size.Evaluate(prog) * t.distProgCurve.Evaluate(prog) * t.sizeMult);
					prog += (countDown / allBranches[tParent.branches[i]].length) / tParent.segLength;
				}
				//countDown = (t.size.Evaluate(prog) * t.sizeMult);
				//prog += countDown / allBranches[tParent.branches[i]].length;
				bool hasAlternateControll = true;
				while (prog < t.endBranches)
				{
					//length
					float l =
						(t.length.Evaluate(prog) * 
						(t.isLeaf? 1 : allBranches[tParent.branches[i]].maxLength / tParent.lengthMult * tParent.segLength) *
						(t.isLeaf? 1 : 1 - prog) *
						t.lengthMult +
						 UnityEngine.Random.Range(-t.lengthRandomness, t.lengthRandomness)) / t.segLength;
					//direction of parent

					float height = prog * allBranches[tParent.branches[i]].maxLength * tParent.segLength;
					int high = Mathf.CeilToInt(prog * allBranches[tParent.branches[i]].length); //(allBranches[tParent.branches[i]].offsets.Count - 1));//high for lerp
					int low = Mathf.FloorToInt(prog * allBranches[tParent.branches[i]].length);// (allBranches[tParent.branches[i]].offsets.Count - 1));//low for lerp
					float between = (prog * allBranches[tParent.branches[i]].length) - low; //(prog * (allBranches[tParent.branches[i]].offsets.Count-1) - low);//get amount to lerp

					float spaceTilltop = 1f - ((low + between) / allBranches[tParent.branches[i]].maxLength);// (allBranches[tParent.branches[i]].offsets.Count - 1f));

					Vector3 offset = Vector3.Lerp(allBranches[tParent.branches[i]].offsets[low], allBranches[tParent.branches[i]].offsets[high], between);//lerp

					//the position of the parent branch at the point the new branch will be created
					Vector3 parentBranchPos = new Vector3(0, height, 0) + offset;
					//apply parent branch rotation
					parentBranchPos = Quaternion.Euler(allBranches[tParent.branches[i]].rot) * parentBranchPos;
					//make angle for new branch
					Vector3 tempAngle = new Vector3
					{
						x = t.angle + UnityEngine.Random.Range(-t.angleRandomness, t.angleRandomness) + 90,
						z = t.randomZRotation ? UnityEngine.Random.Range(0, 360) : 0
					};

					if (t.type == TreeLayerType.Alternate || t.type == TreeLayerType.Random)
					{
						if (t.type == TreeLayerType.Alternate)
						{
							if (hasAlternateControll)
							{
								//if (t.ao.mirror) {
								//	tempAngle.y = t.ao.mirrorAngle + Mathf.DeltaAngle(LastAlternateAngle + 180, t.ao.mirrorAngle);
								//	//tempAngle.y = UnityEngine.Random.Range(t.ao.minAngle1, t.ao.maxAngle1);
								//}
								//else
								//{
								if (t.ao.deg90)
								{
									tempAngle.y = LastAlternateAngle + 90 + UnityEngine.Random.Range(t.ao.minAngle1, t.ao.maxAngle1);
									LastAlternateAngle = tempAngle.y;
								}
								else
								{
									tempAngle.y = UnityEngine.Random.Range(t.ao.minAngle1, t.ao.maxAngle1);
									LastAlternateAngle = tempAngle.y;
								}

								//}
							}
							else
							{
								if (t.ao.mirror)
								{
									//tempAngle.y = LastAlternateAngle + 180;
									tempAngle.y = t.ao.mirrorAngle + Mathf.DeltaAngle(LastAlternateAngle + 180, t.ao.mirrorAngle);
								}
								else
								{
									tempAngle.y = LastAlternateAngle + 180;
								}
								LastAlternateAngle = tempAngle.y;
							}
							hasAlternateControll = !hasAlternateControll;

						}
						if (t.type == TreeLayerType.Random)
						{
							tempAngle.y = UnityEngine.Random.Range(0, 360);
						}
						tempAngle = (Quaternion.Euler(allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle)).eulerAngles;

						// add the branch to the current treelayer
						Branch b1 = new Branch(
							l,
							l,
							allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
							tempAngle,
							allBranches[tParent.branches[i]].scale,
							spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
							tParent.branches[i],
							tIndex,
							allBranches.Count,
							spaceTilltop
						);
						t.branches.Add(allBranches.Count);
						allBranches[tParent.branches[i]].children.Add(allBranches.Count);
						allBranches.Add(b1);


					}
					if (t.type == TreeLayerType.Opposite)
					{
						Vector3 tempAngle1 = tempAngle;
						Vector3 tempAngle2 = tempAngle;
						float r = UnityEngine.Random.Range(t.oo.minAngle1, t.oo.maxAngle1);
						tempAngle1.y = r;
						if (t.oo.mirror)
						{
							tempAngle2.y = t.oo.mirrorAngle + Mathf.DeltaAngle(r, t.oo.mirrorAngle);
						}
						else
						{
							tempAngle2.y = r + 180;
						}

						tempAngle1 = (Quaternion.Euler(allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle1)).eulerAngles;
						tempAngle2 = (Quaternion.Euler(allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle2)).eulerAngles;


						// add the branch to the current treelayer
						Branch b1 = new Branch(
							l,
							l,
							allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
							tempAngle1,
							allBranches[tParent.branches[i]].scale,
							spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
							tParent.branches[i],
							tIndex,
							allBranches.Count,
							spaceTilltop
						);

						////////////////////////
						l =
							(t.length.Evaluate(prog) *
							(t.isLeaf ? 1 : allBranches[tParent.branches[i]].maxLength / tParent.lengthMult * tParent.segLength) *
							(t.isLeaf ? 1 : 1 - prog) *
							t.lengthMult +
							UnityEngine.Random.Range(-t.lengthRandomness, t.lengthRandomness)) / t.segLength;
						////////////////////////

						Branch b2 = new Branch(
							l,
							l,
							allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
							tempAngle2,
							allBranches[tParent.branches[i]].scale,
							spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
							tParent.branches[i],
							tIndex,
							allBranches.Count + 1,
							spaceTilltop
						);
						allBranches.Add(b1);
						allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
						t.branches.Add(allBranches.Count - 1);
						allBranches.Add(b2);
						allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
						t.branches.Add(allBranches.Count - 1);
						//t.branches.Add(b1);
						//t.branches.Add(b2);
					}
					if (t.type == TreeLayerType.Whorled)
					{
						float r = UnityEngine.Random.Range(t.wo.minAngle1, t.wo.maxAngle1);
						int branchNumber = Mathf.RoundToInt(t.wo.amount.Evaluate(prog) * t.wo.amountMult + UnityEngine.Random.Range(t.wo.amountRandomMin, t.wo.amountRandomMax));
						//Branch[] tempBranches = new Branch[branchNumber];
						for (int jj = 0;jj <  branchNumber;jj++)
						{
							float tempProg = prog + UnityEngine.Random.Range(t.wo.minHeight / allBranches[tParent.branches[i]].maxLength, t.wo.maxHeight / allBranches[tParent.branches[i]].maxLength);
							//the above was faulty, could generate values above the length. Below is a fix
							if (tempProg > 1) tempProg = 0.9999f;
							if (tempProg < 0) tempProg = 0.0001f;

							///////////////////////////////////
							l =
								(t.length.Evaluate(tempProg) *
								(t.isLeaf ? 1 : allBranches[tParent.branches[i]].maxLength / tParent.lengthMult * tParent.segLength) *
								(t.isLeaf ? 1 : 1 - tempProg) *
								t.lengthMult +
								UnityEngine.Random.Range(-t.lengthRandomness, t.lengthRandomness)) / t.segLength;
							///////////////////////////////////
							height = tempProg * allBranches[tParent.branches[i]].maxLength * tParent.segLength;
							//problem is here -- its too large, only when length is int
							high = Mathf.CeilToInt(tempProg * allBranches[tParent.branches[i]].length); //(allBranches[tParent.branches[i]].offsets.Count - 1));//high for lerp
							low = Mathf.FloorToInt(tempProg * allBranches[tParent.branches[i]].length);// (allBranches[tParent.branches[i]].offsets.Count - 1));//low for lerp
							between = (tempProg * allBranches[tParent.branches[i]].length) - low; //(tempProg * (allBranches[tParent.branches[i]].offsets.Count-1) - low);//get amount to lerp

							spaceTilltop = 1f - ((low + between) / allBranches[tParent.branches[i]].maxLength);// (allBranches[tParent.branches[i]].offsets.Count - 1f));

							offset = Vector3.Lerp(allBranches[tParent.branches[i]].offsets[low], allBranches[tParent.branches[i]].offsets[high], between);//lerp

							//the position of the parent branch at the point the new branch will be created
							parentBranchPos = new Vector3(0, height, 0) + offset;
							//apply parent branch rotation
							parentBranchPos = Quaternion.Euler(allBranches[tParent.branches[i]].rot) * parentBranchPos;
							Branch b1 = new Branch(
								l,
								l,
								allBranches[tParent.branches[i]].pos + parentBranchPos,// * tempProg * t.parent.branches[i].maxLength
								tempAngle + (Vector3.up * (UnityEngine.Random.Range(t.wo.minAngle1, t.wo.maxAngle1) + (jj * 360f/branchNumber))),
								allBranches[tParent.branches[i]].scale,
								spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
								tParent.branches[i],
								tIndex,
								allBranches.Count,
								spaceTilltop
							);
							allBranches.Add(b1);
							allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
							t.branches.Add(allBranches.Count - 1);
						}

						///////////////////
						//tempAngle1.y = r;
						//if (t.oo.mirror)
						//{
						//	tempAngle2.y = t.oo.mirrorAngle + Mathf.DeltaAngle(r, t.oo.mirrorAngle);
						//}
						//else
						//{
						//	tempAngle2.y = r + 180;
						//}

						//tempAngle1 = (Quaternion.Euler(allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle1)).eulerAngles;
						//tempAngle2 = (Quaternion.Euler(allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle2)).eulerAngles;


						//// add the branch to the current treelayer
						//Branch b1 = new Branch(
						//	l,
						//	l,
						//	allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
						//	tempAngle1,
						//	allBranches[tParent.branches[i]].scale,
						//	spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
						//	tParent.branches[i],
						//	tIndex,
						//	allBranches.Count,
						//	spaceTilltop
						//);
						//Branch b2 = new Branch(
						//	l,
						//	l,
						//	allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
						//	tempAngle2,
						//	allBranches[tParent.branches[i]].scale,
						//	spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
						//	tParent.branches[i],
						//	tIndex,
						//	allBranches.Count + 1,
						//	spaceTilltop
						//);
						//allBranches.Add(b1);
						//allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
						//t.branches.Add(allBranches.Count - 1);
						//allBranches.Add(b2);
						//allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
						//t.branches.Add(allBranches.Count - 1);
						////////////////
					}

					countDown = (t.size.Evaluate(prog) * t.distProgCurve.Evaluate(prog) * t.sizeMult);
					prog += (countDown / allBranches[tParent.branches[i]].length) / tParent.segLength;//  /tparent.seglength to even out it when a branch has seglength other than 1
				}

			}
		}
		return t;
	}

	//TreeLayer generateLayerLeafes(TreeLayer t){
	//	int tIndex = layers.IndexOf(t); 
	//	t.branches = new List<int> ();
	//	TreeLayer tParent = layers[t.parent];
	//	for (int i = 0; i < tParent.branches.Count; i++) {
	//		float countDown;
	//		float prog = 0;
	//		countDown = 1 / (t.size.Evaluate (prog) * t.sizeMult);
	//		prog += countDown;
	//		while (prog < 1) {
	//			//direction of parent

	//			int high = Mathf.CeilToInt (prog * (allBranches[tParent.branches[i]].offsets.Count - 1));//high for lerp
	//			int low = Mathf.FloorToInt (prog * (allBranches[tParent.branches[i]].offsets.Count - 1));//low for lerp
	//			float between = (prog * (allBranches[tParent.branches[i]].offsets.Count-1) - low);//get amount to lerp

	//			Vector3 offset = Vector3.Lerp(allBranches[tParent.branches[i]].offsets [low], allBranches[tParent.branches[i]].offsets [high], between);//lerp

	//			//the position of the parent branch at the point the new branch will be created
	//			//make angle for new branch
	//			Vector3 tempAngle = new Vector3 (
	//				90 + t.angle + UnityEngine.Random.Range (-t.angleRandomness, t.angleRandomness),
	//				 UnityEngine.Random.Range (0, 360),
	//				0
	//			);
	//			tempAngle = (Quaternion.Euler (allBranches[tParent.branches[i]].rot) * Quaternion.Euler(tempAngle)).eulerAngles;
	//			float spaceTilltop = 1f-((low + between)/(allBranches[tParent.branches[i]].offsets.Count - 1f));//TODO this is wrong, but it works for now
	//			float height = (1-spaceTilltop) * (allBranches[tParent.branches[i]].offsets.Count - 1f);//allBranches[tParent.branches[i]].offsets.Count - 1f /!\wrong/!\
	//			Vector3 parentBranchPos = Quaternion.Euler(allBranches[tParent.branches[i]].rot) * (new Vector3(0, height, 0) + offset);

	//			// add the branch to the current treelayer
	//			Branch b1 = new Branch(
	//				1,
	//				1,
	//				allBranches[tParent.branches[i]].pos + parentBranchPos,// * prog * t.parent.branches[i].maxLength
	//				tempAngle,
	//				allBranches[tParent.branches[i]].scale,
	//				spaceTilltop * allBranches[tParent.branches[i]].progTillTop,//1-prog,
	//				tParent.branches[i],
	//				tIndex,
	//				allBranches.Count,
	//				spaceTilltop
	//			);

	//			allBranches.Add(b1);
	//			t.branches.Add(allBranches.Count - 1);
	//			allBranches[tParent.branches[i]].children.Add(allBranches.Count - 1);
	//			countDown = 1 / (t.size.Evaluate (prog) * t.sizeMult);
	//			prog += countDown;
	//		}

	//	}
	//	return t;
	//}

	//makes the meshes for branches of a tree layer
	//TODO: 

	//TODO: something, i forgot
	TreeLayer makeLayerBranches(TreeLayer t, bool redoOffsets = true)
	{

		tempMesh = new List<List<Mesh>>();//2d lod#,branch#
		tempMeshE = new List<List<Mesh>>();//2d lod#,branch#

		//t.mesh = new List<Mesh>();//1d lod#
		//t.endMesh = new List<Mesh>();//1d lod#

		/** TODO
		 * 1.make script generate multiple of the same tree, then
		 * 2.asign the diffrent trees to diffrent parents
		 * 3.make a LOD group with the diffrent trees assigned
		 * 4.make each lod tree (see 1) simpler than last, probably by decreasing subdivs and making new segmesh.
		 * **/
		for (int ii = 0; ii < numberOfLods; ii++)
		{
			tempMesh.Add(new List<Mesh>());
			tempMeshE.Add(new List<Mesh>());
		}

		List<int> indexes = new List<int>();//The indexes (branch mesh) which needed a mesh, weren't completely hidden
		List<int> indexesE = new List<int>();//The indexes (branch but mesh, endmesh) which needed a mesh, weren't completely hidden
		

		for (int i = 0; i < t.branches.Count; i++)
		{
			int br = t.branches[i];
			bool canDo = true;
			while (br != 0)
			{
				if ((1 - allBranches[br].prog) >
					allBranches[allBranches[br].parentBranch].length /
					allBranches[allBranches[br].parentBranch].sizeScale)
				{
					canDo = false;
					break;
				}
				br = allBranches[br].parentBranch;
			}
			if (!canDo)
			{
				allBranches[t.branches[i]].lengthRelative = 1;//changed, check if works
				allBranches[t.branches[i]].maxLength = 0;
				continue;
			}
			if (allBranches[t.branches[i]].length > 0)
			{
				allBranches[t.branches[i]] = MakeBranch(allBranches[t.branches[i]], t, redoOffsets);
				indexes.Add(i);
				if (!t.isLeaf)
				{
					indexesE.Add(i);
				}
			}
		}

		CombineInstance[] ci = new CombineInstance[indexes.Count];
		CombineInstance[] cie = new CombineInstance[indexesE.Count];

		for (int i = 0; i < indexes.Count; i++)
		{
			Branch b = allBranches[t.branches[indexes[i]]];
			ci[i].mesh = tempMesh[lod][i];
			ci[i].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
		}
		for (int i = 0; i < indexesE.Count; i++)
		{
			Branch b = allBranches[t.branches[indexesE[i]]];
			cie[i].mesh = tempMeshE[lod][i];
			cie[i].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
		}
		tempTreeLayerMesh = new Mesh();
		tempTreeEndMesh = new Mesh();
		tempTreeLayerMesh.CombineMeshes(ci);
		tempTreeEndMesh.CombineMeshes(cie);
		//for (int i = 0; i < t.branches.Count; i++)
		//{
		//	Destro y(allBranches[t.branches[i]].m);
		//	Destro y(allBranches[t.branches[i]].mEnd);
		//}
		tempMesh[lod].Clear();
		tempMeshE[lod].Clear();

		tempTreeLayerMesh.RecalculateBounds();
		//tempTreeEndMesh.RecalculateBounds();

		t.mesh[lod] = tempTreeLayerMesh;// (Mesh)Instantiate(tempTreeLayerMesh);
		t.endMesh[lod] = tempTreeEndMesh;// (Mesh)Instantiate(tempTreeEndMesh);
		return t;
	}

	//IEnumerator makeLayerBranchesAsync(TreeLayer t, bool redoOffsets, int speed)
	//{
	//	makingBranches = true;
	//	tempMesh = new List<List<Mesh>>();//2d lod#,branch#
	//	tempMeshE = new List<List<Mesh>>();//2d lod#,branch#

	//	//t.mesh = new List<Mesh>();//1d lod#
	//	//t.endMesh = new List<Mesh>();//1d lod#

	//	/** TODO
	//	 * 1.make script generate multiple of the same tree, then
	//	 * 2.asign the diffrent trees to diffrent parents
	//	 * 3.make a LOD group with the diffrent trees assigned
	//	 * 4.make each lod tree (see 1) simpler than last, probably by decreasing subdivs and making new segmesh.
	//	 * **/
	//	for (int ii = 0; ii < numberOfLods; ii++)
	//	{
	//		tempMesh.Add(new List<Mesh>());
	//		tempMeshE.Add(new List<Mesh>());
	//	}

	//	List<int> indexes = new List<int>();//The indexes (branch mesh) which needed a mesh, weren't completely hidden
	//	List<int> indexesE = new List<int>();//The indexes (branch but mesh, endmesh) which needed a mesh, weren't completely hidden
	//	tempTreeLayerMesh = new Mesh();
	//	tempTreeEndMesh = new Mesh();
	//	int count = 0;
	//	for (int i = 0; i < t.branches.Count; i++)
	//	{
	//		int br = t.branches[i];
	//		bool canDo = true;
	//		while (br != 0)
	//		{
	//			if ((1 - allBranches[br].prog) >
	//				allBranches[allBranches[br].parentBranch].length /
	//				allBranches[allBranches[br].parentBranch].sizeScale)
	//			{
	//				canDo = false;
	//				break;
	//			}
	//			br = allBranches[br].parentBranch;
	//		}
	//		if (!canDo)
	//		{
	//			allBranches[t.branches[i]].lengthRelative = 1;//changed, check if works
	//			allBranches[t.branches[i]].maxLength = 0;
	//			continue;
	//		}
	//		if (allBranches[t.branches[i]].length > 0)
	//		{
	//			if (!sw.IsRunning) sw.Start();

	//			if(count >= speed)
	//			{
	//				count = 0;
	//				sw.Stop();
	//				yield return new WaitForEndOfFrame();
	//			}
	//			allBranches[t.branches[i]] = MakeBranch(allBranches[t.branches[i]], t, redoOffsets);
	//			count++;
	//			indexes.Add(i);
	//			if (!t.isLeaf)
	//			{
	//				indexesE.Add(i);
	//			}
	//		}
	//	}

	//	CombineInstance[] ci = new CombineInstance[indexes.Count];
	//	CombineInstance[] cie = new CombineInstance[indexesE.Count];

	//	for (int i = 0; i < indexes.Count; i++)
	//	{
	//		Branch b = allBranches[t.branches[indexes[i]]];
	//		ci[i].mesh = tempMesh[lod][i];
	//		ci[i].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
	//	}
	//	for (int i = 0; i < indexesE.Count; i++)
	//	{
	//		Branch b = allBranches[t.branches[indexesE[i]]];
	//		cie[i].mesh = tempMeshE[lod][i];
	//		cie[i].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
	//	}

	//	tempTreeLayerMesh.CombineMeshes(ci);
	//	tempTreeEndMesh.CombineMeshes(cie);
	//	//for (int i = 0; i < t.branches.Count; i++)
	//	//{
	//	//	Destro y(allBranches[t.branches[i]].m);
	//	//	Destr y(allBranches[t.branches[i]].mEnd);
	//	//}
	//	tempMesh[lod].Clear();
	//	tempMeshE[lod].Clear();

	//	tempTreeLayerMesh.RecalculateBounds();
	//	tempTreeEndMesh.RecalculateBounds();

	//	t.mesh[lod] = tempTreeLayerMesh;// (Mesh)Instantiate(tempTreeLayerMesh);
	//	t.endMesh[lod] = tempTreeEndMesh;// (Mesh)Instantiate(tempTreeEndMesh);
	//									 //return t;
	//	makingBranches = false;
	//	yield return new WaitForEndOfFrame();
	//}

	//50% performance
	Branch MakeBranch(Branch b, TreeLayer t, bool redoOffsets, float prog = 0, float length = -1)
	{
		if (length < 0)
		{
			length = b.length;
		}
		Mesh branch;//make a mesh for the branch
		Mesh endMesh;
		float ran = UnityEngine.Random.Range(-1234, 1234);//random used for noise
		List<CombineInstance> cb = new List<CombineInstance>();//list to store branch section combine instances.
		if (!t.isLeaf)
		{
			branch = new Mesh();
			endMesh = new Mesh();
			float a;//the angle that the branch sways
			float amount;//the amount that it sways
			if (redoOffsets)
			{
				b.offsets = new List<Vector3>();//offsets for tree sections
			}
			
			for (int i = 0; i < length; i++)
			{//cycle through branch length, adding sections
				int co = Mathf.FloorToInt(prog * b.maxLength);
				if (i < co && prog != 0) continue;//skip if outside the cut area

				CombineInstance c = new CombineInstance();//temporairy combineInstance for current branch section
				if (i < b.IntLength)
				{
					if (i > co || prog == 0)
					{
						c.mesh = t.segmantMesh;// defaultMesh;//set the mesh ot combine
					}
					else// if (!Mathf.Approximately((prog * b.maxLength) % 1, 0))
					{
						c.mesh = GetMeshFromPoints(0.5f, 0.5f, ((prog * b.maxLength) % 1), t.subdivs, t.uvYScale);// defaultMesh;//set the mesh ot combine
						float tempProg = (prog * b.maxLength);
						endMesh = GetEndMesh(0.5f, subdivs, Vector3.up * (tempProg) * t.segLength);
					}
				}
				else// if(!Mathf.Approximately(b.length % 1, 0))
				{
					//print("gen " + b.length % 1);
					c.mesh = GetMeshFromPoints(0.5f, 0.5f, length % 1, t.subdivs, t.uvYScale, true);// defaultMesh;//set the mesh ot combine
					float tempProg = length;
					endMesh = GetEndMesh(0.5f, subdivs, Vector3.up * tempProg * t.segLength);
				}

				//c.mesh = t.segmantMesh;// defaultMesh;//set the mesh ot combine
				c.transform = Matrix4x4.TRS(i * Vector3.up * t.segLength, Quaternion.identity, new Vector3(1, t.segLength, 1));//the transform for combining meshes
																																//if (i == b.length - 1)
																																//{//if the segmant is last, scale by the vurrent growth of the branch
																																//	c.transform = Matrix4x4.TRS(i * Vector3.up * t.segLength, Quaternion.identity, new Vector3(1, t.segLength, 1) * b.growthLength);
																																//}
				cb.Add(c);//add temporairy combineInstance to list
				if (redoOffsets)
				{
					//set a (see above)
					a = Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle + ran) * t.noiseScale * angleNoiseScale, 0) * 360;
					//set amount (see above)
					amount = (Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle + ran) * t.noiseScale, 0) - 0.5f) * t.randomness * t.segLength * 2;
					//add offsets

					if (i > 0)
					{
						b.offsets.Add(new Vector3(Mathf.Cos(a) * amount, 0, Mathf.Sin(a) * amount) + b.offsets[i - 1]);
					}
					else
					{
						b.offsets.Add(new Vector3(0, 0, 0));
					}
				}
			}
			
            //////////////////////
			if (redoOffsets)
			{
				//add one more offset  because of Off By One
				a = (Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle) * t.noiseScale * angleNoiseScale, 0) - 0.5f) * 360;
				amount = (Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle) * t.noiseScale, 0) - 0.5f) * t.randomness * t.segLength * 2;
				if (b.offsets.Count > 0)
				{
					b.offsets.Add(new Vector3(Mathf.Cos(a) * amount, 0, Mathf.Sin(a) * amount) + b.offsets[b.offsets.Count - 1]);
				}
				else
				{
					b.offsets.Add(new Vector3(0, 0, 0));
				}
			}
			while (b.offsets.Count <= Mathf.CeilToInt(b.length))
			{
				//add one more offset  because of Off By One
				a = (Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle) * t.noiseScale * angleNoiseScale, 0) - 0.5f) * 360;
				amount = (Mathf.PerlinNoise((b.offsets.Count + t.lengthMult + t.angle) * t.noiseScale, 0) - 0.5f) * t.randomness * t.segLength * 2;
				if (b.offsets.Count > 0)
				{
					b.offsets.Add(new Vector3(Mathf.Cos(a) * amount, 0, Mathf.Sin(a) * amount) + b.offsets[b.offsets.Count - 1]);
				}
				else
				{
					b.offsets.Add(new Vector3(0, 0, 0));
				}
			}
			branch.CombineMeshes(cb.ToArray());//combine all the offsets

			Vector3[] vert = branch.vertices;//get the verticies in a list
			Vector3[] endVert = endMesh.vertices;
			//Vector2[] uvs = branch.uv;
			//float uvScale = 10;// 1 / t.thickness * b.progTillTop;

			for (int i = 0; i < vert.Length; i++)
			{//go through them
				float f = Mathf.Lerp(1, 0, vert[i].y / (b.maxLength * t.segLength)) * t.thickness * b.thickness * b.progTillTop;//get the thickness at the point of vertex
				vert[i] = new Vector3(vert[i].x * f, vert[i].y, vert[i].z * f);//set thickness
				float yy = vert[i].y / t.segLength;
				vert[i] += Vector3.Lerp(b.offsets[Mathf.FloorToInt(yy)], b.offsets[Mathf.CeilToInt(yy)], yy - Mathf.Floor(yy));//change by offset

				//uvs[i].Scale(new Vector2(1, uvScale));
			}
			bool skip = true;
			for (int i = 0; i < endVert.Length; i++)
			{//go through them
				float f = Mathf.Lerp(1, 0, endVert[i].y / (b.maxLength * t.segLength)) * t.thickness * b.thickness * b.progTillTop;//get the thickness at the point of vertex
				endVert[i] = new Vector3(endVert[i].x * f, endVert[i].y, endVert[i].z * f);//set thickness
				float yy = endVert[i].y / t.segLength;
				if (endVert[i].x * endVert[i].x + endVert[i].z * endVert[i].z > 0.001f)
				{
					skip = false;
				}
				endVert[i] += Vector3.Lerp(b.offsets[Mathf.FloorToInt(yy)], b.offsets[Mathf.CeilToInt(yy)], yy - Mathf.Floor(yy));//change by offset

				//uvs[i].Scale(new Vector2(1, uvScale));
			}
			if (skip)
			{
				endMesh = new Mesh();
			}
			else
			{
				endMesh.vertices = endVert;
			}
			branch.vertices = vert;//set the verticies back from the list v
			tempMeshE[lod].Add(endMesh);
			tempTreeEndMesh = endMesh;

		}
		else
		{
			//branch = (Mesh)Instantiate(t.segmantMesh);

			Vector3[] v = t.segmantMesh.vertices;
			float f = b.length * t.thickness * b.thickness * t.progCurve.Evaluate(b.progTillTop); //b.length;// t.length.Evaluate(b.progTillTop) * (t.lengthMult + UnityEngine.Random.Range(-t.lengthRandomness, t.lengthRandomness));// Mathf.Lerp(1, 0, v[i].y / (b.length)) * t.thickness * b.progTillTop;

			for (int i = 0; i < v.Length; i++)
			{
				v[i] = new Vector3(v[i].x * f, v[i].y * f, v[i].z * f);//set thickness
																	   //v [i] += b.offsets [Mathf.FloorToInt (v [i].y)];//change by offset
			}
			branch = new Mesh
			{
				vertices = v,
				uv = t.segmantMesh.uv,
				normals = t.segmantMesh.normals,
				triangles = t.segmantMesh.triangles
			};

		}
		tempMesh[lod].Add(branch);
		tempTreeLayerMesh = branch;

		//}
		//CombineInstance[] cbranch = new CombineInstance[2];//combine to other branches
		//cbranch[0].mesh = branch;
		//cbranch[0].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
		//cbranch[1].mesh = (Mesh)Instantiate(tempTreeLayerMesh);
		//cbranch[1].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
		//tempTreeLayerMesh.CombineMeshes(cbranch);
		//CombineInstance[] cbranch = new CombineInstance[2];//combine to other branches
		//cbranch[0].mesh = endMesh;
		//cbranch[0].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
		//cbranch[1].mesh = (Mesh)Instantiate(tempTreeEndMesh);
		//cbranch[1].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
		//tempTreeEndMesh.CombineMeshes(cbranch);
		//Vector3[] tempv = new Vector3[tempTreeLayerMesh.vertexCount + b.m.vertexCount];
		//Vector3[] tempnorm = new Vector3[tempTreeLayerMesh.vertexCount + b.m.vertexCount];
		//Vector3[] temptemp = b.m.vertices;
		//Vector3[] temptempnorm = b.m.normals;
		////copy vertecies
		//Array.Copy(tempTreeLayerMesh.vertices, 0, tempv, 0, tempTreeLayerMesh.vertexCount);
		//Array.Copy(b.m.vertices, 0, tempv, tempTreeLayerMesh.vertexCount, b.m.vertexCount);
		////copy normals
		//Array.Copy(tempTreeLayerMesh.normals, 0, tempnorm, 0, tempTreeLayerMesh.vertexCount);
		//Array.Copy(b.m.normals, 0, tempnorm, tempTreeLayerMesh.vertexCount, b.m.vertexCount);

		//int vertexCount = tempTreeLayerMesh.vertexCount;
		//for (int i = vertexCount; i < tempv.Length; i++)
		//{
		//	tempv[i] = (Quaternion.Euler(b.rot) * tempv[i]) + b.pos;
		//	tempnorm[i] = (Quaternion.Euler(b.rot) * tempnorm[i]);
		//}
		//int t1 = tempTreeLayerMesh.triangles.Length;
		//int t2 = b.m.triangles.Length;

		//int[] tempt = new int[t1 + t2];


		////copy triangles
		//Array.Copy(tempTreeLayerMesh.triangles, 0, tempt, 0, t1);
		//Array.Copy(b.m.triangles, 0, tempt, t1, t2);
		//int triCount = t1;// tempt.length;
		//for (int i = triCount; i < tempt.Length; i++)
		//{
		//	tempt[i] += vertexCount;
		//}

		//Vector2[] tempuv = new Vector2[tempTreeLayerMesh.vertexCount + b.m.vertexCount];
		//Array.Copy(tempTreeLayerMesh.uv, 0, tempuv, 0, tempTreeLayerMesh.vertexCount);
		//Array.Copy(b.m.uv, 0, tempuv, tempTreeLayerMesh.vertexCount, b.m.vertexCount);

		//tempTreeLayerMesh.vertices = tempv;
		//tempTreeLayerMesh.uv = tempuv;//.SetUVs(0, tempuv);
		//tempTreeLayerMesh.SetTriangles(tempt, 0);
		//tempTreeLayerMesh.normals = tempnorm;//.SetNormals(tempnorm);
		//tempTreeLayerMesh.RecalculateBounds();

		//////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////

		/*

		if (endMesh.vertexCount > 0)
		{
			tempv = new Vector3[tempTreeEndMesh.vertexCount + endMesh.vertexCount];
			tempnorm = new Vector3[tempTreeEndMesh.vertexCount + endMesh.vertexCount];
			temptemp = endMesh.vertices;
			temptempnorm = endMesh.normals;
			//copy vertecies
			Array.Copy(tempTreeEndMesh.vertices, 0, tempv, 0, tempTreeEndMesh.vertexCount);
			Array.Copy(endMesh.vertices, 0, tempv, tempTreeEndMesh.vertexCount, endMesh.vertexCount);
			//copy normals
			Array.Copy(tempTreeEndMesh.normals, 0, tempnorm, 0, tempTreeEndMesh.vertexCount);
			Array.Copy(endMesh.normals, 0, tempnorm, tempTreeEndMesh.vertexCount, endMesh.vertexCount);

			vertexCount = tempTreeEndMesh.vertexCount;
			for (int i = vertexCount; i < tempv.Length; i++)
			{
				tempv[i] = (Quaternion.Euler(b.rot) * tempv[i]) + b.pos;
				tempnorm[i] = (Quaternion.Euler(b.rot) * tempnorm[i]);
			}
			t1 = tempTreeEndMesh.triangles.Length;
			t2 = endMesh.triangles.Length;

			tempt = new int[t1 + t2];


			//copy triangles
			Array.Copy(tempTreeEndMesh.triangles, 0, tempt, 0, t1);
			Array.Copy(endMesh.triangles, 0, tempt, t1, t2);
			triCount = t1;// tempt.length;
			for (int i = triCount; i < tempt.Length; i++)
			{
				tempt[i] += vertexCount;
			}

			tempuv = new Vector2[tempTreeEndMesh.vertexCount + endMesh.vertexCount];
			Array.Copy(tempTreeEndMesh.uv, 0, tempuv, 0, tempTreeEndMesh.vertexCount);
			Array.Copy(endMesh.uv, 0, tempuv, tempTreeEndMesh.vertexCount, endMesh.vertexCount);

			tempTreeEndMesh.vertices = tempv;
			tempTreeEndMesh.uv = tempuv;//.SetUVs(0, tempuv);
			tempTreeEndMesh.SetTriangles(tempt, 0);
			tempTreeEndMesh.normals = tempnorm;//.SetNormals(tempnorm);
			tempTreeEndMesh.RecalculateBounds();
		}

		*/

		/////////////////////////////
		/////////////////////////////

		/*
		tempv = new List<Vector3>();
		tempt = new List<int>();
		temptemp = endMesh.vertices;
		tempv.AddRange(tempTreeEndMesh.vertices);
		tempv.AddRange(temptemp.ToList());
		vertexCount = tempTreeEndMesh.vertexCount;
		//tempv.AddRange(endMesh.vertices);
		for (int i = vertexCount; i < tempv.Count; i++)
		{
			tempv[i] = (Quaternion.Euler(b.rot) * tempv[i]) + b.pos;
		}
		tempt.AddRange(tempTreeEndMesh.triangles);
		triCount = tempt.Count;
		tempt.AddRange(endMesh.triangles);
		for (int i = triCount; i < tempt.Count; i++)
		{
			tempt[i] += vertexCount;
		}
		tempuv = new List<Vector2>();
		tempuv.AddRange(tempTreeEndMesh.uv);
		tempuv.AddRange(endMesh.uv);
		tempnorm = new List<Vector3>();
		tempnorm.AddRange(tempTreeEndMesh.normals);
		tempnorm.AddRange(endMesh.normals);
		tempTreeEndMesh.SetVertices(tempv);
		tempTreeEndMesh.SetUVs(0, tempuv);
		tempTreeEndMesh.SetTriangles(tempt, 0);
		tempTreeEndMesh.SetNormals(tempnorm);
		tempTreeEndMesh.RecalculateBounds();
		CombineInstance[] cbranch = new CombineInstance[2];//combine to other branches
		cbranch[0].mesh = endMesh;
		cbranch[0].transform = Matrix4x4.TRS(b.pos, Quaternion.Euler(b.rot), b.scale);
		cbranch[1].mesh = tempTreeLayerMesh;// (Mesh)Instantiate(tempTreeLayerMesh);
		cbranch[1].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
		tempTreeLayerMesh.CombineMeshes(cbranch);
		tempv = null;
		tempuv = null;
		tempnorm = null;
		tempt = null;
		temptemp = null;
		temptempnorm = null;
		*/

		return b;
	}

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles, Vector3 scale)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler(angles) * dir; // rotate it
											  //dir.Scale(scale);
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	//function used to generate tree. Calls generateLayerBranches and makeLayerBranches for every tree layer.
	//also makes sure that the parent tree layer is done already before doing child
	public void GenerateAll(bool redoOffsets = true)
	{//if false doesn't work
		long tickStart = DateTime.Now.Ticks;
		//long tick = 0;
		//print("GEN_TREE Starting... " + ((DateTime.Now.Ticks-tickStart)/10000f).ToString());

		if (redoOffsets) allBranches = new List<Branch>();

		
		int cc = transform.childCount;
		for (int j = 0; j < cc; j++)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		for (int ii = 0; ii < layers.Count; ii++)
		{
			layers[ii].mesh = new List<Mesh>();
			layers[ii].endMesh = new List<Mesh>();
			//layers[ii].mat = new List<Material>();
			//layers[ii].cutMat = new List<Material>();

			for (int iii = 0; iii < numberOfLods; iii++)
			{
				layers[ii].mesh.Add(null);
				layers[ii].endMesh.Add(null);
				//layers[ii].mat.Add(null);
				//layers[ii].cutMat.Add(null);
			}
		}
		meshesToShow = new List<List<Mesh>>();
		mats = new List<List<Material>>();
		for (lod = 0; lod < numberOfLods; lod++)
		{
			for (int iii = 0; iii < layers.Count; iii++)
			{
				if (layers[iii].autoCreateMesh)
				{
					subdivs = Mathf.CeilToInt(layers[iii].subdivs * lodMults[lod]);
					if (subdivs < 3) subdivs = 3;
					layers[iii].segmantMesh = GetMeshFromPoints(0.5f, 0.5f, 0, subdivs, layers[iii].uvYScale);
				}
			}
			meshesToShow.Add(new List<Mesh>());
			mats.Add(new List<Material>());//TODO
			GameObject g = new GameObject("Lod_" + lod);
			g.transform.SetParent(transform);
			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;
			g.transform.localScale = Vector3.one;
			//print("GEN_TREE Starting generating branches... " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());
			List<int> done = new List<int>();
			while (done.Count < layers.Count)
			{
				for (int i = 0; i < layers.Count; i++)
				{
					if (layers[i].parent == i || done.Contains(layers[i].parent))
					{
						if (redoOffsets)
						{
							//if (!layers [i].isLeaf) {
							layers[i] = generateLayerBranches(layers[i]);
							//} else {
							//layers [i] = generateLayerLeafes (layers [i]);
							//}
						}
						//tick = DateTime.Now.Ticks;
						layers[i] = makeLayerBranches(layers[i], redoOffsets);
						//print("Made layer branches" + ((DateTime.Now.Ticks - tick) / 10000f).ToString());

						done.Add(i);
						//print(layers[i].mat.Count + ", " + ,mats[);
						meshesToShow[lod].Add(layers[i].mesh[lod]);
						mats[lod].Add(layers[i].mat[lod]);
						meshesToShow[lod].Add(layers[i].endMesh[lod]);
						mats[lod].Add(layers[i].cutMat[lod]);
					}
				}
				redoOffsets = false;//make all lods of the same tree
			}
			//GC.Collect();
			//print("GEN_TREE Finished generating branches, starting meshes... " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());

			SetMeshes(meshesToShow[lod], g.transform, mats[lod]);
		}


		LODGroup lg = gameObject.GetComponent<LODGroup>();
		LOD[] lods = new LOD[transform.childCount];

		if (lg == null)
		{
			lg = gameObject.AddComponent<LODGroup>();
			for(int iii = 0;iii < lods.Length; iii++)
			{
				lods[iii].fadeTransitionWidth = (iii / (lods.Length + 1f));
			}
		}
		else
		{
			lods = lg.GetLODs();
			if(lods.Length > numberOfLods)
			{
				List<LOD> templod = lods.ToList();
				templod.RemoveRange(numberOfLods, lods.Length);
				lods = templod.ToArray();
			}
			if (lods.Length < numberOfLods)
			{
				List<LOD> templod = lods.ToList();
				for(int k = lods.Length;k < numberOfLods; k++)
				{
					LOD lodtemp = new LOD
					{
						fadeTransitionWidth = (k / (numberOfLods + 1f))
					};
					templod.Add(lodtemp);
				}
				lods = templod.ToArray();
			}
		}

		for (int ii = 0; ii < transform.childCount; ii++)
		{
			lods[ii].renderers = transform.GetChild(ii).GetComponentsInChildren<Renderer>();
		}
		lg.SetLODs(lods);
		GC.Collect();
		print("GEN_TREE Finished. " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());

	}
#region dfakl
    /*public IEnumerator GenerateAllAsync(bool redoOffsets = true, int speed = 5, List<bool> lodsToDo = null)
	{//if false doesn't work
	 //TODO: make this function last over a few frames to prevent from freezing game
	 //long tickStart = DateTime.Now.Ticks;
		if(lodsToDo == null)
		{
			lodsToDo = new List<bool>();
			for(int i = lodsToDo.Count;i < numberOfLods; i++)
			{
				lodsToDo.Add(true);
			}
		}
		sw.Restart();
		//long tick = 0;
		//print("GEN_TREE Starting... " + ((DateTime.Now.Ticks-tickStart)/10000f).ToString());
		
		if (redoOffsets) allBranches = new List<Branch>();

		List<Transform> lodGameObjects = new List<Transform>();

		for (int ii = 0; ii < layers.Count; ii++)
		{
			layers[ii].mesh = new List<Mesh>();
			layers[ii].endMesh = new List<Mesh>();
			//layers[ii].mat = new List<Material>();
			//layers[ii].cutMat = new List<Material>();

			for (int iii = 0; iii < numberOfLods; iii++)
			{
				layers[ii].mesh.Add(null);
				layers[ii].endMesh.Add(null);
				//layers[ii].mat.Add(null);
				//layers[ii].cutMat.Add(null);
			}
		}
		meshesToShow = new List<List<Mesh>>();
		mats = new List<List<Material>>();
		for (lod = 0; lod < numberOfLods; lod++)
		{
			for (int iii = 0; iii < layers.Count; iii++)
			{
				if (layers[iii].autoCreateMesh)
				{
					subdivs = Mathf.CeilToInt(layers[iii].subdivs * lodMults[lod]);
					if (subdivs < 3) subdivs = 3;
					layers[iii].segmantMesh = GetMeshFromPoints(0.5f, 0.5f, 0, subdivs, layers[iii].uvYScale);
				}
			}
			meshesToShow.Add(new List<Mesh>());
			mats.Add(new List<Material>());//TODO
			GameObject g = new GameObject("Lod_" + lod);
			lodGameObjects.Add(g.transform);
			//print("GEN_TREE Starting generating branches... " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());
			List<int> done = new List<int>();
			while (done.Count < layers.Count)
			{
				for (int i = 0; i < layers.Count; i++)
				{
					if (layers[i].parent == i || done.Contains(layers[i].parent))
					{
						if (redoOffsets)
						{
							//if (!layers [i].isLeaf) {
							layers[i] = generateLayerBranches(layers[i]);
							//} else {
							//layers [i] = generateLayerLeafes (layers [i]);
							//}
						}
						//tick = DateTime.Now.Ticks;
						if (lodsToDo[i] == true)
						{
							yield return StartCoroutine(makeLayerBranchesAsync(layers[i], redoOffsets, speed));

						}
						else
						{
							layers[i].mesh[lod] = new Mesh();// (Mesh)Instantiate(tempTreeLayerMesh);
							layers[i].endMesh[lod] = new Mesh();
						}
						//while (makingBranches) yield return new WaitForSeconds(0.05f);
						//print("Made layer branches" + ((DateTime.Now.Ticks - tick) / 10000f).ToString());

						done.Add(i);
						//print(layers[i].mat.Count + ", " + ,mats[);
						meshesToShow[lod].Add(layers[i].mesh[lod]);
						mats[lod].Add(layers[i].mat[lod]);
						meshesToShow[lod].Add(layers[i].endMesh[lod]);
						mats[lod].Add(layers[i].cutMat[lod]);
					}
				}
				redoOffsets = false;//make all lods of the same tree
			}
			//GC.Collect();
			//print("GEN_TREE Finished generating branches, starting meshes... " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());
			g.SetActive(false);
			SetMeshes(meshesToShow[lod], g.transform, mats[lod], false, true);
			
		}


		int cc = transform.childCount;
		for (int j = 0; j < cc; j++)
		{
			foreach (Renderer r in transform.GetChild(0).GetComponentsInChildren<Renderer>())
			{
				r.GetComponent<Renderer>().enabled = false;
			}
			DestroyImmediate(transform.GetChild(0).gameObject);
		}





		for (int j = 0;j < lodGameObjects.Count; j++)
		{
			Transform g = lodGameObjects[j];
			g.gameObject.SetActive(true);
			g.SetParent(transform);
			g.localPosition = Vector3.zero;
			g.localRotation = Quaternion.identity;
			g.localScale = Vector3.one;
		}
		LODGroup lg = gameObject.GetComponent<LODGroup>();
		LOD[] lods = new LOD[transform.childCount];

		if (lg == null)
		{
			lg = gameObject.AddComponent<LODGroup>();
			for (int iii = 0; iii < lods.Length; iii++)
			{
				lods[iii].fadeTransitionWidth = (iii / (lods.Length + 1f));
			}
		}
		else
		{
			lods = lg.GetLODs();
			if (lods.Length > numberOfLods)
			{
				List<LOD> templod = lods.ToList();
				templod.RemoveRange(numberOfLods, lods.Length);
				lods = templod.ToArray();
			}
			if (lods.Length < numberOfLods)
			{
				List<LOD> templod = lods.ToList();
				for (int k = lods.Length; k < numberOfLods; k++)
				{
					LOD lodtemp = new LOD
					{
						fadeTransitionWidth = (k / (numberOfLods + 1f))
					};
					templod.Add(lodtemp);
				}
				lods = templod.ToArray();
			}
		}
		for (int ii = 0; ii < transform.childCount; ii++)
		{
			lods[ii].renderers = transform.GetChild(ii).GetComponentsInChildren<Renderer>();
		}
		lg.SetLODs(lods);
		GC.Collect();
		print("GEN_TREE Finished. " + (sw.ElapsedTicks / 10000f).ToString());
		sw.Stop();


	}*/
#endregion
    public GameObject CutBranch(int b, float prog)
	{
		float t1 = Time.realtimeSinceStartup;
		GameObject g = new GameObject("Branch");
        List<List<Mesh>> meshes = new List<List<Mesh>>();
        meshesToShow = new List<List<Mesh>>();
        mats = new List<List<Material>>();
        List<List<Material>> mat = new List<List<Material>>();
        tempMesh = new List<List<Mesh>>();//2d lod#,branch#
        tempMeshE = new List<List<Mesh>>();//2d lod#,branch#
        for (int ii = 0; ii < layers.Count; ii++)
        {
            layers[ii].mesh = new List<Mesh>();
            layers[ii].endMesh = new List<Mesh>();
            //layers[ii].mat = new List<Material>();
            //layers[ii].cutMat = new List<Material>();

            for (int iii = 0; iii < numberOfLods; iii++)
            {
                layers[ii].mesh.Add(null);
                layers[ii].endMesh.Add(null);
                //layers[ii].mat.Add(null);
                //layers[ii].cutMat.Add(null);
            }
        }
        for (lod = 0; lod < numberOfLods; lod++)
		{
            tempMesh.Add(new List<Mesh>());
            tempMeshE.Add(new List<Mesh>());
			List<List<Mesh>> tempm = new List<List<Mesh>>();
			List<List<Mesh>> tempmE = new List<List<Mesh>>();

			List<List<int>> indexes = new List<List<int>>();//The indexes (branch mesh) which needed a mesh, weren't completely hidden
			List<List<int>> indexesE = new List<List<int>>();//The indexes (branch but mesh, endmesh) which needed a mesh, weren't completely hidden

			for (int i = 0;i < layers.Count; i++)
			{
				tempm.Add(new List<Mesh>());
				indexes.Add(new List<int>());
				tempmE.Add(new List<Mesh>());
				indexesE.Add(new List<int>());
			}
			meshesToShow.Add(new List<Mesh>());
			mats.Add(new List<Material>());//TODO
            //mat.Add(new List<Material>());
			GameObject g1 = new GameObject("Lod_" + lod);
			g1.transform.SetParent(g.transform);
			g1.transform.localPosition = Vector3.zero;
			g1.transform.localRotation = Quaternion.identity;
			g1.transform.localScale = Vector3.one;

			//List<TreeLayer> done = new List<TreeLayer>();
			
			List<int> branches = new List<int>() { b };
			List<int> branches1 = new List<int>();
			List<int> destroyThese = new List<int>();
			int times = 0;

			while (branches.Count > 0 && times < 10)
			{
                print("making branches ");
				for (int i = 0; i < branches.Count; i++)
				{
					tempTreeLayerMesh = new Mesh();
					if (times == 0)
					{
						//does this add new verticies to temptreelayermesh? it should
						//make branch except cut
						MakeBranch(allBranches[branches[i]], layers[allBranches[branches[i]].layer], false, prog);
						for (int j = 0; j < allBranches[branches[i]].children.Count; j++)
						{
							if (1 - allBranches[allBranches[branches[i]].children[j]].progTillTop > prog)
							{
								branches1.Add(allBranches[branches[i]].children[j]);
							}
						}
					}
					else
					{
						MakeBranch(allBranches[branches[i]], layers[allBranches[branches[i]].layer], false);
						branches1.AddRange(allBranches[branches[i]].children);
					}
					indexes[allBranches[branches[i]].layer].Add(branches[i]);
					tempm[allBranches[branches[i]].layer].Add(tempTreeLayerMesh);
					if (!layers[allBranches[branches[i]].layer].isLeaf)
					{
						indexesE[allBranches[branches[i]].layer].Add(branches[i]);
						tempmE[allBranches[branches[i]].layer].Add(tempTreeEndMesh);
					}
					
				}
				//meshesToShow[lod].Add(tempTreeLayerMesh);
				//mat[lod].Add(layers[allBranches[branches[0]].layer].mat[lod]);

				branches = branches1;
				destroyThese.AddRange(branches1);
				branches1 = new List<int>();
				times++;
			}
			for (int q = 0; q < layers.Count; q++)
			{
				CombineInstance[] ci = new CombineInstance[tempm[q].Count];
				CombineInstance[] cie = new CombineInstance[tempmE[q].Count];

				for (int i = 0; i < tempm[q].Count; i++)
				{
					Branch bb = allBranches[indexes[q][i]];
					ci[i].mesh = tempm[q][i];
					ci[i].transform = Matrix4x4.TRS(bb.pos, Quaternion.Euler(bb.rot), bb.scale);
				}
				for (int i = 0; i < tempmE[q].Count; i++)
				{
					Branch bb = allBranches[indexesE[q][i]];
					cie[i].mesh = tempmE[q][i];
					cie[i].transform = Matrix4x4.TRS(bb.pos, Quaternion.Euler(bb.rot), bb.scale);
				}
				tempTreeLayerMesh = new Mesh();
				tempTreeEndMesh = new Mesh();
				tempTreeLayerMesh.CombineMeshes(ci);
				tempTreeEndMesh.CombineMeshes(cie);
				meshesToShow[lod].Add(tempTreeLayerMesh);
                mats[lod].Add(layers[q].mat[lod]);
				meshesToShow[lod].Add(tempTreeEndMesh);
				mats[lod].Add(layers[q].cutMat[lod]);
				//print("Prepaired a mesh for cutting with verticies: " + tempTreeLayerMesh.vertexCount);

			}
			if (times >= 10)
			{
				UnityEngine.Debug.LogError("infinite loop");
				Application.Quit();
			}

			for (int i = 0; i < destroyThese.Count; i++)
			{
				//allBranches[destroyThese[i]].maxLength = 0;
				allBranches[destroyThese[i]].lengthRelative = 0;
				allBranches[destroyThese[i]].wasCut = true;
				//allBranches[destroyThese[i]].GrowthLength = 0;
			}
			//int iii = Mathf.FloorToInt(prog * allBranches[b].maxLength);
			//allBranches[b].GrowthLength = (prog * allBranches[b].maxLength)%1;
			allBranches[b].length = prog * allBranches[b].maxLength;
			for (int i = 0; i < meshesToShow[lod].Count; i++)
			{
				print("Ready to make a mesh for cutting with verticies: " + meshesToShow[lod][i].vertexCount);
			}
			//print(meshesToShow[lod].Count + " " + mat[slod].Count);
			SetMeshes(meshesToShow[lod], g1.transform, mats[lod], true);

		}
		GenerateAll(false);
		//////////////////
		LODGroup lg = g.AddComponent<LODGroup>();//TODO change this to work for cutBranch
        LODGroup lgTree = gameObject.GetComponent<LODGroup>();
        LOD[] lods = new LOD[g.transform.childCount];

		lods = lgTree.GetLODs();//here
		if (lods.Length > numberOfLods)
		{
			List<LOD> templod = lods.ToList();
            templod.RemoveRange(numberOfLods, lods.Length-numberOfLods);
			lods = templod.ToArray();
		}
		if (lods.Length < numberOfLods)
		{
			List<LOD> templod = lods.ToList();
			for (int k = lods.Length; k < numberOfLods; k++)
			{
				LOD lodtemp = new LOD
				{
					fadeTransitionWidth = (k / (numberOfLods + 1f))
				};
				templod.Add(lodtemp);
			}
			lods = templod.ToArray();
		}

		for (int ii = 0; ii < g.transform.childCount; ii++)
		{
			lods[ii].renderers = g.transform.GetChild(ii).GetComponentsInChildren<Renderer>();
		}
		lg.SetLODs(lods);
		//GC.Collect();
		//print("GEN_TREE Finished. " + ((DateTime.Now.Ticks - tickStart) / 10000f).ToString());
		////////////

		GC.Collect();
		print("Cut branch, took " + (Time.realtimeSinceStartup - t1).ToString() + "s");
		return g;
		//return new GameObject("ERROR__CUTBRANCH IN PROG LOD NOT WORK");
	}

	//TODO:performance
	public Mesh GetMeshFromPoints(float r1, float r2, float prog, int sd, float uvY, bool flipped = false)
	{
		Mesh m = new Mesh();
		r2 = Mathf.Lerp(r2, r1, prog);
		Vector3[] vert = new Vector3[sd * 4];
		Vector3[] norm = new Vector3[sd * 4];
		Vector2[] uv = new Vector2[sd * 4];
		int[] tri = new int[sd * 6];
		float ri1pos = flipped ? 0 : prog;
		float ri2pos = flipped ? prog : 1;
		for (int i = 0; i < sd; i++)
		{
			float i1 = i, i2 = i + 1;

			float a1 = 360f / sd * i1 * Mathf.Deg2Rad;
			float a2 = 360f / sd * i2 * Mathf.Deg2Rad;
			float CosA1 = Mathf.Cos(a1);
			float CosA2 = Mathf.Cos(a2);
			float SinA1 = Mathf.Sin(a1);
			float SinA2 = Mathf.Sin(a2);


			vert[i * 2] = new Vector3(CosA1 * r1, ri1pos, SinA1 * r1);// *r1 and *r2 is wrong, but it works for now.
			vert[i * 2 + 1] = new Vector3(CosA2 * r1, ri1pos, SinA2 * r1);

			vert[sd * 2 + i * 2] = new Vector3(CosA1 * r2, ri2pos, SinA1 * r2);
			vert[sd * 2 + i * 2 + 1] = new Vector3(CosA2 * r2, ri2pos, SinA2 * r2);

			norm[i * 2] = new Vector3(CosA1, 0, SinA1);
			norm[i * 2 + 1] = new Vector3(CosA2, 0, SinA2);

			norm[sd * 2 + i * 2] = new Vector3(CosA1, 0, SinA1);
			norm[sd * 2 + i * 2 + 1] = new Vector3(CosA2, 0, SinA2);

			uv[i * 2] = new Vector2(i1 / sd, ri1pos * uvY);
			uv[i * 2 + 1] = new Vector2(i2 / sd, ri1pos * uvY);

			uv[sd * 2 + i * 2] = new Vector2(i1 / sd, ri2pos * uvY);
			uv[sd * 2 + i * 2 + 1] = new Vector2(i2 / sd, ri2pos * uvY);
		}

		//make rings have faces between
		for (int i = 0; i < sd; i++)
		{
			int i1 = i + sd;
			tri[i * 6 + 0] = i1 * 2;
			tri[i * 6 + 1] = i * 2 + 1;
			tri[i * 6 + 2] = i * 2;

			tri[i * 6 + 3] = i * 2 + 1;
			tri[i * 6 + 4] = i1 * 2;
			tri[i * 6 + 5] = i1 * 2 + 1;
		}

		m.vertices = vert;
		m.triangles = tri;
		m.normals = norm;
		m.uv = uv;

		return m;
	}

	public Mesh GetEndMesh(float rad, int sd, Vector3 pos, bool flipped = false)
	{
		Vector3[] v = new Vector3[sd * 3];
		Vector2[] uv = new Vector2[sd * 3];
		int[] t = new int[sd * 3];
		Vector3[] n = new Vector3[sd * 3];


		float offset = UnityEngine.Random.Range(0, 360);
		float divsd = 360f / (sd);

		for (float i = 0; i < 360; i += divsd)
		{
			int index = Mathf.RoundToInt(i / divsd);
			float idivsd = i + divsd;
			if (flipped)
			{
				v[index * 3 + 1] = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * rad;
				v[index * 3 + 0] = new Vector3(Mathf.Cos((idivsd) * Mathf.Deg2Rad), 0, Mathf.Sin((idivsd) * Mathf.Deg2Rad)) * rad;
			}
			else
			{
				v[index * 3 + 0] = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * rad;
				v[index * 3 + 1] = new Vector3(Mathf.Cos((idivsd) * Mathf.Deg2Rad), 0, Mathf.Sin((idivsd) * Mathf.Deg2Rad)) * rad;
			}
			//v[index * 3 + 0] = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * rad;
			//v[index * 3 + 1] = new Vector3(Mathf.Cos((idivsd) * Mathf.Deg2Rad), 0, Mathf.Sin((idivsd) * Mathf.Deg2Rad)) * rad;
			v[index * 3 + 2] = Vector3.zero;

			Vector2 offset1 = Vector2.one / 2;
			uv[index * 3 + 0] = offset1 + new Vector2(Mathf.Cos((i + offset) * Mathf.Deg2Rad) / 2, Mathf.Sin((i + offset) * Mathf.Deg2Rad) / 2);
			uv[index * 3 + 1] = offset1 + new Vector2(Mathf.Cos((idivsd + offset) * Mathf.Deg2Rad) / 2, Mathf.Sin((idivsd + offset) * Mathf.Deg2Rad) / 2);
			uv[index * 3 + 2] = offset1;

			t[index * 3 + 0] = (index + 1) * 3 - 3;
			t[index * 3 + 1] = (index + 1) * 3 - 1;
			t[index * 3 + 2] = (index + 1) * 3 - 2;

			n[index * 3 + 0] = Vector3.forward;
			n[index * 3 + 1] = Vector3.forward;
			n[index * 3 + 2] = Vector3.forward;

		}

		for (int i = 0; i < v.Length; i++)
		{
			Vector3 tmp = v[i];
			//tmp.Scale(scale);
			tmp += pos;
			//tmp = Quaternion.Euler(rot) * tmp;
			//tmp += globalPos;
			v[i] = tmp;
		}
		Mesh m = new Mesh
		{
			vertices = v,
			normals = n,
			uv = uv,
			triangles = t
		};
		return m;
	}

	//take the meshes generated, and orginize them into meshes and gameObjects
	//
	//void SetMeshes(){ SetMeshes(meshesToShow, transform, false, null); }
	void SetMeshes(List<Mesh> meshes, Transform parent, List<Material> mat, bool autoScaleToTree = false, bool deleteChildren = false)
	{
		print(meshes.Count + ", " + mat.Count);
		while (deleteChildren && parent.childCount > 0)
		{
			DestroyImmediate(parent.GetChild(0).gameObject);
		}
		List<int> done = new List<int>();
		for (int i = 0; i < meshes.Count; i++)
		{
			print(meshes[i].vertexCount);
			if (mat[i] == null ||
                meshes[i].vertexCount == 0)
			{
				print("Removed Unnessasary Mesh " + (mat[i] == null).ToString());

				meshes.RemoveAt(i);
				mat.RemoveAt(i);
				i--;
			}
		}
		//List<int> done1 = new List<int>();
		while (done.Count < meshes.Count)
		{
			GameObject g = new GameObject("Mesh " + parent.childCount);

			MeshFilter mf = g.AddComponent<MeshFilter>();
			MeshRenderer mr = g.AddComponent<MeshRenderer>();

			g.transform.SetParent(parent);
			g.transform.localPosition = Vector3.zero;
			g.transform.localScale = autoScaleToTree ? parent.localScale : Vector3.one;

			int verticiesDone = 0;
			Mesh m = new Mesh();
			Material material = null;
			List<CombineInstance> ci = new List<CombineInstance>();
			for (int i = 0; i < meshes.Count; i++)
			{
				if (!done.Contains(i) &&
					meshes[i].vertexCount + verticiesDone < 65000 && (material == null || mat[i] == material))
				{
					if (material == null)
					{
						material = mat[i];
					}

					verticiesDone += meshes[i].vertexCount;
					if (meshes[i].vertexCount > 65000)
					{
						UnityEngine.Debug.Log("Tree Layer mesh has more than 65000 verticies.");
						return;
					}
					if (verticiesDone > 65000)
					{
						UnityEngine.Debug.LogError("Tree Layer mesh has more than 65000 verticies.");
						break;
					}
					//Debug.Log("Added mesh with " + meshes[i].vertexCount + " verticies.");

					CombineInstance c = new CombineInstance();
					c.mesh = meshes[i];

					c.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
					ci.Add(c);
					done.Add(i);
				}

			}
			m.subMeshCount = ci.Count;
			m.CombineMeshes(ci.ToArray());
			m.RecalculateBounds();
			//m.RecalculateNormals();
			//m.RecalculateTangents();
			mr.material = material;
			mf.sharedMesh = (Mesh)Instantiate(m);
			print(g.GetComponent<Renderer>().bounds.ToString());
		}

		//Debug.LogFormat("Set {0} meshes of {1} gameobjects.", meshesToShow.Count, parent.childCount);
		//		meshesToShow = new List<Mesh>();
	}

	public bool settingMeshes;
	public int setMeshLodsDone;
	//public int setMeshProgInLod;
	public IEnumerator SetMeshesAsync(List<List<Mesh>> meshes, Transform parent, List<List<Material>> mat, bool autoScaleToTree = false, bool deleteChildren = false)
	{
		settingMeshes = true;
		setMeshLodsDone = 0;
		while (parent.childCount > 0)
		{
			DestroyImmediate(parent.GetChild(0).gameObject);
		}

		List<Mesh> tMeshes = new List<Mesh>();
		List<Material> tMat = new List<Material>();

		for (lod = 0;lod < numberOfLods;lod++) {
			List<int> done = new List<int>();
			for (int i = 0; i < meshes[lod].Count; i++)
			{
				if (mat[lod][i] == null || 
                    meshes[lod][i].vertexCount == 0)
				{
					//print("Removed Unnessasary Mesh " + (mat[i] == null).ToString());

					meshes[lod].RemoveAt(i);
					mat[lod].RemoveAt(i);
					i--;
				}
			}
			//List<int> done1 = new List<int>();
			while (done.Count < meshes[lod].Count)
			{
				GameObject g = new GameObject("Mesh " + parent.childCount);

				MeshFilter mf = g.AddComponent<MeshFilter>();
				MeshRenderer mr = g.AddComponent<MeshRenderer>();

				g.transform.SetParent(parent);
				g.transform.localPosition = Vector3.zero;
				g.transform.localScale = autoScaleToTree ? parent.localScale : Vector3.one;

				int verticiesDone = 0;
				Mesh m = new Mesh();
				Material material = null;
				List<CombineInstance> ci = new List<CombineInstance>();
				for (int i = 0; i < meshes[lod].Count; i++)
				{
					if (!done.Contains(i) &&
						meshes[lod][i].vertexCount + verticiesDone < 65000 && (material == null || mat[lod][i] == material))
					{
						if (material == null)
						{
							material = mat[lod][i];
						}

						verticiesDone += meshes[lod][i].vertexCount;
						if (meshes[lod][i].vertexCount > 65000)
						{
							//UnityEngine.Debug.Log("Tree Layer mesh has more than 65000 verticies.");
							yield return new WaitForEndOfFrame();
						}
						if (verticiesDone > 65000)
						{
							UnityEngine.Debug.LogError("Tree Layer mesh has more than 65000 verticies.");
							break;
						}
						//Debug.Log("Added mesh with " + meshesToShow[i].vertexCount + " verticies.");

						CombineInstance c = new CombineInstance();
						c.mesh = meshes[lod][i];

						c.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
						ci.Add(c);
						done.Add(i);
					}

				}
				m.subMeshCount = ci.Count;
				m.CombineMeshes(ci.ToArray());
				m.RecalculateBounds();
				//m.RecalculateNormals();
				//m.RecalculateTangents();
				//mr.material = material;
				tMat.Add(material);
				tMeshes.Add(m);
				//mf.sharedMesh = (Mesh)Instantiate(m);

			}
			yield return new WaitForEndOfFrame();
		}

		//Debug.LogFormat("Set {0} meshes of {1} gameobjects.", meshesToShow.Count, parent.childCount);
		//		meshesToShow = new List<Mesh>();
		settingMeshes = false;
		yield return new WaitForEndOfFrame();
	}

	#endregion


	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	public void OnValidate()//TODO save data so stuff like segmant meshes don't get lost when i exit unity
	{
		TreeLayer.defaultMesh = defaultMesh;

		//for(int i = 0; i < layers.Count; i++)
		//{
		//	TreeLayer l = layers[i];
		//	FileStream fs = File.Create(pathToSaveData + "/TREELAYER_" + i + "_MESH.treedata");
		//	BinaryWriter bw = new BinaryWriter(fs);
		//	BinaryFormatter bf = new BinaryFormatter();
		//	bw.Write(l.mesh.vertexCount);
		//	bf.Serialize(fs, l.segmantMesh);
		//	fs.Close();
		//	fs.Dispose();
		//}
	}
}
