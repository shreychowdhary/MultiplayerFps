using UnityEngine;
using System.Collections;

public class WeaponData : MonoBehaviour {
    public enum GunType {Sniper = 0,Pistol = 1,AssaultRifle = 2};
    public float fireRate = 1.2f;
    public float headShot = 100f;
    public float damage = 50f;
    public float smoothFOV = 10.0F;
    public float aimDownFOV = 12.5f;
    public Vector3 gunHipPos;
    public Vector3 RecoilRotation;
    public Vector3 RecoilKickBack;
    public float clipSize = 15f;
    public float currentBullets;
    public float reloadTime = 2f;
    public float hipToAimSpeed = 0.075f;
    public bool automatic;
    public bool isAiming;
    public GunType gunType;
    public float aimRecoilReduction;
    public float aimRotationReduction;
    public float aimBulletSpreadReduction;
    public float recoilAdd;
    public float changeTime;
    public float returnBulletSpread;
    public float returnBulletSpreadSpeed;
    public float maxDist = 1f;
    public float aimDownSensitivityX;
    public float aimDownSensitivityY;
    public Vector3 bulletSpread;
    public Vector3 weaponDataPos;
    public Vector3 gunReturnPos;
    private GameObject gun;
    AimDown aimDown;
	// Use this for initialization
	void Start () {
        currentBullets = clipSize;
        gun = GameObject.FindGameObjectWithTag("Gun");
        aimDown = gameObject.GetComponentInParent<Camera>().GetComponent<AimDown>();
	}
	
	// Update is called once per frame
	void Update () {

	    weaponDataPos = gameObject.transform.position;
        gunReturnPos = new Vector3(gunHipPos.x * aimDown.racioHipHold, gunHipPos.y * aimDown.racioHipHold, gunHipPos.z);
	}
}
