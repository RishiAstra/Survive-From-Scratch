using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: add good wandering behavior
public class NPCControl : MonoBehaviour
{
	public LayerMask targetMask;
	public Movement movement;
	public float spotRange;
	public string[] enemyString;
	public Transform checkAttack;
	public float checkAttackRadius;

	public List<Abilities> targets;

	private Abilities abilities;

	// Start is called before the first frame update
	void Start()
    {
		abilities = GetComponent<Abilities>();
		movement = GetComponent<Movement>();
		targets = new List<Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
		//if (Input.GetKey(KeyCode.Space)) movement.AttemptJump();
		//movement.SetAngle(cam.transform.eulerAngles.y);
		//Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		//dir = Quaternion.Euler(0, cam.pivot.eulerAngles.y, 0) * dir;
		//movement.SetDirection(dir);

		//TODO: can spot through things
		float angle = abilities.skills[0].useAngle;
		Spot();
		if(targets.Count > 0)
		{
			Abilities t = targets[0];//TODO: find optimal target, depending on intelligence
			Transform attackPlace = abilities.attackTranforms[0];
			Vector3 off = t.transform.position - attackPlace.position;
			Quaternion targetAngle = Quaternion.LookRotation(off, transform.up);
			float angleOff = Quaternion.Angle(transform.rotation, targetAngle);
			movement.SetAngle(targetAngle);
			movement.SetDirection(off.normalized);
			foreach (Collider col in Physics.OverlapSphere(checkAttack.position, checkAttackRadius, targetMask))//TODO: this can change depending on intelligence, dumb can attack at wrong distance. This should be based off of the skill's attack area/dist.
			{
				TagScript tagScript = col.GetComponent<TagScript>();
				if (tagScript != null && tagScript.ContainsTag(enemyString))
				{

					Abilities temp = col.GetComponent<Abilities>();
					if (temp != null)
					{
						abilities.UseSkill(0);
					}
				}

			}
		}
		else
		{
			//movement.SetAngle(targetAngle);
			movement.SetDirection(Vector3.zero);
		}
	}
	
	void Spot()
	{
		targets = new List<Abilities>();
		foreach(Collider col in Physics.OverlapSphere(transform.position, spotRange, targetMask))
		{
			TagScript tagScript = col.GetComponent<TagScript>();
			if(tagScript != null && tagScript.ContainsTag(enemyString))
			{

				Abilities temp = col.GetComponent<Abilities>();
				if (temp != null)
				{
					targets.Add(temp);
				}
			}
			
		}
	}
}
