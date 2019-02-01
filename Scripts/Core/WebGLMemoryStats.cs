using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLMemoryStats : MonoBehaviour {
	[DllImport("__Internal")]
	private static extern uint GetTotalMemorySize();

	[DllImport("__Internal")]
	private static extern uint GetTotalStackSize();

	[DllImport("__Internal")]
	private static extern uint GetStaticMemorySize();

	[DllImport("__Internal")]
	private static extern uint GetDynamicMemorySize();
}