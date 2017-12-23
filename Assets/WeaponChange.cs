using UnityEngine;
using System.Collections;

public class WeaponChange : MonoBehaviour {
    public GameObject[] guns;
    public GameObject[] outsideGuns;
    GameObject[] avaliableGuns = new GameObject[2];
    GameObject[] avaliableOutsideGuns = new GameObject[2];
    GameObject activeGun;
    GameObject previousGun;
    GameObject activeOutsideGun;
    GameObject previousOutsideGun;
    public bool changingWeapons;
    public int activeGunIndex;
    WeaponData weaponData;
    float changeTime;
	// Use this for initialization
	void Start () {         
        if (!PlayerPrefs.HasKey("ClassGun")) {
            PlayerPrefs.SetInt("ClassGun", 0);
        }
        if (gameObject.GetComponent<PhotonView>().isMine) {
            guns[PlayerPrefs.GetInt("ClassGun")].SetActive(true);
        }
        avaliableGuns[0] = guns[PlayerPrefs.GetInt("ClassGun")];
        avaliableGuns[1] = guns[1];
        activeGun = avaliableGuns[0];
        Debug.Log(System.Array.IndexOf(avaliableGuns, activeGun));
        if (!gameObject.GetComponent<PhotonView>().isMine) {
            outsideGuns[PlayerPrefs.GetInt("ClassGun")].SetActive(true);
        }
        avaliableOutsideGuns[0] = outsideGuns[PlayerPrefs.GetInt("ClassGun")];
        avaliableOutsideGuns[1] = outsideGuns[1];
        activeOutsideGun = avaliableOutsideGuns[0];
        weaponData = gameObject.GetComponentInChildren<WeaponData>();
        changeTime = weaponData.changeTime;
	}
	
	// Update is called once per frame
	void Update () {
	    string input = Input.inputString;
        if (gameObject.GetComponent<PhotonView>().isMine) {
            switch (input) {
                case "1":
                    activeGunIndex = 0;
                    break;
                case "2":
                    activeGunIndex = 1;
                    break;
            }
        }

        
        if (gameObject.GetComponent<PhotonView>().isMine) {

            if (avaliableGuns[activeGunIndex] != activeGun) {
                weaponData = gameObject.GetComponentInChildren<WeaponData>();
                changeTime = weaponData.changeTime;
                StartCoroutine("ChangingWeapons");
                previousGun = activeGun;
                activeGun = avaliableGuns[activeGunIndex];
                previousGun.SetActive(false);
                activeGun.SetActive(true);                
            }        
        }
        if (!gameObject.GetComponent<PhotonView>().isMine) {
            if (avaliableOutsideGuns[activeGunIndex] != activeOutsideGun) {
                previousOutsideGun = activeOutsideGun;
                activeOutsideGun = avaliableOutsideGuns[activeGunIndex];
                previousOutsideGun.SetActive(false);
                activeOutsideGun.SetActive(true);               
            }
        }
	}

    IEnumerator ChangingWeapons() {
        Debug.Log("Sda");
        changingWeapons = true;
        yield return new WaitForSeconds(changeTime);
        changingWeapons = false;
    }
}
