/**
	Copyright (C) 2019 NyangireWorks. All Rights Reserved.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerHomework
{

	public class FadeObject : MonoBehaviour {

		private float opacity = 1f;

		//Renderer renderer;

		private Color c;

		private float fadeOutSpeed = 10f;
		public float FadeOutSpeed
		{
			get { return fadeOutSpeed; }
			set { 
					if( value < 0f)
					{
						fadeOutSpeed = 0f;
					}
					else
					{
						fadeOutSpeed = value; 
					}
				}
		}

		private float fadeInSpeed = 10f;
		public float FadeInSpeed
		{
			get { return fadeInSpeed; }
			set { 
					if( value < 0f)
					{
						fadeInSpeed = 0f;
					}
					else
					{
						fadeInSpeed = value; 
					}
				}
		}

		private Shader fadeShader;
		public Shader FadeShader
		{
			get { return fadeShader; }
			set { fadeShader = value; }
		}

		private float targetTransparency;

		private float originalTransparency;

		private Material originalMaterial;
		private Shader originalShader;
		//private BlendMode originalRenderMode;

		private bool fadeIn = true;

		private bool debugOn = false;
		public bool DebugOn 
		{
			get { return debugOn; }
			set { debugOn = value; }
		}

		// Use this for initialization
		void Start () {
			//renderer = this.GetComponent<Renderer> ();

			originalMaterial = GetComponent<Renderer>().sharedMaterial;
			Material mat = GetComponent<Renderer>().material;
			originalTransparency = mat.color.a;

			if(fadeShader)
			{
				originalShader = mat.shader;
				mat.shader = fadeShader;
			}
			
			// mat.SetFloat("_Mode", 2);
			// mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			// mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// mat.SetInt("_ZWrite", 0);
			// mat.DisableKeyword("_ALPHATEST_ON");
			// mat.EnableKeyword("_ALPHABLEND_ON");
			// mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			// mat.renderQueue = 3000;

		}
		
		// Update is called once per frame
		void Update () {

			//FadeIn
			if(fadeIn)
			{
				if(fadeInSpeed == 0)
				{
					opacity = targetTransparency;
				}
				else
				{
					opacity -= ((1.0f-targetTransparency)*Time.deltaTime) * fadeInSpeed;	
				}
				//reached target transparency
				if(opacity <= targetTransparency)
				{
					opacity = targetTransparency;
					fadeIn = false;
				}
				Color C = GetComponent<Renderer>().material.color;
				C.a = opacity;
				GetComponent<Renderer>().material.color = C;
			}
			
			//FadeOut
			else
			{
				if (opacity < originalTransparency)
				{
					Color C = GetComponent<Renderer>().material.color;
					C.a = opacity;
					GetComponent<Renderer>().material.color = C;
				}
				else
				{
					Material mat = GetComponent<Renderer>().material;

					Color c = GetComponent<Renderer>().material.color;
					c.a = originalTransparency;
					

					if(fadeShader)
					{
						mat.shader = originalShader;
						mat.SetColor(originalShader.name, c);
						//mat.EnableKeyword("_ALPHABLEND_ON");
						GetComponent<Renderer>().material = originalMaterial;
					}
					else
					{
						mat.color = c;
					}

					

					// mat.SetFloat("_Mode", 0);
					// mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					// mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					// mat.SetInt("_ZWrite", 1);
					// mat.DisableKeyword("_ALPHATEST_ON");
					// mat.DisableKeyword("_ALPHABLEND_ON");
					// mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					// mat.renderQueue = 2000;

					// And remove this script
					Destroy(this);
				}

				if(fadeOutSpeed == 0)
				{
					opacity = originalTransparency;
				}
				else
				{
					opacity += ((1.0f-targetTransparency)*Time.deltaTime) * fadeOutSpeed;
				}
			}


			if(debugOn)
			{
				UnityEngine.Debug.Log("opacity: " + opacity);
			}
		
		}

		public void SetTransparency(float newTP){
			targetTransparency = newTP;
			fadeIn = true;
			//opacity = targetTransparency;
		}

	}
}