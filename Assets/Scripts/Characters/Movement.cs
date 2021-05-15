using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: falling small distances triggers the landing animation
public class Movement : MonoBehaviour
{
	public const float ANGLE_THRESHOLD = 2f;//degrees off that is considered good enough
	public const float CLOSE_ANGLE = 45f;//below this angle turning animations might slow down
	public const float GROUND_THRESHOLD = 0.1f;
	public const float SMALL_INPUT = 0.01f;//below this means no input (e.g. not trying to move)


	public float jumpForce;
	public LayerMask ground;
	public Vector3 groundNormal;
	public GroundCheck[] groundCheck;
	[Range(0f, 90f)]
	public float maxMoveAngle;

	public Collider[] mainColliders;
	public PhysicMaterial movingMaterial;
	public PhysicMaterial stopMaterial;
	public bool tryingToMove;

	public float maxSpeed;
	public float acceleration;
	//affects the maxSpeed and acceleration for moving in a direction that you are not facing (e.g. walking backwards)
	public float minAngleForMove;
	public float maxAngleForMove;
	//public float maxTurnSpeed;
	//public float minTurnSpeed;
	//public float maxTurnSpeedAngle;
	//public float maxTurnAccel;
	//public float minTurnAccel;

	public float turnSpeed;
	
	public Rigidbody rig;
	
	
	public Animator anim;
	
	public bool jumping;
	private bool prevJump;
	//private float yRot;
	//private Vector3 ArmatureStartRot;
	//private Vector3 dir;
	//private float lastA;
	//private float lastArmatureRot;
	//public Transform Armature;
	[Space(40)]
	public bool targetPos;//target position or direcion?
	public Vector3 direction;//the direction to move
	public Vector3 pos;//the target position
	public Quaternion angle;//target angle to face
	[Space(40)]
	public bool grounded;
	[Space(40)]
	public float[] idleWeights;
	public float jumpCooldown = 0.25f;//used to prevent double-jumping, should always be bigger than 0.01 or so to prevent jumping twice
	private float totalIdleWeight;
	private float angleOff;

	private Abilities abilities;//used to determine if need to pause to use a skill
	private bool died;

	private List<Collider> groundOverlaps;
	//public bool canFly;//TODO: add flying
	// Start is called before the first frame update
	void Start()
    {
		abilities = GetComponent<Abilities>();
		for(int i = 0; i < idleWeights.Length; i++)
		{
			totalIdleWeight += idleWeights[i];
		}
		//ArmatureStartRot = Armature.localEulerAngles;
		//lastArmatureRot = Armature.localEulerAngles.y;
		//yRot = transform.eulerAngles.y;
		rig = GetComponent<Rigidbody>();
		angle = transform.rotation;
	}

	#region boring
	public void SetDirection(float x, float z) { SetDirection(new Vector3(x, 0, z)); }
	public void SetDirection(float x, float y, float z) { SetDirection(new Vector3(x, y, z)); }
	public void SetDirection(Vector3 dir)
	{
		dir.y = 0;
		direction = dir;
	}

	public void AddAngle(float a) { AddAngle(Quaternion.Euler(0, a, 0)); }
	public void AddAngle(Quaternion a)
	{
		angle = angle * a;
	}

	public void SetAngleFromDirection()
	{
		Vector3 targetDirection = direction;
		targetDirection.y = 0;
		//only change the angle if actually moving
		if(direction.magnitude > SMALL_INPUT) SetAngle(Quaternion.LookRotation(targetDirection, Vector3.up));
	}

	public void SetAngle(float a) { SetAngle(Quaternion.Euler(0, a, 0)); }
	public void SetAngle (Quaternion a)
	{
		angle = a;
	}
	#endregion
	// Update is called once per frame
	//void Update()
 //   {

