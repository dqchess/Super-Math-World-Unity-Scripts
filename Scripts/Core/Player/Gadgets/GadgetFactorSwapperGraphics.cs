using UnityEngine;
using System.Collections;

public class GadgetFactorSwapperGraphics : MonoBehaviour {


	public Transform leftTarget;
	public Transform rightTarget;
	public Transform leverTarget;
	public Transform lever;

	void Start(){
		leverTarget = leftTarget;
	}

	// Update is called once per frame
	void Update () {
		float speed = 3;
		lever.transform.rotation = Quaternion.Slerp(lever.transform.rotation,leverTarget.transform.rotation,Time.deltaTime * speed);
	}
}
