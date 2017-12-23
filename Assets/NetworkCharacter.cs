using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
    WeaponChange weaponChange;
    bool gotFirstUpdate = false;

	// Use this for initialization
	void Start () {
        weaponChange = gameObject.GetComponent<WeaponChange>();
	}
	
	// Update is called once per frame
	void Update () {
		if( photonView.isMine ) {
			// Do nothing -- the character motor/input/etc... is moving us
		}
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			// This is OUR player. We need to send our actual position to the network.

			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
            stream.SendNext(weaponChange.activeGunIndex);
		}
		else {
			// This is someone else's player. We need to receive their position (as of a few
			// millisecond ago, and update our version of that player.
            
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
            weaponChange.activeGunIndex = (int)stream.ReceiveNext();
            if (gotFirstUpdate == false) {
                transform.position = realPosition;
                transform.rotation = realRotation;
                gotFirstUpdate = true;
            }     
		}

	}
}