	//}
	bool transitionCaptured;//
	private void FixedUpdate()
	{
		if (abilities.myStat.dead)
		{
			if (!died)
			{
				anim.SetTrigger("Dead");
				died = true;
			}
			return;
		}

		SwapPhysicsMaterialsIfNecessary();

		angleOff = Quaternion.Angle(transform.rotation, angle);
		idle = (direction.magnitude < 0.01f) && (angleOff < ANGLE_THRESHOLD) && (rig.IsSleeping());//not moving and on target required to be idle

		if (anim.IsInTransition(0))
		{
			if (!transitionCaptured)
			{
				if (idle)
				{
					float rand = Random.Range(0, totalIdleWeight);
					int index = -1;
					for (int i = 0; i < idleWeights.Length; i++)
					{
						rand -= idleWeights[i];
						if (rand <= 0)
						{
							index = i;
							break;
						}
					}
					anim.SetInteger("IdleIndex", index);
				}


				transitionCaptured = true;
			}

		}
		else
		{
			transitionCaptured = false;
		}

		if (previouslyIdle && !idle)//used to be idle, but not anymore
		{
			anim.SetTrigger("IdleEnd");
		}

		previouslyIdle = idle;

		CheckGround();

		HandleMove();
		HandleJump();

		//CheckGround();

	}

	private void SwapPhysicsMaterialsIfNecessary()
	{
		bool pTryingToMove = tryingToMove;
		tryingToMove = (direction.magnitude > SMALL_INPUT);
		if (!tryingToMove && pTryingToMove)//(direction.magnitude < SMALL_INPUT))
		{
			foreach (Collider c in mainColliders)
			{
				c.sharedMaterial = stopMaterial;
			}
			//tryingToMove = false;
		}
		else
		{
			if (tryingToMove && !pTryingToMove)//(direction.magnitude >= SMALL_INPUT))
			{
				foreach (Collider c in mainColliders)
				{
					c.sharedMaterial = movingMaterial;
				}
			}
			//tryingToMove = true;
		}
		//tryingToMove = (direction.magnitude > SMALL_INPUT);
	}

	void CheckGround()
	{
		grounded = false;
		float bestDirDot = -2f;


		//find all ground that character is on, then find the ground contact most aligned (dot) with character's direction.
		foreach(GroundCheck t in groundCheck)
		{
			RaycastHit[] hit = Physics.SphereCastAll(t.transform.position, t.radius, t.transform.up, t.distance, ground, QueryTriggerInteraction.Ignore);
			//print(hit.Length);
			foreach (RaycastHit h in hit)
			{
				//-hit.normal should be the normal of the point on this "collider"/spherecast that hit (that's why negative)
				float angle = Vector3.Angle(-h.normal, Vector3.down);
				Vector3 deltaPositionHit = h.point - transform.position;
				if (deltaPositionHit.magnitude > 0.01f)
				{
					deltaPositionHit.Normalize();
				} else deltaPositionHit = Vector3.zero;
				float dirDot = Vector3.Dot(deltaPositionHit, direction.normalized);
				//print(angle);
				if (angle < maxMoveAngle)
				{
					//print("Grounded");
					grounded = true;

					if (!tryingToMove)
					{
						groundNormal = -h.normal;
						return;
					} else if (dirDot > bestDirDot)
					{
						groundNormal = -h.normal;
						bestDirDot = dirDot;

					}
				}
			}
			
		}
	}

	/*
	//IEnumerator CheckJump()
	//{
	//	yield return new WaitForSeconds(0.1f);//wait a little before checking, allowing the ground check points to exit the ground if they can
	//										  //by now, if still grounded, the jump failed. This means jump end should happen.
	//	if (grounded)
	//	{
	//		tryingToJump = false;
	//	}
	//}
	*/

	public bool attemptJump;
	public void AttemptJump()
	{
		attemptJump = true;
	}

	void HandleJump()
	{
		if (attemptJump)
		{
			if (grounded && rig.velocity.y < jumpForce)
			{
				Vector3 tempV = rig.velocity;
				tempV.y = jumpForce;
				rig.velocity = tempV;
			}
			attemptJump = false;
		}
		//if (grounded)
		//{

		//	if (attemptJump && jumpCooldownLeft < 0)
		//	{
		//		anim.ResetTrigger("JumpStart");
		//		anim.SetTrigger("JumpStart");
		//		anim.ResetTrigger("JumpEnd");
		//		Jump();//TODO: organize this
		//		//tryingToJump = true;
		//		jumpCooldownLeft = jumpCooldown;
		//		//jumpInProg = true;
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
		//	attemptJump = false;

		//}
		//else if (prevJump)
		//{
		//	jumping = true;
		//}
		//prevJump = grounded;
	}
	public bool idle;
	bool previouslyIdle = true;
	public float jumpCooldownLeft;
	public bool jumpInProg;

