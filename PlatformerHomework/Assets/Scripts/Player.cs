using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformerHomework;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    private CharacterController cc;
    public PlatformerCamera camScript;
    private Animator animator;

    public Slider healthSlider;
    public Text crystalsTxt;

   // public Syphon syphonScript;


    private Camera cam;
    [Header("Stats")]

    public float walkSpeed = 1.5f;
    public float runSpeed = 6f;
    public float rotSpeed = 20f;
    public float jumpSpeed = 5f;
    public float maxHealth = 100f;
    public int maxJumpAmount = 2;

    [Header("Out of bounds")]

    public float yKillHeight = -100f;
    public float outOfBoundsDamage = 30f;

    [Header("Physics")]
    public float gravity = 9.81f;
    public float gravityAccelOnGround = -0.3f;

    public float raycastLength;
    public LayerMask groundRaycastLayer;
    public float impulsePlatformSpeed = 20f;
    bool impulseLeap = false;


    float vSpeed = 0f;
    int jumpAmount = 0;

    Vector3 moveDirection;
    float health;
    int crystals;
    int maxCrystals;

    SphereCollider syphonCollider;

    MovingPlatform currentPlatform;

    private struct RespawnPoint
    {
        public Vector3 position;
        public Quaternion lookDirection;

        public RespawnPoint(Vector3 pos, Quaternion lookDir)
        {
            position = pos;
            lookDirection = lookDir;
        }
    }

    List<RespawnPoint> checkpoints = new List<RespawnPoint>();

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (camScript != null)
        {
            cam = camScript.gameObject.GetComponent<Camera>();
        }
        else
        {
            cam = Camera.main;
        }

        moveDirection = Vector3.zero;
        health = maxHealth;
        maxCrystals = GameObject.FindGameObjectsWithTag("Crystal").Length;
        RespawnPoint startPoint = new RespawnPoint(this.transform.position, this.transform.localRotation);
        checkpoints.Add(startPoint);
        healthSlider.value = health/maxHealth;
        WriteCrystals();
        syphonCollider = GetComponentInChildren<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {

        

        // movement input
        //float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Horizontal");
        float h = 0;

        float animSpeed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

        animator.SetFloat("Speed", animSpeed);
        animator.SetBool("Grounded", cc.isGrounded);
        //Vector3 moveDirection = Vector3.zero;

        //if the player falls to y kill height kill the player
        if (this.transform.position.y < yKillHeight)
        {
            OutOfBounds();
            return;
        }

        if (animSpeed > 0.1f)
        {
            Vector3 moveHorizontal = new Vector3(h, 0, v);
            // direction on the XZ plane determined by the inputs
            // make the direction value lie on a unit circle
            moveHorizontal.Normalize();
            // the strength of the movement is as big as the highest input
            moveHorizontal *= Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

            // buffer, input should exceed 0.1 for the character to move
            if (animSpeed < 0.1)
            {
                moveHorizontal = Vector3.zero;
            }


            // rotate the direction by the cameras local y rotation
            //moveHorizontal = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * moveDirection;

            // we linearly interpolate between the walk and run speeds using the animation factor thats in the 0-1 interval
            float movementSpeed = Mathf.Lerp(walkSpeed, runSpeed, animSpeed);

            moveHorizontal *= Time.deltaTime * movementSpeed;

            // move the character
            //cc.Move(moveHorizontal);

            //turn the movement direction into a rotation
            Quaternion rotation = Quaternion.LookRotation(moveHorizontal.normalized);
            // rotate the characetr in the direction of movement
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);

            moveDirection.x = moveHorizontal.x;
            moveDirection.z = moveHorizontal.z;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        //switches the camera look offset depending on whether we are going forward or back
        //SwitchDirection(v); ubija me, ne treba za sad


        if (cc.isGrounded && !impulseLeap)
        {
            jumpAmount = 0;
            moveDirection.y = 0f;
            vSpeed = gravityAccelOnGround;
        }
        else
        {
            vSpeed -= gravity * Time.deltaTime;
            // speed of movement * time is the amount we need to move the character

        }

        //jumping allowes double jump
        if (Input.GetButtonDown("Jump") && jumpAmount < maxJumpAmount)
        {
            animator.SetBool("Leap", true);
        }

        moveDirection.y = vSpeed * Time.deltaTime;

        cc.Move(Vector3.forward * 0.001f);

        // move the character!
        cc.Move(moveDirection);

        //check for impulse platforms
        //CheckGround();

    }

    public void LateUpdate()
    {
        CheckGround();
    }


    void SwitchDirection(float v)
    {
        Vector3 lookOffset = camScript.Lookoffset;

        if (v > 0)
        {
            lookOffset.z = Mathf.Abs(lookOffset.z);
            camScript.Lookoffset = lookOffset;
        }
        else if (v < 0)
        {
            lookOffset.z = -Mathf.Abs(lookOffset.z);
            camScript.Lookoffset = lookOffset;
        }
    }

    public void Leap()
    {
        vSpeed = jumpSpeed;
        jumpAmount++;
        cc.Move(Vector3.up * vSpeed * Time.deltaTime);
        animator.SetBool("Leap", false);

    }
    public void ImpulseLeapEnd()
    {
        animator.SetBool("ImpulseLeap", false);
        impulseLeap = false;
    }

    public void OutOfBounds()
    {
        TakeDamage(outOfBoundsDamage);
        //respawn
        Respawn();

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health/maxHealth;
        if (health <= 0)
        {
            //death animation
            //Respawn();
        }
    }

    public void Respawn()
    {
        if (health > 0)
        {
            this.transform.position = checkpoints[checkpoints.Count - 1].position;
            this.transform.localRotation = checkpoints[checkpoints.Count - 1].lookDirection;
        }
        else
        {
            this.transform.position = checkpoints[0].position;
            this.transform.localRotation = checkpoints[0].lookDirection;
            // respawn with full health
            health = maxHealth;
            healthSlider.value = health;
        }
    }

    void WriteCrystals()
    {
        crystalsTxt.text = "Crystals: " + crystals + "/" + maxCrystals;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            checkpoints.Add(new RespawnPoint(other.transform.position, other.transform.localRotation));
        }

        if (other.gameObject.tag == "Crystal")
        {
            crystals++;
            WriteCrystals();
            Destroy(other.gameObject);
        }
    }

    void CheckGround()
    {
        if (!cc.isGrounded)
        {
            //clear the moving platform we were on
            transform.parent = null;
            currentPlatform = null; 

            return;
        }

        RaycastHit hit;
        Vector3 start = transform.position + cc.center;
        Vector3 direction = Vector3.down;


        // spherecast toward the ground
        if (Physics.SphereCast(start, cc.radius, direction, out hit, raycastLength, groundRaycastLayer))
        {
            GameObject other = hit.collider.gameObject;
            if (cc.isGrounded && other.tag == "ImpulsePlatform" && !impulseLeap)
            {
                Debug.DrawRay(start,  direction * raycastLength, Color.red);
                impulseLeap = true;
                // shoot the character up by impulse speed
                vSpeed = impulsePlatformSpeed;
                // cannot jump while being impulsed
                jumpAmount = maxJumpAmount;
                //animate the leap
                animator.SetBool("ImpulseLeap", true);
            }
            else
            {
                Debug.DrawRay(start, direction * raycastLength, Color.green);
            }

            // activate dissapearing platform script when jumping on a dissapearing platform
            // could be optimized (rn checking every frame)
            if (cc.isGrounded && other.tag == "DissapearingPlatform")
            {
                TimedDissapearing timedScript = other.GetComponent<TimedDissapearing>();
                if (timedScript != null)
                {
                    // enable existing script to start the dissapearing timer
                    timedScript.enabled = true;
                }
            }


            if (currentPlatform == null)
            {
                currentPlatform = other.GetComponent<MovingPlatform>();
                // if the gameobject we are standing on has a moving platform script
                if (currentPlatform)
                {
                    // we parent the player to the moving platform to move the player with the moving platform
                    transform.parent = currentPlatform.transform;
                }

            }
           
        }
    }

    
}
