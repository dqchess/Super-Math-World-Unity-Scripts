using UnityEngine;
using System.Collections;

public class JustAteFX : MonoBehaviour {

	// When animals eat something, a thought bubble is put over their head to indicate what they just ate.
	// It should be created on the animal('s head), rise up, grow, fade out, and destruct.

	public Renderer justAteIcon;
	public CCText cctext;
	public Renderer clouds;
	public Renderer numberSphereIcon;
//	public Renderer smiley;
	Material[] mats;
	// Use this for initialization
	void Start () {
		
		mats = new Material[] {
			justAteIcon.material,
			clouds.material,
//			smiley.material,
			numberSphereIcon.material
		};
	}
	
	// Update is called once per frame
	float growthConstant = 0.2f;
	float timeBeforeFadeStarts = 3f;
	float fadeConstant = 1f;
	Color fadeColor = new Color(1,1,1,0);
	float timer = 0;
	float destroyTime = 5;
	void Update () {
		timer += Time.deltaTime;
		transform.localScale += Vector3.one * Time.deltaTime * growthConstant;
		if (timer > timeBeforeFadeStarts){
			foreach(Material m in mats){
				m.color = Color.Lerp(m.color,fadeColor, Time.deltaTime * fadeConstant);
			}
			cctext.Color = Color.Lerp(cctext.Color,fadeColor, Time.deltaTime * fadeConstant);
		}
		if (timer > destroyTime){
			Destroy(gameObject);
		}

	}

//	void LateUpdate(){
//		// Alway face player Y
//		if (Camera.main){
//			transform.LookAt(Camera.main.transform);
//			Quaternion rot = transform.rotation;
//			rot.eulerAngles = new Vector3(0,transform.rotation.y,0);
//			transform.rotation = rot;
//		}
//	}
}
