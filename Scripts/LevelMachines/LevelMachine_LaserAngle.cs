using UnityEngine;
using System.Collections;

public class LevelMachine_LaserAngle : MonoBehaviour {


	public int angle;
	public Transform textPivot;
	public Transform laserPivot;
	public Transform arc;
	public CCText degreesText;

	// Alpha cutoff conversion chart
	// 0 --- 1
	// 90 ° --- 0.819f --- offset +.06f
	// 180 ° -- 0.59f -- offset +0.09f
	// 270 ---- .307f -- offset +.06f


	// Use this for initialization
	void Start () {
		SetAngle(angle);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.D)){
//			angle = (angle + 15);
			SetAngle(angle+15);
		}
	}



	void SetAngle(int a){
		a %= 360;
		angle = a;
		laserPivot.transform.localEulerAngles = new Vector3(0,a,0);
		textPivot.transform.localEulerAngles = new Vector3(0,a/2f,0);
		float cutOffValue = 1 - (a / 360f) + .06f;
		arc.gameObject.GetComponent<Renderer>().material.SetFloat("_Cutoff", cutOffValue);
		degreesText.Text = a.ToString() + "°";
	}


}
