using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(SkillObject))]
public class AOEDamage : MonoBehaviour
{
    public float lifeTime;
    public float damageTime;
    public bool hitSameEnemyMultipleTimes;
    public AttackType type;

    private List<StatScript> hit;//what enemies this has already hit
    private SkillObject so;//the skillobject of this skill/spell/explosion/thing/etc.
    private Stopwatch sw;
    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw.Start();
        so = GetComponent<SkillObject>();
        hit = new List<StatScript>();
        Destroy(gameObject, lifeTime);//apply lifetime
    }

	private void OnTriggerEnter(Collider other)
	{
        float timePassed = sw.ElapsedMilliseconds / 1000f;
        if (timePassed > damageTime) return;//don't damage if can no longer damage

        //might collide with child collider
        TagScript t = other.GetComponentInParent<TagScript>();

		if (t != null && t.ContainsTag(so.parent.enemyString))
		{
            //tag script is on the root character gameobject, so no need to search parents
            StatScript a = t.GetComponent<StatScript>();
            //only attack each one once unless hitSameEnemyMultipleTimes
            if (a != null)
            {
                if (hitSameEnemyMultipleTimes || !hit.Contains(a)){
                    hit.Add(a);
                    a.Damage(so.parent.myStat.stat.atk, so.parent, other, type);
                }
            }
			else
			{
				UnityEngine.Debug.LogError("Enemy tag but no abilities");
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
