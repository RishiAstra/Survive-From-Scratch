using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//note: this can also be  MP, ENG, etc. bar
public class HPBar : MonoBehaviour
{
	public enum StatType
	{
		hp,
		mp,
		eng,
		mor,
		atk,
		xp,
	}

	public StatType type = StatType.hp;
    public SpriteRenderer hpBarSprite;
	public Image hpBarImage;
	//TODO: make hp bar inactive since it's on ui for player

	[Tooltip("Shows the hp")]
	public TextMeshPro hpText;
	public TextMeshProUGUI hpTextUI;
	public bool changeHpTextColor;
	public bool changeHpBarColor = true;
	public Gradient gradient = new Gradient
	{
		colorKeys = new GradientColorKey[] {
			new GradientColorKey { time = 0, color = Color.red },
			new GradientColorKey { time = 0.5f, color = new Color(1, 1, 0) },
			new GradientColorKey { time = 1, color = Color.green }
		},
		alphaKeys = new GradientAlphaKey[]
		{
			new GradientAlphaKey{time = 0, alpha = 1f }
		}
	};


	[Tooltip("Shown before the hp number")]
	public string prefix;
	[Tooltip("Shown if dead")]
	public string dead = "Dead";

	[Tooltip("This will face towards the camera")]
    public Transform hpHolder;

	public bool autoHide = true;
	public float hideTime = 3f;

	private StatScript a;
	private float hideTimeLeft;
	private float previousHp;
	private bool isHiding;
	private bool isDead;
    // Start is called before the first frame update
    void Start()
    {
		a = GetComponent<StatScript>();
		previousHp = GetStatValue();
		UpdateDisplayLive(false);
		if(autoHide)SetHpBarsActive(false);
    }

	float GetStatValue()
	{
		switch (type)
		{
			case StatType.hp:
				return a.stat.hp;
			case StatType.mp:
				return a.stat.mp;
			case StatType.eng:
				return a.stat.eng;
			case StatType.mor:
				return a.stat.mor;
			case StatType.atk:
				return a.stat.atk;
			case StatType.xp:
				return a.xp - StatScript.GetRequiredXPForLvl(a.lvl);
			default:
				return 0;
		}
	}
	float GetMaxStatValue()
	{
		switch (type)
		{
			case StatType.hp:
				return a.maxStat.hp;
			case StatType.mp:
				return a.maxStat.mp;
			case StatType.eng:
				return a.maxStat.eng;
			case StatType.mor:
				return a.maxStat.mor;
			case StatType.atk:
				return a.maxStat.atk;
			case StatType.xp:
				return StatScript.GetRequiredXPForLvl(a.lvl + 1) - StatScript.GetRequiredXPForLvl(a.lvl);
			default:
				return 0;
		}
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
		if (GetStatValue() != previousHp)
		{
			previousHp = GetStatValue();
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
			Color color = Color.magenta;// Color.magenta;//error color
			Color temp = gradient.Evaluate(GetStatValue() / GetMaxStatValue()) ;

			if (changeHpBarColor)
			{
				if (sprite) hpBarSprite.color = temp;
				if (image) hpBarImage.color = temp;
			}
			
			if (changeHpTextColor) color = temp;
			else color = new Color(0, 0, 0);

			//if (GetStatValue() > (GetMaxStatValue() / 2))
			//{
			//	Color greenHalf = new Color(1 - (GetStatValue() - 0.5f * GetMaxStatValue()) / (GetMaxStatValue() / 2), 1, 0);
			//	if (sprite) hpBarSprite.color = greenHalf;
			//	if (image) hpBarImage.color = greenHalf;
			//	if (changeHpTextColor) color = greenHalf;
			//	else color = new Color(0, 0, 0);
			//}
			//else
			//{
			//	Color redHalf = new Color(1, GetStatValue() / (GetMaxStatValue() / 2), 0);
			//	if (sprite) hpBarSprite.color = redHalf;
			//	if (image) hpBarImage.color = redHalf;
			//	if (changeHpTextColor) color = redHalf;
			//	else color = new Color(0, 0, 0);
			//}

			string tempText = Mathf.RoundToInt(GetStatValue()) + "/" + Mathf.RoundToInt(GetMaxStatValue());//TODO: use Math.Round(hp, 2) to make it 2 decimal places

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

			Vector3 scale = new Vector3(GetStatValue() / GetMaxStatValue(), 1, 1);
			if (sprite) hpBarSprite.transform.localScale = scale;
			if (image) hpBarImage.transform.localScale = scale;
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
