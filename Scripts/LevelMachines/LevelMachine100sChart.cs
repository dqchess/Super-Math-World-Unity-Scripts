using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMachine100sChart : UserEditableObject {


	public GameObject numberFlowerPrefab;

	#region UserEditable
	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(JsonUtil.dimensionsKey)){
			DeleteFloor();
			SetupFloor(N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey].AsInt,N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey].AsInt);

//			Debug.Log("Matrix floor Set prop:"+N.ToString());
		}
	}

	public override void OnLevelBuilderObjectPlaced(){
//		Debug.Log("Placed");
		DeleteFloor();
		SetupFloor(sizeX,sizeZ);
	}


	public override SimpleJSON.JSONClass GetProperties(){
		// return the properties
		SimpleJSON.JSONClass N = base.GetProperties();// new SimpleJSON.JSONClass();
//		Debug.Log("Matrix floor Get prop:"+N.ToString());
//		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey].AsInt = sizeX;
		N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey].AsInt = sizeZ;
		return N;
	}

	public override GameObject[] GetUIElementsToShow() {
		List<GameObject> ui = new List<GameObject>();
		ui.AddRange(base.GetUIElementsToShow());
		ui.Add(LevelBuilder.inst.POCMheightButton);
		ui.Add(LevelBuilder.inst.POCMmatrixFloorSizeButton);
		return ui.ToArray();
//		return new GameObject[] { LevelBuilder.inst.POCMheightButton, LevelBuilder.inst.POCMmatrixFloorSizeButton };
	}

	#endregion



	void DeleteFloor(){
		if (squares != null){
			foreach(GameObject sq in squares){
				if (sq && sq.gameObject) Destroy(sq.gameObject);
			}
		}
	}
		
	public static string posX = "pX"; // for each floor space posiiton, short because there will be a lot of them
	public static string posZ = "pZ"; 


	float gridScale = 10.0f;
	public int sizeX = 10;
	public int sizeZ = 20;

	GameObject[,] squares;


	bool initialSetupCompleted = false;
	void Start () {
//		Debug.Log("start");
//		squares = new MatrixFloorSquare[sizeX,sizeZ];
//		SetupFloor(sizeX,sizeZ);
		if (LevelBuilder.inst.levelBuilderIsShowing){
			DeleteFloor();
			SetupFloor(sizeX,sizeZ);
		}

	}



	void SetupFloor(int x,int z){
		initialSetupCompleted = true;
//		Debug.Log("setting up lfoor;"+x+","+z);
//		foreach(MatrixFloorSquare mfs in squares){
//			Destroy(mfs.gameObject);
//		}
		squares = new GameObject[x,z];
		sizeX = x;
		sizeZ = z;
		Fraction f = new Fraction(1,1);
		for (int i=0;i<x;i++){
			for (int j=0;j<z; j++){
				GameObject q = (GameObject)Instantiate(numberFlowerPrefab);
//				LevelBuilderObjectManager.inst.AddToPlacedObjects(SceneSerializationType.Instance,q.GetComponent<UserEditableObject>());
				q.transform.rotation = transform.rotation;
				q.transform.position = transform.position + transform.right * j * gridScale + transform.forward * i * gridScale + Vector3.up * .03f; //new Vector3(transform.i*gridScale,0.3f,j*gridScale);
				q.transform.parent = transform;
				q.GetComponentInChildren<NumberInfo>().SetNumber(f);
				squares[i,j] = q;
				f = Fraction.Add(new Fraction(1,1),f);
			}
		}
	}

	float finishedCheckTimer = 0;
	bool finished =  false;
	void Update(){

	}




}
