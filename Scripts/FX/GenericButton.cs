using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericButton : MonoBehaviour {

	public Transform graphics;
	public Transform pressed;
	public Transform normal;
	public GameObject objToBroadcast;
	public string messageToBroadcast;
	bool moving = false;

	public void ButtonPressed(GameObject o){
		if (!moving && o.GetComponent<Player>()){
			PressMe();

		}
	}

	public void PressMe(){
		moving = true;
		graphics.position = pressed.position;
		AudioManager.inst.PlayHeavyClick(transform.position);
		objToBroadcast.SendMessage(messageToBroadcast,SendMessageOptions.DontRequireReceiver);
	}



	// Update is called once per frame
	void Update () {
		
		if (moving){
			float buttonReturnSpeed = 0.5f;
			graphics.position = Vector3.MoveTowards(graphics.position,normal.position,Time.deltaTime * buttonReturnSpeed);
			float dist = (graphics.position-normal.position).sqrMagnitude;
			if (dist < .01f){
				graphics.position = normal.position;
				moving = false;
			}

		}
		
	}
}
