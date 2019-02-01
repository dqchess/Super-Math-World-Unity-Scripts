using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public bool exemptPlayerFrozen=false;
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public bool invertY = false;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public float rotationY = 0F;

#if UNITY_IOS && !UNITY_EDITOR
	// nothing
#else
	void Update ()
	{
		if (Player.frozen && !exemptPlayerFrozen || PauseMenu.paused){
			return;
		}

		if (Time.timeScale == 0 && !exemptPlayerFrozen) return; // ahh...hmm..really

		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.mouseMovedDistance,Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")));


		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX){
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		} else {
			int invert = invertY ? -1 : 1;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY * invert;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
#endif	

}