using UnityEngine;
using System.Collections;

public class Location : UserEditableObject {



	#region UserEditable 



	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		return N;
	}


//	public override bool Exclude () {
//		return true;
//	}

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[]{ LevelBuilder.inst.POCMheightButton };

	}





	/* footpring was: (){
		return 1;
	 */

	public override void OnGameStarted(){
		base.OnGameStarted();

	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
	}
	#endregion




	public override void OnLevelBuilderObjectPlaced(){
//		System.Type T = GetComponent<T>();
		DestroyDuplicates<Location>();
	}

	public virtual void DestroyDuplicates<T> () where T : Location
	{
//		// commented Debug.Log("T:");
		foreach(T t in FindObjectsOfType<T>()){
			if (t == this) continue;
			Destroy(t.gameObject);
		}
//		FindO
//		T asset = ScriptableObject.CreateInstance<T> ();
	}

//	public virtual void DestroyDuplicates(System.Type t){
//		
//	}
}
