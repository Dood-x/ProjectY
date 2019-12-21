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

		private float[] originalTransparency;

		private Material[] originalMaterial;
		private Shader[] originalShader;
		//private BlendMode originalRenderMode;

		private bool fadeIn = true;

		private bool debugOn = false;
		public bool DebugOn 
		{
			get { return debugOn; }
			set { debugOn = value; }
		}

        private bool[] fadeOutComplete;

		// Use this for initialization
		void Start () {
			//renderer = this.GetComponent<Renderer> ();

			originalMaterial = GetComponent<Renderer>().sharedMaterials;
			Material[] mat = GetComponent<Renderer>().materials;

            fadeOutComplete = new bool[mat.Length];

            originalTransparency = new float[mat.Length];
            originalShader = new Shader[mat.Length];

            for (int i = 0; i < mat.Length; i++)
            {
			    originalTransparency[i] = mat[i].color.a;

                if (fadeShader)
                {
                    originalShader[i] = mat[i].shader;
                    mat[i].shader = fadeShader;
                }
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

                Material[] mats = GetComponent<Renderer>().materials;

                for (int i = 0; i < mats.Length; i++)
                {
                    Color C = mats[i].color;
                    C.a = opacity;
                    GetComponent<Renderer>().materials[i].color = C;
                }
				
			}
			
			//FadeOut
			else
			{
                Material[] mats = GetComponent<Renderer>().materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (opacity < originalTransparency[i])
                    {
                        Color C = mats[i].color;
                        C.a = opacity;
                        GetComponent<Renderer>().materials[i].color = C;
                    }
                    else if(!fadeOutComplete[i])
                    {
                        Material mat = GetComponent<Renderer>().materials[i];

                        Color c = GetComponent<Renderer>().materials[i].color;
                        c.a = originalTransparency[i];


                        if (fadeShader)
                        {
                            mat.shader = originalShader[i];
                            mat.SetColor(originalShader[i].name, c);
                            //mat.EnableKeyword("_ALPHABLEND_ON");
                            GetComponent<Renderer>().materials[i] = originalMaterial[i];

                            fadeOutComplete[i] = true;
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

                        
                    }

                    if (fadeOutSpeed == 0)
                    {
                        opacity = originalTransparency[i];
                    }
                    else
                    {
                        opacity += ((1.0f - targetTransparency) * Time.deltaTime) * fadeOutSpeed;
                    }


                   

                }

                // check if everyhting returned to normal
                bool allClear = true;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (!fadeOutComplete[i])
                        allClear = false;
                }
                // And remove this script
                if(allClear)
                    Destroy(this);

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