using UnityEngine;
using System.Collections;

public class LevelBuilderDownLaserVisible : MonoBehaviour {

	LineRenderer lr;

	void Start(){
		lr = GetComponent<LineRenderer>();
	}
	// Update is called once per frame
	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing){
			lr.SetVertexCount(2);
			lr.SetPositions(new Vector3[] { transform.position, transform.position+ -Vector3.up * 100 });
		}
	}
}
