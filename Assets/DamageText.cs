using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public AnimationCurve occupacity;
    public float lifeTime;
    public TextMeshPro text;

    private Stopwatch sw;
    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
    }

	public void ResetText(int damageAmount)
	{
        sw.Restart();
        text.text = damageAmount.ToString();
    }

	// Update is called once per frame
	void Update()
    {
		if ()
		{

		}

		Color color = text.color;
		color.a = occupacity.Evaluate(sw.ElapsedMilliseconds);
        text.color = color;
    }
}
