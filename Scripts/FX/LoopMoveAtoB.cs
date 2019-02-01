using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (LevelBuilderObject))]
public class LoopMoveAtoB : MonoBehaviour {

	public Transform A;
	public Transform B;

	float t = 0.0f;
	float duration = 2.5f;
	float duration2 = 10;

	float moveSpeed = 1f;
	Renderer rend;
	float targetAlpha = 1;
//	Color clearColor;
	UserEditableObject ueo;
	void Start(){
		rend = GetComponent<Renderer>();
		moveSpeed = Vector3.Distance(A.position,B.position);
//		clearColor = rend.material.color;

		ueo = transform.root.GetComponent<UserEditableObject>();
		if (!ueo){
			Destroy(gameObject);
		}
//		StopLooping();
	}
	public void BeginLooping(){
//		Debug.Log("begin loop:"+name);
		t=0;
		looping = true;
		transform.position = A.position;
	}

	public void StopLooping(){
//		// commented Debug.Log("stoploop:"+name);
		looping = false;
		if (rend) rend.material.SetColor("_Color", new Color(rend.material.color.r,rend.material.color.g,rend.material.color.b,0));
	}
	bool looping = false;
	void Update(){
		if (!LevelBuilder.inst.levelBuilderIsShowing) StopLooping(); // level builder fx only
		if (looping){
			t += Time.unscaledDeltaTime;
			if (t < duration){
				transform.position += -transform.up * Time.unscaledDeltaTime * Mathf.Sin(t * Mathf.PI / duration) * moveSpeed/Mathf.PI * 2f;
				targetAlpha = 1;
			} else if (t < duration + duration2){
				targetAlpha = 0;
			} else {
				rend.material.SetColor("_Color",new Color(rend.material.color.r,rend.material.color.g,rend.material.color.b,0));
				transform.position = A.position;
				t = 0;
	//			CheckIfSelected();
			}
			if (Mathf.Abs(rend.material.color.a - targetAlpha) > .01f){
				float colorSpeed = 4f;
				float newAlpha = Mathf.Lerp(rend.material.color.a,targetAlpha,Time.unscaledDeltaTime * colorSpeed);
				rend.material.SetColor("_Color", new Color(rend.material.color.r,rend.material.color.g,rend.material.color.b,newAlpha));
			}
		}
//		// commented Debug.Log("setting to newalpha:"+newAlpha);
	}

//	void CheckIfSelected(){
//		if (!LevelBuilder.inst) {
//			UnsetAsCurrentObject();
//		} else if (LevelBuilder.inst.currentPiece == null){
//			UnsetAsCurrentObject();
//		} else {
//			if (LevelBuilder.inst.currentPiece.GetComponent<LevelBuilderObject>() != this || !LevelBuilder.inst.placedObjectContextMenu.activeSelf){
//				UnsetAsCurrentObject();
//			}
//		}
//	}


//	float timeStamp = 0;
//	float duration = 2.5f;
//	float downtime = 2.5f;
//	float factor = 1;
//	void Update () {
//		if (!LevelBuilder.inst) return;
//		if (Time.realtimeSinceStartup - timeStamp <= duration){
//			factor = (float)Cubic.EaseInOut(Time.realtimeSinceStartup - timeStamp, 0, 1, duration);
//		} else {
//			if (Time.realtimeSinceStartup - timeStamp <= duration * downtime){
//				GetComponent<Renderer>().enabled = false;
//			} else {
//				GetComponent<Renderer>().enabled = true;
//				timeStamp = Time.realtimeSinceStartup;
//			}
//		}
//	}
//
//
//	void FixedUpdate() {
//		if (!LevelBuilder.inst) return;
//		Vector3 changePosition = A.position + factor * (B.position - A.position);
//		transform.position = changePosition;
//	}
}
