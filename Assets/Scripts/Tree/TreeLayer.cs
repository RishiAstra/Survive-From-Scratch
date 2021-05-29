/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public enum TreeLayerType { Alternate, Opposite, Whorled, Random };

namespace MyStuff
{
	[System.Serializable]
	public class TreeAlternateObj : System.Object
	{
		public float minAngle1, maxAngle1, mirrorAngle;
		public bool mirror, deg90;
	}
	[System.Serializable]
	public class TreeOppositeObj : System.Object
	{
		public float minAngle1, maxAngle1, mirrorAngle;
		public bool mirror;
	}
	[System.Serializable]
	public class TreeWhorledObj : System.Object
	{
		//public AnimationCurve amount;
		public AnimationCurve amount;
		public float amountMult;
		public float minAngle1, maxAngle1,
			minHeight, maxHeight,
			amountRandomMin, amountRandomMax;
	}
	[System.Serializable]
	public class TreeRandomObj : System.Object
	{

	}
}

[System.Serializable]
public class Branch
{
	public List<Vector3> offsets;//list of offsets, used for crincling branches
	public float maxLength;//Actual maximum length. If actual length = this, then the branch is not cut
	public float sizeScale;

	//public int IntMaxLength
	//{//I changed from maxLength being a int to it being a float (to make tree growth easier), but added this to replace places that need an intiger
	//	get { return Mathf.FloorToInt(maxLength); }
	//	set { maxLength = maxLength % 1 + value; }
	//}//the current maximum length. If length is smaller, then the end of branch will look cut (higher than 0 radius)

	public float length
	{
		get { return lengthRelative * maxLength; }
		set { lengthRelative = value / maxLength; }
	}

	public float lengthRelative;

	public float GrowthLength
	{
		get { return length % 1; }
		set { length = Mathf.FloorToInt(length) + value; }
	}//

	public int IntLength
	{
		get { return Mathf.FloorToInt(length); }
		set { length = length % 1 + value; }
	}//the current length (when added to growthLength), see above

	public float compProg;

	/// <summary>
	/// The position of the branch, relative to parent
	/// </summary>
	public Vector3 pos;
	/// <summary>
	/// The rotation of the branch, in euler angles, relative to parent
	/// </summary>
	public Vector3 rot;
	/// <summary>
	/// the scale of the branch, relative to parent
	/// </summary>
	public Vector3 scale;
	/// <summary>
	/// How far this branch is from the top of parent branch (0 = at bottom of parent, 1 = at top of parent)
	/// </summary>
	public float progTillTop;
	public float prog;
	/// <summary>
	/// This branches parent branch 
	/// (stored as a int, access the branch with allbranches[myBranch.parent])
	/// </summary>
	public int parentBranch;
	/// <summary>
	/// The index of this branch's tree layer
	/// </summary>
	public int layer;
	/// <summary>
	/// A multiplier for this branches thickness (relative xz)
	/// </summary>
	public float thickness;
	/// <summary>
	/// All children of this branch (branches that come off of it)
	/// (stored as a int, access the branch with allbranches[myBranch.parent])
	/// </summary>
	public List<int> children;
	/// <summary>
	/// This branch
	/// (stored as a int, access the branch with allbranches[myBranch.parent])
	/// </summary>
	public int index;

	public bool wasCut;

	/// <summary>
	/// A mesh, used when generating or updating the tree
	/// </summary>
	[SerializeField]
	public Mesh m;
	[SerializeField]
	public Mesh mEnd;

	/// <summary>
	/// Create a new branch
	/// </summary>
	/// <param name="maxLength">This branches length, when undamaged</param>
	/// <param name="length">This branches current length, even if damaged</param>
	/// <param name="growthLength"></param>
	/// <param name="pos">This branch's position relative to its parent</param>
	/// <param name="rot">This branch's rotation relative to its parent</param>
	/// <param name="scale">This branch's scale |NOT| relative to its parent</param>
	/// <param name="progTillTop">Y-Position relative to parent branch. 0 = at bottom of parent branch, 1 = at top</param>
	/// <param name="parentBranch">The parent branch's index in allBranches list</param>
	/// <param name="l">The layer of this branch</param>
	/// <param name="myIndex">This branch's index in allBranches list</param>
	public Branch(float maxLength, float length, Vector3 pos, Vector3 rot, Vector3 scale, float progTillTop, int parentBranch, int l, int myIndex, float ProgOnParent)
	{
		this.maxLength = maxLength;
		this.sizeScale = maxLength;
		this.length = length;
		//this.GrowthLength = growthLength;
		this.pos = pos;
		this.rot = rot;
		this.scale = scale;
		this.offsets = new List<Vector3>();
		this.children = new List<int>();
		this.progTillTop = progTillTop;
		this.parentBranch = parentBranch;
		this.layer = l;
		this.index = myIndex;
		this.m = new Mesh();
		this.thickness = 1;
		prog = ProgOnParent;
	}
}

