using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

    float coolDown = 0f;
    float lastHitTime;
    public Texture2D hitmarker;
    FXManager fxManager;
    WeaponData weaponData;
    WeaponChange weaponChange;
    GameObject gun;
    public GameObject bulletDecal;
    float inFrontOfWall = 0.0075f;
    float _isHipAim;
    Vector3 CurrentRecoil1;
    Vector3 CurrentRecoil2;
    Vector3 CurrentRecoil3;
    Vector3 CurrentRecoil4;
    float rotationReduction;
    float bulletSpreadReduction;
    float recoilReduction;
    float reloadCoolDown;
    float gradualRecoil = 0.075f;
    GUIStyle fontSize;



    public float isHipAim {
        get { return _isHipAim; }
        set { _isHipAim = value; }
    }
    void Start() {
        fxManager = GameObject.FindObjectOfType<FXManager>();
        weaponChange = gameObject.GetComponent<WeaponChange>();
        gun = GameObject.FindGameObjectWithTag("Gun");
        weaponData = gun.GetComponentInChildren<WeaponData>();
        fontSize = new GUIStyle();
        fontSize.fontSize = 40;
        fontSize.normal.textColor = Color.white;
        if (fxManager == null) {
            Debug.Log("couldn't find FxManager");
        }
        if (weaponData == null) {
            Debug.Log("couldn't find Weapon Data");
        }
    }

	// Update is called once per frame
	void Update () {
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

        
        RecoilController();
        
        coolDown -= Time.deltaTime;
        reloadCoolDown -= Time.deltaTime;
        

        if (!weaponData.automatic) {
            if (Input.GetButtonDown("Fire1") && coolDown <= 0 && !weaponChange.changingWeapons && reloadCoolDown <=0 && weaponData.currentBullets > 0) {
                if (gradualRecoil <= 1)
                    gradualRecoil += weaponData.recoilAdd;

                CurrentRecoil1 += new Vector3(weaponData.RecoilRotation.x * rotationReduction, Random.Range(weaponData.RecoilRotation.y * rotationReduction, -weaponData.RecoilRotation.y * rotationReduction));
                CurrentRecoil3 += new Vector3(Random.Range(weaponData.RecoilKickBack.x * recoilReduction, -weaponData.RecoilKickBack.x * recoilReduction), Random.Range(weaponData.RecoilKickBack.y * recoilReduction, -weaponData.RecoilKickBack.y * recoilReduction), weaponData.RecoilKickBack.z* recoilReduction);
                Fire();
                weaponData.currentBullets--;
            }
        }
        if (weaponData.automatic ) {
            if (Input.GetButton("Fire1") && coolDown <= 0 && !weaponChange.changingWeapons && reloadCoolDown <= 0 && weaponData.currentBullets > 0) {
                if(gradualRecoil <= 1)
                    gradualRecoil += weaponData.recoilAdd;
                
                CurrentRecoil1 += new Vector3(weaponData.RecoilRotation.x * rotationReduction, Random.Range(weaponData.RecoilRotation.y * rotationReduction, -weaponData.RecoilRotation.y * rotationReduction));
                CurrentRecoil3 += new Vector3(Random.Range(weaponData.RecoilKickBack.x * recoilReduction, -weaponData.RecoilKickBack.x * recoilReduction), Random.Range(weaponData.RecoilKickBack.y * recoilReduction, -weaponData.RecoilKickBack.y * recoilReduction), weaponData.RecoilKickBack.z * recoilReduction);
                Fire();
                weaponData.currentBullets--;
            }
        }
        if (weaponData.isAiming) {
            rotationReduction = weaponData.aimRotationReduction * gradualRecoil;
            recoilReduction = weaponData.aimRecoilReduction;
            bulletSpreadReduction = weaponData.aimBulletSpreadReduction * gradualRecoil;
        }
        else {
            rotationReduction = 1 * gradualRecoil;
            bulletSpreadReduction = 1 * gradualRecoil;
            recoilReduction = 1;
        }

        if (Input.GetKeyDown(KeyCode.R) || (Input.GetButtonDown("Fire1") && weaponData.currentBullets == 0)) {
            Reload();
        }
        
	}

    void Fire() {
        
        if (coolDown > 0) {
            return;
        }
        
        if (weaponData == null) {
            weaponData = gameObject.GetComponentInChildren<WeaponData>();
            if (weaponData == null) {
                Debug.Log("couldn't find Weapon Data");
            }
            return;
        }
        Vector3 direction = gun.transform.forward;
        direction.x += Random.Range(-weaponData.bulletSpread.x * bulletSpreadReduction, weaponData.bulletSpread.x * bulletSpreadReduction);
        direction.y += Random.Range(-weaponData.bulletSpread.y * bulletSpreadReduction, weaponData.bulletSpread.y * bulletSpreadReduction);
        direction.z += Random.Range(-weaponData.bulletSpread.z * bulletSpreadReduction, weaponData.bulletSpread.z * bulletSpreadReduction);
        Ray ray = new Ray(Camera.main.transform.position, direction);
        Transform hitTransform;
        Transform originalHit;
        Vector3 hitPoint;
        Vector3 hitNormal;
        hitTransform = FindClosestHitObject(ray, out hitPoint,out hitNormal);
        originalHit = hitTransform;
        if (hitTransform != null) {
            Debug.Log("We hit: " + hitTransform.name);
            Health h = hitTransform.transform.GetComponent<Health>();

            if (h == null) {
                if (hitTransform.parent)    
                    hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            if (bulletDecal && hitTransform.gameObject.tag != "Player") {
                GameObject bulletHole = (GameObject)Instantiate(bulletDecal, hitPoint + (hitNormal * inFrontOfWall), Quaternion.LookRotation(hitNormal));
                Destroy(bulletHole, 5);
            }


            if (h != null) {
                PhotonView pv = h.GetComponent<PhotonView>();
                if (pv == null) {
                    Debug.LogError("object has no photon view");
                }
                else {
                    TeamMember tm = hitTransform.GetComponent<TeamMember>();
                    TeamMember myTm = this.GetComponent<TeamMember>();

                    if (tm == null || tm.teamID == 0 || myTm == null || myTm.teamID == 0 || myTm.teamID != tm.teamID) {
                        if (originalHit.gameObject.tag == "Head") {
                            pv.RPC("TakeDamage", PhotonTargets.AllBuffered, weaponData.headShot, gameObject.GetComponent<PhotonView>().owner.ID);
                            
                            lastHitTime = Time.time;
                        }
                        else {
                            pv.RPC("TakeDamage", PhotonTargets.AllBuffered, weaponData.damage, gameObject.GetComponent<PhotonView>().owner.ID);
                            lastHitTime = Time.time;
                        }
                    }
                }
            }
            if (fxManager != null) {
                DoGunFX(hitPoint,(int)weaponData.gunType);
            }
        }
        else {
            if (fxManager != null) {
                hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 150f);
                DoGunFX(hitPoint, (int)weaponData.gunType);
            }
        }
        coolDown = weaponData.fireRate;
    }

    void Reload() {
        weaponData.currentBullets = weaponData.clipSize;
        reloadCoolDown = weaponData.reloadTime;
    }

    void DoGunFX(Vector3 hitPoint,int gunType) {
        fxManager.GetComponent<PhotonView>().RPC("BulletFX", PhotonTargets.All, weaponData.weaponDataPos, hitPoint,gunType);
    }
    Transform FindClosestHitObject (Ray ray, out Vector3 hitPoint, out Vector3 hitNormal) {

        RaycastHit[] hits = Physics.RaycastAll(ray,weaponData.maxDist);

        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;
        hitNormal = Vector3.zero;
        foreach (RaycastHit hit in hits) {
            if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
                hitNormal = hit.normal;
            }
        }
        return closestHit;
    }

    public void RecoilController() {
        CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, 0.1f);
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, 0.1f);
        CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, weaponData.gunReturnPos, 0.1f);
        CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, 0.1f);
        gradualRecoil = Mathf.Lerp(gradualRecoil, weaponData.returnBulletSpread, weaponData.returnBulletSpreadSpeed);
        gun.transform.localEulerAngles = CurrentRecoil2;
        gun.transform.localPosition = CurrentRecoil4;
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        if (Time.time - lastHitTime < 1f) {
            GUI.DrawTexture(new Rect(Screen.width/2-10,Screen.height/2-10,20,20),hitmarker);
        }
        if (GetComponent<PhotonView>().isMine)
            GUI.Box(new Rect(Screen.width - Screen.width/8, Screen.height - Screen.height / 7, 300, 150),"");
            GUI.Label(new Rect(Screen.width - Screen.width/8, Screen.height - Screen.height / 7, 300, 150), weaponData.currentBullets.ToString(), fontSize);
        GUILayout.EndArea();
    }
}

