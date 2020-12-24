using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Branches spawn when parent branch isn't quite at right length, ProgTillTop is slightly off?
//[ExecuteInEditMode()]
public class TreeLife : MonoBehaviour
{
	public bool randomTreeOnStart;
	public bool growTree;
	public bool repairTree;
	[Range(0, 10)]
	public float repairReloadTime;
	[Range(0, 10)]
	public float growReloadTime;
	[Range(0, 1000)]
	public int speed;
	[Range(0, 0.1f)]
	public float GrowAmount;
	public CustomTree target;
	[Range(0, 1)]
	public float mult;
	[Range(0, 1)]
	public float repair;

	private float repairReloadLeft;
	private float growReloadLeft;
	// Use this for initialization
	void Start()
	{
		target = GetComponent<CustomTree>();
		repairReloadLeft = repairReloadTime;
		growReloadLeft = growReloadTime;
		if (randomTreeOnStart) GrowTree();
	}

	// Update is called once per frame
	void Update()
	{
		if (repairTree)
		{
			repairReloadLeft -= Time.deltaTime;
			if(repairReloadLeft <= 0)
			{
				RepairTree();
				repairReloadLeft += repairReloadTime;
			}
		}
		if (growTree && mult != 1)
		{
			growReloadLeft -= Time.deltaTime;
		
			if (growReloadLeft <= 0)
			{
				if (mult < 1) mult += GrowAmount;
				if (mult > 1) mult = 1;
				GrowTree();
				growReloadLeft += growReloadTime;
			}
		}
		
	}

	//private void OnValidate()
	//{
	//	//GrowTree();
	//}

	public void RepairTree()
	{
		foreach (Branch b in target.allBranches)
		{
			b.length = (b.length + repair < b.maxLength) ? b.length + repair : b.maxLength;
		}
		//target.generateAll(false);
		GrowTree();
		//target.generateAll(false);
	}

	public void GrowTree()
	{
		//for(int i = 0;i<target.)
		foreach (Branch b in target.allBranches)
		{
			//float pMaxLength = b.maxLength;
			float parentMult = 1;
			if (b.wasCut && b.parentBranch != b.index) {
				float lr = target.allBranches[b.parentBranch].lengthRelative;
				float bp = 1-b.prog;

				parentMult = (lr-bp)/(1-bp);
				if (parentMult < 0) parentMult = 0;
				if (parentMult == 1) b.wasCut = false;
			}
			b.maxLength = b.sizeScale * ((mult) - (1 - b.progTillTop)) * parentMult / (1 - (1 - b.progTillTop));

			b.thickness = (mult - (1 - b.progTillTop)) / (1 - (1 - b.progTillTop));
			
			//if(b.length > 0)//TODO length messed up once it reaches 0
			//{
			//	b.length = b.length * b.maxLength / pMaxLength;
			//}
			
		}
		//target.GenerateAllAsync(false);
		//target.StopCoroutine("GenerateAllAsync");
		target.StopAllCoroutines();
		target.GenerateAll(false);

		//StartCoroutine(target.SetMeshesAsync(false, speed));
		//UnityEngine.Debug.Log("Growed tree");
	}
}
