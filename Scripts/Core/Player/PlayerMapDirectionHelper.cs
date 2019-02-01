using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class PlayerMapDirectionHelper : MonoBehaviour {

	List<Transform> mapDestinations = new List<Transform>();
//	Dictionary<Transform,string> mapDestinations = new Dictionary<Transform,string>();
	public GameObject markerPrefab;
	float heightOffset = 150;
	List<GameObject> shownMarkers = new List<GameObject>();
	List<GameObject> shownLines = new List<GameObject>();
	public GameObject lineRendererPrefab;
//	bool showing = false;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)){
			Show ();
		}


		if (Input.GetKeyUp(KeyCode.Tab)){
			Hide ();
		}


	}


	void ScaleMarkersOnPlayerDistance(){
		float minScale = 1;
		foreach(GameObject o in shownMarkers){
			float distToPlayer = Vector3.Distance(o.transform.position,transform.position);
//			// commented Debug.Log ("dist to player:"+distToPlayer);
			float distScale = distToPlayer / 1000f;
			float distHeight = distToPlayer / 50f;
			o.transform.localScale = Mathf.Max (minScale,distScale) * Vector3.one;
			o.transform.GetChild (0).localPosition = new Vector3(0,distHeight,0);
		}
	}

	bool showing = false;
	void Show(){
		if (showing) return;
//		// commented Debug.Log ("shoulda shown by now.");
		showing = true;
		mapDestinations.RemoveAll(delegate (Transform o) { return o == null; });
		foreach(Transform t in mapDestinations){
//			// commented Debug.Log ("Showing?");
			DrawMarker(t,t.GetComponent<PlayerMapDirectionDestinationTrigger>().name);
			DrawHelperLine(t);
		}
		ScaleMarkersOnPlayerDistance();
	}


	public void ShowForSeoncds(float s){
		StartCoroutine (ShowForSecondsE(s));
	}

	IEnumerator ShowForSecondsE(float s){
		Show ();
		yield return new WaitForSeconds(s);
		Hide ();
		
	}

	void Hide(){
		showing = false;
		foreach(GameObject o in shownMarkers){
			Destroy (o);
		}
		foreach(GameObject o in shownLines){
			Destroy (o);
		}
		shownMarkers.Clear();
		shownLines.Clear();
	}


	void DrawHelperLine(Transform t){
		GameObject helperLine = (GameObject)Instantiate(lineRendererPrefab);
		helperLine.GetComponent<PlayerMapDirectionHelperLineRendererObject>().Init(t);
		shownLines.Add (helperLine);
	}

	void DrawMarker(Transform t, string s){
		GameObject marker = (GameObject)Instantiate(markerPrefab,t.position + Vector3.up * heightOffset,Quaternion.identity);
		marker.transform.LookAt(Camera.main.transform);
		marker.GetComponentInChildren<CCText>().Text = s;
		shownMarkers.Add(marker);
	}


	public void AddDestination(Transform t){
		mapDestinations.Add (t);
	}




}
