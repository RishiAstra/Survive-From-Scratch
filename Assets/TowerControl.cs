using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    public static TowerControl main;

    public Menu towerMenu;
    public GameObject towerLevelButton;
    // Start is called before the first frame update
    void Start()
    {
        if (main != null) Debug.LogError("two TowerControl");
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
