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

	public bool autoHide;
	public float hideTime;

	private StatScript a;
	private float hideTimeLeft;
	private float previousHp;
	private bool isHiding;
	private bool isDead;
    // Start is called before the first frame update
    void Start()
    {
		a = GetComponent<StatScript>();
		previousHp = a.stat.hp;
		UpdateDisplayLive(false);
		if(autoHide)SetHpBarsActive(false);
    }

	public void SetWorldHpBarVisible(bool visible)
	{
		hpHolder.gameObject.SetActive(visible);
	}

    // Update is called once per frame
    void Update()
    {
		bool statIsDead = a.dead;

		bool shouldHide = !statIsDead && autoHide && hideTimeLeft <= 0f;

		if(shouldHide != isHiding)
		{
			SetHpBarsActive(!shouldHide);
			isHiding = shouldHide;
		}

		//bool sprite = hpBarSprite != null;
		//bool image = hpBarImage != null;
		//bool text = hpText != null;
		//bool textUI = hpTextUI != null;
		//if (hpHolder != null) hpHolder.LookAt(GameControl.mainCamera.transform);//TODO: optimize or change rendering to flat on screen by shader or something

		if(!isHiding && hpHolder != null) hpHolder.LookAt(GameControl.mainCamera.transform);

		//update if alive and hp changed
		//also keep track of countdown to hide hp bars and update previousHp
		if (a.stat.hp != previousHp)
		{
			previousHp = a.stat.hp;
			hideTimeLeft = hideTime;
			if(!a.dead)	UpdateDisplayLive(false);
		}

		if (statIsDead != isDead)
		{
			UpdateDisplayLive(statIsDead);
			isDead = a.dead;
		}

		if (!statIsDead && autoHide)
		{
			hideTimeLeft -= Time.deltaTime;
		}

		//if (a.dead)
		//{
		//	//if (autoHide)
		//	//{
		//	//	SetHpBarsActive(true);
		//	//}
		//	UpdateDisplayLive(true);

		//	return;
		//}
		//else
		//{
		//	if (!autoHide || hideTimeLeft >= 0f)
		//	{
		//		//if(autoHide) SetHpBarsActive(true);
		//		UpdateDisplayLive(false);
		//	}
		//	//if (autoHide && hideTimeLeft < 0f) SetHpBarsActive(false);
		//	if (autoHide) hideTimeLeft -= Time.deltaTime;
		//}
	}

	private void UpdateDisplayLive(bool dead)
	{
		bool sprite = hpBarSprite != null;
		bool image = hpBarImage != null;
		bool text = hpText != null;
		bool textUI = hpTextUI != null;

		if (dead)
		{
			if (sprite)
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
		}
		else
		{
			Color color = Color.magenta;//error color
			if (a.stat.hp > (a.maxStat.hp / 2))
			{
				Color greenHalf = new Color(1 - (a.stat.hp - 0.5f * a.maxStat.hp) / (a.maxStat.hp / 2), 1, 0);
				if (sprite) hpBarSprite.color = greenHalf;
				if (image) hpBarImage.color = greenHalf;
				if (changeHpTextColor) color = greenHalf;
				else color = new Color(0, 0, 0);
			}
			else
			{
				Color redHalf = new Color(1, a.stat.hp / (a.maxStat.hp / 2), 0);
				if (sprite) hpBarSprite.color = redHalf;
				if (image) hpBarImage.color = redHalf;
				if (changeHpTextColor) color = redHalf;
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

	private void SetHpBarsActive(bool active)
	{
		bool sprite = hpBarSprite != null;
		bool image = hpBarImage != null;
		bool text = hpText != null;
		bool textUI = hpTextUI != null;

		if (sprite) hpBarSprite.gameObject.SetActive(active);
		if (image) hpBarImage.gameObject.SetActive(active);
		if (text) hpText.gameObject.SetActive(active);
		if (textUI) hpTextUI.gameObject.SetActive(active);
	}
}
