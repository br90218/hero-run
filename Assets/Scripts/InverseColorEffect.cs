using UnityEngine;
using System.Collections;

public class InverseColorEffect: MonoBehaviour
{
	[SerializeField] private Material m_Material;

	public float ControlValue;

	void Start ()
	{  
		//ControlValue = 1f;
		if (!m_Material) {
			Debug.LogWarning (gameObject.name + ": Material is not assigned. Disabling image effect.", this.gameObject);
			enabled = false;
		}
	}

	void OnRenderImage (RenderTexture src, RenderTexture dst)
	{
		if (m_Material) {
			m_Material.SetFloat ("_InvertMultiplier", ControlValue);
			Graphics.Blit (src, dst, m_Material);

		} else {
			Graphics.Blit (src, dst);
			Debug.LogWarning (gameObject.name + ": Shader is not assigned. Disabling image effect.", this.gameObject);
			enabled = false;
		}
	}
		
}