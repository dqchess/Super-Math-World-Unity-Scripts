using UnityEngine;
using System.Collections;

public class PostItemFX : MonoBehaviour {


	public Transform destination;

	// Update is called once per frame
	void Update () {
		if (destination){
			float speed = 4;
//			transform.position = Vector3.Lerp(transform.position,destination.position,Time.deltaTime * speed); // lerping moves more slowly as it gets closer
			transform.position = Vector3.MoveTowards(transform.position,destination.position,Time.deltaTime * speed); // MoveTowards moves at a constant rate always
			float rotateSpeed = 1;
			transform.Rotate(Vector3.up,rotateSpeed * Time.deltaTime);
			if (Vector3.Distance(transform.position,destination.position) < 1) {
				Destroy(gameObject);
			}
		}
	}
}
