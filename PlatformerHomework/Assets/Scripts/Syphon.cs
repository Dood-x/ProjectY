using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syphon : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float acceleration = 10f;

    float speed = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider Other)
    {
        if (Other.gameObject.tag == "Player")
        {
            //go after the player
            if(speed < maxSpeed)
            {
                speed += acceleration * Time.fixedDeltaTime;
            }

        }
    }

}
