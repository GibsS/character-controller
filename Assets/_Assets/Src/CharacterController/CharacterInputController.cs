using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour {

    CharacterMotor2D motor;
    
	void Start () {
        motor = GetComponent<CharacterMotor2D>();
	}
	
	void Update () {
        motor.goLeft = Input.GetKey(KeyCode.Q);
        motor.goRight = Input.GetKey(KeyCode.D);
        motor.jump = Input.GetKey(KeyCode.Z);
        motor.drop = Input.GetKey(KeyCode.S);

        motor.holdLadder = Input.GetKey(KeyCode.LeftShift);
        motor.goLeftLadder = Input.GetKey(KeyCode.Q);
        motor.goRightLadder = Input.GetKey(KeyCode.D);
        motor.goUpLadder = Input.GetKey(KeyCode.Z);
        motor.goDownLadder = Input.GetKey(KeyCode.S);
    }
}
