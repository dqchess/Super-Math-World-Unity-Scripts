using UnityEngine;
using System.Collections;

public enum CameraMode {
	Normal,
	Vehicle

}

public class PlayerCamera : MonoBehaviour {

	public CameraMode mode = CameraMode.Normal;
	public Transform cameraTarget;
	public Transform playerCameraTarget;
	public GameObject[] mainCameras;
	public static PlayerCamera inst;
	Vector3 startingPosition;
	Quaternion startingRot;
	public void ResetPositionToStart(){
		transform.localPosition = startingPosition;
		transform.localRotation = startingRot;
	}
	void Start(){
		inst = this;
		SetCameraMode(CameraMode.Normal,null);
		startingPosition = transform.localPosition;
		startingRot = transform.localRotation;
	}
	public void SetCameraMode(CameraMode m, Vehicle v){
		mode = m;
		switch(m){
		case CameraMode.Normal:
			cameraTarget = playerCameraTarget;

//			if (v) v.vehicleCamera.gameObject.SetActive(false);
			foreach(GameObject c in mainCameras){
//				// commented Debug.Log("t");
				c.GetComponent<Camera>().fieldOfView = GameConfig.mainCameraFieldOfView;
			}
//			cameraTarget = playerCameraTarget;
			break;
		case CameraMode.Vehicle:
			if (v){
				cameraTarget = v.vehicleCamera.transform;
			} else {
				// commented Debug.Log("no vehicle for camera swap");
			}
//			if (v) v.vehicleCamera.gameObject.SetActive(true);
//			// commented Debug.Log("eh? len:"+mainCameras.Length);
//			foreach(GameObject c in mainCameras){
////				// commented Debug.Log("f");
//				c.SetActive(false);
//			}
//			cameraTarget = v.camTransform;
			break;
		default:break;
		}

	}

	void LateUpdate(){
		transform.position = cameraTarget.position;
		Quaternion rot = cameraTarget.rotation;
		rot.eulerAngles = new Vector3(rot.eulerAngles.x,rot.eulerAngles.y,0);
		transform.rotation = rot;
//		transform.rotation = cameraTarget.rotation;
	}
}
