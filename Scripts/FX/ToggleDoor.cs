using UnityEngine;
using System.Collections;

[System.Serializable]
public class Door{
	public Transform obj;
	public Transform open;
	public Transform closed;
	public Transform target;
	
}

public class ToggleDoor : MonoBehaviour {

	public float rotSpeed=2.7f;
	bool gateIsOpen=false;
	bool moving = false;
	[SerializeField] public Door[] doors = new Door[2];
	bool doorIsOpen = false;
	void Start(){


	}

	void Update(){
		if (playerCanActivate){
			if (Input.GetKeyDown(KeyCode.F)){
				if (!moving){
					if (doorIsOpen){
						CloseDoor();
					} else {
						OpenDoor();
					}
				}
			}
		}
		if (moving){
//			// commented Debug.Log ("we're moving.");
			foreach(Door d in doors){
				d.obj.rotation = Quaternion.RotateTowards(d.obj.rotation,d.target.rotation,Time.deltaTime * rotSpeed);
				if (Vector3.Angle(d.obj.forward,d.target.forward) < 2){
					d.obj.rotation = d.target.rotation;
					moving = false;
					AudioManager.inst.PlayClunk(transform.position);
					if (d.target == d.open){
						doorIsOpen = true;
					} else {
						doorIsOpen = false;
					}
				}
			}
		} 
	}


	public void OpenDoor(){
		moving = true;
		AudioManager.inst.PlayDoorOpen(transform.position);
		foreach(Door d in doors){
			d.target = d.open;
		}
	}

	public void CloseDoor(){
		moving = true;
		AudioManager.inst.PlayDoorOpen(transform.position);
		foreach(Door d in doors){
			d.target = d.closed;
		}

		
	}

	bool playerCanActivate = false;
	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Player")){
			playerCanActivate = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag("Player")){
			playerCanActivate = false;
		}
	}
}
