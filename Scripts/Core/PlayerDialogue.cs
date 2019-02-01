using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDialogue : MonoBehaviour {

	public static PlayerDialogue inst;
	public GameObject playerDialogue;

	public CCText displayedMessageSmall;
	public CCText displayedMessageBig;

	public Image iconImage;
	public bool showing=false;

	public delegate void PlayerPressedOKDelegate();
	public PlayerPressedOKDelegate playerPressedOKDelegate;
	public delegate void PlayerPressedCancelDelegate();
	public PlayerPressedCancelDelegate playerPressedCancelDelegate;
	public delegate void PlayerPressedBackboardDelegate();
	public PlayerPressedBackboardDelegate playerPressedBackboardDelegate;

	public GameObject loadingDialogue;

	void Start(){
		GameManager.inst.onGameStartedDelegate += HideLoadingDialogue;
	}

	public void SetInstance(){
		inst = this;
	}

	public void ShowLoadingDialogue(){
		loadingDialogue.SetActive(true);
	}

	public void HideLoadingDialogue(){
		loadingDialogue.SetActive(false);
	}

	public void ShowPlayerDialogue(string messageSmall, string messageBig, Sprite sprite = null){
		if (showing) return;
		Inventory.inst.DisableBelt(); 
		
		if (sprite){
			iconImage.sprite = sprite;
			iconImage.color = Color.white;
		} else {
			iconImage.sprite = null;
			iconImage.color = Color.white;
		}


		if (LevelBuilder.inst) {
			if (LevelBuilder.inst.levelBuilderIsShowing) return;
		}
		showing = true;
		playerDialogue.GetComponent<SinPop>().Begin();
		displayedMessageSmall.Text = messageSmall;
		displayedMessageBig.Text = messageBig;
//		AudioListener.pause = true;
		MouseLockCursor.ShowCursor(true,"show player dialogue");
		Player.inst.FreezePlayer("Player Dialogue shown.");
//		Player.
//		// commented Debug.Log("showing..");
	}

	public void HidePlayerDialogueInstant(){
		playerDialogue.SetActive(false);
		Inventory.inst.UpdateBeltPosition();
	}

	public void HidePlayerDialogue(){
		if (!showing) return;
		Inventory.inst.UpdateBeltPosition();
		showing = false;
		playerDialogue.GetComponent<ShrinkAndDisable>().Begin();
//		AudioListener.pause = false;
		MouseLockCursor.ShowCursor(false,"hide player dialogue");
		Player.inst.UnfreezePlayer("Player Dialogue hidden.");
	}

	public void PlayerPressedOK(){
//		else // commented Debug.Log("Player pressed OK, but nothing happened.");
		HidePlayerDialogue();
		if (playerPressedOKDelegate != null) {
			//			// commented Debug.Log("Player pressed ok fired delegate");
			playerPressedOKDelegate();
		}
	}

	public void PlayrePressedCancel(){
		HidePlayerDialogue();
		if (playerPressedCancelDelegate != null){
			playerPressedCancelDelegate();
		}
	}

	public void PlayerClickedBackboard (){
		if (playerPressedBackboardDelegate != null){
			playerPressedBackboardDelegate();
		}
	}



}
