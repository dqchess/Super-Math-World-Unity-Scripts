using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItem : MonoBehaviour {

	public GameObject item3d;
	public GameObject digits;

	public void SetUpDigits(NumberInfo ni){
//		Debug.Log("setting up digits;"+ni.fraction);
		if (digits) {
			Destroy(digits);
		} 
		digits = NumberManager.inst.CreateNumberAmmo(ni.fraction,ni.myShape);
//		if (ni.GetComponent
//		Debug.Log("set dig");
		SetUpNumberGraphics(ni);


		digits.transform.SetParent(transform);
		digits.transform.localPosition = Vector3.zero;
		NumberInfo nim = digits.GetComponent<NumberInfo>();
		foreach(Renderer r in digits.GetComponentsInChildren<Renderer>()){
			r.material = NumberManager.inst.inventoryDigitMaterial;
		}
		Destroy(nim.childMeshRenderer.gameObject); // we created a 3D ammo object (which isn't ideal but it gives us mostly what we want) but we dont want the 3d sphere/cube
		if (nim.digits.gameObject.GetComponent<SometimesFacePlayer>()) Destroy(nim.digits.gameObject.GetComponent<SometimesFacePlayer>()); // don't want digits moving about
		if (nim.GetComponentsInChildren<CubeNumberFacePlayer90>().Length > 0){ 
			foreach(CubeNumberFacePlayer90 c90 in nim.GetComponentsInChildren<CubeNumberFacePlayer90>()){
				Destroy(c90);// don't want digits moving about
			}
		}
		Quaternion rot = new Quaternion();
		digits.transform.localScale = Vector3.one*55f; // Need to increase digits size for UI. We really should be pulling from a UI number prefab instead of chopping up NumberAmmoPrefab which is not intended for UI use.
		if (nim.myShape == NumberShape.Sphere) {
			rot.eulerAngles = new Vector3(0,180,0);
		} else if (nim.myShape == NumberShape.Cube) {
			rot.eulerAngles = new Vector3(0,225,0);
		} else if (nim.myShape == NumberShape.Tetrahedron) {
			rot.eulerAngles = new Vector3(0,270,15);
			nim.digits.transform.localPosition = new Vector3(0.037f,0.012f,0.004f);
			nim.digits.transform.localScale = Vector3.one * 0.5f;
		}else if (nim.myShape == NumberShape.Schur) {
			rot.eulerAngles = new Vector3(0,180,0);
//			nim.digits.transform.localPosition = new Vector3(0.037f,0.012f,0.004f);
//			nim.digits.transform.localScale = Vector3.one * 0.5f;
		}
		nim.digits.transform.localRotation = rot;
		SMW_GF.inst.SetLayerRecursively(digits,LayerMask.NameToLayer("UI"));

		Destroy(nim);

		return;

//		bool frac = false;
//		if (ni.fraction.denominator > 1) frac = true;
//
//		digits = (GameObject)Instantiate(Inventory.inst.digitsPrefab);
//		digits.transform.SetParent(transform,false);// = transform;
////		digits.transform.localPosition = Vector3.zero;
//		digits.GetComponent<InventoryDigits>().numerator.text = ni.texts[0].numerator.Text;
//		digits.GetComponent<InventoryDigits>().denominator.text = ni.texts[0].denominator.Text;
//		digits.GetComponent<InventoryDigits>().line.text = ni.texts[0].line.Text;
//		digits.GetComponent<InventoryDigits>().integer.text = ni.texts[0].integer.Text;
//

		// List different cases for number sizes/combos and assign absolute positions and scales for digits based on these cases
		/*
			1
			12
			123
			1234
			12345
			123456
			-123456
			1/2
			9/10
			11/9
			123/3
			12/25
			1/123
			123/32
			32/123
			123/123
			123/1234
			1234/123
			1234/1234

			1 1/2
			2 1/2


		 * */






	}

	void SetUpNumberGraphics(NumberInfo ni){
		if (ni.GetComponent<PickUppableObject>().inventoryIcon != null) {
			// commented Debug.Log("inv icon not null");
			GetComponent<Image>().sprite = ni.GetComponent<PickUppableObject>().inventoryIcon;
			return;
		}


//		// commented Debug.Log("ni getcomp playerpickup invicon null on;"+ni);
		if (ni.fraction.numerator > 0) {
			GetComponent<Image>().sprite = ni.myShape == NumberShape.Sphere ? Inventory.inst.sprites.posNumSphere : Inventory.inst.sprites.posNumCube;
		} else {
			GetComponent<Image>().sprite = ni.myShape == NumberShape.Sphere ? Inventory.inst.sprites.negNumSphere : Inventory.inst.sprites.negNumCube;
		}

	}
}
