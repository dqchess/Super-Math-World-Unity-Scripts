using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMapDirectionDestinationTrigger : MonoBehaviour {

	public string destinationName;

	void OnTriggerEnter(Collider other){
		if (other.CompareTag("Player")){
			PlayerMapDirectionHelper pm = FindObjectOfType<PlayerMapDirectionHelper>();
			pm.AddDestination(transform);
		}
	}

}
