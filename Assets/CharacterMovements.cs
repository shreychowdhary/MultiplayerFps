using UnityEngine;
using System.Collections;

public class CharacterMovements : MonoBehaviour {
    public float sprintTime = 5.0f;
    public float sprintWaitTime = 8.0f;
    public float sprintSpeed = 12.0f;
    float defaultSprint;
    bool sprintWaitOn;
    CharacterMotor cm;
	// Use this for initialization
	void Start () {
        cm = gameObject.GetComponent<CharacterMotor>();
        defaultSprint = cm.movement.maxForwardSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftShift) && sprintTime > 0 && sprintWaitTime < 0) {
            cm.movement.maxForwardSpeed = sprintSpeed;
            sprintTime -= Time.deltaTime;
        }
        else {
            cm.movement.maxForwardSpeed = defaultSprint;
        }
        if (sprintTime < 0.01 && sprintWaitOn) {
            sprintTime = 5.0f;
            sprintWaitOn = false;
        }
        if (sprintTime < 0.01 && sprintWaitTime < 0) {
            sprintWaitTime = 8.0f;
            sprintWaitOn = true;
        }
        sprintWaitTime -= Time.deltaTime;

	}
}
