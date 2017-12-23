using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {
    public GameObject sniperBulletFXPrefab;
    public GameObject BulletFXPrefab;
	[RPC]
    void BulletFX(Vector3 startPos, Vector3 endPos,int gunType) {
        switch (gunType) { 
            case 0:
                if (sniperBulletFXPrefab != null) {
                    GameObject sniperFX = (GameObject)Instantiate(sniperBulletFXPrefab, startPos, Quaternion.LookRotation(endPos-startPos));
                        LineRenderer lr = sniperFX.transform.Find("LineFX").GetComponent<LineRenderer>();
                        lr.SetPosition(0, startPos);
                        lr.SetPosition(1, endPos);
                }
                else {
                    Debug.LogError("No fxPrefab");
                }
                break;
            default:
                if (BulletFXPrefab != null) {
                    GameObject pistolFX = (GameObject)Instantiate(BulletFXPrefab, startPos, Quaternion.LookRotation(endPos-startPos));
                }
                else {
                    Debug.LogError("No fxPrefab");
                }
                break;

        }
    }

}
