using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Text.RegularExpressions;

public enum RotationType {
	X,
	Y,
	Z
}

public class UIValueCommRotation : UIValueComm {


	public InputField xInput;
	public InputField yInput;
	public InputField zInput;

	public RotationType rotationType = RotationType.X;

	void OnEnable(){
//		Debug.Log("enabled!");
		SetRotationTextToCurrentPiece();
	}

	public void SetRotationTextToCurrentPiece(){
		if (LevelBuilder.inst.currentPiece) {
			xInput.text = Mathf.RoundToInt(LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles.x).ToString();
			yInput.text = Mathf.RoundToInt(LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles.y).ToString();
			zInput.text = Mathf.RoundToInt(LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles.z).ToString();
//			Debug.Log("set rot to:"+LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles);
		} else {
//			// commented Debug.Log("no curpiece on rot enable");
		}
	}



	void RotateCurrentPieceEuler(Vector3 r){
		r = Utils.RoundVector3ToInteger(r);
		if (rotationType == RotationType.Y){
//			Debug.Log("rotate curr euler:"+r);
			Quaternion rot = LevelBuilder.inst.currentPiece.transform.rotation;
//			Vector3 newRot = MathUtils.FriendlyEulerAngles(eulerAngles + r);
			rot.eulerAngles += r;
			rot.eulerAngles = Utils.RoundVector3ToInteger(rot.eulerAngles);
			LevelBuilder.inst.currentPiece.transform.rotation = rot;
//			Debug.Log("set:"+LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles.y);
		} else {
			LevelBuilder.inst.currentPiece.transform.Rotate(r,Space.World);
		}
		SetRotationTextToCurrentPiece();
	}

	public void AddEulerX(int e){
		int x = 0;
		int.TryParse(xInput.text, out x);
		x += e;
		x %= 360;
		xInput.text = x.ToString();
		SetRotationBasedOnInputs();
	}
	public void AddEulerY(int e){
		int x = 0;
		int.TryParse(yInput.text, out x);
		x += e;
		x %= 360;
		yInput.text = x.ToString();
		SetRotationBasedOnInputs();
	}
	public void AddEulerZ(int e){
		int x = 0;
		int.TryParse(zInput.text, out x);
		x += e;
		x %= 360;
		zInput.text = x.ToString();
		SetRotationBasedOnInputs();
	}


	void InitRotation(RotationType type, float amount, Vector3 dir){
		rotationType = type;
		rotateDir = dir;
		RotateCurrentPieceEuler(rotateDir);
		SnapToRotation(rotationSnap,rotationType); // rotate first time before repeating happens after repeat delay elapses
		mouseHeldDownTime = Time.time;
		rotating = true;
	}

	bool rotating = false;
	Vector3 rotateDir = Vector3.zero;
	public void RotateLeftPointerDown(int amount=5){
		Vector3 dir = new Vector3(0,-1*amount,0);
		InitRotation(RotationType.Y,amount,dir);
	}
	public void RotateRightPointerDown(int amount=5){
		
		Vector3 dir = new Vector3(0,1*amount,0);
		InitRotation(RotationType.Y,amount,dir);
	}

	float rt2 = Mathf.Sqrt(2);
	public void RotateUpPointerDown(int amount=5){
		
		Vector3 dir = Vector3.zero;
		switch(LevelBuilder.inst.cameraMode){
		case CameraPositionMode.North:
			dir = new Vector3(-1*amount,0,0);
			break;
		case CameraPositionMode.NorthEast:
			dir = new Vector3(-1*amount*rt2,0,1*amount*rt2);
			break;
		case CameraPositionMode.East:
			dir = new Vector3(0,0,1*amount);
			break;
		case CameraPositionMode.SouthEast:
			dir = new Vector3(1*amount*rt2,0,1*amount*rt2);
			break;
		case CameraPositionMode.South:
			dir = new Vector3(1*amount,0,0);
			break;
		case CameraPositionMode.SouthWest:
			dir = new Vector3(1*amount*rt2,0,-1*amount*rt2);
			break;
		case CameraPositionMode.West:
			dir = new Vector3(0,0,-1*amount);
			break;
		case CameraPositionMode.NorthWest:
			dir = new Vector3(-1*amount*rt2,0,-1*amount*rt2);
			break;
		default:break;
		}

		InitRotation(RotationType.X,amount,dir);

	}
	public void RotateDownPointerDown(int amount=5){
		Vector3 dir = Vector3.zero;
		switch(LevelBuilder.inst.cameraMode){
		case CameraPositionMode.North:
			dir = new Vector3(1*amount,0,0);
			break;
		case CameraPositionMode.NorthEast:
			dir = new Vector3(1*amount*rt2,0,-1*amount*rt2);
			break;
		case CameraPositionMode.East:
			dir = new Vector3(0,0,-1*amount);
			break;
		case CameraPositionMode.SouthEast:
			dir = new Vector3(-1*amount*rt2,0,-1*amount*rt2);
			break;
		case CameraPositionMode.South:
			dir = new Vector3(-1*amount,0,0);
			break;
		case CameraPositionMode.SouthWest:
			dir = new Vector3(-1*amount*rt2,0,1*amount*rt2);
			break;
		case CameraPositionMode.West:
			dir = new Vector3(0,0,1*amount);
			break;
		case CameraPositionMode.NorthWest:
			dir = new Vector3(1*amount*rt2,0,1*amount*rt2);
			break;
		default:break;

		
		}

		InitRotation(RotationType.X,amount,dir);
	}


