using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

    public Texture2D crosshairTexture;
    bool _aimDown;

    public bool aimDown{
        get { return _aimDown; }
        set { _aimDown = value; }
    }

	
	// Update is called once per frame
	void OnGUI () {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        if (!_aimDown) {
            GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshairTexture.width / 2), (Screen.height / 2) - (crosshairTexture.height / 2), crosshairTexture.width, crosshairTexture.height), crosshairTexture);
        }
        GUILayout.EndArea();
   }
}
