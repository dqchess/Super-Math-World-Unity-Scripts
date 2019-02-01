using UnityEngine;
using System.Collections;

public class PlayerVisibleEditorToggleEditor : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
//		if (Input.GetKeyDown(KeyCode.P)){
//			PlayerCostumeController.inst.curCharInfo.playerHead.gameObject.SetActive(!PlayerCostumeController.inst.curCharInfo.playerHead.gameObject.activeSelf);
//			PlayerCostumeController.inst.curCharInfo.playerBodyGraphics.enabled = !PlayerCostumeController.inst.curCharInfo.playerBodyGraphics.enabled;
//		}
		#endif
	
	}
}