	public void SetRotationBasedOnInputs(){
		Quaternion rot = new Quaternion();
		int x = 0;
		int y = 0;
		int z = 0;
		int.TryParse(xInput.text, out x);
		int.TryParse(yInput.text, out y);
		int.TryParse(zInput.text, out z);
		if (x > 360) {
			x %= 360; 
			xInput.text = x.ToString();
		}
		if (y > 360) {
			y %= 360; 
			yInput.text = y.ToString();
		}
		if (z > 360) {
			z %= 360; 
			zInput.text = z.ToString();
		}
		rot.eulerAngles = new Vector3(x,y,z);
		LevelBuilder.inst.currentPiece.transform.transform.rotation = rot;
//		rot.eulerAngles = new Vector3(int.Parse(xInput.text),int.Parse(yInput.text),int.Parse(zInput.text));
	}

	public void PointerUp(){
		rotating = false;
	}


	int frames = 0;
	int rotationSnap = 5;
//	int degPerRotation = 5;
	int framesToSkip = 5;
	float timeBeforeRepeat = 0.5f;
	float mouseHeldDownTime = 0;
	void FixedUpdate(){
		frames ++;
		if (rotating && Time.time - mouseHeldDownTime > timeBeforeRepeat){
			
			if (frames > framesToSkip){
				frames = 0;
//				Debug.Log("dir;"+rotateDir);
				RotateCurrentPieceEuler(rotateDir);
				SnapToRotation(rotationSnap,rotationType);
			}
		}
	}

	void SnapToRotation(int snap, RotationType type){
//		return;
//		Quaternion rot = LevelBuilder.inst.currentPiece.transform.rotation;
//		Vector3 fe = rot.eulerAngles;
		Vector3 fe = LevelBuilder.inst.currentPiece.transform.localEulerAngles;
		float x = type == RotationType.X ? Mathf.RoundToInt(fe.x/snap)*snap : fe.x;
		float y = type == RotationType.Y ? Mathf.RoundToInt(fe.y/snap)*snap : fe.y;
		float z = type == RotationType.X ? Mathf.RoundToInt(fe.z/snap)*snap : fe.z;
		fe = MathUtils.FriendlyEulerAngles(new Vector3(x,y,z));
//		Debug.Log("xyz type:"+x+","+y+","+z+" .. "+type.ToString());
//		rot.eulerAngles = fe;
		LevelBuilder.inst.currentPiece.transform.localEulerAngles = fe;// transform.rotation = rot;

	}


	int resetRotationIndex = 0;
	public void ResetCurrentPieceRotation(){
//		if (LevelBuilder.inst.currentPiece){
//			Quaternion rot = new Quaternion();
//			switch(resetRotationIndex){
//			case 0: 
//				LevelBuilder.inst.currentPiece.transform.rotation = Quaternion.identity;
//				break;
//				case 1:
//				rot.eulerAngles = new Vector3(270,0,0);
//				LevelBuilder.inst.currentPiece.transform.rotation = rot;
//				break;
//				case 2:
//				rot.eulerAngles = new Vector3(270,0,0);
//			}
//
//		}
		LevelBuilder.inst.currentPiece.transform.rotation = Quaternion.identity;
		SetRotationTextToCurrentPiece();
//		resetRotationIndex++ %= 3;
	}

}
