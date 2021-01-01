using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public SpriteRenderer hpBarSprite;
	public Image hpBarImage;

	[Tooltip("Shows the hp")]
	public TextMeshPro hpText;
	public bool changeHpTextColor;
	[Tooltip("Shown before the hp number")]
	public string prefix;
	[Tooltip("Shown if dead")]
	public string dead = "Dead";

	[Tooltip("This will face towards the camera")]
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
		bool sprite = hpBarSprite != null;
		bool image = hpBarImage != null;
		if (hpHolder != null) hpHolder.LookAt(Camera.main.transform);//TODO: optimize or change rendering to flat on screen by shader or something

		if (a.dead)
		{
			hpText.text = "Dead";
			if(sprite)
			{
				hpBarSprite.transform.localScale = new Vector3(0, 1, 1);
				hpBarSprite.color = new Color(0, 0, 0);
			}
			if (image)
			{
				hpBarImage.transform.localScale = new Vector3(0, 1, 1);
				hpBarImage.color = new Color(0, 0, 0);
			}

			hpText.color = new Color(0, 0, 0);
			return;
		}
		else
		{
			if (a.stat.hp > (a.maxStat.hp / 2))
			{
				if (sprite) hpBarSprite.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				if (image) hpBarImage.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				if(changeHpTextColor) hpText.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				else hpText.color = new Color(0, 0, 0);
			}
			else
			{
				if (sprite) hpBarSprite.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				if (image) hpBarImage.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				if(changeHpTextColor) hpText.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				else hpText.color = new Color(0, 0, 0);
			}
			hpText.text = Mathf.Round(a.stat.hp) + "/" + Mathf.Round(a.maxStat.hp);//TODO: use Math.Round(hp, 2) to make it 2 decimal places

			if (sprite) hpBarSprite.transform.localScale = new Vector3(a.stat.hp / a.maxStat.hp, 1, 1);
			if (image) hpBarImage.transform.localScale = new Vector3(a.stat.hp / a.maxStat.hp, 1, 1);
		}
	}
}
