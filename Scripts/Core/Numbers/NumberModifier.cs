using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class NumberModifier : UserEditableObject
{
//	public virtual ListType GetListType() { return ListType.Addition; }

	#region UserEditable 



	public override SimpleJSON.JSONClass GetProperties(){ 
		//		Dictionary<string,string> properties = new Dictionary<string,string>();
		SimpleJSON.JSONClass N = base.GetProperties();
		return N;
	}


//	public override bool Exclude () {
//		return true;
//	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> elements = new List<GameObject>();
		elements.AddRange(base.GetUIElementsToShow());
		elements.AddRange(new GameObject[] {
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMFractionButton,
			LevelBuilder.inst.POCMheightButton
		});


		return elements.ToArray();


	}





	/* footpring was: (){
		return 3;
	 */

	public override void OnGameStarted(){
		base.OnGameStarted();
		
	}



	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
//		// commented Debug.Log("props:"+props);
//		SimpleJSON.JSONNode n = SimpleJSON.JSON.Parse(props);


	}
	#endregion





	public delegate Fraction ModifyOperation(Fraction frac);
	
	virtual public  void Start() {
		
	}
	
	public abstract string GetEquation(Fraction original);
	public abstract Fraction GetModifiedFraction(Fraction original);
	
	public void ModifyNumber(NumberInfo ni)
	{
//		// commented Debug.Log ("modiifyingL:"+ni+"from "+name);
		Fraction original = ni.fraction;
		if (GetModifiedFraction(original).numerator == 0){
			Destroy (ni.gameObject);
		}
		Fraction f = GetModifiedFraction(original);
//		// commented Debug.Log("orig;"+ni.fraction+", for hoop:"+GetComponent<NumberHoopMultiply>().frac+", getmod;"+f);
		ni.SetNumber(f);
		PostModifyNumber(original, ni);
	}
	


	public virtual void PostModifyNumber(Fraction original, NumberInfo ni) {

		SMW_FX.CreateSmallPurpleExplosion(ni.transform.position); //,1.5f,.4f);	

		if (ni.fraction.numerator != 0){
			ni.SetColor();
		}
		if (ni.fraction.numerator == 0){
			Destroy (ni.gameObject);
			SMW_FX.CreateTextEffect(ni.transform.position,"zero");
			SMW_FX.CreateShards(ni.transform.position);
		}
	}


	public void CleanComponentsFromDyingNumber(GameObject dyingNumber, System.Type[] ex = null){
//		// commented Debug.Log("dying?"+dyingNumber);
		if (dyingNumber.GetComponent<FixedJoint>()){
			Destroy(dyingNumber.GetComponent<FixedJoint>()); // must do this BEFORE the rigidbody or get error
		}
		List<System.Type> exclude = new List<System.Type>();
		if (ex != null) exclude.AddRange( ex.ToList());
		exclude.AddRange( new System.Type[] {
			typeof(MeshFilter),
			typeof(Transform),
			typeof(GameObject),
			typeof(MeshRenderer),
			typeof(SkinnedMeshRenderer),
			typeof(CCText),
		} );
			

		List<Component> comps = new List<Component>();
		comps.AddRange(dyingNumber.GetComponentsInChildren(typeof(Component)));
//		comps.AddRange(dyingNumber.GetComponents(typeof(Component)));
		foreach(Component comp in comps){
			if (comp == null) continue;
			bool destroy = true;
			
			foreach(System.Type t in exclude){
				if ((comp.GetType() == t || comp.GetType().IsSubclassOf(t)) && t != typeof(UserEditableObject)) {
//					// commented Debug.Log("comp subclass of t:"+comp+","+t);
					destroy = false;
				} else {
//					// commented Debug.Log("comp;"+comp.GetType()+","+t);
				}
//				if (comp.GetType() == t)
			}
			if (destroy) {
//				// commented Debug.Log("Dest:"+comp.ToString());
				Destroy(comp);
			} else {
//				// commented Debug.Log("spare:"+comp.ToString());
			}
//			System.Type t = comp.GetType();
//			foreach(System.Type T in exclude){
//				Component c = comp as System.Type;
//			}
//			if (exclude.Contains(t)) continue;

		}
	}


}

