using UnityEngine;
using System.Collections;


public class GameConfig : MonoBehaviour {

	public static int mainCameraFieldOfView = 60;
	public static string websiteRoot = "https://supermathworld.com/";
	public static Vector2 screenResolution = new Vector2(1024,576);
	public static Color juneYellow = new Color(0.976f,0.8f,0,1); // F8CC00FF
	public static Color juneYellowTransparent = new Color(0.976f,0.8f,0,0); 
	public static Color darkGrayTransparent = new Color(0.2f,0.2f,0.2f,0);
	public static Color darkGray = new Color(0.2f,0.2f,0.2f,1);
	public static float globalWaterHeight = 41f;
	public enum PlayMode {
		a2D,
		a3D

	}
	
	public static GameConfig inst;
	public PlayMode playMode = PlayMode.a3D;
	public float numberScale = 2f; // oops, this is also in NumberManager!

	// Use this for initialization
	void Start () {
		inst = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
