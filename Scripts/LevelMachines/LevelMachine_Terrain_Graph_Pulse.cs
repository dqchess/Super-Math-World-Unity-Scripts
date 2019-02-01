using UnityEngine;
using System.Collections;

public class LevelMachine_Terrain_Graph_Pulse : MonoBehaviour {

	public Terrain terr; // terrain to modify 
	int hmWidth; // heightmap width 
	int hmHeight; // heightmap height
	int posXInTerrain; // position of the game object in terrain width (x axis) 
	int posYInTerrain; // position of the game object in terrain height (z axis)
	int size = 100; // the diameter of terrain portion that will raise under the game object 
	public float[,] oldHeights;
	int offset = 0;

	void Start () {
		SaveHeights ();
//		RestoreHeights ();
//		SaveHeights ();

//		oldHeights = terr.terrainData.GetHeights(posXInTerrain-offset,posYInTerrain-offset,size,size);
		hmWidth = terr.terrainData.heightmapWidth;
		hmHeight = terr.terrainData.heightmapHeight;
		
	}

	public void SaveHeights() {
		terr = MapManager.inst.currentMap.map.GetComponent<Terrain>();
		oldHeights = terr.terrainData.GetHeights(0,0,terr.terrainData.heightmapWidth,terr.terrainData.heightmapHeight);
	}

	public void RestoreHeights(){
		terr = MapManager.inst.currentMap.map.GetComponent<Terrain>();
		terr.terrainData.SetHeights(0,0,oldHeights);
	}

//	int debug = 0;

	int frames = 0;
	void Update () {
		frames--;
		if (frames > 0) return;
		frames = 1;


		// get the normalized position of this game object relative to the terrain
		Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terr.terrainData.size.x;
		coord.y = tempCoord.y / terr.terrainData.size.y;
		coord.z = tempCoord.z / terr.terrainData.size.z;
		// get the position of the terrain heightmap where this game object is
		posXInTerrain = (int) (coord.x * hmWidth); 
		posYInTerrain = (int) (coord.z * hmHeight);
		// we set an offset so that all the raising terrain is under this game object
		offset = size / 2;

		float[,] heights = terr.terrainData.GetHeights(posXInTerrain-offset,posYInTerrain-offset,size,size);
		// we set each sample of the terrain in the size to the desired height
		for (int i=0; i<size; i++){
			for (int j=0; j<size; j++){
				// At an i,j position, how far away was this position from the one at halfway point?
				// subtract the vector i,j from the vector size/2,size/2
				// 

				float radI = size/2f - i;
				float radJ = size/2f - j;
				float pulsateSpeed = 0.9f;
				float amplitude = .025f;
				float distFromCenter = Mathf.Sqrt(Mathf.Pow (radI,2)+Mathf.Pow (radJ,2)) / 6f;

				float modHeight = Mathf.Sin ((Time.time + distFromCenter) * pulsateSpeed) * amplitude;
				heights[i,j] = oldHeights[i,j] + modHeight;
			}
		}
		// set the new height
		terr.terrainData.SetHeights(posXInTerrain-offset,posYInTerrain-offset,heights);
	}

}
