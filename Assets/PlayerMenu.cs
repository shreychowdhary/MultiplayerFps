using UnityEngine;
using System.Collections;

public class PlayerMenu : MonoBehaviour {
    public bool menuOn = false;
    MouseLook mouseLookX;
    MouseLook mouseLookY;
    LockCursorOnClickScript mouseLock;
    PlayerShooting playerShooting;
    AimDown aimDown;


	// Use this for initialization
	void Start () {
        mouseLookX = GetComponent<MouseLook>();
        mouseLookY = gameObject.GetComponentInChildren<Camera>().GetComponent<MouseLook>();
        mouseLock = gameObject.GetComponentInChildren<Camera>().GetComponent<LockCursorOnClickScript>();
        aimDown = gameObject.GetComponentInChildren<Camera>().GetComponent<AimDown>();
        playerShooting = GetComponent<PlayerShooting>();
        if (PlayerPrefs.HasKey("velMagX")) {
            mouseLookX.velMagX = PlayerPrefs.GetFloat("velMagX");
        }
        else {
            PlayerPrefs.SetFloat("velMagX", mouseLookX.velMagX);
        }

        if (PlayerPrefs.HasKey("velMagY")) {
            mouseLookY.velMagY = PlayerPrefs.GetFloat("velMagY");
        }
        else {
            PlayerPrefs.SetFloat("velMagY", mouseLookY.velMagY);
        }
	}
	
	// Update is called once per frame
	void Update () {  
        if (Input.GetKeyDown(KeyCode.Escape)) {
            menuOn = !menuOn;          
            gameObject.GetComponent<FPSInputController>().enabled = !gameObject.GetComponent<FPSInputController>().enabled;
            aimDown.enabled = !aimDown.enabled;
            mouseLookX.enabled = !mouseLookX.enabled;       
            mouseLookY.enabled = !mouseLookY.enabled;
            mouseLock.mouseLock = !mouseLock.mouseLock;
            playerShooting.enabled = !playerShooting.enabled;
            if (mouseLookX.velMagX != PlayerPrefs.GetFloat("velMagX")) {
                PlayerPrefs.SetFloat("velMagX", mouseLookX.velMagX);
                Debug.Log("changing sensitivity");
            }
            if (mouseLookX.velMagY != PlayerPrefs.GetFloat("velMagY")) {
                PlayerPrefs.SetFloat("velMagY", mouseLookY.velMagY);
            }
            //if (mouseLock.mouseLock) {
            //    mouseLock.mouseLock = false;
            //}
            //else {
            //    mouseLock.mouseLock = true;
            //}
        }
        
	}

    void OnGUI() {
        if (menuOn && GetComponent<PhotonView>().isMine) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            if (GUI.Button(new Rect(Screen.width / 8, Screen.height / 6, 100, 30), "Disconnect")) {
                NetworkManager networkManager = GameObject.FindObjectOfType<NetworkManager>();
                networkManager.disconnect = true;
            }
            if (GUI.Button(new Rect(Screen.width / 7, Screen.height / 4, 100, 30), "Sniper")) {
                PlayerPrefs.SetInt("ClassGun", 0);
            }
            if (GUI.Button(new Rect(Screen.width / 7, Screen.height / 4 +30, 100, 30), "Assault Rifle")) {
                PlayerPrefs.SetInt("ClassGun", 2);
            }
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(Screen.width / 2-10, Screen.height / 2 -35, 200, 50), "X Sensitivity: ");
            mouseLookX.velMagX = GUI.HorizontalSlider(new Rect(Screen.width / 2, Screen.height / 2-5, 200, 50), mouseLookX.velMagX, 1, 20);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 + 25, 200, 50), "Y Sensitivity: ");
            
            mouseLookY.velMagY = GUI.HorizontalSlider(new Rect(Screen.width / 2, Screen.height / 2 + 50, 200, 50), mouseLookY.velMagY, 1, 20);
            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
