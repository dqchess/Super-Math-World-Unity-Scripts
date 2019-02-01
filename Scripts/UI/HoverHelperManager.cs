using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;
using UnityEngine.UI;



[System.Serializable]
public class HoverHelper{
	public GameObject parent;
	public Text title;
	public Text description;
	public Text skipText;
	public Image backboard;
	public Image backboardShadow;
	public Image arrow;
	public Image arrowShadow;
	public Transform body;
	public GameObject skipTutorialButton;

}

public class HoverHelperManager : MonoBehaviour {


	public static HoverHelperManager inst;
	[SerializeField] public HoverHelper[] hoverHelper;
	public int helperIndex = 0; // 0 is for arrow left, 1 is for arrow down.
	public GameObject hoverParent; // anchor mid left for pointings. Should have an arrow
	public GameObject speechBubbleGraphicParent;
	public bool hoverShowing = false;

	public void SetInstance(){
		inst =this;
	}
	void Start(){
		
		hoverParent.SetActive(false);

	}

	Transform targetHoverTransform;
	public void SetHoverPosition(Transform t, bool snap=false, bool overrideCurrentShowing = false){
//		// commented Debug.Log("try set pos:"+t.name);
		if (overrideCurrentShowing) {
			fading = false;
			hoverShowing = false;
		}
		if (hoverShowing) return; 
		hoverShowing = true;
		if (snap) {
			hoverParent.transform.position = t.position;
			smoothMoveHelperFocus = false;
		}
		else {
			targetHoverTransform = t;
			smoothMoveHelperFocus =true;
		}
		// Sets the hover helper window position based on the location on the screen, so that the helper doesn't go off the screen.
		Vector2 vp = LevelBuilder.inst.camUI.WorldToViewportPoint(t.position); // Screen position of object for which hover helper is displaying
		Vector2 sp = new Vector2(vp.x*GameConfig.screenResolution.x,vp.y*GameConfig.screenResolution.y);
		// First check left/right and select the hover index accordingly, so that the helper window will be to the left of the object if the object is in the right half of the screen, and vice versa
		if (sp.x <= GameConfig.screenResolution.x/2f) helperIndex = 0; // Object Left, window right
		else helperIndex = 1;

//		// commented Debug.Log("sp x;"+sp.x+", gameres x /2:"+GameConfig.screenResolution.x/2f);
		for (int i=0;i<hoverHelper.Length;i++){
			hoverHelper[i].parent.gameObject.SetActive(false);
		}

		hoverParent.SetActive(true);
		if (snap) FadeIn(0.4f);

		// Get distance D of t from bottom or top of screen, and adjust the hover box accordingly if D < some threshhold (half the height of the helper box.)
		float td = 95f; // threshhold distance
		float deltaY = 0; // default position is centered, may move up or down if helping object is towards top or bottom.
		if (sp.y < td) {
			deltaY = td - sp.y;
		}
		if (sp.y > GameConfig.screenResolution.y - td) {
			deltaY = -(sp.y - GameConfig.screenResolution.y + td);
		} else {
			// no action if object is within the "middle" of the screen -- don't need to mvoe window up or down as it wasnt off screen
		}
//		// commented Debug.Log("Deltay:"+deltaY);

		hoverHelper[helperIndex].body.transform.localPosition = new Vector3(0,deltaY,0);

		killTimer = 1.6f;
	}

	float killTimer = 0f;
	bool fading = false;
	float fadeTimer = 0;
	Image[] fadeImages;
	Text[] fadeText;
	Image[] fadeBackgrounds;
	UIHoverHelp currentHelper;
	public void FadeIn(float ft){
		HoverHelper hh = hoverHelper[helperIndex];
		if (!fading){
			fadeTimer = ft;
//			// commented Debug.Log("started fading.");
			hh.parent.gameObject.SetActive(true);

//			fadingHelper = hh;
			fadeImages = new Image[]{
				hh.arrow,
				hh.backboard,

			};

			fadeBackgrounds = new Image[] {
				hh.arrowShadow,
				hh.backboardShadow
			};
			fadeText = new Text[]{
				hh.title,
				hh.description,
				hh.skipText
			};
			foreach(Image im in fadeImages){
				im.color = GameConfig.juneYellowTransparent;
			}
			foreach(Text t in fadeText){
				t.color = GameConfig.darkGrayTransparent;
			}
			foreach(Image im in fadeBackgrounds){
				im.color = new Color(0,0,0,0);
			}
			fading = true;
		}
	}

