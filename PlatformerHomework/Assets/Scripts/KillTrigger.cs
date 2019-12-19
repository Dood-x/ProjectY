using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player playerScript = other.gameObject.GetComponent<Player>();

            if (playerScript)
            {
                playerScript.OutOfBounds();
            }
        }
    }
}
