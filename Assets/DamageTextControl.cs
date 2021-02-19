using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextControl : MonoBehaviour
{
    public static DamageTextControl main;
    public static List<GameObject> texts;
    public static List<DamageText> damageTexts;
    public static List<GameObject> textsReady;
    public static List<DamageText> damageTextsReady;

    public GameObject damageTextPrefab;


    // Start is called before the first frame update
    void Start()
    {
        if (main != null)
		{
			Debug.LogError("Two DamageTextControl");
            DestroyImmediate(gameObject);
            return;
		}
		main = this;

        texts = new List<GameObject>();
        damageTexts = new List<DamageText>();
        textsReady = new List<GameObject>();
        damageTextsReady = new List<DamageText>();
    }

    public static void PutDamageText(Vector3 position, int damageAmount)
	{
        GameObject g;
        DamageText d;
        if(textsReady.Count > 0)
		{
            int index = textsReady.Count - 1;
            g = textsReady[index];
            d = damageTextsReady[index];
            textsReady.RemoveAt(index);
            damageTextsReady.RemoveAt(index);
		}
		else
		{
            g = Instantiate(main.damageTextPrefab);
            d = g.GetComponent<DamageText>();
            if (d == null) Debug.LogError("damage text prefab must have DamageText component");
		}

        g.transform.position = position;
        d.ResetText(damageAmount);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
