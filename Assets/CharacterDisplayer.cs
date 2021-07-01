using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CharacterDisplayer : MonoBehaviour
{
	public Image icon;
	public TMP_Text nameText;
	public HPBar hpbar;
	public HPBar mpbar;
	public HPBar engbar;
	public HPBar xpbar;
	// Start is called before the first frame update
	void Start()
	{
		hpbar.hpBarImage.color = GameControl.main.hpColor;
		mpbar.hpBarImage.color = GameControl.main.mpColor;
		engbar.hpBarImage.color = GameControl.main.engColor;
		xpbar.hpBarImage.color = GameControl.main.xpColor;
	}

	public void SetTarget(PartyMember t)
	{
		nameText.text = t.name;

		StatScript s = t.g.GetComponent<StatScript>();
		if(s == null)
		{
			Debug.LogError("Party member StatScript not found");
			return;
		}
		hpbar.SetTarget(s);
		mpbar.SetTarget(s);
		engbar.SetTarget(s);
		xpbar.SetTarget(s);

		StartCoroutine(SetIcon(t.type));
	}

	public IEnumerator SetIcon(string type)
	{
		string iconPath = SaveEntity.spawnPath + type + " i";
		AsyncOperationHandle<Sprite> spriteFetchOperation = Addressables.LoadAssetAsync<Sprite>(iconPath);
		
		yield return spriteFetchOperation;
		
		Sprite result = spriteFetchOperation.Result;
		if (result == null)
		{
			Debug.LogError("Icon for character \"" + type + "\" not found at location \"" + iconPath + "\"");
		}

		icon.sprite = spriteFetchOperation.Result;
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
