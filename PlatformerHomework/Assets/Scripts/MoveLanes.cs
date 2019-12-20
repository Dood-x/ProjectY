using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLanes : MonoBehaviour
{
    public GameObject messageIn;
    public GameObject messageOut;
    bool active = false;
    bool activated = false;
    Player playerScript;
    public Transform lane1;
    public Transform lane2;

    public bool lockCameraDistance = false;

    int laneIndex;
    // Start is called before the first frame update
    void Start()
    {
        if (messageIn)
            messageIn.SetActive(false);
        if(messageOut)
            messageOut.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (!activated && active)
        {
            activated = Input.GetKeyDown(laneIndex == 0 ? KeyCode.W : KeyCode.S);

           
            float dist1 = (lane1.position - playerScript.transform.position).sqrMagnitude;
            float dist2 = (lane2.position - playerScript.transform.position).sqrMagnitude;

            laneIndex = dist1 > dist2 ? 1 : 0;

            Transform laneStart = laneIndex == 0 ? lane1 : lane2;
            Transform laneEnd = laneIndex == 0 ? lane2 : lane1;

            if (!playerScript.DisableInput)
            {
                //activate tutorial of closer lane

                if (messageIn && messageOut)
                {
                    GameObject msg1 = laneIndex == 0 ? messageIn : messageOut;
                    if (msg1)
                        msg1.SetActive(true);

                    GameObject msg2 = laneIndex == 1 ? messageIn : messageOut;
                    if (msg2)
                        msg2.SetActive(false);
                }

                   

            }

            if (activated && playerScript.Cc.isGrounded)
            {

                //move the character to th other lane
                Vector3 destinaiton = playerScript.gameObject.transform.position;

                destinaiton.x = laneEnd.position.x;

                //Vector3 moveDir = (laneEnd.position - laneStart.position).normalized;

                playerScript.MoveToLane(destinaiton, this);


            }

            activated = false;

        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            // check which lane the player is on (whitch lane marker is closest)
            laneIndex = 0;

            float dist1 = (lane1.position - other.transform.position).sqrMagnitude;
            float dist2 = (lane2.position - other.transform.position).sqrMagnitude;
            playerScript = other.gameObject.GetComponent<Player>();

            laneIndex = dist1 > dist2 ? 1 : 0;
            if (messageIn && messageOut)
            {
                GameObject msg = laneIndex == 0 ? messageIn : messageOut;

                if (!activated && msg)
                {
                    msg.SetActive(true);
                }
            }

            active = true;
        }


    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (messageIn && messageOut)
            {
                GameObject msg = laneIndex == 0 ? messageIn : messageOut;
                if (msg)
                    msg.SetActive(false);
            }

            active = false;
        }
    }

}