	bool smoothMoveHelperFocus = false;
	void Update(){
		// killtimer is the single most ugliest thing I've ever written
		// I wrote it because I don't understand how this hover helper bullshit i wrote works and I don't feel like relearning
		// this is to solve the issue of mouseleave not firing after a hoverhelper was executed on a ui element that was then destroyed, causing the hoverhelper to linger
		if (hoverShowing){
			killTimer -= Time.deltaTime;
			if (killTimer < 0){
//				Debug.Log("killtimer 0");
				if (currentHelper != null){
					foreach(RaycastResult rr in LevelBuilder.inst.objectsHit){
						UIHoverHelp hh = rr.gameObject.GetComponent<UIHoverHelp>();
						if (hh == currentHelper){
							killTimer = 1.2f;
//							Debug.Log("found 0:"+hh);
						}
					}
				}
				if (killTimer < 0 && CurrentHelperIsStillUnderCursor()){
					// if it wasn't reset by virtue of the fact we were still hovering over the same one....
					HideHoverHelp();
				}
			}
		}
		float dt = Time.unscaledDeltaTime;
		fadeTimer -= dt;
		if (fading && fadeTimer < 0){
			float fadeSpeed = 10f;
			Color boardColor = Color.Lerp(fadeImages[0].color,GameConfig.juneYellow,dt * fadeSpeed);
			Color textColor = Color.Lerp(fadeText[0].color,GameConfig.darkGray,dt * fadeSpeed);
			Color backgroundColor = Color.Lerp(fadeBackgrounds[0].color,Color.black,dt * fadeSpeed);

			foreach(Image im in fadeImages){
				im.color = boardColor;
			}
			foreach(Text t in fadeText){
				t.color = textColor;
			}
			foreach(Image im in fadeBackgrounds){
				im.color = backgroundColor;
			}

			if (boardColor.a > 0.98f){
				foreach(Image im in fadeImages){
					im.color = GameConfig.juneYellow;
				}
				foreach(Text t in fadeText){
					t.color = GameConfig.darkGray;
				}
//				// commented Debug.Log("finished fading.");
				fading = false;
			}
		}

		if (smoothMoveHelperFocus) {
			float lerpSpeed = 6f;
			hoverParent.transform.position = Vector3.Lerp(hoverParent.transform.position,targetHoverTransform.position,dt * lerpSpeed);
			float deltaP = Vector3.SqrMagnitude(hoverParent.transform.position-targetHoverTransform.position);
			if (deltaP > 1.4f){
				speechBubbleGraphicParent.SetActive(false);
			} else {
//				if (groupIndex < groups.Count && instructionsIndex < groups[groupIndex].instructions.Count) ResizeInputRestrictionPlane();
				hoverParent.transform.position = targetHoverTransform.position;
				speechBubbleGraphicParent.SetActive(true);
				FadeIn(0f);
				smoothMoveHelperFocus = false;
				TutorialManager.inst.currentTutorial.ResizeInputRestrictionPlane();
			}
		}
	}

	public void SetHelperText(string title, string description){
		hoverHelper[helperIndex].title.text = title;
		hoverHelper[helperIndex].description.text = description;
	}

	public void SetTutorialButtonActive(bool f){
		hoverHelper[helperIndex].skipTutorialButton.SetActive(f);
	}

	public void HideHoverHelp(){
//	 	Debug.Log("hide hov");
		hoverShowing = false;
		fading = false;
		hoverParent.SetActive(false);
	}

	bool CurrentHelperIsStillUnderCursor(){
		foreach(RaycastResult h in LevelBuilder.inst.objectsHit){
			UIHoverHelp uh = h.gameObject.GetComponent<UIHoverHelp>();
			if (uh && uh == currentHelper){
				return true;
			}
		}
		return false;
	}

}
