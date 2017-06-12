using UnityEngine;
using System.Collections;

public class InverseColorEffect: MonoBehaviour {
	public Shader m_Shader = null;
	private Material m_Material;

	void Start()
	{  
		if (m_Shader)
		{
			m_Material = new Material(m_Shader);
			m_Material.name = "ImageEffectMaterial";
			m_Material.hideFlags = HideFlags.HideAndDontSave;
		}

		else
		{
			Debug.LogWarning(gameObject.name + ": Shader is not assigned. Disabling image effect.", this.gameObject);
			enabled = false;
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (m_Shader && m_Material)
		{
			Graphics.Blit(src, dst, m_Material);
		}

		else
		{
			Graphics.Blit(src, dst);
			Debug.LogWarning(gameObject.name + ": Shader is not assigned. Disabling image effect.", this.gameObject);
			enabled = false;
		}
	}

	void OnDisable()
	{
		if (m_Material)
		{
			DestroyImmediate(m_Material);
		}
	}
}