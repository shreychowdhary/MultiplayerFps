using UnityEngine;
using System.Collections;

public class Killstreak : MonoBehaviour {
    PhotonView pv;
    Health health;
    bool healthRegen;
    GUIStyle title = new GUIStyle();
    GUIStyle grey = new GUIStyle();
    GUIStyle white = new GUIStyle();
	// Use this for initialization
	void Start () {
        pv = gameObject.GetComponent<PhotonView>();
        health = gameObject.GetComponent<Health>();
        grey.normal.textColor = Color.grey;
        grey.fontSize = 25;
        white.normal.textColor = Color.white;
        white.fontSize = 25;
        title.normal.textColor = Color.white;
        title.fontSize = 20;
	}
	
	// Update is called once per frame
	void Update () {


        switch ((int)pv.owner.customProperties["Killstreak"]) {
            case 3:
                ExitGames.Client.Photon.Hashtable playerStats = pv.owner.customProperties;
                playerStats["KillstreaksAvailable1"] = 1;
                pv.owner.SetCustomProperties(playerStats);
                break;
        }

        if ((int)pv.owner.customProperties["KillstreaksAvailable1"] == 1 && pv.isMine) {
            healthRegen = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && healthRegen) {
            health.CurrentHitPoints = 100f;
            ExitGames.Client.Photon.Hashtable playerStats = pv.owner.customProperties;
            playerStats["KillstreaksAvailable1"] = 0;
            pv.owner.SetCustomProperties(playerStats);
        }
	}

    void OnGUI() {
        GUI.Label(new Rect(0, Screen.height *2/3 - 20, 75, 30), "Killstreaks:",title);
        if (healthRegen) {
            GUI.Label(new Rect(0, Screen.height - Screen.height / 3, 75, 30), "Full Health", white);
        }
        else {
            GUI.Label(new Rect(0, Screen.height - Screen.height / 3, 75, 30), "Full Health", grey);
        }
    }
}
