using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneBasicCodes;
using ActionBasicCodes;

public class UserGUI : MonoBehaviour {
	private IUserAction action;
	bool isFirst = true;
	private GameObject cam;
	GUIStyle buttonStyle;
	GUIStyle labelStyle;
	// Use this for initialization
	void Start () {
		action = Director.getInstance().currentSceneController as IUserAction;
		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = Screen.width/30;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.normal.textColor = Color.white;
		labelStyle = new GUIStyle("label");
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontSize = Screen.height/40;
		labelStyle.normal.textColor = Color.white;
	}
	
	void OnGUI() {
		if(action.getGameState() == 0 && GUI.Button(new Rect(Screen.width/2 - Screen.width/12, Screen.height/2 - Screen.height/16, Screen.width/6, Screen.height/8),"Restart", buttonStyle)) {
			action.getGameStart();
		}
		else if(action.getGameState() == 0)
			GUI.Label(new Rect(Screen.width/2 - Screen.width/12, Screen.height/5 + Screen.height/16, Screen.width/6, Screen.height/8), "Score: "+ action.getScore(), labelStyle);
		else {
			GUI.Label(new Rect(Screen.width/10, Screen.height/5,  Screen.width/10,  Screen.height/10), "Round: " + action.getRound(), labelStyle);
			GUI.Label(new Rect(Screen.width/10, Screen.height/5 + 100,  Screen.width/10,  Screen.height/10), "Score: "+ action.getScore(), labelStyle);
		}

	}

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 mp = Input.mousePosition;
			action.hit(mp);
		}
	}
}
