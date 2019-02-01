using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum NudgeDirection {
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest
}
public class UIValueComm_MoveNudge : MonoBehaviour {
// local, not global, compass directions (because cam may be facing a diff dir than north for player.)
//	public void Nudge(NudgeDirection nd){
	public void PointerUp(){
		nudging = false;

	}
	bool nudging = false;
	Vector3 dir;
	float nudgeTimer = 0;
	float repeatDelay = 0.25f;
	float repeatInterval = 0.01f;
	float repeatIntervaldynamic = 0;
	float nudgeAmountWhileRepeating = 0.2f;
	float timeUntilRepeatBig = 2f;
	void Update(){
		if (nudging){
			nudgeTimer += Time.deltaTime;
			if (nudgeTimer > repeatDelay){
				repeatIntervaldynamic -= Time.deltaTime;
				if (LevelBuilder.inst.currentPiece && repeatIntervaldynamic < 0){
					if (nudgeTimer > timeUntilRepeatBig){
						NudgeCurrentPiece(5f);
						repeatIntervaldynamic = 0.2f;
					} else {
						NudgeCurrentPiece(nudgeAmountWhileRepeating);
						repeatIntervaldynamic = repeatInterval;
					}
				}
			}
		}
	}

	bool nudgeDebug = false;
	public void BeginNudging(int nd) {
		nudging=true;
		nudgeTimer = 0;
		repeatIntervaldynamic = 0;
		// edit: NudgeDirection is not a selectable thing in the drop down for Unity's limited UI system
		// So instead, we will pass it an integer. Can you deal with INTEGER values Unity? I realize ENUMS are too much for you
		float x=0;
		float z=0;
		float r2 = Mathf.Sqrt(2);
		switch(nd){
		case 0:	
//		case NudgeDirection.North:
			x = -1;
			z = -1;
			break;
		case 1:
//		case NudgeDirection.NorthEast:
			x = -r2;
			z = 0;
			break;
		case 2:
		//		case Nudg}eDirection.East:
			x = -1;
			z = 1;
			break;
		case 3:	
//		case NudgeDirection.SouthEast:
			x = 0;
			z = r2;
			break;
		case 4:	
//		case NudgeDirection.South:
			x = 1;
			z = 1;
			break;
		case 5:	
//		case NudgeDirection.SouthWest:
			x = r2;
			z = 0;
			break;
		case 6:	
//		case NudgeDirection.West:
			x = 1;
			z = -1;
			break;
//		case NudgeDirection.NorthWest:
		case 7:	
			x = 0;
			z = -r2;
			break;

		default:break;
		}
		if (nudgeDebug) Debug.Log("init vec:"+dir);
		dir = new Vector3(x,0,z); // This direction will nudge the object NORTH if cam sky is already facing North. Therefore, we transpose this value depending on levelbuilder.cameramode
		switch(LevelBuilder.inst.cameraMode){
		case CameraPositionMode.North:
			// no change
			// This direction will nudge the object NORTH if cam sky is already facing North. 
			// Therefore, we don't modify dir, but we will transpose this value depending on levelbuilder.cameramode for other cases
//			dir = new Vector2(x,z);
			break;
		case CameraPositionMode.NorthEast:
			dir = Quaternion.AngleAxis(45,Vector3.up) * dir;
			break;
		case CameraPositionMode.East:
			dir = Quaternion.AngleAxis(90,Vector3.up) * dir;
			break;
		case CameraPositionMode.SouthEast:
			dir = Quaternion.AngleAxis(135,Vector3.up) * dir;
			break;
		case CameraPositionMode.South:
			dir = Quaternion.AngleAxis(180,Vector3.up) * dir;
			break;
		case CameraPositionMode.SouthWest:
			dir = Quaternion.AngleAxis(225,Vector3.up) * dir;
			break;
		case CameraPositionMode.West:
			dir = Quaternion.AngleAxis(270,Vector3.up) * dir;
			break;
		case CameraPositionMode.NorthWest:
			dir = Quaternion.AngleAxis(315,Vector3.up) * dir;
			break;
		default:break;
//				vector = Quaternion.Euler(0, -45, 0) * vector;
		}
		if (nudgeDebug) Debug.Log("result vec for"+LevelBuilder.inst.cameraMode+";"+dir);
		if (LevelBuilder.inst.currentPiece) {
			NudgeCurrentPiece();
		}
	
	}

	void NudgeCurrentPiece(float nudgeAmount = .1f){
		LevelBuilder.inst.currentPiece.transform.position += dir * nudgeAmount; //new Vector3(dir.x,0,dir.y);
		LevelBuilder.inst.SnapPanToCurrentObject();
	}
}
