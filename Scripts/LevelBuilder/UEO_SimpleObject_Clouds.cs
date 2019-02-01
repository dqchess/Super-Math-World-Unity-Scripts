using UnityEngine;
using System.Collections;

public class UEO_SimpleObject_Clouds : UEO_SimpleObject {



//	public GameObject particles2;

	public MeshFilter cloudMesh;

	void Start(){
//		GetComponentInChildren<ParticleSystem>().maxParticles = 0;

//		particles2.SetActive(false);
	}

	#region UserEditable 

//	public override GameObject[] GetUIElementsToShow(){
//		return new GameObject[]{ LevelBuilder.inst.POCMheightButton };
//	}

	public override void OnGameStarted(){
		base.OnGameStarted();
//		GetComponentInChildren<CloudParticles>().Init();

//		particles2.SetActive(true);
//		GetComponentInChildren<ParticleSystem>().maxParticles = 400;
	}
	#endregion
}
