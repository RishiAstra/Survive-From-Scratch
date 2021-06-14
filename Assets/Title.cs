using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//TODO: merge this with HPBar and use TMP_Text in it. This merged script would handle all of displaying lvl, hp etc.
//TODO: display title on mouse hover
public class Title : MonoBehaviour
{
    public GameObject hpBar;
    public TMP_Text text;
    public SaveEntity save;
    public StatScript s;
    // Start is called before the first frame update
    void Start()
    {
        save = GetComponentInParent<SaveEntity>();
        s = GetComponentInParent<StatScript>();
    }

	private void OnEnable()
	{
		SetActiveIfHPBarActive();
	}

	private void OnDisable()
	{
		SetActiveIfHPBarActive();
	}

	// Update is called once per frame
	void Update()
	{
		//for now, display title based on if hpbar is displayed

		SetActiveIfHPBarActive();

		if (save != null && s != null)
		{
			text.text = save.type + " lvl " + s.lvl;
		}
	}

	private void SetActiveIfHPBarActive()
	{
		text.gameObject.SetActive(hpBar.activeInHierarchy);
	}
}
