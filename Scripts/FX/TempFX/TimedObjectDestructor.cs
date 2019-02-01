using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour {
	
	// a simple "move" script that is used for moving along Sine waves and possibly platforms.
	public bool autoDestruct = false; // cvn changed from false to true because some "nom nom" texts were not destroying uatomatically.
	public float autoDestructInSeconds = 2;

	//float startTime;
		// Use this for initialization
	void Start () {
		//startTime = Time.time;
		if (autoDestruct)
			StartCoroutine(AutoDestruct(autoDestructInSeconds));
		
	}



	
	public void DestroyNow(float seconds){
		StartCoroutine(DestroyNowE(seconds));
	}
	
	public IEnumerator DestroyNowE(float seconds){

		yield return new WaitForSeconds(seconds);


		if (gameObject) Destroy(gameObject);

	}
	
	public IEnumerator AutoDestruct(float seconds){

		yield return new WaitForSeconds(seconds);

		if (gameObject && autoDestruct) {
			//float secondsElapsed = Time.time - startTime;
//			// commented Debug.Log("Destroyed rocket after "+secondsElapsed+" from AutoDestruct()");

			Destroy(gameObject);

		}
		
	}
		
}