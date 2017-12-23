using UnityEngine;
using System.Collections;



public class Jetpack : MonoBehaviour {
    CharacterController cc;
    CharacterMotor cm;
    public float jetpackTime = 5.0f;
    float buttonCooler = 0.5f;
    float buttonCount;
    bool jetpackOn;
    GUIStyle fontSize = new GUIStyle();
    public GameObject leftFire;
    public GameObject rightFire;
   
    
    void Start() {
        cc = (CharacterController)gameObject.GetComponent<CharacterController>();
        cm = (CharacterMotor)gameObject.GetComponent<CharacterMotor>();
        fontSize.fontSize = 60;
        fontSize.normal.textColor = Color.white;
        fontSize.alignment = TextAnchor.LowerCenter;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (buttonCooler > 0 && buttonCount == 1) {
                jetpackOn = !jetpackOn;
            }
            else {
                buttonCooler = 0.5f;
                buttonCount += 1;
            }             
        }

        if (buttonCooler > 0) {
            buttonCooler -= Time.deltaTime;
        }
        else {
            buttonCount = 0;
        }

        if (jetpackOn && jetpackTime > 0) {
            Vector3 velocity = new Vector3(cc.velocity.x, 14, cc.velocity.z);
            cm.SetVelocity(velocity);
            jetpackTime -= Time.deltaTime;
            if (gameObject.GetComponent<PhotonView>().isMine) {
                gameObject.GetComponent<PhotonView>().RPC("JetPackOn", PhotonTargets.All);
            }
        }
        else {
            if (gameObject.GetComponent<PhotonView>().isMine) {
                if (leftFire.activeSelf == true && rightFire.activeSelf == true) {
                    gameObject.GetComponent<PhotonView>().RPC("JetPackOff", PhotonTargets.All);
                    Debug.Log(leftFire.activeSelf);
                }
            }

        }
        if(cc.isGrounded){
            jetpackTime += Time.deltaTime/3f;
        }
        if (jetpackTime <= 0) {
            jetpackOn = false;
        }
        if (jetpackTime > 3.0f) {
            jetpackTime = 3.0f;
        }
    }

    [RPC]
    void JetPackOn() {
        leftFire.SetActive(true);
        rightFire.SetActive(true);
    }
    [RPC]
    void JetPackOff() {
        leftFire.SetActive(false);
        rightFire.SetActive(false);
    }
   
}
