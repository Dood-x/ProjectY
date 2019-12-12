using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{

    public float damageAmount = 10f;
    public float minDamagingVelocity = 0f;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    public void OnTriggerEnter(Collider other)
    {
        //Collider other = collision.collider;
        if (other.gameObject.tag == "Player")
        {
            Player playerScript = other.gameObject.GetComponent<Player>();
            if (minDamagingVelocity == 0 || rb.velocity.magnitude > minDamagingVelocity)
            {
                if (playerScript)
                {
                    playerScript.TakeDamage(damageAmount);
                }
            }
        }
    }
}
