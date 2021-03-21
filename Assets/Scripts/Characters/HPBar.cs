using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public SpriteRenderer hpBarSprite;
	public Image hpBarImage;
	//TODO: make hp bar inactive since it's on ui for player

	[Tooltip("Shows the hp")]
	public TextMeshPro hpText;
	public TextMeshProUGUI hpTextUI;
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

	public void SetWorldHpBarVisible(bool visible)
	{
		hpHolder.gameObject.SetActive(visible);
	}

    // Update is called once per frame
    void Update()
    {
		bool sprite = hpBarSprite != null;
		bool image = hpBarImage != null;
		bool text = hpText != null;
		bool textUI = hpTextUI != null;
		if (hpHolder != null) hpHolder.LookAt(GameControl.mainCamera.transform);//TODO: optimize or change rendering to flat on screen by shader or something

		if (a.dead)
		{			
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
			if (text)
			{
				hpText.text = "Dead";
				hpText.color = new Color(0, 0, 0);
			}
			if (textUI)
			{
				hpTextUI.text = "Dead";
				hpTextUI.color = new Color(0, 0, 0);
			}

			return;
		}
		else
		{
			Color color = Color.magenta;//error color
			if (a.stat.hp > (a.maxStat.hp / 2))
			{
				if (sprite) hpBarSprite.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				if (image) hpBarImage.color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				if(changeHpTextColor) color = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				else color = new Color(0, 0, 0);
			}
			else
			{
				if (sprite) hpBarSprite.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				if (image) hpBarImage.color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				if(changeHpTextColor) color = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				else color = new Color(0, 0, 0);
			}

			string tempText = Mathf.RoundToInt(a.stat.hp) + "/" + Mathf.RoundToInt(a.maxStat.hp);//TODO: use Math.Round(hp, 2) to make it 2 decimal places

			if (text)
			{
				hpText.text = tempText;
				hpText.color = color;
			}
			if (textUI)
			{
				hpTextUI.text = tempText;
				hpTextUI.color = color;
			}

			if (sprite) hpBarSprite.transform.localScale = new Vector3(a.stat.hp / a.maxStat.hp, 1, 1);
			if (image) hpBarImage.transform.localScale = new Vector3(a.stat.hp / a.maxStat.hp, 1, 1);
		}
	}
}
