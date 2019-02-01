using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEO_SnapToTerrain : MonoBehaviour {

	public Transform[] snappers; // if snapping to a surface like a terrain there can only ever be THREE of these or fewer!
	public LayerMask snapToLayerMask;
	Ray ray = new Ray();
	float db = 10f; // dist to terrain should get to .1 tolerance
	int maxIter = 1000;
	int iter = 0;
	RaycastHit hit = new RaycastHit();
	float upOffset =5f; //in case one of the T's was already buried under terrain, start with top
	float maxDistToSnap = 25f;

	public void SnapToTerrain(){
		StepOne();
		StepTwo();
		StepThree();
	}
	public void StepOne(){
		// First raycast from A
		// move the object in y direction so that this point touches the terrain.
		ray = new Ray(snappers[0].position + Vector3.up * upOffset,Vector3.down);
		if (Physics.Raycast(ray,out hit,maxDistToSnap+upOffset,snapToLayerMask)){
			transform.position += Vector3.up * (hit.point.y - snappers[0].position.y);
		}
	}
	public void StepTwo(){

		// Second step
		// Rotate object along an axis perpendicular to A--B until B touches terrain
		iter = 0;
		Vector3 terrainPointUnderB = Vector3.zero;
		while(db > .1f && iter < maxIter){
			iter ++;
			ray = new Ray(snappers[1].position + Vector3.up * upOffset,Vector3.down);
			if (Physics.Raycast(ray,out hit,maxDistToSnap+upOffset,snapToLayerMask)){
				terrainPointUnderB = hit.point;
				db = hit.distance;
			}
			Quaternion rotate90 = Quaternion.Euler(0,90,0);
			Vector3 rightAngleDir = rotate90 * Utils.FlattenVector(Vector3.Normalize(terrainPointUnderB - snappers[0].position)); // a "flat" right angle from the line snappers[0] to snappers[1] so we can rotate the object about this axis
			int rotationDirection = 1;
			if (snappers[1].transform.position.y < terrainPointUnderB.y) rotationDirection = -1;
			transform.RotateAround(snappers[0].position,rightAngleDir * rotationDirection,Time.deltaTime);
//			Debug.Log("rotated:"+Time.time);
//			db = Vector3.SqrMagnitude(snappers[1].position-hit.point); // distance 
		}

	}
	public void StepThree() {
		

		// Third and final step, 
		// Calc the angle A between 2 vectors N and M
		// Vector N is from point 0 and to point 2
		// Vector3 M is from point 0 and to the raycast point hitting terrain
		// Rotate the object about point 0 about the axis from point 0 to 1 by the angle A
		// Unforutnaely we don't know if angle should be positive or negative without ugly calculations
		// so we simply do the rotation and chek dist before and after. If dist was farther after we know the rotation was wrong, undo it and do the opposite rotation.
		// lol

		// get ray hit point
		db = 10f;
		Vector3 terrainPointUnderC = Vector3.zero;
		iter = 0;
		ray = new Ray(snappers[2].position + Vector3.up * upOffset,Vector3.down);
		if (Physics.Raycast(ray,out hit,maxDistToSnap+upOffset,snapToLayerMask)){
			terrainPointUnderC = hit.point;
			db = hit.distance;
		}
		Vector3 rotationAxis = (snappers[0].position-snappers[1].position);
		float angle = Vector3.Angle(snappers[2].position-snappers[0].position,terrainPointUnderC-snappers[0].position);
//		transform.RotateAround(snappers[0].position,rotationAxis,angle);
//		Quaternion rot = Quaternion.AngleAxis(angle,snappers[0].position,rotationAxis);
		float beforedist = Vector3.SqrMagnitude(terrainPointUnderC - snappers[2].position);

		transform.RotateAround(snappers[0].position,rotationAxis,angle);
		float afterdist =  Vector3.SqrMagnitude(terrainPointUnderC - snappers[2].position);
		if (afterdist > beforedist) {
			// oops! We moved the wrong way.
			// rotate backwards by double - once to return, once to complete the desired rotation.
			transform.RotateAround(snappers[0].position,rotationAxis,-angle*2f);
		}
		


	}
}
