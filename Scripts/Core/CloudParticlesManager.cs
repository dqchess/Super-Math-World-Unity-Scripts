using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CloudParticlesManager : MonoBehaviour {


//	List<MeshFilter> cloudMeshes = new List<MeshFilter>();
//	List<Vector3> cloudParticlePositions = new List<Vector3>();
	Dictionary<MeshFilter,List<int>> cloudGroup = new Dictionary<MeshFilter,List<int>>();

	float timer = 0;
	float timer2 = 0;
	float particleFrequency = 0.1f;
	float particleLife = 20f;
	float particleSize = 20f;
	int particlesPerFrame = 20;

	void Update(){

//		timer -= Time.deltaTime;
//		if (timer < 0){
//			timer = Random.Range(1,2f); // every 0-10 seconds check for more clouds.
//			UpdateCloudMeshes();
//		}

		timer2 -= Time.deltaTime;
		if (timer2 < 0){
			timer2 = Random.Range(1,2f);
			UpdateCloudMeshes();
			foreach(KeyValuePair<MeshFilter,List<int>> kvp  in cloudGroup){
				for (int i=0;i<particlesPerFrame;i++){
					// each list of int in triangles is a list of integers for a single clouds triangles.
					// Get a random triangle from this cloud
					int r = Random.Range(0,kvp.Value.Count/3-3); // note the -3 prevents us from picking a triangle that will hit list's end when we ++
					Vector3 a = kvp.Key.mesh.vertices[kvp.Value[r]];
					Vector3 b = kvp.Key.mesh.vertices[kvp.Value[r+1]];
					Vector3 c = kvp.Key.mesh.vertices[kvp.Value[r+2]];
					var rndA = Random.value;
					var rndB = Random.value;
					var rndC = Random.value;
					Vector3 p = (rndA * a + rndB * b + rndC * c) / (rndA + rndB + rndC);
					p = kvp.Key.transform.TransformPoint(p);
					float ps = Random.Range(particleSize * 0.8f, particleSize * 1.4f);
					float pl = Random.Range(particleLife * 0.8f, particleLife * 1.4f);
					EffectsManager.inst.MakeCloudParticle(p,ps,pl);
				}

			}
//			foreach(Vector3 pos in cloudParticlePositions){
//				if (true){ //Random.Range(0,100) > (1000 - (particleFrequency*1000))){
//					Vector3 p = pos + Random.onUnitSphere * 3f;
//					float ps = Random.Range(particleSize * 0.8f, particleSize * 1.4f);
//					float pl = Random.Range(particleLife * 0.8f, particleLife * 1.4f);
//					EffectsManager.inst.MakeCloudParticle(p,ps,pl);
//				}
//			}
		}

	}

	void UpdateCloudMeshes(){
//		cloudMeshes.Clear();
		cloudGroup.Clear();
		UEO_SimpleObject_Clouds[] clouds = FindObjectsOfType<UEO_SimpleObject_Clouds>();
		for(int i=0;i<clouds.Length;i++){
			List<int> cloudVerts = new List<int>();
			cloudVerts.AddRange(clouds[i].cloudMesh.mesh.triangles);
			cloudGroup.Add(clouds[i].cloudMesh,cloudVerts);

		}
	}
}
