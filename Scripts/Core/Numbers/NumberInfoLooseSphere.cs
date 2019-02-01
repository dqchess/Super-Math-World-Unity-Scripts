using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberInfoLooseSphere : NumberInfo {

	public override void OnLevelBuilderObjectPlaced(){
		base.OnLevelBuilderObjectPlaced();
		transform.localScale = Vector3.one * 2f;
		SetProperties(GetProperties()); // awkward way to initate something dont ya think
	}
}
