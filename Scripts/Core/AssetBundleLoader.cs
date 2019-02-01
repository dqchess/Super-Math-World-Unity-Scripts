using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Application.ExternalEval("LoadAssetBundle()"); // because we need the *url* of the asset bundle which is dynamic and depends on the build name e.g. build_jan_26_01
	}

	public void LoadAssetBundleCallback(string assetBundleRootUrl){
		StartCoroutine (LoadAssetBundle(assetBundleRootUrl + "webgl_03.assetbundle"));
	}

	IEnumerator LoadAssetBundle(string url){
		while (!Caching.ready)
			yield return null;
		// Start a download of the given URL
		WWW www = WWW.LoadFromCacheOrDownload (url, 1);

		// Wait for download to complete
		yield return www;

		// Load and retrieve the AssetBundle
		AssetBundle bundle = www.assetBundle;

		// Load the object asynchronously
		AssetBundleRequest request = bundle.LoadAssetAsync ("myObject", typeof(GameObject));

		// Wait for completion
		yield return request;

		// Get the reference to the loaded object
		GameObject obj = request.asset as GameObject;

		// Unload the AssetBundles compressed contents to conserve memory
		bundle.Unload(false);

		// Frees the memory from the web stream
		www.Dispose();
		WebGLComm.inst.Debug("Asset bundle loaded!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