	void HandleMove()
	{
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump Start")) jumpCooldownLeft -= Time.fixedDeltaTime;//TODO: add event to anim to trigger this instead for optimization
		float a = angle.eulerAngles.y;
		float t = transform.eulerAngles.y;
		float deltaAngle = Mathf.DeltaAngle(a, t);
		
		float velocityInDirection = Vector3.Dot(direction.normalized, rig.velocity);
		//print(velocityInDirection / maxSpeed);
		anim.SetBool("Idle", idle);
		if (idle)
		{
			anim.SetFloat("Direction", 0);
			anim.SetFloat("Speed", 0);
		}
		else
		{
			anim.SetFloat("Direction", Mathf.Clamp(deltaAngle / CLOSE_ANGLE, -1, 1), 0.2f, Time.fixedDeltaTime);//player tries to turn in mid-air
			if (!grounded) anim.SetFloat("Speed", Mathf.Clamp01(direction.magnitude), 0.2f, Time.fixedDeltaTime);//player tries to run in mid-air
			if (grounded)
			{
				anim.SetFloat("Speed", velocityInDirection / maxSpeed, 0.2f, Time.fixedDeltaTime);//player is running on ground
																							 //TODO: move only now

				//attack angle:


				//TODO: for now it only rotates the y

				transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, turnSpeed * Time.fixedDeltaTime);
				//float ab = Mathf.Abs(deltaAngle);
				////deltaAngle left is negative
				//float tempMaxTurnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, ab / maxTurnSpeedAngle);// minTurnSpeed + (maxTurnSpeed - minTurnSpeed) * ab / maxTurnSpeedAngle;
				

				////if(tempMaxTurnSpeed > ab * )
				////don't overshoot in 1 frame
				//if (tempMaxTurnSpeed > ab / Time.fixedDeltaTime) tempMaxTurnSpeed = ab / Time.fixedDeltaTime;
				
				//if (deltaAngle > 0) tempMaxTurnSpeed *= -1;

				////try to stop if close enough to the right angle
				//if (angleOff < ANGLE_THRESHOLD) tempMaxTurnSpeed = 0;

				////how off is the turn velocity from target
				//float turnVelOff = tempMaxTurnSpeed - rig.angularVelocity.y * Mathf.Rad2Deg;

				////the maximum turn velocity change allowed this frame
				//float turnVelChange = maxTurnAccel * Time.deltaTime;

				////if turnVelChange is bigger than needed set it equal
				//if (turnVelChange > Mathf.Abs(turnVelOff))
    //            {
    //                turnVelChange = turnVelOff;
    //            }else if(turnVelOff < 0)//otherwise, check that the signs are same direction
    //            {
				//	turnVelChange *= -1;
    //            }
				//rig.angularVelocity += new Vector3(0, turnVelOff * Mathf.Deg2Rad, 0);
				//rig.angularVelocity += Vector3.up * turnVelChange * Mathf.Deg2Rad;
				//rig.AddTorque(turnVelChange * Mathf.Deg2Rad * Vector3.up, ForceMode.VelocityChange);

                //print(tempMaxTurnSpeed + "|" + rig.angularVelocity.y * Mathf.Rad2Deg);
				/*old method*/
				//float ab = Mathf.Abs(deltaAngle);

				//bool cw = deltaAngle > 0;//should the force be applied clockwise
				//float av = rig.angularVelocity.y * Mathf.Rad2Deg;
				//av = cw ? -av : av;//how fast it is turning in the desired direction

				//float temp = maxTurnSpeedAngle;// * av;
				//							   //if (temp < 45) temp = 45;

				//float tempMaxTurnSpeed = minTurnSpeed + (maxTurnSpeed - minTurnSpeed) * ab / temp;
				//float tempMaxTurnAccel = minTurnAccel + (maxTurnAccel - minTurnAccel) * ab / temp;
				//tempMaxTurnAccel = cw ? -tempMaxTurnAccel : tempMaxTurnAccel;

				//float angularVelocityChange = maxTurnAccel;
				//float wrongSpeed = maxTurnSpeed - Mathf.Abs(av);

				//if (av < tempMaxTurnSpeed)//av < tempMaxSpeed)
				//{//TODO: change rig.AddTorque mode
				//	//print(av / tempMaxTurnSpeed);
				//	//help prevent overshoot causing "vibration" rotation
				//	rig.AddTorque(0, tempMaxTurnAccel * Time.deltaTime * rig.mass, 0);
				//}
				//else
				//{
				//	//print(av / tempMaxTurnSpeed);
				//	//help prevent overshoot causing "vibration" rotation
				//	rig.AddTorque(0, -tempMaxTurnAccel * Time.deltaTime * rig.mass, 0);
				//}
				

				//float dot = Vector3.Dot(direction.normalized, (angle * Vector3.forward).normalized);
				float mult = 1 - Mathf.Clamp01((Vector3.Angle(angle * Vector3.forward, direction) - minAngleForMove) / (maxAngleForMove - minAngleForMove));

				//find the velocity in the direction

				Vector3 v = rig.velocity;
				//v.y = 0;
				//v = transform.InverseTransformVector(v);
				Vector3 d = direction;
				d.y = 0;
				//adjust the direction to account for uphill/downhill
				//this will remove any component of d that is parallel to groundNormal
				d -= groundNormal * Vector3.Dot(groundNormal, d);

				d = d.normalized * maxSpeed;
				Vector3 off = v - d;
				if(off.magnitude < acceleration * Time.fixedDeltaTime)
				{
					v = d;
					//print("(" + v.x + ", " + v.y + ", " + v.z + ")");
				}
				else
				{
					v -= off.normalized * Time.fixedDeltaTime * acceleration;
				}

				//v = transform.TransformVector(v);
				rig.velocity = v;
				//if (v == Vector3.zero) rig.Sleep();
				

				//Vector3 velOff = direction.normalized - rig.velocity.normalized;
				//float velDot = Vector3.Dot(rig.velocity, direction.normalized);
				//Vector3 newDir = direction * (1-velDot) + velOff * velDot;
				//if (rig.velocity.magnitude > 0.01f && (direction.magnitude < SMALL_INPUT || velDot < -0.5f))//if you are moving in a really wrong direction, slow down
				//{
				//	Vector3 v = rig.velocity;
				//	v.y = 0;
				//	if(v.magnitude < acceleration)
				//	rig.AddForce(-rig.velocity.normalized * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);// * mult
				//}
				//else
				//{
				//	if (rig.velocity.magnitude < maxSpeed * mult)
				//	{
				//		//print(transform.InverseTransformDirection(rig.velocity).z / (maxSpeed * mult));
				//		rig.AddForce(direction.normalized * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);// * mult
				//	}
				//}
			}
		}
		
		
	}

	public void Jump()
	{
		//jumping = true;
		rig.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
		//StartCoroutine(CheckJump());
		//jumpInProg = false;
		//jumpCooldownLeft = jumpCooldown;
	}

	//private void OnCollisionEnter(Collision collision)
	//{
		
	//}

	//private void OnCollisionStay(Collision collision)
	//{
	//	if (grounded) return;//don't need extra ground checking if we already know that it's grounded

	//	GameObject g = collision.gameObject;
	//	if ((ground & (1 << g.layer)) > 0)// && !groundOverlaps.Contains(other.transform))
	//	{
	//		foreach (ContactPoint c in collision.contacts)
	//		{
	//			//TODO: I'm unsure about c.normal, whether it's my normal or the other collider's normal.
	//			//right now, I'm supposing that it's the other collider's normal
	//			//Vector3.Angle returns the unsigned angle, <= 180
	//			float angle = Vector3.Angle(-c.normal, Vector3.down);
	//			print(angle);
	//			if (angle < maxMoveAngle)
	//			{
	//				print("Grounded");
	//				grounded = true;
	//				return;
	//			}
	//		}
	//	}
	//}

	//private void OnCollisionExit(Collision collision)
	//{
	//	//GameObject g = collision.gameObject;
	//	//if ((ground & (1 << g.layer)) > 0 && !groundOverlaps.Contains(other.transform))
	//	//{

	//	//}
	//}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + angle * Vector3.forward);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + direction + Vector3.up * 0.1f);

		Gizmos.color = Color.blue;
		Vector3 d = direction;
		d.y = 0;
		//adjust the direction to account for uphill/downhill
		//this will remove any component of d that is parallel to groundNormal
		d -= groundNormal * Vector3.Dot(groundNormal, d);

		d = d.normalized * maxSpeed;

		Gizmos.DrawLine(transform.position, transform.position + d);
	}
}
