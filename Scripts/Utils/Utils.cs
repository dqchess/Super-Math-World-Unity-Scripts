using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public static class Utils {
	

	public static Color LightBlueColor(){
		return new Color(0,.5f,1f,0.4f);
	}

	public static long GetGMTInMS(){
		var unixTime = System.DateTime.Now.ToUniversalTime() -
			new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

		return (long)unixTime.TotalMilliseconds;
	}



	public static GameObject DebugSphere(Vector3 p){
		GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		ret.transform.position = p;
		ret.transform.localScale = Vector3.one * 3;
		ret.name = "debug sphere "+Time.time;
		Object.Destroy(ret.GetComponent<Collider>());
		return ret;
	}

	public static string UsePlayerNameInText(string input){
		
		string pattern = "{{\\s?name\\s?}}";
		string replacement = WebGLComm.loggedInAsUser;
		string result = Regex.Replace( input, pattern, replacement, RegexOptions.IgnoreCase);
		return result;

//		string[] names = new string[] { "{{ Name }}","{{Name}}"
//			s.Replace( WebGLComm.loggedInAsUser)
	}


	public static string AddSpacesToSentence(string t, bool preserveAcronyms=false)
	{
		if (string.IsNullOrEmpty(t))
			return string.Empty;
		char[] text = t.ToCharArray();

		string newText = text[0].ToString();
//		newText.Add(text[0].ToString());
		for (int i = 1; i < text.Length; i++)
		{
			if (char.IsUpper(text[i]))
			if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
				(preserveAcronyms && char.IsUpper(text[i - 1]) && 
					i < text.Length - 1 && !char.IsUpper(text[i + 1])))
				newText += " "; //.Add(" ");
			newText += (text[i].ToString());
		}
//		string returnText = "";
//		foreach(string s in newText){
//			returnText += s;
//		}
		return newText;
	}

	public static Quaternion FlattenRotation(Quaternion rot){
		rot.eulerAngles = new Vector3(0,rot.eulerAngles.y,0);
		return rot;
	}

	public static bool IsRootTransform(Transform t){
		return t.root == t;
	}


	public static void ResetLerpedTime(){ //ugh
		lerpedTime = 0;
	}
	static float lerpedTime = 0;
	public static Vector3 LerpFixedSpeed(Transform movingObject, Transform target, float totalLerpTime){
//		StartCoroutine(LerpFixedSpeed(movingObject,target,totalLerpTime)
		// PROBLEM: This relies on lerpedtime getting reset to zero for every NEW lerp.
		// Solution: either don't call this function every from from client script's UPDATE function, and totally self contain here as being called only once.
		// Manage state of frog lerping tongue state by state, instead of running everything in update? How to get it to fire only once? Booleans?

		if (lerpedTime > totalLerpTime) lerpedTime = totalLerpTime;
		lerpedTime += Time.deltaTime;
		float percTime = lerpedTime / totalLerpTime;
		return Vector3.Slerp(movingObject.position,target.position,percTime);
	}



	public static Vector3 FlattenVector(Vector3 vectorA){
		return new Vector3(vectorA.x,0,vectorA.z);
	}


	public static Vector3 FlattenVectorZ(Vector3 vectorA){
		return new Vector3(vectorA.x,vectorA.y,0);
	}

	public static Material FadeMaterial(){
		Material m = new Material(Shader.Find("Standard"));
		m.SetFloat("_Mode", 2);
		m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		m.SetInt("_ZWrite", 0);
		m.DisableKeyword("_ALPHATEST_ON");
		m.EnableKeyword("_ALPHABLEND_ON");
		m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m.renderQueue = 3000;
		return m;
	}

	public static void SetEulerAngles(Transform t, Vector3 r){
		Quaternion rot = t.rotation;
		rot.eulerAngles = r;
		t.rotation = rot;
	}

	public static Color RandomColor(){
		int rand = Random.Range(0,12);
		switch(rand){
		case 0: return new Color(1,.4f,.4f,1); break; // red
		case 1: return new Color(.3f,.3f,.3f,1); break; // black
		case 2: return new Color(.45f,.3f,1,1); break; // lite blue
		case 3: return new Color(.4f,1,.5f,1); break; // green
		case 4: return new Color(.2f,1,1,1); break; // cyan
		case 5: return new Color(1,1,.2f,1); break; // yellow
		case 6: return new Color(1,.4f,0,1); break; // orange
		case 7: return new Color(1,.3f,1,1); break; // purple
		case 8: return new Color(1,.5f,1,1); break; // light purple
		case 9: return new Color(1,0,.4f,1); break; // magenta
		case 10: return new Color(0,.4f,.1f,1); break; // forest green
		case 11: return new Color(0,0,.5f,1); break; // dark blue
		case 12: return new Color(.7f,.7f,.7f,1); break; // off white
//		case 13: return new Color(,100,100,1); break;
//		case 14: return new Color(255,100,100,1); break;
//		case 15: return new Color(255,100,100,1); break;
		default: return Color.white; break;
		}
	}


	public static Color RandomColorDelta(float f, float t){
		return new Color(Random.Range(f,t),Random.Range(f,t),Random.Range(f,t),0);
	}

	public static string FakeToRealQuotes(string s){
		s = s.Replace("^quot^","'");
		s = s.Replace("^dquot^","\"");
		return s;
	}

	public static string RealToFakeQuotes(string s){
		s = s.Replace("'","^quot^");
		s = s.Replace("\"","^dquot^");
		return s;
	}


	public static float HighestY(Transform tt){
		// Takes the transformand all children and goes through each inddividual mesh's world bounds to figure out what is the highest worldspace point occupied by any child
		// Useful if you want to know what the highest position of a group of objects is in world space, e.g. "does this group of objects cross some surface of height Y"
		float highestY = Mathf.NegativeInfinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newY = r.gameObject.transform.position.y + r.bounds.extents.y;
				if (newY > highestY) {
					highestY = newY;
				}
			}
		}
		return highestY;
	}
		
	public static float LowestY(Transform tt){
		// same sas highesty but for lowest point
		// Used together, you can get the total world space height of a group of objects
		float lowestY = Mathf.Infinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newY = r.gameObject.transform.position.y - r.bounds.extents.y;
				if (newY < lowestY) {
					lowestY = newY;
				}
			}
		}
		return lowestY;
	}

	public static float LeftestX(Transform tt){
		float leftestX = Mathf.Infinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newX = r.gameObject.transform.position.x - r.bounds.extents.x;
				if (newX < leftestX) {
					leftestX = newX;
				}
			}
		}
		return leftestX;
	}

	public static float RightestX(Transform tt){
		float rightestX = -Mathf.Infinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newX = r.gameObject.transform.position.x + r.bounds.extents.x;
				if (newX > rightestX) {
					rightestX = newX;
				}
			}
		}
		return rightestX;
	}

	public static float BackestZ(Transform tt){
		float backestZ = Mathf.Infinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newZ = r.gameObject.transform.position.z - r.bounds.extents.z;
				if (newZ < backestZ) {
					backestZ = newZ;
				}
			}
		}
		return backestZ;
	}


	public static float ForwardestZ(Transform tt){
		float forwardestZ = -Mathf.Infinity;
		foreach(Transform t in tt){
			foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
				if (r.gameObject.GetComponent<IgnoreSize>()) continue;
				float newZ = r.gameObject.transform.position.z + r.bounds.extents.z;
				if (newZ > forwardestZ) {
					forwardestZ = newZ;
				}
			}
		}
		return forwardestZ;
	}

	public static Vector2 ApproxSizeXZ(Transform tt){
		return new Vector2(ForwardestZ(tt)-BackestZ(tt),RightestX(tt)-LeftestX(tt));
	}


	public static Bounds boundsOfChildren(Transform t){
		// What is the total encapsulating bounds of all children of t?
		Bounds b = new Bounds(Vector3.zero,Vector3.zero);
		foreach(MeshRenderer r in t.GetComponentsInChildren<MeshRenderer>()){
			if (b.size == Vector3.zero){
				b = r.bounds;	
			} else {
				b.Encapsulate(r.bounds);
			}
		}
		return b;
	}
	
	public static string ToString(float n, int decimalPlaces){
		string zeros = "";
		for(int i=0;i<decimalPlaces;i++){
			zeros += "0";
		}
		return n.ToString("0."+zeros);
		
	}

	public static float RealHeight(Transform t){
		return HighestY(t) - LowestY(t);
	}

	public static Vector3 ClampY(Vector3 dir, float ymin, float ymax){
		return new Vector3(dir.x,Mathf.Clamp(dir.y,ymin,ymax),dir.z);
	}


	public static float CubicInOut(float x){
		// a bell curve style cubic beginning and ending at zero.
		// used for elevator to start and end slow but move quite fast in the middle.
		float ret = new AnimationCurve(
			new Keyframe(0, 0), 
			new Keyframe(0.02f, 0.05f), 
			new Keyframe(0.2f, 0.75f), 
			new Keyframe(0.3f, 0.9f), 
			new Keyframe(0.4f, 1), 
			new Keyframe(0.6f, 1), 
			new Keyframe(0.7f, 0.9f), 
			new Keyframe(0.8f, 0.75f),
			new Keyframe(0.98f, 0.05f),
			new Keyframe(1, 0)
		).Evaluate(x);
//		Debug.Log("ret:"+ret);
		return ret;
	}

	public static float BellCurve(float x){
		
		// which creates a bell curve of max y value 1 between interval 0, 1 on x axis.
		float ret = (1+Mathf.Cos(Mathf.PI * 2 * x - Mathf.PI)) / 2f;

//		Debug.Log("ret:"+ret);
		return ret;
	}


	public static void SetTrailRendererColorKeys(TrailRenderer tr, Color randColor){
		Gradient grad = new Gradient();
		grad.SetKeys(Utils.GenerateShurikenGradientColorKeys(randColor),Utils.GenerateShurikenAlphaKeys());
		tr.colorGradient = grad;

	}



	public static void SetParticleSystemGradientToColor(ParticleSystem ps, Color randColor){
		var col = ps.colorOverLifetime;
		col.enabled = true;
		Gradient grad = new Gradient();
		grad.SetKeys(Utils.GenerateShurikenGradientColorKeys(randColor),Utils.GenerateShurikenAlphaKeys());
		col.color = grad;

	}

	public static GradientColorKey[] GenerateShurikenGradientColorKeys(Color randColor){
		return new GradientColorKey[] { new GradientColorKey(randColor, 0.0f), new GradientColorKey(randColor, 1.0f) };

	}

	public static GradientAlphaKey[] GenerateShurikenAlphaKeys(){
		return new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, .7f), new GradientAlphaKey(0.0f, 1.0f) };
	}

	public static string DisplayAsTimeFromSeconds(int seconds){
		int minutes = Mathf.FloorToInt(seconds/60);
		seconds %= 60;
		string r = ""; // the return string
		r = minutes.ToString();
		if (r.Length == 1) { 
			// for minutes 1 - 9 add a 0 for 01-09
			r = "0"+r;
		}
		r += ":";
		string s = seconds.ToString();
		if (s.Length == 1){
			s = "0"+s;
		}
		r += s;
		return r;

	}


	public static Vector3[] GetCircleOfPoints(float degreesToComplete=360, float radius=150, float scale=5){ // lower scale = higher point count
		// This method can be used to get a set of Vector3 points that draw a cirle about the Y axis.
		// Useful if you want to cast a spell that creates an arrangement of objects in a circle around the spellcaster or target.
		// Note: The Vector3[] array will exist in local space

		int count = (int)(degreesToComplete * radius / scale / 60);
		float arcLength = degreesToComplete / count;
		Vector3[] ret = new Vector3[count];
		for (int i=0;i<count;i++){
			// commented Debug.Log ("radius:"+radius);
			float xPos = Mathf.Sin(Mathf.Deg2Rad*i*arcLength)*(radius); // x and y calculated with "trigonometry"
			float yPos = Mathf.Cos(Mathf.Deg2Rad*i*arcLength)*(radius);
			ret[i] = new Vector3(xPos,0,yPos);
		}
		return ret;
	}

	public static void RemoveDeathComponents(GameObject origObject){
		if (origObject.GetComponent<ShrinkAndDisappear>()){
			Object.Destroy(origObject.GetComponent<ShrinkAndDisappear>());
		}
		if (origObject.GetComponent<DoesDieOnImpact>()){
			Object.Destroy(origObject.GetComponent<DoesDieOnImpact>());
		}
		if (origObject.GetComponent<MultiblasterBullet>()){
			Object.Destroy(origObject.GetComponent<MultiblasterBullet>());
		}
		if (origObject.GetComponent<TimedObjectDestructor>()){
			Object.Destroy(origObject.GetComponent<TimedObjectDestructor>());
		}
	}


	public static string LoadStringFile(string fileName)
	{

		string lines = "";
		string line;
		// Create a new StreamReader, tell it which file to read and what encoding the file
		// was saved as
		StreamReader theReader = new StreamReader(Application.dataPath + "/texts/" + fileName, Encoding.Default);
		// Immediately clean up the reader after this block of code is done.
		// You generally use the "using" statement for potentially memory-intensive objects
		// instead of relying on garbage collection.
		// (Do not confuse this with the using directive for namespace at the 
		// beginning of a class!)
		using (theReader)
		{
			// While there's lines left in the text file, do this:
			do
			{
				line = theReader.ReadLine();

				if (line != null)
				{
					lines += line;
				}
			}
			while (line != null);
			// Done reading, close the reader and return true to broadcast success    
			theReader.Close();
			return lines;
		}
		return "nope";

	}


	public static void RemoveInteractableLevelBuilderComponents(GameObject fakeCopy){
		if (!fakeCopy) return;
		List<Component> comps = new List<Component>();
		comps.AddRange(fakeCopy.GetComponentsInChildren(typeof(Component)));
		foreach(UserEditableObject ueo in fakeCopy.GetComponentsInChildren<UserEditableObject>()){
//			Debug.Log("ignore destroy flags?");
			ueo.ignoreDestroyFlags = true; // don't care if this object is destroyed as it is a "fake" (eg. don't try to remove it from placed objects.");
		}
		foreach(Component comp in comps){
			//			// commented Debug.Log ("comp: " +comp.GetType() + "typoef " + typeof(MeshFilter));
			if (comp == null) continue;
			if ( comp.GetType()==typeof(Transform)
				|| comp.GetType()==typeof(MeshFilter)
				|| comp.GetType()==typeof(MeshRenderer)
				|| comp.GetType()==typeof(Renderer)
				|| comp.GetType()==typeof(RectTransform)
				|| comp.GetType()==typeof(CanvasRenderer)
			) {
				continue;
			} else {
				Object.Destroy(comp);
			}
		}

		
	}
