using UnityEngine;
using System.Collections;

public class PlayerMapWrap : MonoBehaviour {

	public Transform sisterCube;
	public bool moveAxisX;
	public bool moveAxisZ;
//	Material m;
//	void Start(){
//		m = GetComponent<Renderer>().sharedMaterial;
//	}
//
//	void Update(){
//		float xDist = Mathf.Abs(Player.inst.transform.position.x-transform.position.x);
//		float zDist = Mathf.Abs(Player.inst.transform.position.z-transform.position.z);
//		Debug.Log(xDist+","+zDist);
//		// if xdist or zdist is 30 or less, begin fading in
//		float visibleDist = 30f; //
//		float alpha = Mathf.Max(Mathf.Max(0,(visibleDist-xDist)/visibleDist),Mathf.Max(0,(visibleDist-zDist)/visibleDist));
//		Color c = m.GetColor("_TintColor");
//		c = new Color(c.r,c.g,c.b,alpha);
//		m.SetColor("_TintColor",c);
//	}
//
	void OnTriggerEnter(Collider other){
		Vector3 startPos = other.transform.position;

		GameObject moveObj = other.gameObject;
		if (moveObj.transform.root.GetComponentInChildren<Vehicle>()){
			moveObj = moveObj.transform.root.GetComponentInChildren<Vehicle>().gameObject;
		}
//		SMW_GF.inst.GetComponentInParentRecursively(typeof(Vehicle))
//		if (moveObj.GetComponentInParent<Vehicle>()){
//			moveObj = moveObj.transform.parent.gameObject;
//		}
		// Where do I want to end up? Exactly opposite TERRAIN so use terrain size/position (terrain is at origin anyway) with an offset
			
//		// commented Debug.Log("other "+other+" ... enter at:"+other.transform.position);
		Vector3 dir = Vector3.Normalize(transform.position - sisterCube.transform.position);
		float offset = 25f; //moveAxisX ? transform.localScale.x * 2f : transform.localScale.z * 2f;
		float x = moveAxisX ? sisterCube.transform.position.x + dir.x * offset : other.transform.position.x;
		float y = moveObj.transform.position.y;
		float z = moveAxisZ ? sisterCube.transform.position.z + dir.z * offset : other.transform.position.z;
		moveObj.transform.position = new Vector3(x,y,z);
		if (moveObj.GetComponent<Player>()){
			
			WebGLComm.inst.Debug("Moved player:"+startPos+" to "+moveObj.transform.position);
		}
//		// commented Debug.Log("Wrapping! "+moveObj.name+" start/end pos:"+startPos+","+other.transform.position);
//		// commented Debug.Log("new pos:"+new Vector3(x,y,z));
	}
}
