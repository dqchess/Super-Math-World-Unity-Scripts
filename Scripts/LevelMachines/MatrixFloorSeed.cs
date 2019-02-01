using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatrixFloorSeed : UserEditableObject, IMyUpdateable, IMyPickupable {


	// transparent sphere with x,y cubes inside.


	// There could be multiple matrix floors, so the seed will look for all of them and keep an index
	LevelMachineMatrixFloor activeFloor;

	List<MatrixFloorSquare> floorSquares = new List<MatrixFloorSquare>(); // keep a list of the squares we would activate if succesfully planted.
	bool plantable = false; 
	public int sizeX;
	public int sizeZ;
	public NumberInfo xAmmo;
	public NumberInfo zAmmo; // for player visible x by z numbers ("ears" on the seed)
	public Color seedColor;
	public int flowerPrefabIndex; // will "grow" into a little shape on the end of a stem.
	bool equipped = false;

	void SetEquipped(bool f){
		equipped  = f;
		if (!f) {
//			foreach(LevelMachineMatrixFloor mf in floors){
			if (activeFloor) activeFloor.SeedUnequipped();
//			}
		}
	}

	void Start(){

		Inventory.inst.beltSpaceSelectedDelegate += SelectedBeltSpace;
		foreach(NumberInfoAmmo nam in GetComponentsInChildren<NumberInfoAmmo>()){
			nam.InitSinGrowAttributes(.25f,true);
		}
//		Debug.Log("added delegate.");
	}

	public void SetColor(Color c){
		seedColor = c;
		GetComponentInChildren<Renderer>().material.color = c;
		GetComponent<PickUppableObject>().inventoryIconColor = c;
	}

	bool throwDelegateAttached = false;
	void SelectedBeltSpace(Slot selectedSlot){
		GameObject o = selectedSlot.GetItem3D();
		if (o) {
			SetEquipped(gameObject == o);
			if (equipped) {
				AttachThrowDelegate();

			} else {
				DetachThrowDelegate();
			}
			//			Debug.Log("equip, with o:"+o.name);
		} else {
			SetEquipped(false);
		}
		//		else Debug.Log("equipped empty");
	}

	void DetachThrowDelegate(){
		if (throwDelegateAttached){
			throwDelegateAttached = false;
			PlayerGadgetController.inst.gadgetThrow.onPlayerThrow -= OnPlayerThrow;

		}
	}
	void AttachThrowDelegate(){
		if (!throwDelegateAttached){
			throwDelegateAttached = true;
			PlayerGadgetController.inst.gadgetThrow.onPlayerThrow += OnPlayerThrow;
		}
	}



	public override void OnDestroy(){
		base.OnDestroy();
		Inventory.inst.beltSpaceSelectedDelegate -= SelectedBeltSpace;
	}

	void OnPlayerThrow(){}
	void OnPlayerThrow(GameObject o){
		if (GetComponent<Rigidbody>()){
			GetComponent<Rigidbody>().useGravity = true;
		}
		DetachThrowDelegate();

//		Debug.Log(" THROW plantable;"+plantable+",florsqct:"+floorSquares.Count);
//		Debug.Log("plantable:"+plantable);
		if (plantable && floorSquares.Count > 0){ 
			// if floorsquares count is zero we aren't over the floor
			// if plantable is false we are over the floor, but, can't plant.
			gameObject.SetActive(false);
			foreach(MatrixFloorSquare mfs in floorSquares){
//				mfs.GetComponent<Renderer>().material.color = color;
				mfs.Plant(this.flowerPrefabIndex,seedColor); // planted = true;

			}
		} else {
//			Debug.Log("failplant :"+floorSquares.Count);
		}
		SetEquipped(false);
	}

	public void Update(){
		// Unfortunately, seeds are DEACTIVATED when I pick them up, and update is not called.
		// So we make them a gadget esque thing that calls update even on inactive objects.
//		Debug.Log("updateing");
		plantable = false;
		if (equipped){
			activeFloor = GetActiveFloor();
			if (activeFloor){
				
				floorSquares = activeFloor.GetPanels(this);
				plantable = activeFloor.seedSizeMatchesFloorSquares;
				
				// Allow player to right-click with seed selected to flip its z and z
			}
		}
		if (Input.GetMouseButtonDown(1)){
			FlipXZ();
			GadgetThrow.inst.UpdateAmmoGraphics(true);
		}
		foreach(LevelMachineMatrixFloor mf in previousFloors){
			if (activeFloor != mf){
				mf.SeedUnequipped();
			}
		}
//		Debug.Log(" UPDS plantable;"+plantable+",florsqct:"+floorSquares.Count);
	}


	void FlipXZ(){
		int temp = sizeX;
		SetSizeX(sizeZ);
		SetSizeZ(temp);

//		sizeX = sizeZ;
//		sizeZ = temp;
	}

	public void SetSizeX(int x){
		sizeX = x;
		xAmmo.SetNumber(new Fraction(x,1),true);
	}

	public void SetSizeZ(int z){
		sizeZ = z;
		zAmmo.SetNumber(new Fraction(z,1),true);
	}

	List<LevelMachineMatrixFloor> previousFloors = new List<LevelMachineMatrixFloor>();

	public LevelMachineMatrixFloor GetActiveFloor(){
		LevelMachineMatrixFloor[] mflrs = FindObjectsOfType<LevelMachineMatrixFloor>();
		LevelMachineMatrixFloor closest = null;
		float dist = Mathf.Infinity;
			
		foreach(LevelMachineMatrixFloor mf in mflrs){
			float diff = Vector3.SqrMagnitude(mf.transform.position + new Vector3(mf.sizeX/2f*mf.gridScale,0,mf.sizeZ/2f*mf.gridScale)-Player.inst.transform.position);
			if (diff < dist){
				dist = diff;
				closest = mf;
			}
		}
		return closest;
	}
		

	public void OnPlayerPickup(){
		string s = "You got a "+sizeX+" x "+sizeZ+" seed! Right click to flip.";
		if (sizeX == sizeZ) s = "You got a seed of size "+sizeX+" x "+sizeZ;
		PlayerNowMessage.inst.Display(s,Player.inst.transform.position);
	}

}
