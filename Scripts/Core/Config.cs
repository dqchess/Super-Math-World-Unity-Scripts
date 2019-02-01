using UnityEngine;
using System.Collections;

public class Config
{

#if UNITY_WEBGL
//	public static string serverBaseUrl = "127.0.0.1:8000";
	public static string serverBaseUrl = "https://supermathworld.com/";
#else
	public static string serverBaseUrl = "https://supermathworld.com/";
#endif
//		public static string serverBaseUrl = "127.0.0.1:8000/";
	//	public static string versionNumber = "1.0.0.0"; # Base product we launched in March. Had lots of problems [ like Level 3 was broken, Wrong error codes for student logins, Crash on Mac after 50 minutes gameplay.
	//	public static string versionNumber = "1.0.0.1"; // Updated on Thurs March 5 2015 -- but forgot to set serverbaseurl to .com
	//	public static string versionNumber = "1.0.0.2"; // Updated on Sat March 7 2015 -- but left as development version
//	public static string versionNumber = "1.0.0.3"; // Updated on March 8 2015 -- bees improved, 
//	public static string versionNumber = "1.0.0.4"; // Mar 11 2015 -- maybe fixed WWW crash, Windows now signed.
//	public static string versionNumber = "1.0.0.4a"; // Mar 28 2015 -- ipad build, fixed many things, mg fires too fast?
//	public static string versionNumber = "1.0.0.5"; // Mar 28 2015 -- ipad build, fixed many things, mg fires too fast?
	public static string versionNumber = "1.0.0.6"; // Mar 28 2015 -- Random crash due to web 504 error when jsonparser. Probably fixed. Rearranged project, broke some materials, everything seems fixed/better than before.


	public static string buildTarget = "Standalone";
//	public static string buildTarget = "WebGL";
}

