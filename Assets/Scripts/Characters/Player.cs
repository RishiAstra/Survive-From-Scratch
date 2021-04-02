using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using bobStuff;
using System;

public class Player : MonoBehaviour
{
	//TODO: this system of a player list should be replaced
	public static List<Player> bobs = new List<Player>();
	public static Player main;
	//public static List<ItemType> itemTypes;//TODO: this might be bad
	public static int amountOfInstances = 0;
	//	public Material notMe;
	//public int score;
	//	public GUIStyle inventoryStyle;

	//public float jumpForce;
	//public LayerMask ground;
	//public Transform groundCheck;
	//public float maxHp;
	//public float hp;
	//public float damage;
	//public float range;
	//public float maxSpeed;
	//public float acceleration;
	//public float maxTurnSpeed;
	//public float turnSpeed;
	public float grabDist;
	[Space(10)]
	//public Transform hpBar;
	//public GameObject scoreBoard;
	//public List<Transform> scores = new List<Transform>();
	//public GameObject scorePref;
	public Transform rightHand;
	//public Transform camHolder;

	public inventoryStuff invStuff;
	[HideInInspector()] public Rigidbody rig;
	public Inventory inv;
	public int invSel = -1;
	//[HideInInspector()]public PhotonView ph;
	[HideInInspector()] public bool isDead;
	//private int id;
	//	private float brakeThreshold = 0.1f;

	//private Animator anim;
	//private Cam myCam;
	public Transform cam;
	//private Canvas canvas;
	//private CanvasScaler canvasScaler;
	//private int kills = 0;
	//private int deaths = 0;
	private GameObject placeing;
	//private bool clickedThisFrame = true;//used for selecting inventory
	//private ItemTag selectable = ~ItemTag.Item;
	//public bool jumping;
	//private bool prevJump;
	//private float yRot;
	//private Vector3 ArmatureStartRot;
	//private Vector3 dir;
	//private float lastA;
	//private float lastArmatureRot;
	//private Vector3 camLocalPosWhenRespawn;//use this to position the camera when respawning
	//private Quaternion camHoldRotWhenRespawn;//see above
	//public Transform Armature;
	// Use this for initialization
	void Start()
	{
		//ArmatureStartRot = Armature.localEulerAngles;
		//lastArmatureRot = Armature.localEulerAngles.y;
		//yRot = transform.eulerAngles.y;
		bobs.Add(this);
		cam = Camera.main.transform;
		//canvas = GameObject.FindObjectOfType<Canvas>();
		//canvasScaler = GameObject.Find("Scaled Canvas").GetComponent<CanvasScaler>();
		//itemTypes = gameControll.itemTypes;
		//anim = GetComponent<Animator>();
		//scoreBoard = GameObject.Find("Scaled Canvas").transform.GetChild(0).gameObject;//Resources.FindObjectsOfTypeAll<Canvas>()[0].transform.GetChild(0).gameObject;
		//		scoreBoard.SetActive (false);
		//id = amountOfInstances;
		amountOfInstances++;
		rig = GetComponent<Rigidbody>();
		//myCam = GetComponentInChildren<Cam>();
		inv = GetComponent<Inventory>();
		//if(Player.main == this)
		//{
		//	hpBar.gameObject.SetActive(false);
		//	hpBar = GameObject.Find("CanvasHpBar").transform;
		//}
		//ph = GetComponent<PhotonView> ();

		//		if (!ph.isMine) {
		//			Destroy (transform.GetChild (0).gameObject);
		////			GetComponent<Renderer> ().material = notMe;
		//		}
	}

	//called by animations
	//public void Attack()
	//{
	//	foreach (Weapon w in rightHand.GetComponentsInChildren<Weapon>())
	//	{
	//		w.Attack();
	//	}
	//}
	//called by animations
	//public void StopAttack()
	//{
	//	foreach (Weapon w in rightHand.GetComponentsInChildren<Weapon>())
	//	{
	//		w.StopAttack();
	//	}
	//}

	//public void Jump()
	//{
	//	//jumping = true;
	//	rig.AddForce(Vector3.up * jumpForce);
	//}
	//public void EndJump()
	//{
	//	//jumping = false;
	//	//rig.AddForce(Vector3.up * jumpForce);
	//}

	//public IEnumerator TakeOff()
	//{
	//	yield return new WaitForSeconds(0.1f);
	//	jumping = true;
	//}

