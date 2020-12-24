#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode()]
public class terraintools : MonoBehaviour {
	[Range(0.00001f,0.1f)]
	public float xzScale1 = 0.006f;
	[Range(0.001f,10f)]
	public float yScale1 = 1f;
	[Range(0,1000f)]
	public float offsetX1 = 0;
	[Range(0,1000f)]
	public float offsetZ1 = 0;

	[Range(0.00001f, 0.1f)]
	public float xzScale2 = 0.03f;
	[Range(0.001f,10f)]
	public float yScale2 = 0.2f;
	[Range(0.001f,1000f)]
	public float offsetX2 = 0;
	[Range(0.001f,1000f)]
	public float offsetZ2;

	[Range(0.00001f, 0.1f)]
	public float xzScale3 = 0.3f;
	[Range(0.001f,10f)]
	public float yScale3 = 0.01f;
	[Range(0.001f,1000f)]
	public float offsetX3 = 0;
	[Range(0.001f,1000f)]
	public float offsetZ3 = 0;

	[Range(0,1)]
	public float vallyHeight = 0.35f;
	[Range(0,1)]
	public float vallyHeightScale = 0.35f;
	[Range(0,0.5f)]
	public float vallySmooth = 0.3f;

	[Range(-1,1)]
	public float height;

	[Header("Where should be edited?")]
	public Vector2 effectStart = Vector2.zero;
	public Vector2 effectSize = new Vector2(9999,9999);

	[Header("Distance to smoothen. 10 is normal")]
	[Range(0,1000)]
	public float smooth = 10;
	//public bool paint;
//	[Range(2,100)]
//	public int brushRadius;
//	public Texture2D brush;

	[Header("Drop terrain here")]
	public Terrain me;
	[HideInInspector()]
	public TerrainData td;
	private float lowest = 0;
	private List<float[,]> past = new List<float[,]>();
	// Use this for initialization
	void Start () {
		//me = GetComponent<Terrain> ();
		//EditorApplication.update += Update();
		me = GetComponent<Terrain>();
		td = me.terrainData;
	}

	public void undo(){
		if(past.Count<=0){
			return;
		}
		me.terrainData.SetHeights (0,0,past[past.Count-1]);
		past.RemoveAt (past.Count-1);
	}

	public void lower(){
		td = me.terrainData;
		past.Add (td.GetHeights(0,0,td.heightmapResolution, td.heightmapResolution));
		if (past.Count > 5) {
			past.RemoveAt (0);
		}
		float[,] h = td.GetHeights (0,0,td.heightmapResolution,td.heightmapResolution);
		int xs = Mathf.RoundToInt (effectStart.x);
		int zs = Mathf.RoundToInt (effectStart.y);
		int zm = Mathf.RoundToInt(effectSize.x+zs);
		int xm = Mathf.RoundToInt(effectSize.y+xs);
		if (xs < 0) {
			xs = 0;
		}
		if (zs < 0) {
			zs = 0;
		}
		if (xm > td.heightmapResolution) {
			xm = td.heightmapResolution;
		}
		if (zm > td.heightmapResolution) {
			zm = td.heightmapResolution;
		}
		for(int z = zs;z<zm;z++){
			for(int x = xs;x<xm;x++){
				int closest = int.MaxValue;
				if(z<closest&&zs!=0){
					closest = z;
				}
				if(x<closest&&xs!=0){
					closest = x;
				}
				if(xm+xs-x<closest&&xm<td.heightmapResolution){
					closest = xm - x;
				}
				if(zm+zs-z<closest&&zm<td.heightmapResolution){
					closest = zm - z;
				}


				float v = 
					h[z,x]+height;
				if(v<vallyHeight){
					if(vallyHeight-v<vallySmooth){
						v += ((vallyHeight-v)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);
					}else{
						v += (vallyHeight-v)*vallyHeightScale;
					}
				}
				if(closest<=smooth){
					//print (closest+"|"+h[z,x]+"|"+v+"|"+Mathf.Lerp (h [z, x], v, (smooth - closest) / smooth));
					float amount = 1-((smooth - closest)*1f / smooth*1f)*1f;
					//					if (amount < 0.5f) {
					//						amount += (0.5f-amount)/2;
					//					} else {
					//						amount -= (amount - 0.5f)/2;
					//						if (amount > 1) {
					//							amount = 1;
					//						}
					//
					//					}
					v = Mathf.Lerp (h [z, x], v, amount);//((smooth-closest)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);					
				}
				//				if(value<vallyHeight&&z>0&&x>0&&z<h.GetLength(0)-1&&x<h.GetLength(1)-1){
				//					value = h[z+1,x+1]+h[z+1,x]+h[z+1,x-1]+h[z,x+1]+h[z,x]+h[z,x-1]+h[z-1,x+1]+h[z-1,x]+h[z-1,x-1];
				//				}
				h [z, x] = v;

			}
		}
		//		for(int z = zs;z<zm;z++){
		//			for(int x = xs;x<xm;x++){
		//				float v = h [z, x];
		//				if(v<vallyHeight){
		//					if(vallyHeight-v<vallySmooth){
		//						v += ((vallyHeight-v)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);
		//					}else{
		//						v += (vallyHeight-v)*vallyHeightScale;
		//					}
		//				}
		//				h [z, x] = v;
		//
		//			}
		//		}
		if (effectStart.x <= 0 && effectStart.y <= 0 && effectSize.x >= td.heightmapResolution && effectSize.y >= td.heightmapResolution) {
			lowest = 99999999f;
			for (int z = 0; z < h.GetLength (0); z++) {
				for (int x = 0; x < h.GetLength (1); x++) {
					if (h [z, x] < lowest) {
						lowest = h [z, x];
					}
				}
			}
			for (int z = 0; z < h.GetLength (0); z++) {
				for (int x = 0; x < h.GetLength (1); x++) {
					h [z, x] -= lowest;
				}
			} 
		} else {
			//			for (int z = zs; z < zm; z++) {
			//				for (int x = xs; x < xm; x++) {
			//					h [z, x] -= lowest;
			//				}
			//			} 
		}

		td.SetHeights (0,0,h);
	}

