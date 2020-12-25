using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public const float ANGLE_THRESHOLD = 0.5f;//degrees off that is considered good enough
	public const float GROUND_THRESHOLD = 0.05f;
	
	public float jumpForce;
	public LayerMask ground;
	public Transform[] groundCheck;

	public float maxSpeed;
	public float acceleration;
	//affects the maxSpeed and acceleration for moving in a direction that you are not facing (e.g. walking backwards)
	public float minAngleForMove;
	public float maxAngleForMove;
	public float maxTurnSpeed;
	public float minTurnSpeed;
	public float maxTurnSpeedAngle;
	public float maxTurnAccel;
	public float minTurnAccel;
	
	public Rigidbody rig;
	
	
	public Animator anim;
	
	public bool jumping;
	private bool prevJump;
	//private float yRot;
	//private Vector3 ArmatureStartRot;
	//private Vector3 dir;
	//private float lastA;
	private float lastArmatureRot;
	public Transform Armature;
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
		lastArmatureRot = Armature.localEulerAngles.y;
		//yRot = transform.eulerAngles.y;
		rig = GetComponent<Rigidbody>();
		angle = transform.rotation;
	}

	#region boring
	public void SetDirection(float x, float z) { SetDirection(new Vector3(x, 0, z)); }
	public void SetDirection(float x, float y, float z) { SetDirection(new Vector3(x, y, z)); }
	public void SetDirection(Vector3 dir)
	{
		direction = dir;
	}

	public void AddAngle(float a) { AddAngle(Quaternion.Euler(0, a, 0)); }
	public void AddAngle(Quaternion a)
	{
		angle = angle * a;
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
		if (abilities.dead){
			if (!died)
			{
				anim.SetTrigger("Dead");
				died = true;
			}
			return;
		}
		angleOff = Quaternion.Angle(transform.rotation, angle);
		idle = (direction.magnitude < 0.01f) && (angleOff < ANGLE_THRESHOLD) && (rig.velocity.magnitude < 0.05f);//move very slow and be on target to be idle
		if (true)//abilities == null)  || !abilities.busy)
		{
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
			HandleMove();
			HandleJump();
		}
		

		CheckGround();
		

	}

	void CheckGround()
	{
		grounded = false;
		foreach(Transform t in groundCheck)
		{
			if(Physics.CheckSphere(t.position, GROUND_THRESHOLD, ground))
			{
				grounded = true;
				return;
			}
		}
		
	}
	//IEnumerator CheckJump()
	//{
	//	yield return new WaitForSeconds(0.1f);//wait a little before checking, allowing the ground check points to exit the ground if they can
	//										  //by now, if still grounded, the jump failed. This means jump end should happen.
	//	if (grounded)
	//	{
	//		tryingToJump = false;
	//	}
	//}

	public bool attemptJump;
	public void AttemptJump()
	{
		attemptJump = true;
	}

	void HandleJump()
	{
		if (grounded)
		{

			if (attemptJump && jumpCooldownLeft < 0)
			{
				attemptJump = false;
				anim.ResetTrigger("JumpStart");
				anim.SetTrigger("JumpStart");
				anim.ResetTrigger("JumpEnd");
				//tryingToJump = true;
				jumpCooldownLeft = jumpCooldown;
				//jumpInProg = true;
				//anim.SetBool("JumpEnd", false);
				//StartCoroutine(TakeOff());
				//jumping = true;
				//rig.AddForce(transform.up * jumpForce);
				//Jump();
			}

			if (jumping)
			{
				jumping = false;
				//anim.SetBool("JumpStart", false);
				anim.ResetTrigger("JumpEnd");
				anim.SetTrigger("JumpEnd");
				anim.ResetTrigger("JumpStart");
				//jumping = false;
				//StartCoroutine(LandFromJump());
			}


		}
		else if (prevJump)
		{
			jumping = true;
		}
		prevJump = grounded;
	}
	bool idle;
	bool previouslyIdle;
	public float jumpCooldownLeft;
	public bool jumpInProg;
	void HandleMove()
	{
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump Start")) jumpCooldownLeft -= Time.deltaTime;
		float a = angle.eulerAngles.y;
		float t = transform.eulerAngles.y;
		float d = Mathf.DeltaAngle(a, t);
		
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
			anim.SetFloat("Direction", Mathf.Clamp(d / maxTurnSpeedAngle, -1, 1), 0.2f, Time.deltaTime);//player tries to turn in mid-air
			if (!grounded) anim.SetFloat("Speed", Mathf.Clamp01(direction.magnitude), 0.2f, Time.deltaTime);//player tries to run in mid-air
			if (grounded)
			{
				anim.SetFloat("Speed", velocityInDirection / maxSpeed, 0.2f, Time.deltaTime);//player is running on ground
																							 //TODO: move only now

				//attack angle:

				if (angleOff > ANGLE_THRESHOLD)
				{
					//TODO: for now it only rotates the y

					float ab = Mathf.Abs(d);

					bool cw = d > 0;//should the force be applied clockwise
					float av = rig.angularVelocity.y * Mathf.Rad2Deg;
					av = cw ? -av : av;//how fast it is turning in the desired direction

					float temp = maxTurnSpeedAngle;// * av;
												   //if (temp < 45) temp = 45;

					float tempMaxTurnSpeed = minTurnSpeed + (maxTurnSpeed - minTurnSpeed) * ab / temp;
					float tempMaxTurnAccel = minTurnAccel + (maxTurnAccel - minTurnAccel) * ab / temp;
					tempMaxTurnAccel = cw ? -tempMaxTurnAccel : tempMaxTurnAccel;
					if (av < tempMaxTurnSpeed)//av < tempMaxSpeed)
					{
						//print(av / tempMaxTurnSpeed);
						//help prevent overshoot causing "vibration" rotation
						rig.AddTorque(0, tempMaxTurnAccel * Time.deltaTime * rig.mass, 0);
					}
					else
					{
						//print(av / tempMaxTurnSpeed);
						//help prevent overshoot causing "vibration" rotation
						rig.AddTorque(0, -tempMaxTurnAccel * Time.deltaTime * rig.mass, 0);
					}
				}

				//float dot = Vector3.Dot(direction.normalized, (angle * Vector3.forward).normalized);
				float mult = 1 - Mathf.Clamp01((Vector3.Angle(angle * Vector3.forward, direction) - minAngleForMove) / (maxAngleForMove - minAngleForMove));

				//find the velocity in the direction

				//Vector3 velOff = direction.normalized - rig.velocity.normalized;
				float velDot = Vector3.Dot(rig.velocity, direction.normalized);
				//Vector3 newDir = direction * (1-velDot) + velOff * velDot;
				if (velDot < -0.5f)
				{
					rig.AddForce(-rig.velocity.normalized * acceleration * Time.deltaTime, ForceMode.VelocityChange);// * mult
				}
				else
				{
					if (rig.velocity.magnitude < maxSpeed * mult)
					{
						//print(transform.InverseTransformDirection(rig.velocity).z / (maxSpeed * mult));
						rig.AddForce(direction.normalized * acceleration * Time.deltaTime, ForceMode.VelocityChange);// * mult
					}
				}
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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + angle * Vector3.forward);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + direction + Vector3.up * 0.1f);
	}
}
