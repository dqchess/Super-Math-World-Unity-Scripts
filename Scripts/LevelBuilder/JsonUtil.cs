using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class JsonUtil : MonoBehaviour {


	public static string sizeXkey = "sizeX";
	public static string sizeZkey = "sizeZ";
	public static string dimensionsKey = "dimensions";

	public static string scaleKey = "scale";
	public static string GetTruncatedPosition(Transform t){ // Positions' 3 values are scaled up 100x so we don't save decimals, so 235.436464 xPos becomes 23443 which when we parse out later becomes 234.43. I know it's not ideal.

		// We're implementing "precise" floating positions by converting the float to hex
//		Debug.Log("float x:"+t.position.x);
//		Debug.Log("hex x:"+FloatToHex(t.position.x));


//		double x = Mathf.RoundToInt(t.position.x*100);
//		double y = Mathf.RoundToInt(t.position.y*100);
//		double z = Mathf.RoundToInt(t.position.z*100);
//		Debug.Log("FloatToString(t.position):"+FloatToString(t.position));
		return Vector3ToString(t.position);
	}

	public static string Vector3ToString(Vector3 p){
		return	p.x.ToString() + "," + p.y.ToString() + "," + p.z.ToString();
	}

	public static string GetRotation(Transform t){ // Rotations' 4 values are scaled up 10x so that we don't save decimals
//		Debug.Log("t rot:"+t.rotation.x+","+t.rotation.y);
		Vector3 r = t.rotation.eulerAngles;
		return Mathf.RoundToInt(r.x)+","+Mathf.RoundToInt(r.y)+","+Mathf.RoundToInt(r.z);
		// This was the 1.1f way of rotations = quaternion floats as Unity made floats to strings which wasn't accurate enough
//		float x = t.rotation.x; // Mathf.RoundToInt(t.rotation.x*100)/10f;
//		float y = t.rotation.y; 
//		float z = t.rotation.z; 
//		float w = t.rotation.w; 
		// This was the 1.0f way we did rotations, simply take the quaternion float and get 2 decimals, it wasn't accurate enough
//		float y = Mathf.RoundToInt(t.rotation.y*100)/10f;
//		float z = Mathf.RoundToInt(t.rotation.z*100)/10f;
//		float w = Mathf.RoundToInt(t.rotation.w*100)/10f;
//		return x + "," + y + "," + z +"," +w;
	}
		
	public static string IntVector3ToString(Vector3 v){
		// note vector3 must only have integer values to save, eg. for xyz position inside a machine
		return ((int)(v.x)).ToString()+","+((int)(v.y)).ToString()+","+((int)(v.z)).ToString();
	}

	public static Vector3 StringToIntVector3(string ss){
//		Debug.Log("ss:"+ss);
		// note vector3 must only have integer values to save, eg. for xyz position inside a machine
		string[] s = ss.Split(',');
		return new Vector3(int.Parse(s[0]),int.Parse(s[1]),int.Parse(s[2]));

	}

	// sometimes we pass the json string directly instead of json obj eg "1234,123,513" as position
	public static Vector3 GetRealPositionFromTruncatedPosition(string p1){
//		Debug.Log("p1:"+p1);
		string[] p2 = p1.Split(','); // ["123","123","123"]
		if (JsonLevelLoader.inst.thisLevelVersion > 1.0f || p1.Contains(".")){
//			Debug.Log("new:");
			// New conversion was implemented on Mar 13 2017 for more precision.
			// OR, if p1 contains a decimal point "." (beacuse some levels were save dunder verison 1.0 where they did contain a . from the new system.)
			// We'll know if the level json was saved using this new system based on the version of the json that was loaded.
			Vector3 ret = new Vector3(float.Parse(p2[0]),float.Parse(p2[1]),float.Parse(p2[2]));
//			Debug.Log("ret:"+ret.x);
			return ret;
		} else {
//			Debug.Log("old:");
			return new Vector3(int.Parse(p2[0])/100f,int.Parse(p2[1])/100f,int.Parse(p2[2])/100f);
		}
//		return pos;
	}


	public static Vector3 GetRealPositionFromTruncatedPosition(SimpleJSON.JSONClass N){
		string p1 = N["position"].Value; // "123,454,235"
		return GetRealPositionFromTruncatedPosition(p1);
//		string[] p2 = p1.Split(','); // ["123","123","123"]
//		Vector3 pos = new Vector3(int.Parse(p2[0])/100f,int.Parse(p2[1])/100f,int.Parse(p2[2])/100f);
//		return pos;
	}

	public static Quaternion GetRealRotationFromJsonRotation(SimpleJSON.JSONClass N){
		bool debugRot = false;
		string r1 = N["rotation"].Value;
		string[] r2 = r1.Split(',');
		if (debugRot) WebGLComm.inst.Debug("<color=#0f0>rotating:</color>"+r1);
		Quaternion rot = Quaternion.identity;
		if (JsonLevelLoader.inst.thisLevelVersion > 1.10f){
			if (debugRot) WebGLComm.inst.Debug("rot >1.10f");
			// We started implementing Euler rotations in 1.01f and higher
			// quaternion floats were not saving accurately enough e.g. 179 would become 180
			rot = new Quaternion();
			rot.eulerAngles = new Vector3(int.Parse(r2[0]),int.Parse(r2[1]),int.Parse(r2[2]));
		} else if (JsonLevelLoader.inst.thisLevelVersion == 1.1f || r1.Contains(".")){
			if (debugRot) WebGLComm.inst.Debug("rot v1.1");
			rot = new Quaternion(float.Parse(r2[0]),float.Parse(r2[1]),float.Parse(r2[2]),float.Parse(r2[3]));
		} else {
			if (r2.Length == 4){
				if (debugRot) WebGLComm.inst.Debug("rot v1.0 with 4");
				rot = new Quaternion(float.Parse(r2[0])/10f,float.Parse(r2[1])/10f,float.Parse(r2[2])/10f,float.Parse(r2[3])/10f);
			} else {
				if (debugRot) WebGLComm.inst.Debug("rot v1.0 with 3");
				rot = new Quaternion();
				rot.eulerAngles = new Vector3(int.Parse(r2[0]),int.Parse(r2[1]),int.Parse(r2[2]));	
			}
		}
		if (rot == Quaternion.identity) {
			if (debugRot) WebGLComm.inst.Debug("Quat ident..");
		} else {
//			Debug.Log("Quat reg..");
		}
		return rot;
	}
	public static SimpleJSON.JSONClass ConvertFractionToJson(string fracKey, Fraction frac, SimpleJSON.JSONClass N = null){ // actually APPENDS fraction to an empty root level Json object ..hmm
		if (N == null) N = new SimpleJSON.JSONClass();
		N[fracKey][Fraction.numeratorKey].AsInt = frac.numerator;
		N[fracKey][Fraction.denominatorKey].AsInt = frac.denominator;
		//		// commented Debug.Log("set json frac:"+N.ToString());
		return N;
	}

	public static Fraction ConvertJsonToFraction(string fracKey, SimpleJSON.JSONClass N) {  // Are we getting the full JSON or just the Properties part? hm..
		
		if (N != null && N.GetKeys().Contains(fracKey)) {
			//			// commented Debug.Log("totally frackey:"+fracKey+", whereas numerator for this frackey;"+N[fracKey][numeratorKey].AsInt.ToString());
			return new Fraction(N[fracKey][Fraction.numeratorKey].AsInt,N[fracKey][Fraction.denominatorKey].AsInt);
		}
		else {
			//			// commented Debug.Log("no frac key on:"+N.ToString());
//			if (fracKey == fractionSeqAKey || fracKey == fractionSeqBKey) return new Fraction(0,1);
			return new Fraction(1,1);
//			return null;
		}
	}

	public static string R = "r";
	public static string G = "g";
	public static string B = "b";
	public static string colorKey = "col";

	public static SimpleJSON.JSONClass ConvertColorToJson(Color c){
		SimpleJSON.JSONClass C = new SimpleJSON.JSONClass();
//		C[colorKey] = new SimpleJSON.JSONClass();
		C[colorKey][R].AsInt = Mathf.RoundToInt(c.r * 100);
		C[colorKey][G].AsInt = Mathf.RoundToInt(c.g * 100);
		C[colorKey][B].AsInt = Mathf.RoundToInt(c.b * 100);
		return C;
	}

	public static Color ConvertJsonToColor(SimpleJSON.JSONClass C){
		return new Color(C[colorKey][R].AsInt/100f,C[colorKey][G].AsInt/100f,C[colorKey][B].AsInt/100f,1);
	}

	public static int GetScaleAsInt(Transform t){
		int scale = Mathf.RoundToInt(t.localScale.x*100);
		return scale;
	}

	public static Vector3 GetScaleFromInt(int i){
		return Vector3.one * i/100f;
	}

	// GetExtraProperties, GetFullProeperties, GetDeepProperties
	public static SimpleJSON.JSONClass GetUeoBaseProps(SimpleJSON.JSONClass obj, UserEditableObject ueo,SceneSerializationType serializationType){
		// oops -- we are setting "all" the properties including rotation and position. This SHOULD be warpped in USerEditableObject but we didn't do it that way
		// and a refactor would risk breaking existing levels, 
		// so for now we leave ueo.setprops / ueo.getprops as a "higher level" properties, where as THIS BaseProps is the "full" properties of the object
		if (obj == null){
			Debug.LogError("null obj");
			return obj;
		} else if (ueo == null){
			Debug.LogError("ueo null for "+obj["name"]+ ", str;"+obj.ToString());
			return obj;
		} 
		obj["name"] = ueo.GetName;
		obj["position"] = JsonUtil.GetTruncatedPosition(ueo.transform);
		obj["rotation"] = JsonUtil.GetRotation(ueo.transform);
		obj["active"].AsBool = ueo.gameObject.activeSelf;
		if (serializationType == SceneSerializationType.Instance) {
			obj[JsonUtil.scaleKey].AsInt = JsonUtil.GetScaleAsInt(ueo.transform); 
		}
		obj["properties"] = ueo.GetProperties();
		return obj;
	}

	public static string GetStringFromJson(SimpleJSON.JSONClass N){
		return N.SaveToBase64();
//		return N.ToString();
	}

	public static SimpleJSON.JSONClass GetJsonFromString(string s){
		if (s.Substring(0,10).Contains("{")) {
			// base 64 wont have this
			return (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(s);
		} else {
			return (SimpleJSON.JSONClass)SimpleJSON.JSONNode.LoadFromBase64(s);
			
		}
//		if (System.Text.RegularExpressions.Regex.IsMatch(s,"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$")){
//		} else {
//		}
	}

}