//if parent = index in layers, it means it has no parent
[System.Serializable]
public class TreeLayer : System.Object
{

	//	[SerializeField]
	public static Mesh defaultMesh;

	public TreeLayerType type;
	public TreeAlternateObj ao;
	public TreeOppositeObj oo;
	public TreeWhorledObj wo;
	public TreeRandomObj ro;
	public bool colliderEnable;
	public bool colliderIsTrigger;
	public bool startHigh;
	public bool randomZRotation;

	public float startBranches;
	public float endBranches;
	public bool autoCreateMesh;
	public bool branchRangeGlobal;
	public bool isLeaf;
	public bool allMaterialSame;
	public Material allMaterial;
	public Material allCutMaterial;
	public List<Material> mat;
	public List<Material> cutMat;
	public int parent;
	public Mesh segmantMesh;
	public List<Mesh> mesh;
	public List<Mesh> endMesh;
	[SerializeField]
	public int subdivs;
	[Range(0.05f, 2f)]
	[SerializeField]
	public float segLength;
	[SerializeField]
	public float uvYScale;
	[HideInInspector()] public List<int> branches;
	public AnimationCurve size;
	[Range(0, 30)]
	public float sizeMult;
	public AnimationCurve length;
	[Range(0, 30)]
	public float lengthMult;
	[Range(0, 30)]
	public float lengthRandomness;
	public float randomness;
	public float angleRandomness;
	public float noiseScale;
	[Range(0.001f, 5f)]
	public float thickness;
	[Range(-90, 90)]
	public float angle;
	public AnimationCurve progCurve;
	public AnimationCurve distProgCurve;

	//	public float minAngle;
	//	public float maxAngle;
	//	public float yAngle;
	//public TreeLayer(int parent, List<int> branches, float sizeMult, float randomness, float angleRandomness, float noiseScale, float angle, float lengthMult, float thickness)
	//{
	//	this.parent = parent;
	//	this.branches = branches;
	//	//this.size = size;
	//	this.sizeMult = sizeMult;
	//	this.randomness = randomness;
	//	this.angleRandomness = angleRandomness;
	//	this.noiseScale = noiseScale;
	//	this.angle = angle;
	//	this.size = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });
	//	this.length = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });
	//	this.thickness = thickness;
	//	this.lengthMult = lengthMult;
	//	this.segmantMesh = defaultMesh;
	//	this.mat = null;
	//	this.isLeaf = false;
	//	this.type = TreeLayerType.Alternate;
	//	this.subdivs = 8;
	//	this.segLength = 1;
	//	this.uvYScale = 1;
	//	//		this.minAngle = -90;
	//	//		this.maxAngle = 90;

	//	//		this.yAngle = 180;
	//}
	public TreeLayer(int parent, List<int> branches, float sizeMult, float randomness, float angleRandomness, float noiseScale, float angle,
		float lengthMult = 1,
		float thickness = 1,
		AnimationCurve size = null, 
		AnimationCurve length = null)
	{

		////////////////////////////////////////
		this.parent = parent;
		this.branches = branches;
		//		this.size = size;
		this.sizeMult = sizeMult;
		this.lengthMult = lengthMult;
		this.randomness = randomness;
		this.angleRandomness = angleRandomness;
		this.noiseScale = noiseScale;
		this.angle = angle;
		this.size = size ?? new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });
		this.length = length ?? new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });
		this.thickness = thickness;
		this.lengthMult = lengthMult;
		///////////////////////////////////////

		wo = new TreeWhorledObj()
		{
			amount = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) })
		};
		progCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1, 0, -45), new Keyframe(1, 0, 45, 0) });
		distProgCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });


		segmantMesh = new Mesh();
		isLeaf = false;
		subdivs = 8;
		segLength = 1;
		uvYScale = 1;
		startBranches = 0;
		endBranches = 1;
		branchRangeGlobal = false;
		autoCreateMesh = true;
		allMaterialSame = true;
		startHigh = true;
		this.mat = new List<Material>();
		this.cutMat = new List<Material>();
		//		this.minAngle = 0;
		//		this.maxAngle = 180;

		//		this.yAngle = 180;
	}

}