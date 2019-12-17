using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syphon : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float acceleration = 10f;

    float speed = 0;
    Rigidbody rb;

    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        //if (!player)
        //{
        //    player = GameObject.FindGameObjectWithTag("Player").transform;
        //}
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //go after the player
        if (speed < maxSpeed)
        {
            // accelerate the speed to maxSpeed
            speed += acceleration * Time.fixedDeltaTime;
        }
        // move thowards player
        //Vector3 direction = player.position - transform.position;
        //direction.Normalize();
        //transform.position += direction * speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        //Debug.Log(transform.position - player.position);

        //if (rb)
        //{
        //    //rb.velocity = rb.velocity * speed * Time.fixedDeltaTime;
        //    rb.MovePosition(player.position);
        //}

        Debug.DrawLine(transform.position, player.position, Color.red);
        
    }


}