	//public IEnumerator LandFromJump()
	//{
	//	yield return new WaitForEndOfFrame();// WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
	//	jumping = false;
	//	anim.SetBool("JumpEnd", false);
	//}

	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	public void Respawn()
	{
		spawnPoint[] sp = GameObject.FindObjectsOfType<spawnPoint>();
		int chosen = UnityEngine.Random.Range(0, sp.Length);
		transform.position = sp[chosen].transform.position;
		//hp = maxHp;
		//cam.SetParent(camHolder);
		//cam.localPosition = camLocalPosWhenRespawn;
		//camHolder.rotation = camHoldRotWhenRespawn;
		//ph.RPC ("setHp", PhotonTargets.All, hp);
	}


	//public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	//{
	//	if (stream.isWriting) {
	//		stream.SendNext (hp);
	//	} else {
	//		hp = (float)stream.ReceiveNext ();
	//	}
	//}
	void RefreshSelected()
	{
		//TODO: should this really destroy the current equip gameobjects, or should it check if they changed first
		if (invSel >= inv.items.Count || invSel < 0) return;
		if (rightHand.childCount != 0)
		{
			for (int i = 0; i < rightHand.childCount; i++)
			{
				Destroy(rightHand.GetChild(i).gameObject);
			}
		}
		//TODO: is this necessary?
		if (GameControl.itemTypes[inv.items[invSel].id].tags.Contains(TagScript.rhEquip))
		{
			if (GameControl.itemTypes[inv.items[invSel].id].equipPrefab != null)
			{
				GameObject g = (GameObject)Instantiate(GameControl.itemTypes[inv.items[invSel].id].equipPrefab);
				g.transform.SetParent(rightHand, false);
				g.GetComponent<Equip>().bob = this;
				//print("Selected equipable object");
			}
		}
	}

	private void SetSelected(int index)
	{
		List<ItemIcon> tempI = GameControl.main.hotBarUI.slotI;
		foreach(ItemIcon i in tempI)
		{
			i.selected = false;
			//TODO:update it here (and below)
		}
		invSel = index;
		if(index >= 0 && index < tempI.Count)
		{
			tempI[index].selected = true;
		}
	}

	/// <summary>
	/// Select an Item in the inventory. Some actions such as eating a consumable item may happen.
	/// </summary>
	/// <param name="index">The index in inventory of the item to select</param>
	void SelectInv(int index) {
		if (index >= inv.items.Count) return;// || invSel < 0
		//destroy thing if previously placing thing, see todos below
		Destroy(placeing);

		if (rightHand.childCount != 0) {
			//TODO: this might cause errors if children are deleted instantly
			for (int i = 0; i < rightHand.childCount; i++) {
				Destroy(rightHand.GetChild(i).gameObject);
			}
		}

		//Right Hand Equippable item
		if (GameControl.itemTypes[inv.items[index].id].tags.Contains(TagScript.rhEquip))
		{
			if (invSel != index) {
				if(GameControl.itemTypes[inv.items[index].id].equipPrefab != null)
				{
					GameObject g = (GameObject)Instantiate(GameControl.itemTypes[inv.items[index].id].equipPrefab);
					g.transform.SetParent(rightHand, false);
					g.GetComponent<Equip>().bob = this;
					//print("Selected equipable object");
				}
				SetSelected(index);
				//invSel = index;
			}else
			{
				SetSelected(-1);
				//invSel = -1;
			}
		}

		if (GameControl.itemTypes[inv.items[index].id].tags.Contains(TagScript.placeable))
		{
			if (invSel != index)
			{
				//TODO: should this equip it?
				//TODO: placing prefab should be different from final prefab. The placing prefab could have a placing script that spawns the final
				placeing = (GameObject)Instantiate(GameControl.itemTypes[inv.items[index].id].prefab);
				foreach (MonoBehaviour m in placeing.GetComponentsInChildren<MonoBehaviour>())
				{
					m.enabled = false;
				}
			}
			else
			{
				////see above todos
				//invSel = -1;
			}
		}
		//print(invSel);
	}

