using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamWallManager : MonoBehaviour {

	// Casts a ray from the player's head to the camera ensuring that nothing will block your view of the player.
	// If camera gets too close to playeer's head so that player blocks view of the scene, then turn player materials transparent.

	float defaultCameraDistance;
	public Transform pivot;
	public LayerMask moveCameraForTheseLayers; // may want to not move camera for transparent, or skycamonly layers
	List<Collider> alwaysIgnore = new List<Collider>(); // keep a cached list of things to always ignore so we don't have to detect their properties each time we raycast into them
	float defaultCameraUp = 0;
	Vector3 startPosLocal;
	public bool debug = false;
	public bool isPlayer = false;
	public float forwardOffset = 0;
	void Start(){
		if (pivot.GetComponentInChildren<Player>()){
			isPlayer = true;
		}
		startPosLocal = transform.localPosition;
		defaultCameraDistance = Vector3.Magnitude(transform.position - pivot.position);
		defaultCameraUp = transform.localPosition.y;
		JsonLevelLoader.inst.onLevelLoadedDelegate += OnLevelLoaded; // clear every level load
		LevelBuilder.inst.LevelBuilderPreviewClicked += OnLevelLoaded; // clear on start as well // these prevent the game from sticking camera in a weird position due to player being insdie an object (todo: should rule out this possibility in code below ..but..)
	}

	void OnLevelLoaded(){
		alwaysIgnore.Clear();
		transform.localPosition = startPosLocal;
	}

	// Update is called once per frame
	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (Player.inst != null){
//			Debug.Log("Work?");
			float headOffset=0;
			Vector3 dir = transform.position - pivot.position; //-transform.forward; //transform.position - Player.inst.pivot.position;
			Ray ray = new Ray(pivot.position,dir);
			RaycastHit[] hits = Physics.SphereCastAll(ray, .2f, defaultCameraDistance, moveCameraForTheseLayers); // todo : add "DontCollideWithPlayer" here?
			float shortestHit = Mathf.Infinity; // as we iterate through all hits we keep track of which one was closest to player that wasn't an excluded collider.
			Vector3 hitPoint; // we'll iterate through all the hits of the raycast and get the hit point we care about.
			//			Debug.Log("1");
			foreach(RaycastHit hit in hits){
//				if (debug) Debug.Log("hit:"+hit.collider.name);
//				Debug.Log("hit:"+hit.collider+", hit dist;"+hit.distance);
				//				if (moveCameraForTheseLayers.GetHashCode hit.collider.layer
				if (alwaysIgnore.Contains(hit.collider)) {
					continue; // unsure if this is ACTUALLY faster
				} 
				NumberInfo ni = hit.collider.GetComponent<NumberInfo>();

				// should we ignore and forever ignore this collider?
				if (
					(hit.collider.gameObject.layer==LayerMask.NameToLayer("Player")
					|| hit.collider.transform.root.GetComponent<PlaceableNPC>()
					|| (hit.collider.isTrigger) 
					|| (ni && ni.myShape != NumberShape.Cube && !ni.GetComponent<Animal>()) 
					|| (!hit.collider.GetComponent<Renderer>() && !hit.collider.GetComponent<Terrain>())
					|| hit.collider.GetComponent<Gadget>()
					)
					&& !hit.collider.GetComponent<CamWallManagerAlwaysDetect>()

				){
					if (!ni || ni.myShape != NumberShape.Cube){
//						Debug.Log("hit:"+hit.collider.name+" and ignoring..");
						alwaysIgnore.Add(hit.collider);
						continue;
					}
				}

				if (!isPlayer && hit.collider is CapsuleCollider && hit.collider.GetComponentInChildren<Vehicle>() && hit.collider.transform.root.GetComponentInChildren<CamWallManager>() == this){
//					if (hit.collider.transform.root.GetComponentInChildren
					alwaysIgnore.Add(hit.collider);
				}


				if (hit.distance < shortestHit){
					shortestHit = hit.distance;
					hitPoint = hit.point;
				}
			}

			// Whew! Now that all that raycasting is over, let's look at the results.
			// If the raycasts are hitting something that would be blocking the camera, we should have a "shortest hit" that is not equal to infinity.
			float lerpSpeed = 2;
			float actual = 0f;
			Vector3 finalPos = transform.position;
			if (shortestHit != Mathf.Infinity){
				lerpSpeed *= 10f; // move faster when going "in" towards the head so view is obstructed less of the time
				float buffer= 0.4f; // we're moving the camera because an object obstructed, but let's move it a tiny bit more to avoid clipping.
				shortestHit -= buffer;
//				Debug.Log("shortest hit not inf.");
				// now, say there was a brick behind the player's head and the raycast lenght was 5 between the pivot and the camera. 
				// The normal distance for the camera to be away from the head is 10.
				// So, we SNAP the camera to this shortesthit position. Not lerp (lerping would cause the camera to temporarily be obstructed.)
				float minDist = 1f; // don't get closer than this.
				actual = Mathf.Max(minDist,Mathf.Min(defaultCameraDistance - 0.1f,shortestHit));
//				float camSpeed =4f;
//				transform.position = Vector3.Lerp(transform.position,Player.inst.pivot.position + dir.normalized * actual,Time.deltaTime * camSpeed);
//				Debug.Log("lerp actual:"+actual);
				Vector3 targetPos = pivot.position + dir.normalized * actual + transform.forward * forwardOffset;
				finalPos = Vector3.Lerp(transform.position,targetPos,Time.deltaTime * lerpSpeed);

//				transform.position = Vector3.MoveTowards(transform.position,Player.inst.pivot.position,camSpeed * Time.deltaTime); //+= Time.deltaTime * camSpeed * (transform.forward - transform.up*2f); //Player.inst.pivot.position + dir.normalized * Mathf.Min(defaultCameraDistance - 0.1f, shortestHit);
			} else {
				
				// There was no shortest hit this frame, yay nothing blocks camera! BUT, let's not SNAP the camera back to its oriignal length. Instead let's lerp it smoothly so the player doesn't experience snapping.
				Vector3 targetCamPosition = pivot.position + dir.normalized * defaultCameraDistance;
				finalPos = Vector3.Lerp(transform.position,targetCamPosition,Time.deltaTime * lerpSpeed);
			}
			// actual varies between defaultcameradistance and 0.
			float camcloserfactor = (actual/defaultCameraDistance); // at no zoom this is 1, at full zoom this approach 0

//			transform.localPosition = new Vector3(transform.localPosition.x,defaultCameraUp - (defaultCameraUp * camcloserfactor),transform.localPosition.z);
//			Debug.Log("actual:"+actual+", camclos;"+camcloserfactor);
			//			Debug.Log("2");
//			if (shortestHit -0.5f < camDist){
//				//				Debug.Log ("shortest: "+shortestHit);
//				float minCamDistFromHead =2f;
//				Vector3 lp = camDir * Mathf.Max(minCamDistFromHead,(headOffset + shortestHit - .5f) / s);
//				transform.localPosition = lp;// - 2.5f); // /camStartPos.magnitude;
//
//			} else {
//				//				Debug.Log ("Not short: "+shortestHit);
//				//				Debug.Log("camdr:"+camDir+",camdst:"+camDist+", s:"+s);
//				transform.localPosition = camDir * camDist / s;
//			}

			transform.position = finalPos;







			// Well, if we're too close to the player now, turn player transparent.




			if (isPlayer){
				if (Vector3.Distance(transform.position,Player.inst.transform.position)<5){
					SetPlayerMaterials(false);
				} else {
					SetPlayerMaterials(true);
				}
			}
		}
	}

	bool playerMaterialsState=false;
	public void SetPlayerMaterials(bool f){
		if (f == playerMaterialsState) return;
		//		// commented Debug.Log("setpl:"+f);
		playerMaterialsState = f;
		if (PlayerCostumeController.inst != null){
			if (f) PlayerCostumeController.inst.SetPlayerOpaque();
			else PlayerCostumeController.inst.SetPlayerTransparent();
		}


	}

	void OnDestroy(){
		JsonLevelLoader.inst.onLevelLoadedDelegate -= OnLevelLoaded; 
		LevelBuilder.inst.LevelBuilderPreviewClicked -= OnLevelLoaded; 
	}
}
