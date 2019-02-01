using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GameObject collectedFX;
	public Transform collectedDestination;

	void OnTriggerEnter(Collider other){
		if (other.tag == "COllectable"){
			// Destroy(other.gameobject)
			// Updatecount(1)
			GameObject fx = (GameObject)Instantiate(collectedFX,other.transform.position,other.transform.rotation);
			PostItemFX postFxScript = fx.GetComponent<PostItemFX>();
			postFxScript.destination = collectedDestination;
		}
	}
}