	//TODO: remove this inventory display, it should be replaced with UI gameobjects
	//void OnGUI()
	//{
	//	//if (PhotonNetwork.connected) {
	//	//	if (ph.owner != null) {
	//	//		if (ph.isMine) {
	//	//					int invRows = Mathf.CeilToInt (inventory.Count/inv.buttonsPerInvRow);
	//	float scale = Screen.width / canvasScaler.referenceResolution.x;

	//	for (int i = 0; i < inv.items.Count; i++) {
	//		int x = (int)(i % invStuff.buttonsPerInvRow);
	//		int y = Mathf.CeilToInt((i + 1) / invStuff.buttonsPerInvRow);
	//		float xOffset = (Screen.width / 2 - (invStuff.buttonsPerInvRow * invStuff.invButtonSize / 2) * scale);
	//		if (inv.items.Count < invStuff.buttonsPerInvRow) {
	//			xOffset = (Screen.width / 2 - ((inv.items.Count) * invStuff.invButtonSize / 2) * scale);
	//		}
	//		Rect pos = new Rect(
	//			xOffset + x * invStuff.invButtonSize * scale,
	//			Screen.height - ((invStuff.invButtonSize / 2 + 25 + y * invStuff.invButtonSize) * scale),
	//			invStuff.invButtonSize * scale,
	//			invStuff.invButtonSize * scale
	//		);
	//		Vector2 tempMousePos = Event.current.mousePosition;

	//		if (!clickedThisFrame&&
	//			tempMousePos.x > pos.x &&
	//			tempMousePos.y > pos.y &&
	//			tempMousePos.x < (pos.x + pos.width) &&
	//			tempMousePos.y < (pos.y + pos.height)) {
	//			clickedThisFrame = true;
	//			SelectInv(i);
	//		}
	//		if (invSel == i) {
	//			GUI.DrawTexture(pos, invStuff.invBackground, ScaleMode.ScaleToFit, true, 0, invStuff.selectedColor, 0, 0);
	//		} else {
	//			GUI.DrawTexture(pos, invStuff.invBackground, ScaleMode.ScaleToFit, true, 0, invStuff.normalColor, 0, 0);
	//		}

	//		GUI.color = Color.white;
	//		pos = new Rect(
	//				xOffset + invStuff.invButtonSize * 0.1f + x * invStuff.invButtonSize * scale,
	//				Screen.height - ((invStuff.invButtonSize / 2 + invStuff.invButtonSize * -0.1f + 25 + y * invStuff.invButtonSize) * scale),
	//				invStuff.invButtonSize * 0.8f * scale,
	//				invStuff.invButtonSize * 0.8f * scale
	//		);
	//		GUI.DrawTexture(pos, itemTypes[inv.items[i].id].icon);
	//		if (inv.items[i].amount > 1) {
	//			pos = new Rect(
	//				xOffset + x * invStuff.invButtonSize * scale,
	//				Screen.height - ((invStuff.invButtonSize / 2 + 25 + y * invStuff.invButtonSize) * scale),
	//				invStuff.invButtonSize * scale,
	//				invStuff.invButtonSize * scale
	//			);
	//			GUI.Label(pos, inv.items[i].amount.ToString());
	//		}
	//	}
	//	//if (!isDead) {
	//	//	GUI.Label (new Rect (0, 0, 250, 25), ph.owner.NickName + "'s hp: " + hp);
	//	//} else {
	//	//	GUI.Label (new Rect (0, 0, 250, 25), ph.owner.NickName + "'s hp: DEAD");
	//	//}
	//	//} else {
	//	//	if (!isDead) {
	//	//		GUI.Label (new Rect (0, 15 * id + 15, 250, 25), ph.owner.NickName + "'s hp: " + hp);
	//	//	} else {
	//	//		GUI.Label (new Rect (0, 15 * id + 15, 250, 25), ph.owner.NickName + "'s hp: DEAD");
	//	//	}

	//	//}
	//	//	} else {
	//	//		GUI.Label (new Rect (0, 15 * id + 15, 250, 25), "ERROR CANT FIND NICKNAME");
	//	//	}


	//	//}


	//}

