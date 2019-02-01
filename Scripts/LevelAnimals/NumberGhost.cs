using UnityEngine;
using System.Collections;

public class NumberGhost : MonoBehaviour {


	public Sprite ghostIcon;
	// Update is called once per frame
	void Update () {
		if (Player.frozen) return;
		float moveSpeed = .6f;
		Vector3 target = Player.inst.transform.position + Vector3.up * 2f;
//		Vector3 playerDir = Vector3.Normalize(transform.position,Player.inst.transform.position + Vector3.up*2);
//		target = Vector3.MoveTowards(transform.position,Player.inst.transform.position + Vector3.up*2,moveSpeed*Time.deltaTime);

		// bob up and down
		float interval = 1f;
		float amplitude = 1f;
		target += Vector3.up * Mathf.Sin(Time.time / interval) * amplitude;
		transform.position = Vector3.Lerp(transform.position,target,moveSpeed * Time.deltaTime);
		if (Vector3.SqrMagnitude(target - transform.position) < 2.5f){
			GameManager.inst.ForceRestartLevel("You probably made a big number, didn't you? You gotta restart now..","A ghost got you!",ghostIcon);
		}
	}

	void OnTriggerEnter(Collider other){
//		Debug.Log("trig;"+other.name);
//		if (other.tag == "Player"){
//			
//		}
	}

	bool dropResource = true; // if player kills ghost somehow, drop a resource.
	void OnDestroy(){
//		base.onde
		if (dropResource) GetComponentInChildren<ResourceDrop>().DropResource();
	}

	public void LevelRestarted(){
		dropResource = false; // don't drop resource on level restart
		if (this.gameObject) Destroy(this.gameObject);
	}

}
