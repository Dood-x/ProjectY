using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDissapearing : MonoBehaviour
{

    public float timeToRed = 3f;
    public float timeToDissapear = 1f;
    public Material ogMaterial;
    public Material redMaterial;
    public Material redFadedMaterial;
    public Material ogFadedMaterial;
    public float materialFadeTime = 0.5f;
    public float dissapearFadeTime = 0.5f;

    bool materialChanged;

    float timePassed;
    float timeMaterialChangeStart;
    Renderer rend;
    Renderer[] childRend;
    //Material ogMaterial;

    float timeFadingStart;

    [Header("Respawn")]

    public bool respawn = true;
    public float timeToRespawn = 4f;

    bool deactivateUpdate;
    float respawnFade;
    bool fadeToNormal = false;


    bool isRespawning = false;
    public bool IsRespawning
    {
        get { return isRespawning; }
        private set { isRespawning = IsRespawning; }
    }

    List<Material> newOgMaterials;
    List<Material> newOgFadedMaterials;


    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        //ogMaterial = gameObject.GetComponent<MeshRenderer>().material;
        childRend = gameObject.GetComponentsInChildren<Renderer>();

        if (!ogMaterial)
        {
            ogMaterial = (Material)Resources.Load("Sigil", typeof(Material));
        }

        if (!redMaterial)
        {
            redMaterial = (Material)Resources.Load("SigilRed", typeof(Material));
        }

        if (!redFadedMaterial)
        {
            redFadedMaterial = (Material)Resources.Load("SigilRedFaded", typeof(Material));
        }

        if (!ogFadedMaterial)
        {
            ogFadedMaterial = (Material)Resources.Load("SigilWhiteFaded", typeof(Material));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (deactivateUpdate)
        {
            return;
        }

        if (fadeToNormal)
        {
            if (respawnFade == 0)
            {
                respawnFade = Time.time;

                newOgFadedMaterials = new List<Material>();
                newOgMaterials = new List<Material>();

                for (int i = 0; i < childRend.Length; i++)
                {
                    //childRend[i].material.EnableKeyword("_EMISSION");
                    //childRend[i].material.EnableKeyword("_ALPHABLEND_ON");
                    newOgFadedMaterials.Add(new Material(ogFadedMaterial));
                    childRend[i].material = newOgFadedMaterials[i];
                    //childRend[i].UpdateGIMaterials();
                    newOgMaterials.Add(new Material(ogMaterial));
                }
                //deactivateUpdate = true;
                //return;
            }

            float lerp = (Time.time - respawnFade) / materialFadeTime;
            // from 0 to 1 during the materialfadetime
            //float lerp = respawnFade / materialFadeTime;
            //lerp material instance on the gameobject
            for (int i = 0; i < childRend.Length; i++)
            {
                childRend[i].material.Lerp(childRend[i].material, newOgMaterials[i], lerp);
                //childRend[i].UpdateGIMaterials();
            }



            if (lerp >= 1)
            {
                fadeToNormal = false;
                timePassed = 0f;
                deactivateUpdate = true;
                materialChanged = false;
                //destory the script
                Destroy(this);
            }

            //respawnFade += Time.deltaTime;
            return;
        }

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
                for(int i = 0; i < childRend.Length; i++)
                    childRend[i].material.Lerp(ogMaterial, redMaterial, lerp);

                if (lerp >= 1)
                    materialChanged = true;

                //gameObject.GetComponent<MeshRenderer>().material = redMaterial;
                //materialChanged = true;                
            }
            // once the object starts to dissapear
            if (timePassed > timeToRed + timeToDissapear)
            {
                if (redFadedMaterial != null)
                {
                    //fade and then destroy
                    if (timeFadingStart == 0)
                    {
                        timeFadingStart = Time.time;
                    }

                    float lerp = (Time.time - timeFadingStart) / dissapearFadeTime;
                    rend.material.Lerp(redMaterial, redFadedMaterial, lerp);
                    for (int i = 0; i < childRend.Length; i++)
                        childRend[i].material.Lerp(redMaterial, redFadedMaterial, lerp);

                    if (lerp >= 1)
                    {
                        // destory object once its transparent
                        DeactivatePlatform();
                    }

                }
                else
                {
                    // if we dont have a fading material just destroy the object instantly
                    DeactivatePlatform();
                }
            }
        }
    }

    void DeactivatePlatform()
    {

        if (!respawn)
        {
            Destroy(gameObject);
            return;
        }


        deactivateUpdate = true;


        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc)
            bc.enabled = false;


        StartCoroutine("RespawnPlatform");
    }

    void ActivatePlatform()
    {

        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc)
            bc.enabled = true;

        isRespawning = true;

    }

    IEnumerator RespawnPlatform()
    {
        yield return new WaitForSeconds(timeToRespawn);
        ActivatePlatform();

        fadeToNormal = true;
        deactivateUpdate = false;


        //destory the script!
        // the player will instance another script when activating the platform
        //Destroy(this);
    }

}