	private void Update()
	{
		CheckHotBar();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//if (Time.frameCount % 100 == 0)
		//{
		//	print(invSel);
		//}

		//if (hp > 0) {
		//	isDead = false;
		//	hpBar.localScale = new Vector3(hp / maxHp, 1, 1);
		//} else {
		//	Die();
			
		//	//if (!isDead && ph.isMine) {
		//	//	deaths++;
		//	//}
		//}
		//if (Player.main != this)
		//{
		//	hpBar.transform.LookAt(cam);
		//}

		//if (ph.isMine && PhotonNetwork.connected && ph.owner != null) {
		//anim.SetBool("Whak", Input.GetMouseButton(0)&&!clickedThisFrame);
		//while (inv.items.Count < invStuff.inventorySize) {
		//	inv.items.Add(new Item(0, 0, 0, 0));
		//}
		//if (Input.GetKey(KeyCode.Tab)) {
		//	//Transform content = scoreBoard.transform.GetChild(0).GetChild(0);
		//	//while (scores.Count < PhotonNetwork.playerList.Length) {
		//	//	GameObject g = (GameObject)Instantiate (scorePref);
		//	//	g.transform.SetParent (content);
		//	//	g.transform.localScale = new Vector3 (1, 1, 1);
		//	//	g.transform.localPosition = new Vector3 (400, scores.Count * 25 - 12.5f, 0);
		//	//	scores.Add (g.transform);
		//	//	content.GetComponent<RectTransform> ().sizeDelta += new Vector2 (0, 25);
		//	//}

		//	//				bobPlayer[] bp = GameObject.FindObjectsOfType<bobPlayer> ();
		//	//				List<int> ind = new List<int> (bp.Length);
		//	//				for (int i = 0; i < ind.Count; i++) {
		//	//					ind [i] = i;
		//	//				}
		//	//				ind.Sort ((x,y) => bp[x].kills.CompareTo(bp[y].kills));
		//	//				for(int i = 0;i<pp.Length;i++){
		//	//					
		//	//					for(int j = i;j<pp.Length;j++){
		//	//
		//	//					}
		//	//				}
		//	for (int i = 0; i < bobs.Count; i++) {
		//		//					print (i);
		//		int myPlace = bobs.Count;
		//		for (int j = 0; j < bobs.Count; j++) {
		//			if (score < bobs[j].score) {
		//				myPlace--;
		//			}
		//		}
		//		//scores [myPlace].GetChild (0).GetComponent<Text> ().text = bobs [myPlace].photonView.owner.NickName;
		//		scores[myPlace].GetChild(1).GetComponent<Text>().text = bobs[myPlace].kills.ToString();
		//		scores[myPlace].GetChild(2).GetComponent<Text>().text = bobs[myPlace].deaths.ToString();
		//		if ((bobs[myPlace].kills / bobs[myPlace].deaths) < 10000000) {
		//			scores[myPlace].GetChild(3).GetComponent<Text>().text = (bobs[myPlace].kills / bobs[myPlace].deaths).ToString().Substring(0, 4);
		//		} else {
		//			scores[myPlace].GetChild(3).GetComponent<Text>().text = "---";

		//		}
		//	}
		//	scoreBoard.gameObject.SetActive(true);
		//} else {
		//	scoreBoard.gameObject.SetActive(false);
		//}
		if (!GameControl.main.myAbilities.dead) {
			
			//if (Physics.CheckSphere(groundCheck.position, 0.05f, ground))
			//{
				
			//	if (Input.GetKey(KeyCode.Space)&&!jumping)
			//	{

			//		anim.ResetTrigger("JumpStart");
			//		anim.SetTrigger("JumpStart");
			//		anim.ResetTrigger("JumpEnd");
			//		//anim.SetBool("JumpEnd", false);
			//		//StartCoroutine(TakeOff());
			//		//jumping = true;
			//		//rig.AddForce(transform.up * jumpForce);
			//		//Jump();
			//	}

			//	if (jumping)
			//	{
			//		jumping = false;
			//		//anim.SetBool("JumpStart", false);
			//		anim.ResetTrigger("JumpEnd");
			//		anim.SetTrigger("JumpEnd");
			//		anim.ResetTrigger("JumpStart");
			//		//jumping = false;
			//		//StartCoroutine(LandFromJump());
			//	}


			//}else if (prevJump)
			//{
			//	jumping = true;
			//}
			//prevJump = Physics.CheckSphere(groundCheck.position, 0.05f, ground);
			//if (hp <= 0) {
			//	hp = maxHp;
			//	isDead = true;
			//}

			//				transform.GetChild (transform.childCount-1).localScale = new Vector3 (hp / maxHp, 1, 1);

			if (Input.GetMouseButtonDown(0) && invSel != -1) {
				//TODO: change placing system
				if (placeing == null) {
					//Player[] hitThese = GameObject.FindObjectsOfType<Player>();
					////					bool[] couldHit = new bool[hitThese.Length];
					//for (int i = 0; i < hitThese.Length; i++) {
					//	float dist = Vector3.Distance(hitThese[i].transform.position, transform.position);
					//	if (dist < range && dist > 0.01f) {
					//		//						hitThese [i].GetComponent<PhotonView> ().RPC ("takeDmg",PhotonTargets.All,damage);
					//		//hitThese[i].hp -= damage;//damage other players
					//		//this.photonView.RPC ("setHp", PhotonTargets.All, hitThese [i].hp);
					//	}
					//}
				} else {
					foreach (MonoBehaviour m in placeing.GetComponentsInChildren<MonoBehaviour>()) {
						m.enabled = false;
					}
					RemoveItem(invSel);
					placeing = null;
				}
			}
		} else {//isdead
				//				respawn();
		}
		//Move();
		//idk what this is
		//if (Input.GetMouseButtonDown(0))
		//{
		//	clickedThisFrame = false;
		//}
		//else
		//{
		//	clickedThisFrame = true;
		//}
		//clickedThisFrame = false;
		//}
	}

