using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateVsMove : MonoBehaviour {


	public List<GameObject> pool = new List<GameObject>();
	public GameObject prefabO;
	public List<GameObject> objectsInUse = new List<GameObject>();

	void Update(){
		// four opreations
		// create
		// destroy
		// disable
		// enable / move
		if (Input.GetKeyDown(KeyCode.Q)){
			CreateObjects(100);
		} else if (Input.GetKeyDown(KeyCode.W)){
			DestroyObjects();
		} else if (Input.GetKeyDown(KeyCode.E)){
			MoveObjectsToPool();
		} else if (Input.GetKeyDown(KeyCode.R)){
			CreateFromPool(100);
		} else if (Input.GetKeyDown(KeyCode.T)){
			Debug.ClearDeveloperConsole();
			Debug.Log("objs in use:"+objectsInUse.Count+", pool:"+pool.Count);
		}
	}


	void CreateObjects(int count){
		int c= 0;

		for (int i=0;i<count;i++){
			Vector3 p = Pos(i);
			GameObject n = (GameObject) Instantiate(prefabO,p,Quaternion.identity);
//			n.SetActive(false);
			objectsInUse.Add(n);
			c++;
		}
		Debug.Log("Created "+c+" objects");
	}

	void CreateFromPool(int count){
		int c = 0;
		for(int i=0;i<count;i++){
			GameObject n = pool[0];
			pool.RemoveAt(0);
			n.transform.position = Pos(i);
			n.SetActive(true);
			objectsInUse.Add(n);
			c++;
		}
		Debug.Log("Created "+c+" objects from pool");
	}

	void DestroyObjects(){
		int c = 0;
		foreach(GameObject o in objectsInUse){
			Destroy(o);
			c++;
		}
		objectsInUse.Clear();
		Debug.Log("Destroyed "+c+" objects");
	}

	void MoveObjectsToPool(){
		int c = 0;
		foreach(GameObject o in objectsInUse){
			o.SetActive(false);
			pool.Add(o);
			c++;
		}
		objectsInUse.Clear();
		Debug.Log("Moved "+c+" objects to pool");
	}

	Vector3 Pos(int i){
		float spacing = 3f;
		return transform.position + transform.right * i * spacing;
	}
}
