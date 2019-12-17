using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject spawn;
    public Vector3 spawnRotation;
    public float interval = 3f;

    public Vector3 spawnLaunchSpeed;

    public bool despawnSpawned = true;
    public float despawnTime = 7f;


    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if(spawn == null)
        {
            Destroy(this);
        }
        // we start by spawingin the first object at frame 1
        timer = interval;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer -= interval;

            //spawn 
            GameObject spawned = Instantiate(spawn, transform.position, Quaternion.Euler(spawnRotation));

            if (spawned)
            {
                DamagePlayer dp = spawned.GetComponent<DamagePlayer>();

                if (dp)
                {
                    dp.despawnAfterTime = despawnSpawned;
                    dp.despawnTime = despawnTime;
                }


                Rigidbody rb = spawned.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.velocity = spawnLaunchSpeed;
                }
            }
            

        }
    }
}
