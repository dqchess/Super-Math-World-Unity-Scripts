using UnityEngine;
using System.Collections;

public class UEO_ScaleManipulator : MonoBehaviour {

	public Vector3 cubeSize = Vector3.one;
	public static string key = "cubeSizeManipulator"; // legacy, will break existing levels if you change this. Correct key name should be scaleManipulator
	public static string keyX = "keyX";
	public static string keyY = "keyY";
	public static string keyZ = "keyZ";

	public Transform cloneObject;
	public float scaleFactor = 1f;
	public void IncreaseCubeSizeX(int x){
		Vector3 s = transform.localScale;
		transform.localScale = new Vector3(Mathf.Max(1,s.x+x),s.y,s.z);
		if (cloneObject) cloneObject.transform.localScale = transform.localScale;
	}
	public void IncreaseCubeSizeY(int y){
		Vector3 s = transform.localScale;
		transform.localScale = new Vector3(s.x,Mathf.Max(1,s.y+y),s.z);
		if (cloneObject) cloneObject.transform.localScale = transform.localScale;
	}
	public void IncreaseCubeSizeZ(int z){
		Vector3 s = transform.localScale;
		transform.localScale = new Vector3(s.x,s.y,Mathf.Max(1,s.z+z));
		if (cloneObject) cloneObject.transform.localScale = transform.localScale;
	}

	public void UpdateSize(int x, int y, int z){
//		Debug.Log("updating size;"+x+","+y+","+z+": scalefact;"+scaleFactor+", myname:"+name);
		transform.localScale = new Vector3(x,y,z) * scaleFactor;
		if (cloneObject) cloneObject.transform.localScale = transform.localScale;
	}


}
