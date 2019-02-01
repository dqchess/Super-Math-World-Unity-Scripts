using UnityEngine;
using System.Collections;

public class LocationA : Location {


	public override void OnLevelBuilderObjectPlaced(){
		DestroyDuplicates<LocationA>();
	}
}
