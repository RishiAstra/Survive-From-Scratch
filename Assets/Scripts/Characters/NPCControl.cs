/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: add good wandering behavior
public class NPCControl : MonoBehaviour, ISaveable
{
	public LayerMask targetMask;
	public Movement movement;
	public float spotRange;
	public Transform checkAttack;
	public float checkAttackRadius;

	public List<StatScript> targets;

	public bool guard;
	public Vector3 guardPosition;
	public float maxGuardDist;

	private Abilities abilities;

	// Start is called before the first frame update
	void Start()
    {
		abilities = GetComponent<Abilities>();
		movement = GetComponent<Movement>();
		targets = new List<StatScript>();

		if (guard)
		{
			Minimap.main.AddArrowedPosition(transform);
		}
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
		//float angle = abilities.skills[0].useAngle;
		//TODO: optimize spotting
		if(Time.frameCount % 5 == 0) Spot();//only spot sometimes
		if(targets.Count > 0)
		{
			int index = -1;
			for(int i = 0;i < targets.Count; i++)
			{
				if(targets[i] != null)
				{
					index = i;
				}
			}
			if(index != -1)
			{
				StatScript t = targets[index];//TODO: find optimal target, depending on intelligence
				Transform attackPlace = checkAttack;// abilities.attackTranforms[0];
				Vector3 off = t.transform.position - attackPlace.position;
				Quaternion targetAngle = Quaternion.LookRotation(off, transform.up);
				//float angleOff = Quaternion.Angle(transform.rotation, targetAngle);
				//movement.SetAngle(targetAngle);
				bool usedAttack = false;
				foreach (Collider col in Physics.OverlapSphere(checkAttack.position, checkAttackRadius, targetMask))//TODO: this can change depending on intelligence, dumb can attack at wrong distance. This should be based off of the skill's attack area/dist.
				{
					TagScript tagScript = col.GetComponent<TagScript>();//TODO: consider GetComponentInParent

					//TODO: make targets a struct or something that has tagscript and abilities
					if (tagScript != null && tagScript.ContainsTag(abilities.enemyString))
					{

						StatScript temp = col.GetComponent<StatScript>();
						if (temp != null)
						{
							
							usedAttack = true;
							break;
						}
					}

				}
				if (usedAttack)
				{
					abilities.UseSkill(0);
					movement.SetDirection(Vector3.zero);
				}
				else
				{

					movement.SetDirection(off.normalized);
					//movement.SetAngleFromDirection();
					Vector3 dir = t.transform.position - transform.position;
					dir.y = 0;
					Quaternion ta = Quaternion.LookRotation(dir, Vector3.up);
					movement.SetAngle(ta);
				}
			}
			
		}
		else
		{
			//if guarding a position, move back if too far
			if(guard && (transform.position - guardPosition).sqrMagnitude > maxGuardDist * maxGuardDist)
			{
				movement.SetDirection((guardPosition - transform.position).normalized);
				movement.SetAngleFromDirection();
			}
			else
			{
				//movement.SetAngle(targetAngle);
				movement.SetDirection(Vector3.zero);
			}
		}
	}
	
	void Spot()
	{
		targets = new List<StatScript>();
		foreach(Collider col in Physics.OverlapSphere(transform.position, spotRange, targetMask))
		{
			//below code was removed because the enemies simply wouldn't do anything when the player stood outside the distance and attacked them, resulting in easy kills with no danger
			//if(guard && (col.transform.position - guardPosition).sqrMagnitude > maxGuardDist * maxGuardDist){
			//	continue;//don't target stuff too far
			//}
			TagScript tagScript = col.GetComponent<TagScript>();
			if(tagScript != null && tagScript.ContainsTag(abilities.enemyString))
			{

				StatScript temp = col.GetComponent<StatScript>();
				if (temp != null)
				{
					targets.Add(temp);
				}
			}
		}
	}

	public string GetData()
	{
		SaveDataNPCControl s = new SaveDataNPCControl()
		{
			guard = this.guard,
			guardPosition = this.guardPosition,
			maxGuardDist = this.maxGuardDist
		};

		return JsonConvert.SerializeObject(s, Formatting.Indented, Save.jsonSerializerSettings);
	}

	public void SetData(string data)
	{
		SaveDataNPCControl s = JsonConvert.DeserializeObject<SaveDataNPCControl>(data);
		this.guard = s.guard;
		this.guardPosition = s.guardPosition;
		this.maxGuardDist = s.maxGuardDist;
	}

	public string GetFileNameBaseForSavingThisComponent()
	{
		return "NPC_control";
	}
}
