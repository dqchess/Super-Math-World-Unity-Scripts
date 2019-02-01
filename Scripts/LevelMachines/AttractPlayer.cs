using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	public float pullRange = 42f;
	public float minRange = 10f;
	public float deadRange = 3f;
	public float pullForce = 40000f;
	public float maxForce = 5000f;


	bool ParentBlackHoleHasDestination(){
		return transform.root.GetComponent<LinkLevelPortalPipe>().destinationCode != "";
	}
	void OnTriggerEnter(Collider other){
		if (ParentBlackHoleHasDestination()){
			if (other.GetComponent<Player>() && timeAlive > 5f){
//				canPullPlayer = true;
				// don't pull player for now.. 
			}
			if (other.GetComponent<NumberInfo>() && !pullObjects.Contains(other.gameObject)){
				pullObjects.Add(other.gameObject);
				other.gameObject.AddComponent<UseGravityOnPlayerPickup>();
			}
		}
	}

	bool canPullPlayer = false;
	List<GameObject> pullObjects = new List<GameObject>();
	// Update is called once per frame
	float timeAlive = 0f;
	void Update () {

		timeAlive += Time.deltaTime;
		if (canPullPlayer){
			float playerDist = Vector3.Distance(Player.inst.transform.position,transform.position);
			Vector3 dirFromPlayer = Utils.FlattenVector(transform.position - Player.inst.transform.position).normalized;
		
			if (playerDist < deadRange) {
				FPSInputController.inst.motor.SetMomentum(Vector3.zero);
				return;
			} else {
				float calcForce = CalcForce(playerDist);
				if (playerDist < pullRange){
					FPSInputController.inst.motor.SetMomentum(calcForce * dirFromPlayer);
				}
			}
		}

		if (Utils.IntervalElapsed(2f)){
			pullObjects.Clear();
			foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene(true)){
				if (ni){
					if (Vector3.Distance(ni.transform.position,transform.position) < pullRange ){
						Rigidbody r = ni.GetComponent<Rigidbody>();
						if (r && !r.isKinematic){
							pullObjects.Add(ni.gameObject);
							r.useGravity = false;
						}

					}
				}
			}
		}

		List<GameObject> toRemove = new List<GameObject>();
		foreach(GameObject o in pullObjects){
			if (!o || !o.activeSelf) toRemove.Add(o);
		}
		foreach(GameObject o in toRemove){
			pullObjects.Remove(o);
		}

		foreach(GameObject o in pullObjects){
			if (o){
				Rigidbody r = o.GetComponent<Rigidbody>();
				if (r){
					Vector3 dir = Vector3.Normalize(transform.position-o.transform.position);
					float dist = Vector3.Distance(transform.position,r.transform.position);
					float force = CalcForce(dist) * 10f;
					r.AddForce(dir * force);
					if (dist < 3f){
						Destroy(o);
						AudioManager.inst.PlayCartoonEatLow(transform.position,0.5f,0.3f,0.6f);
					}
				}
			}
		}

	}

	float CalcForce(float dist){
		return Mathf.Min(maxForce,Mathf.Pow(pullRange/Mathf.Max(dist,minRange),2.4f));
	}
}
