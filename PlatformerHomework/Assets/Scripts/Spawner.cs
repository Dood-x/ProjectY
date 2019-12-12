using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject spawn;
    public Vector3 spawnRotation;
    public float interval = 3f;



    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if(spawn == null)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer -= interval;

            //spawn 
            Instantiate(spawn, transform.position, Quaternion.Euler(spawnRotation));

        }
    }
}