	public void noiseHeight(){
		td = me.terrainData;
		past.Add (td.GetHeights(0,0,td.heightmapResolution, td.heightmapResolution));
		if (past.Count > 5) {
			past.RemoveAt (0);
		}
		float[,] h = td.GetHeights (0,0,td.heightmapResolution,td.heightmapResolution);
		int xs = Mathf.RoundToInt (effectStart.x);
		int zs = Mathf.RoundToInt (effectStart.y);
		int zm = Mathf.RoundToInt(effectSize.x+zs);
		int xm = Mathf.RoundToInt(effectSize.y+xs);
		if (xs < 0) {
			xs = 0;
		}
		if (zs < 0) {
			zs = 0;
		}
		if (xm > td.heightmapResolution) {
			xm = td.heightmapResolution;
		}
		if (zm > td.heightmapResolution) {
			zm = td.heightmapResolution;
		}
		print(lowest);
		for(int z = zs;z<zm;z++){
			for(int x = xs;x<xm;x++){
				int closest = int.MaxValue;
				if(z<closest&&zs!=0){
					closest = z;
				}
				if(x<closest&&xs!=0){
					closest = x;
				}
				if(xm+xs-x<closest&&xm<td.heightmapResolution){
					closest = xm - x;
				}
				if(zm+zs-z<closest&&zm<td.heightmapResolution){
					closest = zm - z;
				}


				float v = 
					Mathf.PerlinNoise (x*xzScale1+offsetX1,z*xzScale1+offsetZ1)*yScale1+
					Mathf.PerlinNoise (x*xzScale2+offsetX2,z*xzScale2+offsetZ2)*yScale2+
					Mathf.PerlinNoise (x*xzScale3+offsetX3,z*xzScale3+offsetZ3)*yScale3;
				if(v<vallyHeight){
					if(vallyHeight-v<vallySmooth){
						v += ((vallyHeight-v)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);
					}else{
						v += (vallyHeight-v)*vallyHeightScale;
					}
				}
				if(closest<=smooth){
					//print (closest+"|"+h[z,x]+"|"+v+"|"+Mathf.Lerp (h [z, x], v, (smooth - closest) / smooth));
					float amount = 1-((smooth - closest)*1f / smooth*1f)*1f;
//					if (amount < 0.5f) {
//						amount += (0.5f-amount)/2;
//					} else {
//						amount -= (amount - 0.5f)/2;
//						if (amount > 1) {
//							amount = 1;
//						}
//
//					}
					v = Mathf.Lerp (h [z, x], v, amount);//((smooth-closest)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);					
				}
//				if(value<vallyHeight&&z>0&&x>0&&z<h.GetLength(0)-1&&x<h.GetLength(1)-1){
//					value = h[z+1,x+1]+h[z+1,x]+h[z+1,x-1]+h[z,x+1]+h[z,x]+h[z,x-1]+h[z-1,x+1]+h[z-1,x]+h[z-1,x-1];
//				}
				h [z, x] = v;

			}
		}
//		for(int z = zs;z<zm;z++){
//			for(int x = xs;x<xm;x++){
//				float v = h [z, x];
//				if(v<vallyHeight){
//					if(vallyHeight-v<vallySmooth){
//						v += ((vallyHeight-v)*vallyHeightScale)*((vallyHeight-v)*1/vallySmooth);
//					}else{
//						v += (vallyHeight-v)*vallyHeightScale;
//					}
//				}
//				h [z, x] = v;
//
//			}
//		}
		if (effectStart.x <= 0 && effectStart.y <= 0 && effectSize.x >= td.heightmapResolution && effectSize.y >= td.heightmapResolution) {
			lowest = 99999999f;
			for (int z = 0; z < h.GetLength (0); z++) {
				for (int x = 0; x < h.GetLength (1); x++) {
					if (h [z, x] < lowest) {
						lowest = h [z, x];
					}
				}
			}
			for (int z = 0; z < h.GetLength (0); z++) {
				for (int x = 0; x < h.GetLength (1); x++) {
					h [z, x] -= lowest;
				}
			} 
		} else {
//			for (int z = zs; z < zm; z++) {
//				for (int x = xs; x < xm; x++) {
//					h [z, x] -= lowest;
//				}
//			} 
		}

		td.SetHeights (0,0,h);
	}
	public void OnDrawGizmosSelected(){
		if(me!=null){
			if (effectStart.x == 0 && effectStart.y == 0 && effectSize.x >= td.heightmapResolution && effectSize.y >= td.heightmapResolution) {

			} else {
				Vector3 pos = new Vector3 (effectStart.x, 0, effectStart.y)+me.transform.position;
				Vector3 size = new Vector3 (effectSize.x / td.heightmapResolution * td.size.x, 1000, effectSize.y / td.heightmapResolution * td.size.z);
				pos += size / 2;
				Gizmos.color = new Color (0, 0, 0.2f, 0.2f);
				Gizmos.DrawCube (pos, size);
			}
		}
	}
}
#endif