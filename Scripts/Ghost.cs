using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {

	public GameObject help;
	public CCText activeObj;
	GhostTarget activeTarget;

	void Start(){
		MouseLockCursor.ShowCursor(false,"ghost");
		LevelBuilder.inst.camUI.enabled = false;
		LevelBuilder.inst.camSky.enabled = false;
		LevelBuilder.inst.eventSystem.SetActive(false);
	}
	// Update is called once per frame
	float destroyTimer = 0;

	bool carryingCopyObjecty = false;
	GameObject copyObj = null;
	GameObject moveObj = null;
	Vector3 moveObjOffset = Vector3.zero;
	float moveObjDist = 0;
	float copyObjDist = 0;
	void LateUpdate(){
		if (!LevelBuilder.inst.levelBuilderIsShowing) return;
		
		float fadeSpeed = 3f;
		Color newc= Color.Lerp(lr.material.color,Color.clear,Time.unscaledDeltaTime * fadeSpeed);
		lr.material.SetColor("_Color", newc);
		lineTimer -= Time.unscaledDeltaTime;
		destroyTimer -= Time.unscaledDeltaTime;
		if (lineTimer < 0){
			lr.SetVertexCount(0);

		} else {
			
		}
//		if (Input.GetMouseButtonDown(0)){
//			JsonLevelSaver.inst.LevelBuilderUndo();
//		}
		activeTarget = GetTarget();
		if (activeTarget == null) return;
		UserEditableObject ueo = activeTarget.ueo;
		if (!ueo) {
			moveObj = null;
			copyObj = null;
			return;
		}
		if (Input.GetKey(KeyCode.Backspace) && destroyTimer < 0){
			if (LevelBuilder.inst.levelBuilderIsShowing){
				if (activeTarget.ueo){
					destroyTimer = 0.5f;
					LevelBuilder.inst.DeleteObject(activeTarget.ueo.gameObject);
					DrawTargetLine(Color.red,activeTarget.p);
				}


			}
		}
		float wd = Input.mouseScrollDelta.y; // could be 0.1, 0.5, or 4, 5 agt high speeds
		if (wd != 0){
			wd = wd > 0 ? Mathf.Max(wd,0.5f) : Mathf.Min(-0.5f,wd);
			ueo.transform.position += Vector3.up * Mathf.RoundToInt((wd * 0.5f)*4)*0.5f;
			DrawTargetLine(Color.blue,activeTarget.p);

		}

		float moveFactor = 0.5f;
		if (Input.GetKey(KeyCode.RightBracket)){
			ueo.transform.position += Vector3.up * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKey(KeyCode.LeftBracket)){
			ueo.transform.position += Vector3.down * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKey(KeyCode.F)) {
			ueo.transform.position += Utils.FlattenVector(transform.forward).normalized * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKey(KeyCode.B)) {
			ueo.transform.position += Utils.FlattenVector(-transform.forward).normalized * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKey(KeyCode.R)) {
			ueo.transform.position += Utils.FlattenVector(transform.right).normalized * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKey(KeyCode.L)) {
			ueo.transform.position += Utils.FlattenVector(-transform.right).normalized * moveFactor;
			DrawTargetLine(Color.green,activeTarget.p);
		} else if (Input.GetKeyDown(KeyCode.Equals)){
			
			UEO_ScaleManipulator scale = ueo.GetComponentInChildren<UEO_ScaleManipulator>();
			if (scale){
				scale.transform.localScale += scale.scaleFactor * Vector3.one;
				DrawTargetLine(Color.magenta,activeTarget.p);
			} 
		} else if (Input.GetKeyDown(KeyCode.Minus)){
			
			UEO_ScaleManipulator scale = ueo.GetComponentInChildren<UEO_ScaleManipulator>();
			if (scale){
				scale.transform.localScale -= scale.scaleFactor * Vector3.one;
				DrawTargetLine(Color.magenta,activeTarget.p);
			} 
		}


		if (Input.GetKeyDown(KeyCode.C)){
			carryingCopyObjecty = true;
			copyObj = LevelBuilder.inst.DuplicateObject(ueo.transform.position,ueo.gameObject);
			copyObjDist = Vector3.Distance(copyObj.transform.position,transform.position);
		}

		if (copyObj){
			copyObj.transform.position = transform.position + transform.forward * copyObjDist;
			DrawTargetLine(Color.yellow,copyObj.transform.position);
		}

		if (Input.GetKeyUp(KeyCode.C)){
			copyObj = null;
			carryingCopyObjecty = false;
		}
		int shift = Input.GetKey(KeyCode.LeftShift) ? 9 : 1; // if hold shift, rotate 45 degrees instead of 5
		if (Input.GetMouseButtonDown(1)){
			ueo.transform.Rotate(Vector3.up,5*shift);
			DrawTargetLine(Color.green,ueo.transform.position);
		}
		if (Input.GetMouseButtonDown(2)){
			ueo.transform.Rotate(Vector3.up,-5*shift);
			DrawTargetLine(Color.green,ueo.transform.position);
		}

		if (Input.GetMouseButtonDown(0)){
			// hold right mouse to begin moving
//			Debug.Log("mousdown got:"+moveObj);
			moveObj = ueo.gameObject;
			moveObjOffset = transform.InverseTransformVector(moveObj.transform.position - transform.position);
//			moveObjDist = Vector3.Distance(moveObj.transform.position,transform.position);
		}

		if (moveObj){
			moveObj.transform.position = transform.position + transform.TransformVector(moveObjOffset);
//			moveObj.transform.position = transform.position + transform.forward * moveObjDist + moveObjOffset;
			DrawTargetLine(Color.blue,transform.position + transform.forward * 10f);
		}

		if (Input.GetMouseButtonUp(0)){
			moveObj = null;
		}

	}

	void Update () {
		
		float speed = Input.GetKey(KeyCode.LeftShift) ? 5f : 0.75f;
		speed *= Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt) ? 5 : 1;
		if (Input.GetKeyDown(KeyCode.H)){
			help.SetActive(!help.activeSelf);
		}
		if (Input.GetKey(KeyCode.W)){
			transform.position += transform.forward * speed;
		}
		if (Input.GetKey(KeyCode.A)){
			transform.position += transform.right  * -speed;
		}
		if (Input.GetKey(KeyCode.S)){
			transform.position += transform.forward  * -speed;
		}
		if (Input.GetKey(KeyCode.D)){
			transform.position += transform.right * speed;
		}
		if (Input.GetKey(KeyCode.E)){
			transform.position += Vector3.up * speed;
		}
		if (Input.GetKey(KeyCode.Q)){
			transform.position -= Vector3.up * speed;
		}
		if (Input.GetKeyDown(KeyCode.Minus)){
			Time.timeScale = Mathf.Max(0,Time.timeScale - .1f);
		}
		if (Input.GetKeyDown(KeyCode.Equals)){

			Time.timeScale = Mathf.Min(1,Time.timeScale + .1f);
		}
		if ((Input.GetKey(KeyCode.G) && Input.GetKeyDown(KeyCode.Alpha2)) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Tab)){
			FindObjectOfType<EditorTesting>().EndGhosting();
			Destroy(gameObject);
		}
		if (Input.GetKeyDown(KeyCode.L)){
			foreach(CubeNumberFacePlayer90 cn in FindObjectsOfType<CubeNumberFacePlayer90>()){
				cn.FaceObjectOnce(transform,true);
			}
			foreach(SometimesFacePlayer sf in FindObjectsOfType<SometimesFacePlayer>()){
				sf.FaceObjectOnce(transform);
			}
			foreach(AlwaysFacePlayer af in FindObjectsOfType<AlwaysFacePlayer>()){
				af.transform.LookAt(transform);
			}
		}





	}

	public LineRenderer lr;

	float lineTimer = 0;
	void DrawTargetLine(Color c,Vector3 p){
		lineTimer = 0.5f;
		lr.SetVertexCount(2);
//		lr.SetColors(c,c);
		lr.material.color = c;
		lr.SetPositions(new Vector3[]{transform.position+Vector3.up * -2f,p});
	}

	class GhostTarget {
		public UserEditableObject ueo = null;
		public Vector3 p = Vector3.zero;
		public GhostTarget(UserEditableObject ueo2 = null, Vector3 p2 = default(Vector3)){
			ueo = ueo2;
			p = p2;
		}
	}


	bool GetUserInputThisFrame(){
		return Input.GetKey(KeyCode.C) || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Backspace) 
			|| Input.GetKey(KeyCode.F)
		|| Input.GetKey(KeyCode.B) 
		|| Input.GetKey(KeyCode.R) 
				|| Input.GetKey(KeyCode.L);
	}
	GhostTarget GetTarget() {
//		Debug.Log("getting target:");
		GhostTarget newTarget = new GhostTarget();
		if (moveObj) return new GhostTarget(moveObj.GetComponent<UserEditableObject>(),moveObj.transform.position);
		if (copyObj) return new GhostTarget(copyObj.GetComponent<UserEditableObject>(),copyObj.transform.position);
		if (Utils.IntervalElapsed(0.5f) || GetUserInputThisFrame()){
			float closest = Mathf.Infinity;
	//		RaycastHit hit = new RaycastHit();
			float maxDist = copyObj || moveObj ? 160f : 80f;
			float radius = copyObj || moveObj ? 10f : 2f;
			activeObj.Text = "";
			activeTarget = null;
			foreach(RaycastHit hit in Physics.SphereCastAll(new Ray(transform.position,transform.forward),radius,maxDist)){
				UserEditableObject ueo = hit.collider.transform.root.GetComponent<UserEditableObject>();
				if (ueo && hit.distance < closest) {
					closest = hit.distance;
					newTarget.ueo = ueo;
					newTarget.p = hit.point;
					activeObj.Text = newTarget.ueo.myName;
				}
			}
		}

		return newTarget;
	}
	void OnDestroy(){
		EditorTesting.inst.crudeGhostBlocker.SetActive(false);
		MouseLockCursor.ShowCursor(true,"ghost");
		LevelBuilder.inst.camUI.enabled = true;
		LevelBuilder.inst.camSky.enabled = true;
		LevelBuilder.inst.eventSystem.SetActive(true);
		LevelBuilder.inst.SnapPanToPosition(transform.position);
	}
}
