using UnityEngine;
//using UnityEngine.UI;
using System.Collections;

public class badguy : MonoBehaviour {
	public int target;
	public float speed;
	public float runHPFrac = 0.25f;
	public float turnSpeed;
	public float idleSpeed;
	public float animRunSpeed;
	//public float idleTurn;
	public float damage;
	public float stopDist;
	public float attackRange;
	public float sightRange;
	public float maxHp;
	public float hp;
	public SpriteRenderer hpBar;
	public TextMesh hpText;
	public Transform hpHolder;

	public string[] idles;
	public string run;
	public string die;
	public string[] attacks;

	public byte[] drops;
	public byte[] amounts;

	private float timeTillMove = 2f;
	private bool shouldMove;
	public float direction;
	private Rigidbody rig;
	private float timeTillstop;
	private Animation anim;
	private bool attacking;
	public bool dead;
	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody> ();
		anim = GetComponent<Animation> ();
		direction = Random.Range (0, 360);
		timeTillstop = Random.Range (1,2);
		foreach(string i in idles){
			anim [i].wrapMode = WrapMode.Once;
		}
		foreach(string i in attacks){
			anim [i].wrapMode = WrapMode.Once;
		}
		anim [run].wrapMode = WrapMode.Once;
		anim [die].wrapMode = WrapMode.Once;
		//anim [attack].wrapMode = WrapMode.Once;
		foreach(string i in idles){
			anim [i].speed = idleSpeed;
		}
		anim [run].speed = animRunSpeed;
		//anim [run].speed = 0.5f;
		//anim [attack].speed = 0.5f;
	}
	public void death(){
		anim.Play (die);
		//gameObject.layer = 0;
		//gameObject.tag = "Untagged";
		//for(int i = 0;i<drops.Length;i++){
		//	GameObject g = (GameObject)Instantiate (duplicate.main.drops[drops[i]],transform.position,Quaternion.identity);
		//	g.GetComponent<collectible> ().amount = amounts[i];
		//}
		//enemySpawner.main.baddieAmount--;
		Destroy (gameObject, 5f);//TODO: make this adjustable
	}
	// Update is called once per frame
	void Update () {
		//if(duplicate.paused){
		//	return;
		//}
		//hpText.transform.LookAt(Player.bobs[target].cam);
		//hpText.transform.Rotate(new Vector3(0, 180, 0));

		hpHolder.LookAt(Player.bobs[target].cam);
		if (dead)
		{
			hpText.text = "Dead";
			hpBar.transform.localScale = new Vector3(0, 1, 1);
			hpBar.color = new Color(0, 0, 0);
			hpText.color = new Color(0, 0, 0);
			return;
		}
		else
		{
			if (hp > (maxHp / 2))
			{
				hpBar.color = new Color(1 - (hp - 0.5f * maxHp) / (maxHp / 2), 1, 0);
				hpText.color = new Color(1 - (hp - 0.5f * maxHp) / (maxHp / 2), 1, 0);
			}
			else
			{
				hpBar.color = new Color(1, hp / (maxHp / 2), 0);
				hpText.color = new Color(1, hp / (maxHp / 2), 0);
			}
			hpText.text = hp + "/" + maxHp;//TODO: use Math.Round(hp, 2) to make it 2 decimal places
			hpBar.transform.localScale = new Vector3(hp / maxHp, 1, 1);
		}


		timeTillMove -= Time.deltaTime;
		

		
		if(timeTillMove<=0&&!shouldMove){
			//if (Random.Range ((int)0, (int)2) == 1) {
			//	//rig.AddTorque (Vector3.up * Random.Range (-idleTurn,idleTurn));
			//}
			shouldMove = true;
		}

		bool canSee = Vector3.Distance(transform.position, Player.bobs[target].transform.position) < sightRange;
		if (Player.bobs[target].isDead) canSee = false;

		if (!canSee&&shouldMove){
			attacking = false;
			if (closeAngle(transform.eulerAngles.y, direction)) {
				timeTillstop -= Time.deltaTime;
				rotateTowards(direction, 9999);
				//print ("charge");
				rig.angularVelocity *= 0.5f;
				rig.velocity = (transform.forward * speed);// * Time.deltaTime);
				anim.Play (run);
				RaycastHit hit;
				if (timeTillstop<=0||Physics.Raycast (transform.position+Vector3.up/4, transform.forward-transform.right/2,out hit,stopDist)||Physics.Raycast (transform.position+Vector3.up/4, transform.forward+transform.right/2,out hit,stopDist)) {
					direction -= Random.Range (25,45);
					timeTillMove = Random.Range (6f, 12f);
					timeTillstop = Random.Range (1,2);
					//print ("new drection");
					shouldMove = false;
				}
//				Debug.DrawRay (transform.position+Vector3.up/4,(transform.forward-transform.right/2)*stopDist,Color.red,stopDist);
//				Debug.DrawRay (transform.position+Vector3.up/4,(transform.forward+transform.right/2)*stopDist,Color.red,stopDist);
			} else {
				anim.Play (run);
				rotateTowards(direction, Time.deltaTime * turnSpeed);
				//transform.rotation = Quaternion.RotateTowards (Quaternion.Euler(new Vector3(0,transform.eulerAngles.y,0)),Quaternion.Euler(new Vector3(0,direction,0)),Time.deltaTime*turnSpeed);
//				if(transform.eulerAngles.y<direction){
//					rig.AddTorque (Vector3.up*turnSpeed*Time.deltaTime);
//				}else{
//					rig.AddTorque (-Vector3.up*turnSpeed*Time.deltaTime);
//				}
			}
		}
		if(canSee){
			
			//attack if hp is high enough and within attack range. Might have to rotate first before attack
			if ((hp/maxHp)>runHPFrac&&Vector3.Distance (transform.position, Player.bobs[target].transform.position) < attackRange) {
//				if (!anim.IsPlaying (attack)) {
//					anim.Play (attack);
//					player.main.health -= damage;
//					player.main.vibrateScreen ();
//					//print ("attack");
//				}
				shouldMove = false;
				if (closeAngle(transform.eulerAngles.y, direction) && !anim.isPlaying) {
					if(!attacking){
						anim.Stop ();
						attacking = true;
					}
					anim.Play (attacks[Random.Range(0,attacks.Length)]);
					//Player.bobs[target].hp -= damage;
					//Player.bobs[target].vibrateScreen ();
				} else {
					attacking = false;
					direction = xzAngleToPlayer();
					anim.Play(run);
					//print ("br");
					rotateTowards(direction, Time.deltaTime * turnSpeed);
					//transform.rotation = Quaternion.RotateTowards (Quaternion.Euler(new Vector3(0,transform.eulerAngles.y,0)),Quaternion.Euler(new Vector3(0,direction,0)),Time.deltaTime*turnSpeed);
				}

			} else {
				direction = xzAngleToPlayer();
				//print ("12598");
				if ((hp / maxHp) < runHPFrac) {
					direction -= 180;
				}
				if (closeAngle(transform.eulerAngles.y, direction)) {
					//print (direction);
					//print ("daaa");
					rotateTowards(direction, 9999);
					rig.angularVelocity *= 0.5f;
					rig.velocity = (transform.forward * speed);// * Time.deltaTime);
					anim.Play (run);
				} else {
					rotateTowards(direction, Time.deltaTime * turnSpeed);
					//transform.rotation = Quaternion.RotateTowards (Quaternion.Euler(new Vector3(0,transform.eulerAngles.y,0)),Quaternion.Euler(new Vector3(0,direction,0)),Time.deltaTime*turnSpeed);
					anim.Play(run);//TODO:turn anim
				}
				shouldMove = false;
			}
		}
		if(!anim.isPlaying){
			anim.Play (idles[Random.Range(0,idles.Length)]);
			rig.angularVelocity *= 0.5f;
		}
	}

	private bool closeAngle(float a, float b)
	{
		return Mathf.Abs(Mathf.DeltaAngle(a, b)) < 5;
	}

	private float xzAngleToPlayer()
	{
		return Mathf.Rad2Deg * Mathf.Atan2(Player.bobs[target].transform.position.x - transform.position.x, Player.bobs[target].transform.position.z - transform.position.z);
	}

	private void rotateTowards(float direction, float rotateSpeed)
	{
		float a = Mathf.DeltaAngle(transform.eulerAngles.y, direction);
		if(Mathf.Abs(a) < rotateSpeed)
		{
			transform.Rotate(0, direction - transform.eulerAngles.y, 0);
		}
		else
		{
			float s = a > 0 ? rotateSpeed : -rotateSpeed;
			transform.Rotate(0, s, 0);
		}
		
	}

	public void Damage(float dmg)
	{
		hp -= dmg;
		if (hp <= 0)
		{
			dead = true;
			//hp = 0;//prevent overkill
			death();
		}

	}
}