//	public static GameObject ConstructFakeCopyOfObject(GameObject origObject){
//		GameObject fakeCopy = new GameObject("fakecopy");
////		 (GameObject)GameObject.Instantiate(origObject,origObject.transform.position,origObject.transform.rotation);
//		RemoveInteractableLevelBuilderComponents(fakeCopy);
//		return fakeCopy;
//	}

	public static GameObject ConstructFakeCopyOfAmmo(GameObject origObject){
//		Debug.Log("making copy");
		GameObject fakeCopy = new GameObject("ammo");
		fakeCopy.transform.rotation = origObject.transform.rotation;
		foreach(Transform t in origObject.GetComponentsInChildren<Transform>()){
			Vector3 deltaPos = t.position - origObject.transform.position;
			if (t.parent && t.parent.GetComponent<NumberInfoAmmo>()) continue;
			NumberInfoAmmo nam = t.GetComponent<NumberInfoAmmo>();
			if (nam) {
				GameObject ammoChild = NumberManager.inst.CreateNumberAmmo(nam.fraction,nam.myShape);
				ammoChild.transform.parent = fakeCopy.transform;
				ammoChild.transform.localScale = t.localScale;
				ammoChild.transform.localPosition = t.localPosition;
			} else {
				AddMeshFilterToObj(t,t.lossyScale,deltaPos,origObject,fakeCopy);
			}
		}



		return fakeCopy;
	}



	static void AddMeshFilterToObj(Transform originT,Vector3 scale, Vector3 localPos,GameObject origObject,GameObject fakeCopy){

		if (originT == origObject.transform) localPos = Vector3.zero;
		MeshFilter originMesh = originT.GetComponent<MeshFilter>();
		MeshRenderer originRend = originT.GetComponent<MeshRenderer>();
		if (originMesh && originRend){
			GameObject mChild = new GameObject("mchild");
			mChild.transform.parent = fakeCopy.transform;
			MeshFilter mf = mChild.AddComponent<MeshFilter>();
			mf.sharedMesh = originMesh.sharedMesh;
			MeshRenderer r = mChild.AddComponent<MeshRenderer>();
			r.material = originRend.material;
			if (r.material.HasProperty("_Color") && originRend.material.HasProperty("_Color"))  r.material.color = originRend.material.color;
			mChild.transform.localScale = scale;
			mChild.transform.localPosition = localPos;
		}
	}

	public static bool DropTransformToTerrain(Transform t){
//		RaycastHit hit = new RaycastHit();
		float upoffset = 5f;
		float dist = 120f;
		Ray ray = new Ray(t.position+Vector3.up  *upoffset,Vector3.down);
		Transform highest = null;
		Vector3 highestPoint = Vector3.zero;
		foreach(RaycastHit hit in Physics.RaycastAll(ray,dist,~LayerMask.NameToLayer("Terrain"))){
//			Debug.Log("hit..:"+hit.collider.name);
			if (hit.collider.transform == t) continue;
			if (!highest) {
				highest = hit.collider.transform;
				highestPoint = hit.point;
			}
			else if (hit.point.y > highestPoint.y) {
				
				highestPoint = hit.point;
			}
		}
		if (highestPoint != Vector3.zero){
			t.position = highestPoint;
//			Debug.Log("moved:"+t.position);
			return true;
		} else {
//			Debug.Log("not moved:"+t.position);
			return false;
		}
	}

	public static bool FadeUiElements(Transform t, float targetAlpha, float fadeSpeed  =4f){
		float delta = 0f;
		float threshhold = .05f; // get close to this value before returning false
		foreach(Image im in t.GetComponentsInChildren<Image>()){
			Color targetColor = new Color(im.color.r,im.color.g,im.color.b,targetAlpha);
			im.color = Color.Lerp(im.color, targetColor, Time.deltaTime * fadeSpeed);
			delta = Mathf.Max(delta,Vector4.Magnitude(im.color-targetColor));
		}
		foreach(Text text in t.GetComponentsInChildren<Text>()){
			Color targetColor = new Color(text.color.r,text.color.g,text.color.b,targetAlpha);
			text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * fadeSpeed);
			delta = Mathf.Max(delta,Vector4.Magnitude(text.color-targetColor));
		}
		// snap to alpha when finished
		if (delta < threshhold){
			SnapUiElementsToAlpha(t,targetAlpha);
		}
		return delta < threshhold; // returns true when el doesn't need fading any more
	}


	static void SnapUiElementsToAlpha(Transform t, float targetAlpha){
		foreach(Image im in t.GetComponentsInChildren<Image>()){
			Color targetColor = new Color(im.color.r,im.color.g,im.color.b,targetAlpha);
			im.color = targetColor;

		}
		foreach(Text text in t.GetComponentsInChildren<Text>()){
			Color targetColor = new Color(text.color.r,text.color.g,text.color.b,targetAlpha);

			text.color = targetColor;
		}

	}



	public static bool IntervalElapsed(float t){
	// A handy replacement for using "timers" in various scripts to track actions over intervals
	 // Usage: 
	 /// if (Utils.IntervalElapsed(2)) {
	 //		// This action occurrs every 2 seconds. except it doesnt work for values greater than 1 LOL


		return Time.realtimeSinceStartup > t && Mathf.Abs(((Time.realtimeSinceStartup - t) % t)-t)<Time.unscaledDeltaTime;
	}


	public static WaterCube IsInsideWaterCube(Transform t){
		
		// Detect if the passed transform is inside a water cube (for swimming/animation purposes)
		foreach(WaterCube wc in GameObject.FindObjectsOfType<WaterCube>()){
			Vector3 fudge = (t.position - wc.transform.position).normalized * 2f; // object must be this "deep" inside a water cube to return true
			if (wc.GetComponent<Renderer>().bounds.Contains(t.position + fudge)){
				return wc;
			}
		}
		return null;
	}


	public static Transform  GetClosestObjectOfType<T>(Transform origin) where T : MonoBehaviour {
		// Often not the most efficient, but will help you find the closest object which has type <T> to a transform, "origin"
		/*
		 * Usage: 
		 * Enemy e = Utils.GetClosestObjectOfType<Enemy>(Player.transform);
		 * // if any Enemy objects exist in the scene, e is now a reference to the closest Enemy to the Player
		 * */

		Transform closest = null;
		foreach(Component oo in Component.FindObjectsOfType(typeof(T))){
			GameObject o =  oo.gameObject;
			if (!closest){
				closest = o.transform;
			} else if (Vector3.SqrMagnitude(origin.position-o.transform.position) < Vector3.SqrMagnitude(closest.position-origin.position)){
				closest = o.transform;
			}
		}
		return closest;
	}


	public static Transform GetTransformJustAboveRoot(Transform t, int depth=10) {
		int i = 0;
		if (t.parent == null) {
			return t;
		} else {
			while (i < depth) {
				if (t.parent == t.root) {
					return t;
				} else t = t.parent;
				i++;
			}
		}
		return t;
//		return null;


	}


	public static int GetActiveIndexFromTransform(Transform[] t){
		for (int i=0;i<t.Length;i++){
			if (t[i].gameObject.activeSelf) {
				return i;
			}
		}
		return -1;
	}

	public static bool IsNan(Vector3 v){
		return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
	}

	public static float UnNan(float f){
		if (float.IsNaN(f) || float.IsInfinity(f)){
			return 0;
		} else return f;
	}

	public static Vector3 UnNan(Vector3 p){
		return new Vector3(UnNan(p.x),UnNan(p.y),UnNan(p.z));
	}

	public static string NiceName(string n){
		
		return n.Replace("general ","");
	}



	public static float GetMouseDeltaY(){
		return GameManager.inst.mouseScrollReversed ? Input.mouseScrollDelta.y * -1 : Input.mouseScrollDelta.y;
	}

	public static Vector3 RoundVector3ToInteger(Vector3 r){
		return new Vector3(Mathf.RoundToInt(r.x),Mathf.RoundToInt(r.y),Mathf.RoundToInt(r.z));
	}

	public static Vector3[] GetRandomSubsetOfChildMeshVertsInWorldspace(Transform t, int vertCount){
		List<Vector3> verts = new List<Vector3>();
		foreach(MeshFilter mf in t.GetComponentsInChildren<MeshFilter>()){
			if (mf.gameObject.activeSelf) {
				foreach(Vector3 v in mf.sharedMesh.vertices){
					verts.Add(mf.transform.TransformPoint(v));
				}
			}
		}
		List<Vector3> ret = new List<Vector3>();
		for (int i=0; i<vertCount; i++){
			ret.Add(verts[Random.Range(0,verts.Count)]);
		}
//		verts = verts.OrderBy(x => rnd.Next()).Take(Mathf.Min(verts.Count,vertCount);
		return ret.ToArray();

	}
//	public static string TwoDecimalString(string s){
//		if (s.Contains(".") && s.Split('.').Length == 2){
//			string p2 = s.Split('.')[1].Substring(0,2);
//			return s.Split('.')[0]+"."+p2;
//		} else return s;
//	}

	public static List<T> FindObjectsOfTypeInScene<T>()
	{
		List<T> results = new List<T>();

		{
			GameObject[] allGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

			for (int j = 0; j < allGameObjects.Length; j++){
				var go = allGameObjects[j];
				results.AddRange(go.GetComponentsInChildren<T>(true));
			}
		}
		return results;
	}
//	UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
	
}
