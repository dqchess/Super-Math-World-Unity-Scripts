using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketUI : MonoBehaviour {



	public GameObject marketParent; // enabled and disabled on show/hide
	public Transform itemListParent;
	public MarketUIItem marketObjectPrefab;
	public static MarketUI inst;
	public Text message;

	public void Message(string s){
		message.text = s;
		message.GetComponent<SimpleTypewriter>().InitText();
	}

	public void SetInstance(){
		inst = this;

	}


	public void ShowMarket(UEO_MarketObject[] items){
		if (!showing){
			showing = true;
			MouseLockCursor.ShowCursor(true,"market show");
			Player.inst.FreezePlayer();
			marketParent.GetComponent<SinPop>().Begin();
			foreach(Transform t in itemListParent){
				Destroy(t.gameObject); // clear old market.
			}
			foreach(UEO_MarketObject mo in items){
				// instantiate a new item for each sale item in this market.
				GameObject p = (GameObject)Instantiate(marketObjectPrefab.gameObject);
				MarketUIItem item = p.GetComponent<MarketUIItem>();
	//			Debug.Log("item:"+mo.name);
	//			Debug.Log(",p:"+p);
	//			Debug.Log(",item:"+item);
				item.name.text = mo.itemName;
				item.item = mo; // the prefab reference?
				item.price.text = mo.price.ToString();
				item.transform.SetParent(itemListParent);
				item.transform.localScale = Vector3.one;
				item.icon.sprite = mo.icon;
			}

			// Resize content window to let all contents be visible by scrolling
			ScrollRect sr = GetComponentInChildren<ScrollRect>();
			float itemHeight = 90f;
			sr.content.sizeDelta = new Vector2(sr.content.sizeDelta.x,400 + Mathf.Max(0,(items.Length-2)*itemHeight));
		}
	}

	bool showing = false;
	public void HideMarket(){
		if (showing){
			showing = false;
			Player.inst.UnfreezePlayer();
			MouseLockCursor.ShowCursor(false,"market hide");
			// not used, market is closed only by "leave market" button?!
			// Use INTERFACE to close all IMyCloseableUI
			marketParent.GetComponent<ShrinkAndDisable>().Begin();
	//		marketParent.SetActive(false);
		}
	}
}
