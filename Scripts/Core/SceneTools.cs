using UnityEngine;
using System.Collections;

public class SceneTools : MonoBehaviour {


	string terrainRootName = "Terrain_set";
	public void DropObjectToTerrain(GameObject obj){
		RaycastHit[] hits = Physics.RaycastAll(obj.transform.position,Vector3.down);
		foreach(RaycastHit hit in hits){
			if (hit.collider.transform.root.name == terrainRootName){
				obj.transform.position += Vector3.down * hit.distance;
				return;
			}
		}
	
	
	}



	public void DropNumberWall(GameObject obj){
		// OK...
		
		// Pick all the number cubes out of the selected transform.
		// GetComponentsInChildren<NumberInfo>()
		// foreach their child mesh renderers. They should be ROUNDED CUBES with PREDICTABLE VERTS. Figure out which of the 4 verts are the ones at the BOTTOM SQUARE of the rounded cube, then use only 4 raycasts from these 4 points.
		// Next, iterate through ALL the verts of the rounded cube BUT separate them into 4 numberWallGroups for each of the 4 vertical corners of the rounded cube. Then, drop each GROUP of verts down a certain distance based on how far the hit test for that group's corner was down to the terrain/collider it hit.
		
		
		// 3, 87, 129, 172, 
		//		int[] raycastGroup = new int[4] {191,162,43,53};
		int[] raycastGroup = new int[4] {13,72,102,62};
		int[][] numberWallGroups = new int[4][];
		// order with raycastGRoup matters.
		
		// 191
		numberWallGroups[0] = new int[] {5,159,12,180,150,149,146,98,147,100,144,6,8,15,14,97,99,7,21,101,139,177,96,140,17,138,13,22,142,136,20,192,18,191,10,145,141,3,179,9,143,178,23,4,19,1,0,16,2,11};
		// 162/87
		numberWallGroups[1] = new int[] {127,160,77,196,86,121,166,85,125,162,195,83,76,80,157,188,72,123,79,124,89,187,92,155,78,122,84,163,126,82,91,161,90,81,75,186,95,198,88,199,93,128,94,87,168,197,164,74,73};
		// 43/172
		numberWallGroups[2] = new int[] {107,102,110,169,154,109,106,156,45,176,47,170,153,40,24,38,43,29,28,27,32,31,30,104,172,108,33,200,36,171,39,26,103,105,173,181,167,182,165,42,41,201,174,37,25,44,34,35,158,46,175,183};
		// 53(129)
		numberWallGroups[3] = new int[] {189,114,115,55,135,132,130,113,57,116,69,152,111,133,134,53,67,50,66,65,194,60,68,118,119,131,185,120,58,151,59,49,61,193,62,112,56,51,190,70,129,64,52,148,184,137,63,48,71,117,54};
		float[] rayDistanceGroup = new float[4]; // Will be filled out with 4 raycasthit distances from the 4 corners of each cube.
		
		// UGH. Because we need to find the "shortest raycast distance for the whole group" before modifying verts and positions,
		// We must calculate that first, nearly DOUBLING our execution time to get the absoulte min world distance for the whole friggin group.
		float overallGroupMinDistToGround=Mathf.Infinity;

		foreach(NumberInfo ni in obj.GetComponentsInChildren<NumberInfo>()){
			float minDist = Mathf.Infinity;
			float maxDist = 0;
			Mesh m = ni.childMeshRenderer.GetComponent<MeshFilter>().mesh; // DANGEROUS!
			Mesh m2 = ni.transform.GetChild(2).GetComponent<MeshFilter>().mesh;
			Vector3[] verts = m.vertices;
			int i=0;
			// First, cast 4 rays from the square bottom of the number cube mesh, and save their hit distances.
			for (i=0;i<raycastGroup.Length;i++){
				Vector3 o = ni.transform.TransformPoint(m.vertices[raycastGroup[i]]);
				Ray ray = new Ray(o,Vector3.down);
				//				GameObject db = DebugSphere("origin "+i,o);
				//				db.transform.localScale *= 3;
				
				RaycastHit[] hits = Physics.RaycastAll (ray);
				foreach(RaycastHit hit in hits){
					if (hit.collider.transform.root.name == terrainRootName){
						//						DebugSphere(i+": hit point distance:"+hit.distance,hit.point);
						rayDistanceGroup[i] = hit.distance;
						break;
					}
				}
			}
			
			// Next, given the 4 corners of a cube landing on a slanted surface, what was the MIN distance? 
			float avgDist = 0;
			//			float maxDist = 0;
			for (i=0;i<rayDistanceGroup.Length;i++){
				if (rayDistanceGroup[i] < minDist){
					minDist = rayDistanceGroup[i];
				}
				if (rayDistanceGroup[i] > maxDist){
					maxDist = rayDistanceGroup[i];
				}
				
				avgDist += rayDistanceGroup[i];
			}
//			// commented Debug.Log ("mindist:"+minDist);
			ni.transform.position += Vector3.down * minDist;
			avgDist /= rayDistanceGroup.Length;
			
			//			continue;
			
			float height = ni.transform.localScale.y;
			
			for(i=0;i<numberWallGroups.Length;i++){
				float worldDistToMove = rayDistanceGroup[i] - minDist;
				for (int j=0;j<numberWallGroups[i].Length;j++){
					
					//					float worldDistToGround = rayDistanceGroup[i] - height;
					float localDistToMove = worldDistToMove / height;
					verts[numberWallGroups[i][j]] += Vector3.down * localDistToMove;
				}
			}
			
			m.vertices = verts;
			m.RecalculateBounds();
			m.RecalculateNormals();
			
			// And on the outline mesh (we use two meshes for number cubes to save draw calls or something).
			m2.vertices = verts;
			m2.RecalculateBounds();
			m2.RecalculateNormals();
			
			// after all is said and done.. "nudge" the digits and collider down a tick based on "average raycast dist"
			Vector3 nudgeDist = Vector3.down * (avgDist-minDist);
			ni.GetComponent<BoxCollider>().center += nudgeDist/height; 
			ni.transform.Find("digits").position += nudgeDist;

			
		}
		//		// commented Debug.Log ("overall mindist,  last mindist, last maxdist:"+overallGroupMinDistToGround+","+ minDist+","+maxDist);
	}
	








}
