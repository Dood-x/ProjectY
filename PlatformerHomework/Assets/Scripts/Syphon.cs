using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syphon : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float acceleration = 10f;

    float speed = 0;

    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //go after the player
        if (speed < maxSpeed)
        {
            // accelerate the speed to maxSpeed
            speed += acceleration * Time.fixedDeltaTime;
        }
        // move thowards player
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;

        
    }

}
