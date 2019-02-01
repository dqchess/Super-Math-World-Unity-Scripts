using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_Multiplication_Tables : MonoBehaviour {


	bool grabbing = false;
	public GameObject panelPrefabX;
	public GameObject panelPrefabY;
	public Transform onTriggerEnterSnapToX;
	public Transform onTriggerEnterSnapToY;
	public Transform heldPosX;
	public Transform heldPosY;
	public Transform tableStartX;
	public Transform tableStartY;
	public GameObject prefabXY; //ppppp
	float sizeX = 0;
	float sizeY = 0;
	float panelspacing=2.5f;
	public List<GameObject> panelsX = new List<GameObject>();
	public List<GameObject> panelsY = new List<GameObject>();
	public List<GameObject> panels = new List<GameObject>();
	public List<GameObject> numbers = new List<GameObject>();
	public Vector2 maxSize = new Vector2(10,10);
	public GameObject heldX;
	public GameObject heldY;
	public Transform resultDestination;


	void Start(){

	}

	public void PerformMultiplication(){
		foreach(GameObject o in numbers){
			SMW_GF.inst.MoveObjTo(o,resultDestination.position,3,true,3);
		}
//		int sign=1;
//		if (heldX.GetComponent<NumberInfo>().fraction.numerator * heldY.GetComponent<NumberInfo>().fraction.numerator < 0) sign = -1;
		Fraction resultFrac = new Fraction(heldX.GetComponent<NumberInfo>().fraction.numerator * heldY.GetComponent<NumberInfo>().fraction.numerator,1);
		StartCoroutine (ResultAppearsAfterSeconds(resultFrac));

	}

	IEnumerator ResultAppearsAfterSeconds(Fraction f){
		yield return new WaitForSeconds(2);
//		SMW_GF.inst.GemDrop(resultDestination.position,f.numerator);
		GameObject r  =NumberManager.inst.CreateNumber(f,resultDestination.position);
		r.transform.localScale = Vector3.one * 3;
		ClearPanels();
	}

	IEnumerator PostGrabNumber(GameObject other, Vector3 origin, Vector3 dest, int which){
		other.transform.position = origin;
		SMW_GF.inst.MoveObjTo(other,dest,1,false,3);
		AudioManager.inst.PlayCartoonEat(transform.position,1);
		yield return new WaitForSeconds(.8f);
		AudioManager.inst.PlayPlungerSuck(other.transform.position,.5f);
		if (which==1) StartCoroutine(MakeRow_X(heldX.GetComponent<NumberInfo>().fraction.GetAsFloat()));
		else if (which==2) StartCoroutine(MakeRow_Y(heldY.GetComponent<NumberInfo>().fraction.GetAsFloat()));
	}

	public void OnTriggerEnterX(Collider other){
		if (grabbing) { return; }
		if (!other.GetComponent<NumberInfo>()) { return; }
		if (other.GetComponent<NumberInfo>().fraction.denominator != 1) { return; }
		if (other.GetComponent<NumberInfo>().fraction.numerator > 13) { return; }
		grabbing = true;
//		StopAllCoroutines();
		if (heldX) Destroy (heldX);
		GameObject obj = other.gameObject;
		heldX = obj;
		Clean (obj,2.5f);

		heldPosX.transform.position = tableStartX.position + transform.TransformDirection(new Vector3(obj.GetComponent<NumberInfo>().fraction.numerator+1,0,0) * panelspacing);
		StartCoroutine(PostGrabNumber(obj,onTriggerEnterSnapToX.position,heldPosX.position,1));

	}

	public void OnTriggerEnterY(Collider other){
		if (grabbing) { return; }
		if (!other.GetComponent<NumberInfo>()) { return; }
		if (other.GetComponent<NumberInfo>().fraction.denominator != 1) { return; }
		if (other.GetComponent<NumberInfo>().fraction.numerator > 13) { return; }
		grabbing = true;
		StopAllCoroutines();
		if (heldY) Destroy (heldY);
		GameObject obj = other.gameObject;
		heldY = obj;
		Clean (obj,2.5f);
		heldPosY.transform.position = tableStartY.position + transform.TransformDirection(new Vector3(0,obj.GetComponent<NumberInfo>().fraction.numerator+1,0) * panelspacing);

		StartCoroutine(PostGrabNumber(obj,onTriggerEnterSnapToY.position,heldPosY.position,2));
	}

	void ClearPanels(){
		Destroy(heldX);
		Destroy (heldY);
		foreach(GameObject o in panels){
			Destroy (o);
		}
		panels.Clear();
		foreach(GameObject o in panelsX){
			Destroy (o);
		}
		panelsX.Clear();
		foreach(GameObject o in panelsY){
			Destroy (o);
		}
		panelsY.Clear();
	}

	void MakeXYPanels(){
//		ClearPanels();
//		if (heldX){
//			StartCoroutine(MakeRow_X(heldX.GetComponent<NumberInfo>().fraction.GetAsFloat()));
//		}
//		if (heldY){
//			StartCoroutine(MakeRow_Y(heldY.GetComponent<NumberInfo>().fraction.GetAsFloat()));
//		}
	}

	IEnumerator MakeRow_X(float f){
		foreach(GameObject o in panels){
			Destroy (o);
		}
		panels.Clear();
		foreach(GameObject o in panelsX){
			Destroy (o);
		}
		panelsX.Clear();
		float i;
		yield return new WaitForSeconds(.5f);
		int sign = 1;
		if (f < 0) sign = -1;
		for(i=1;i<Mathf.Abs (f)+1;i++){ // this includes decimal remainder
//			// commented Debug.Log ("making x: "+ i);
			yield return new WaitForSeconds(.15f);
			AudioManager.inst.PlayBubblePop(transform.position,0.5f +i/(f*2));
			MakePanel_X(i*sign);
		}
		StartCoroutine(MakeGridIfComplete());
	}

	IEnumerator MakeGridIfComplete(){
		grabbing = false;
		yield return new WaitForSeconds(1f);
		if (panelsX.Count > 0 && panelsY.Count > 0){
			AudioManager.inst.PlayComboWin();
			yield return new WaitForSeconds(0.5f);
			for (int i=1;i<panelsX.Count+1;i++){
				for (int j=1;j<panelsY.Count+1;j++){
					int sign = 1;
					if (!heldX || !heldY) yield break;
					if (heldX.GetComponent<NumberInfo>().fraction.numerator * heldY.GetComponent<NumberInfo>().fraction.numerator < 0) sign = -1;
					Vector3 pos = transform.TransformDirection(new Vector3(i,j,0) * panelspacing);
					GameObject panel = (GameObject)Instantiate(prefabXY,tableStartX.position + pos,tableStartX.rotation);
					GameObject panelNum = NumberManager.inst.CreateNumber(new Fraction(sign,1),panel.transform.position);
					Clean (panelNum);
					panelNum.transform.parent = panel.transform;
					numbers.Add (panelNum);
					panels.Add(panel);
					AudioManager.inst.PlayBubblePop(transform.position,0.5f +i*j/(panelsY.Count*panelsX.Count*3));
					yield return new WaitForSeconds(.05f);
				}
			}
		}
	}


	IEnumerator MakeRow_Y(float f){
		foreach(GameObject o in panelsY){
			Destroy (o);
		}
		panelsY.Clear();
		foreach(GameObject o in panels){
			Destroy (o);
		}
		panels.Clear();
		float i;
		yield return new WaitForSeconds(.5f);
		int sign = 1;
		if (f < 0) sign = -1;
		for(i=1;i<Mathf.Abs (f)+1;i++){ // this includes decimal remainder
//			// commented Debug.Log ("making y: "+ i);
			yield return new WaitForSeconds(.15f);
			AudioManager.inst.PlayBubblePop(transform.position,0.5f +i/(f*2));
			MakePanel_Y(i*sign);
		}
		StartCoroutine(MakeGridIfComplete());
	}


	void MakePanel_X(float n){
		GameObject newPanel = (GameObject)Instantiate (panelPrefabX);
		int sign=1;
		if (n<0) sign=-1;
		panelsX.Add (newPanel);
		newPanel.transform.parent = tableStartX;
		newPanel.transform.localPosition = -transform.right * n * panelspacing;
		GameObject panelNum = NumberManager.inst.CreateNumber(new Fraction(sign,1),newPanel.transform.position);
		panelNum.transform.parent = newPanel.transform;
		Clean(panelNum);
	}


	void MakePanel_Y(float n){
		GameObject newPanel = (GameObject)Instantiate (panelPrefabY);
		int sign=1;
		if (n<0) sign=-1;
		panelsY.Add (newPanel);
		newPanel.transform.parent = tableStartY;
		newPanel.transform.localPosition = transform.up * n * panelspacing;
		GameObject panelNum = NumberManager.inst.CreateNumber(new Fraction(sign,1),newPanel.transform.position);
		panelNum.transform.parent = newPanel.transform;
		Clean(panelNum);
	}
	
	void Clean(GameObject obj, float s = 1){
		Destroy(obj.GetComponent<Collider>());
		Destroy(obj.GetComponent<Rigidbody>());
		obj.transform.localScale = Vector3.one * s;
	}
}
