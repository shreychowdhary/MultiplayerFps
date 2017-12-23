using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public float hitPoints = 100f;
    float _currentHitPoints;
    GUIStyle fontSize;
    public AudioClip grunt;

    public float CurrentHitPoints {
        get { return _currentHitPoints; }
        set { _currentHitPoints = value; }
    }
    // Use this for initialization
    void Start() {
        _currentHitPoints = hitPoints;
        fontSize = new GUIStyle();
        fontSize.fontSize = 40;
        fontSize.normal.textColor = Color.white;
    }

    void Update() {
        if (gameObject.transform.position.y < -10) {

            Die(gameObject.GetComponent<PhotonView>().owner.ID);
        }
    }

    // Update is called once per frame
    [RPC]
    public void TakeDamage(float damage, int killer) {
        AudioSource.PlayClipAtPoint(grunt, gameObject.transform.position);
        _currentHitPoints -= damage;
        if (_currentHitPoints <= 0) {


            Die(gameObject.GetComponent<PhotonView>().owner.ID,killer);
        }
    }

    /*void OnGUI() {
        if (GetComponent<PhotonView>().isMine && gameObject.tag == "Player") {
            if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 40), "Suicide!")) {
                Die();
            }
        }
    }*/

    void Die(int dead, int killer = -1) {
        PhotonPlayer deadPlayer = PhotonPlayer.Find(dead);        
        string deadName = deadPlayer.name;
        string m;
        if (killer == -1) {            
            m = deadName + " suicided";
        }
        else {
            PhotonPlayer killerPlayer = PhotonPlayer.Find(killer);
            string killerName = killerPlayer.name;
            m = killerName + " wrecked " + deadName;
        }
        PhotonView pV = GetComponent<PhotonView>();
        if (pV.instantiationId == 0) {
            Destroy(gameObject);
        }
        else {
            if (pV.isMine) {
                if (gameObject.tag == "Player") {
                    NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
                    nm.standbyCamera.SetActive(true);
                    nm.respawnTimer = 3f;
                    nm.AddChatMessage(m);
                    nm.ScoreBoard(dead, killer);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }


 
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        if (gameObject.GetComponent<PhotonView>().isMine) {
            GUI.Box(new Rect(0, Screen.height - Screen.height / 7, Screen.width / 8 + 70, 150), "");
            GUI.Label(new Rect(Screen.width / 8, Screen.height - Screen.height / 7, 300, 150), _currentHitPoints.ToString(), fontSize);
        }
        GUILayout.EndArea();
    } 
}
