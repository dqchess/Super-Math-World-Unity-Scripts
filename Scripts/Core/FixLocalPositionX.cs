using UnityEngine;
using System.Collections;

public class FixLocalPositionX : MonoBehaviour {


	public Transform toFollow;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = toFollow.transform.position;
//		float newX = Mathf.Lerp(transform.localPosition.x,
		transform.localPosition = new Vector3(0,transform.localPosition.y,transform.localPosition.z);
	}
}