	//private void Die()
	//{
	//	//hpBar.localScale = new Vector3(0, 1, 1);
	//	isDead = true;
	//	//hp = 0;
	//	//camHoldRotWhenRespawn = camHolder.rotation;
	//	//camLocalPosWhenRespawn = cam.localPosition;
	//	//cam.SetParent(null);

	//	gameObject.SetActive(false);
	//}
	#region Boring Functions
	//[PunRPC ()]
	//public void setHp (float amount)
	//{
	//	hp = amount;
	//}

	////[PunRPC ()]
	//public void setDead (bool v)
	//{
	//	isDead = v;
	//}

	//TODO: check if there is no place to put the item
	//TODO: move these functions to Inventory.cs
	public void GetItem(int id) { GetItem(id, 1); }
	public void GetItem(int id, int amount){
		for (int i = 0; i < inv.items.Count; i++) {
			if(inv.items[i].id == id && Mathf.Abs(inv.items[i].currentStrength-inv.items[i].strength)<0.01f){
				Item temp = inv.items[i];
				temp.amount += amount;
				inv.items[i] = temp;
				//inv.items [i].amount += amount;

				//TODO: this might require inventory to be refreshed
				//if (invSel == i) {
				//	RefreshSelected();
				//	//SelectInv (invSel);
				//}
				return;
			}
		}
		for (int i = 0; i < inv.items.Count; i++) {
			if(inv.items[i].id == 0){
				inv.items [i] = new Item (id, amount, GameControl.itemTypes[id].strength, GameControl.itemTypes[id].strength);
				if (invSel == i) {
					
					RefreshSelected();
					//SelectInv (invSel);
				}
				return;
			}
		}
	}
	/// <summary>
	/// Removes 1 item from stack at given index
	/// </summary>
	/// <param name="index">The index to remove from</param>
	public void RemoveItem(int index) { RemoveItem(index, 1); }
	/// <summary>
	/// Removes amount from stack at <code>hissssssss</code><
	/// </summary>
	/// <param name="index">The index</param>
	/// <param name="amount">Amount to remove</param>
	public void RemoveItem(int index, int amount){
		if (index == -1) return;
		Item temp = inv.items[index];
		temp.amount -= amount;
		inv.items[index] = temp;
		//inv.items [index].amount -= amount;
		if (inv.items [index].amount <= 0) {
			if (inv.items[index].amount < 0) Debug.LogError("removed too much of item");
			inv.items [index] = new Item (0, 0, 0, 0);
			if (invSel == index)
			{
				RefreshSelected();
			}
		}
	}
	void OnDestroy(){
		bobs.Remove (this);
	}
	/// <summary>
	/// If player presses button from 0-9, select the corisponding index in inventory
	/// </summary>
	void CheckHotBar()
	{
		if (Input.GetKeyDown("0")) SelectInv(9);
		for(int i = 1; i < 10; i++)
		{
			if (Input.GetKeyDown(i.ToString()))
			{
				SelectInv(i-1);
				//print("select " + i);
			}
		}
	}
	#endregion
}
