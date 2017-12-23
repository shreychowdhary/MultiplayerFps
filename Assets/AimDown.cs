using UnityEngine;
using System.Collections;

public class AimDown : MonoBehaviour {

    Camera playerCamera;
    GameObject player;
    GameObject gun;
    MouseLook mouseLookY;
    MouseLook mouseLookX;
    PlayerGUI playerGUI;
    PlayerShooting playerShooting;
    public Texture2D sniperScope;
    float defaultSniperFOV;
    public float racioHipHold = 1;
    float racioHipHoldV;
    WeaponData weaponData;
    WeaponChange weaponChange;
    // Use this for initialization
    void Start() {
        playerCamera = gameObject.GetComponent<Camera>();
        player = gameObject.transform.root.gameObject;
        playerShooting = player.GetComponent<PlayerShooting>();
        mouseLookX = player.GetComponent<MouseLook>();
        mouseLookY = playerCamera.GetComponent<MouseLook>();
        playerGUI = playerCamera.GetComponent<PlayerGUI>();
        defaultSniperFOV = playerCamera.fieldOfView;
        weaponChange = player.GetComponent<WeaponChange>();
        gun = GameObject.FindGameObjectWithTag("Gun");
        weaponData = gun.GetComponentInChildren<WeaponData>();
    }

    // Update is called once per frame
    void Update() {
        if (weaponChange.changingWeapons) {
            gun = GameObject.FindGameObjectWithTag("Gun");
            weaponData = gun.GetComponentInChildren<WeaponData>();
        }
        if (gun == null) {
            gun = GameObject.FindGameObjectWithTag("Gun");

        }
        if (weaponData == null) {
            weaponData = GameObject.FindGameObjectWithTag("FirePoint").GetComponent<WeaponData>();
        }
        if (Input.GetButton("Fire2") && !weaponChange.changingWeapons) {
            racioHipHold = Mathf.SmoothDamp(racioHipHold, 0, ref racioHipHoldV, weaponData.hipToAimSpeed);
            playerShooting.isHipAim = 0;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, weaponData.aimDownFOV, Time.deltaTime * weaponData.smoothFOV);
            mouseLookX.aimDown = weaponData.aimDownSensitivityX;
            mouseLookY.aimDown = weaponData.aimDownSensitivityY;
            weaponData.isAiming = true;
            Debug.Log("aiming");
        }

        if(!Input.GetButton("Fire2")||weaponChange.changingWeapons) {
                playerShooting.isHipAim = 1;
                racioHipHold = Mathf.SmoothDamp(racioHipHold, 1, ref racioHipHoldV, weaponData.hipToAimSpeed);
                playerCamera.cullingMask |= (1 << 8);
                playerGUI.aimDown = false;
                weaponData.isAiming = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultSniperFOV, Time.deltaTime * weaponData.smoothFOV);
                playerCamera.fieldOfView = defaultSniperFOV;
                mouseLookX.aimDown = 1f;
                mouseLookY.aimDown = 1f;
        }

        gun.transform.localPosition = new Vector3(weaponData.gunHipPos.x * racioHipHold, weaponData.gunHipPos.y * racioHipHold, gun.transform.localPosition.z);

        if (Vector3.Distance(gun.transform.localPosition, new Vector3(0, 0, gun.transform.localPosition.z)) < 0.01) {
            playerGUI.aimDown = true;
            if (weaponData.gunType == 0 ) {
                playerCamera.cullingMask = ~(1 << 8);
            }
        }
        
    }
    void OnGUI() {
        if (playerGUI.aimDown && weaponData.gunType == 0) {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), sniperScope);
        }
    }
}
