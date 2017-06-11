#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class FogVolumeCreator : Editor {


	[UnityEditor.MenuItem("GameObject/Create Other/Fog Volume")]
	static public void CreateFogVolume()
	{
		GameObject FogVolume = new GameObject();
        //Icon stuff
        Texture Icon = Resources.Load("FogVolumeIcon") as Texture;
        //Icon.hideFlags = HideFlags.HideAndDontSave;
        var editorGUI = typeof(EditorGUIUtility);
        var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        var args = new object[] { FogVolume, Icon };
        editorGUI.InvokeMember("SetIconForObject", bindingFlags, null, null, args);

		FogVolume.name = "Fog Volume";
		FogVolume.AddComponent <MeshRenderer>();
		FogVolume.AddComponent <FogVolume>();
		FogVolume.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		FogVolume.GetComponent<Renderer>().receiveShadows = false;
        FogVolume.GetComponent<Renderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        FogVolume.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        Selection.activeObject = FogVolume;
		if (UnityEditor.SceneView.currentDrawingSceneView) UnityEditor.SceneView.currentDrawingSceneView.MoveToView(FogVolume.transform);
	}

	

}
#endif