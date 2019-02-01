using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMachine_CubeShapeObject : MonoBehaviour {

	public enum CubeShape {
		Burr,
		CubeFrame,
		Staircase1,
		Staircase2
	}
	public CubeShape cubeShape = CubeShape.Burr;
	public int size = 3;
	List<GameObject> cubes = new List<GameObject>();
	float cubeScale = 4f;
	void Start(){
		switch (cubeShape) {
			case CubeShape.Burr:
			for (int i=0;i<size;i++){
				for (int j=0;j<size;j++){
					for (int k=0;k<size;k++){
						if (i==0 || j==0 || k==0){ // on an edge
							if ((i + j + k)==(size-1)/2+1){ // only the center cube gets printed
								GenCube(i,j,k);
							}
						} else if (i==(size-1)/2+1) {
							
						}

					}
				}
			}
			break;
		default:break;
		}
	}

	void GenCube(int i, int j, int k){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = transform.position + new Vector3(i,j,k);
	}
}
