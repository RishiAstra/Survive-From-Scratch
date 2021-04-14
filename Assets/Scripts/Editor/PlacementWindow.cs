using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class PlacementWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool placingEnabled = false;
    float radius = 1.23f;
    int maxAmount = 1;
    //float max

    GameObject toPlace;

    private Vector3 pos;
    private bool hitSomething = false;
    private double refreshTime = 1 / 10f;
    private double lastTime;
    private Stopwatch sw = new Stopwatch();
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Object Placer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PlacementWindow window = (PlacementWindow)EditorWindow.GetWindow(typeof(PlacementWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        toPlace = (GameObject)EditorGUILayout.ObjectField("", toPlace, typeof(GameObject), true);
        myString = EditorGUILayout.TextField("Name", myString);
        placingEnabled = EditorGUILayout.Toggle("Enable Placing", placingEnabled);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        radius = EditorGUILayout.Slider("Radius", radius, 0.01f, 20);
        maxAmount = EditorGUILayout.IntSlider("Amount", maxAmount, 1, 50);
        EditorGUILayout.EndToggleGroup();



    }

	private void Update()
	{
        double nowTime = sw.Elapsed.TotalSeconds;
        if (nowTime > lastTime + refreshTime)
		{
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            lastTime = nowTime;
            //UnityEngine.Debug.Log("d");
        }
    }

    //void AllDrawingStuff()
    //{
    //    Handles.BeginGUI();
    //    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);// HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit)) Handles.DrawWireCube(hit.point, Vector3.one);
    //    Handles.EndGUI();
    //}


    // Window has been selected
    void OnFocus()
    {
        sw.Start();
        lastTime = 0;
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!placingEnabled) return;
        hitSomething = false;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            pos = hit.point;
            hitSomething = true;
        }

        Event e = Event.current;
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		switch (e.GetTypeForControl(controlID))
		{
			case EventType.MouseDown:
				GUIUtility.hotControl = controlID;
				if (Event.current.button == 0)
				{
					if (toPlace != null)
					{
						//Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
						//RaycastHit hit;
						if (hitSomething)//Physics.Raycast(ray, out hit))
						{
                            for(int i = 0; i < maxAmount; i++)
							{
                                Vector3 forward = Random.insideUnitSphere;
                                Vector3 offset = Random.insideUnitCircle * radius;
                                offset.z = offset.y;
                                offset.y = 0;
                                Vector3 location = hit.point + offset + hit.normal * 1;
                                RaycastHit h;
								if (Physics.Raycast(location, -hit.normal, out h, 10f))
								{
                                    forward -= h.normal * Vector3.Dot(forward, hit.normal);//make it perpendicular
                                    Instantiate(toPlace, h.point, Quaternion.LookRotation(forward.normalized, h.normal));    
								}
                                                        
                            }

							
							//print();
						}
					}

                    e.Use();
                }
				break;
			//case EventType.MouseUp:
			//	GUIUtility.hotControl = 0;

			//	e.Use();
			//	break;
			//case EventType.MouseDrag:
			//	GUIUtility.hotControl = controlID;

			//	e.Use();
			//	break;
			//case EventType.KeyDown:
			//	if (e.keyCode == KeyCode.Escape)
			//	{
			//		// Do something on pressing Escape
			//	}
			//	if (e.keyCode == KeyCode.Space)
			//	{
			//		// Do something on pressing Spcae
			//	}
			//	if (e.keyCode == KeyCode.S)
			//	{
			//		// Do something on pressing S
			//	}
			//	break;
		}
		//AllDrawingStuff();
		// Do your drawing here using Handles.
		//Handles.BeginGUI();
        Handles.color = Color.cyan;
		//UnityEngine.Debug.Log(pos);
		if (hitSomething)
		{
            Handles.DrawWireCube(pos, Vector3.one);
            Handles.DrawWireDisc(pos, hit.normal, radius);
        }
        // Do your drawing here using GUI.
        //Handles.EndGUI();
    }
}
