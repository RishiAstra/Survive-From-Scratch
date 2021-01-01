using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public SpriteRenderer hpBar;
    public TextMesh hpText;
    public Transform hpHolder;

	private Abilities a;
    // Start is called before the first frame update
    void Start()
    {
		a = GetComponent<Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
        hpHolder.LookAt(Camera.main.transform);//TODO: optimize or change rendering to flat on screen by shader or something
		if (a.dead)
		{
			hpText.text = "Dead";
			hpBar.transform.localScale = new Vector3(0, 1, 1);
			hpBar.color = new Color(0, 0, 0);
			hpText.color = new Color(0, 0, 0);
			return;
		}
		else
		{
			if (a.stat.hp > (a.maxStat.hp / 2))
			{
				hpBar.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				hpText.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
			}
			else
			{
				hpBar.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				hpText.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
			}
			hpText.text = Mathf.Round(a.stat.hp) + "/" + Mathf.Round(a.maxStat.hp);//TODO: use Math.Round(hp, 2) to make it 2 decimal places
			hpBar.transform.localScale = new Vector3(a.stat.hp / a.maxStat.hp, 1, 1);
		}
	}
}
