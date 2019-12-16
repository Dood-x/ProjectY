using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDissapearing : MonoBehaviour
{

    public float timeToRed = 3f;
    public float timeToDissapear = 1f;
    public Material redMaterial;
    public Material fadedMaterial;
    public float materialFadeTime = 0.5f;
    public float dissapearFadeTime = 0.5f;

    bool materialChanged;

    float timePassed;
    float timeMaterialChangeStart;
    Renderer rend;
    Material ogMaterial;

    float timeFadingStart;


    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        ogMaterial = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > timeToRed)
        {
            //change material to red
            if (redMaterial != null && !materialChanged)
            {
                if (timeMaterialChangeStart == 0)
                {
                    timeMaterialChangeStart = Time.time;
                }
                // from 0 to 1 during the materialfadetime
                float lerp = (Time.time - timeMaterialChangeStart) / materialFadeTime;
                //lerp material instance on the gameobject
                rend.material.Lerp(ogMaterial, redMaterial, lerp);

                if (lerp >= 1)
                    materialChanged = true;

                //gameObject.GetComponent<MeshRenderer>().material = redMaterial;
                //materialChanged = true;                
            }
            // once the object starts to dissapear
            if (timePassed > timeToRed + timeToDissapear)
            {
                if (fadedMaterial != null)
                {
                    //fade and then destroy
                    if (timeFadingStart == 0)
                    {
                        timeFadingStart = Time.time;
                    }

                    float lerp = (Time.time - timeFadingStart) / dissapearFadeTime;
                    rend.material.Lerp(redMaterial, fadedMaterial, lerp);

                    if (lerp >= 1)
                    {
                        // destory object once its transparent
                        Destroy(gameObject);
                    }

                }
                else
                {
                    // if we dont have a fading material just destroy the object instantly
                    Destroy(gameObject);
                }
            }
        }
    }
}
