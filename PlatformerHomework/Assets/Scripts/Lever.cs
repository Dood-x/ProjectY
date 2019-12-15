using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lever : MonoBehaviour
{

    public GameObject portcullis;
    GameObject message;
    bool active = false;
    bool activated = false;
    Animator leverAnim;
    Animator portcullisAnim;
    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro text = GetComponentInChildren<TextMeshPro>();
        if (text)
        {
            message = text.gameObject;
            message.SetActive(false);
        }

        leverAnim = GetComponent<Animator>();

        if(portcullis)
            portcullisAnim = portcullis.GetComponent<Animator>();

        // the animation of the portcullis opening is paused
        if(portcullisAnim)
            portcullisAnim.speed = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (!activated && active)
        {
            activated = Input.GetKeyDown(KeyCode.E);
            if (activated)
            {
                //play animation
                leverAnim.SetTrigger("Activated");
                //play portcullis opening in a coroutine
                StartCoroutine("PortcullisOpen");
                //remove tutorial is there is one
                if (message)
                    message.SetActive(false);

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (message && !activated)
            {
                message.SetActive(true);
            }
            active = true;
        }


    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        { 
            if (message)
            {
                message.SetActive(false);
            }
            active = false;
        }
    }

    IEnumerator PortcullisOpen()
    {
        // lever has an animation intro before starting to open the portcullis
        yield return new WaitForSeconds(1f);
        Debug.Log("Opening");
        // start the portcullis opening animaiton
        if (portcullisAnim)
            portcullisAnim.speed = 1;

    }
}
