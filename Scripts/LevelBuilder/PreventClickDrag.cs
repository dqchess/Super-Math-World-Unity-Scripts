using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PreventClickDrag : ScrollRect {

	public override void OnBeginDrag(PointerEventData eventData) { }
	public override void OnDrag(PointerEventData eventData) { }
	public override void OnEndDrag(PointerEventData eventData) { }
	public Vector3 contentStartPosition;

	public void Start(){
		base.Start();
		contentStartPosition = content.transform.localPosition;
//		// commented Debug.Log("set start position:"+content.transform.position + " for "+name);
	}


//	public override void OnKeyPress
}
