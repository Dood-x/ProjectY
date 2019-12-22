using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformerHomework;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    private CharacterController cc;
    public CharacterController Cc
    {
        get { return cc; }
        private set { cc = Cc; }
    }
    public PlatformerCamera camScript;
    private Animator animator;

    public Slider healthSlider;
    public Text crystalsTxt;


    private Camera cam;
    [Header("Stats")]

    public float walkSpeed = 1.5f;
    public float runSpeed = 6f;
    public float rotSpeed = 20f;
    public float jumpSpeed = 5f;
    public float maxHealth = 100f;
    public int maxJumpAmount = 2;

    [Header("Sounds")]
    AudioSource audioSource;
    public AudioClip leapSound;
    public AudioClip step1Sound;
    public AudioClip step2Sound;
    public AudioClip step3Sound;
    public AudioClip step4Sound;
    public AudioClip landSound;
    public AudioClip gotHitSound;
    public AudioClip collectedCrystalSound;
    public AudioClip respawnedSound;

    bool disableInput;
    public bool DisableInput
    {
        get { return disableInput; }
        set { disableInput = value; }
    }

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

    [Header("GettingHit")]

    // the impulse relative to damage origin that propels the character
    public Vector3 hitRelativeImpulseSpeed;
    public float hitInvincibilityTime = 2f;
    public float invincibilityFlashingInterval = 0.25f;

    public float deathWaitTime = 3f;
    public float respawnTime = 1f;

    [Header("Camera")]
    public bool switchDirection = true;
    public float switchDirectionLerp = 2f;
    public bool collideLookAt = true;
    public LayerMask lookAtCollisionLayers;

    bool respawning;
    //add values to this in order to launch the player in a direction
    Vector3 launchSpeed;
    //float vSpeed = 0f;
    int jumpAmount = 0;

    //Vector3 moveDirection;
    float health;
    int crystals;
    int maxCrystals;

    SphereCollider syphonCollider;

    MovingPlatform currentPlatform;
    bool movingLanes;

    // true during gothit coroutine!
    bool damageInvunerability = false;


    Vector3 moveDestination;

    float forceMoveSpeed;

    bool dead = false;

    int idleCounter;
    int idleCounterTarget = 3;

    //Dictionary<MeshRenderer, Mesh> playerMeshes;
    //Dictionary<SkinnedMeshRenderer, Mesh> skinnedPlayerMeshes;

    Vector3 camLookOffsetStart;
    bool lookOffsetXLocked = false;
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

        //moveDirection = Vector3.zero;
        health = maxHealth;
        maxCrystals = GameObject.FindGameObjectsWithTag("Crystal").Length;
        RespawnPoint startPoint = new RespawnPoint(transform.position, transform.localRotation);
        checkpoints.Add(startPoint);
        healthSlider.value = health / maxHealth;
        WriteCrystals();
        syphonCollider = GetComponentInChildren<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        camLookOffsetStart = camScript.Lookoffset;

        idleCounterTarget = Random.Range(2, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (movingLanes)
        {
            MoveLanes();
            return;
        }
        if (respawning || movingLanes)
        {
            return;
        }

        //CheckMovingPlatform();
        
        
        // movement input
        float v = Input.GetAxis("Horizontal") + Input.GetAxis("Vertical");
        v = Mathf.Clamp(v, -1f, 1f);
        float h = 0;

        if (disableInput)
        {
            v = 0;
            h = 0;
        }

        float animSpeed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));




        animator.SetFloat("Speed", animSpeed);
        animator.SetBool("Grounded", cc.isGrounded);
        Vector3 moveDirection = Vector3.zero;

        //if the player falls to y kill height kill the player
        if (transform.position.y < yKillHeight)
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
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);

            moveDirection.x = moveHorizontal.x;
            moveDirection.z = moveHorizontal.z;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        //switches the camera look offset depending on whether we are going forward or back
        SwitchDirection(transform.forward.z); //ubija me, ne treba za sad
        CollideLookOffset();


        if (cc.isGrounded && !impulseLeap)
        {
            jumpAmount = 0;
            moveDirection.y = 0f;
            // when grounded clear the launch movement, keep gravity to press the player down onto the ground
            launchSpeed = new Vector3(0f, gravityAccelOnGround, 0f);
        }
        else
        {
            // y is decelerated by gravity, x,z s constant
            launchSpeed.y -= gravity * Time.deltaTime;
            // speed of movement * time is the amount we need to move the character
        }

        //if (currentPlatform)
        //    launchSpeed.y = 0;

        //jumping allowes double jump
        if (!disableInput && Input.GetButtonDown("Jump") && jumpAmount < maxJumpAmount)
        {
            animator.SetBool("Leap", true);
        }


        //if character is launched clear the movement and launch them
        if (launchSpeed.x != 0)
        {
            moveDirection.x = launchSpeed.x * Time.deltaTime;
        }

        if (launchSpeed.z != 0)
        {
            moveDirection.z = launchSpeed.z * Time.deltaTime;
        }

        //gravity is always affecting our motion
        moveDirection.y = launchSpeed.y * Time.deltaTime;

        //cc.Move(Vector3.forward * 0.001f);

        if (currentPlatform)
        {
            //moveDirection += currentPlatform.GetMovementDelta();
            //currentPlatform.ClearnMovementDelta();
        }


        // move the character!

        cc.Move(moveDirection);

        CheckGround();


    }

    public void LateUpdate()
    {
        //if (currentPlatform)
        //    cc.Move(currentPlatform.GetMovementDelta());

        //CheckGround();



    }

    void CollideLookOffset()
    {
        if (!collideLookAt)
            return;
        //raycast from characetr offset to look offset;
        RaycastHit hit;
        Vector3 startPosition = transform.position + Vector3.up * camScript.Lookoffset.y;
        Vector3 endPosition = transform.position + camScript.Lookoffset;
        Vector3 dir = endPosition - startPosition;
        dir.y = 0;


        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(startPosition, dir.normalized, out hit, dir.magnitude, lookAtCollisionLayers, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("Did Hit");
            camScript.Lookoffset = hit.point - transform.position;
            //Debug.DrawLine(startPosition, hit.point, Color.red);
        }
        //else
        //{
        //    Debug.DrawLine(startPosition, endPosition, Color.yellow);
            
        //}
    }

    void MoveLanes()
    {
        //if we are forced to move to a diffrent lane
        if (moveDestination != Vector3.zero)
        {
            Vector3 distance = moveDestination - transform.position;
            distance.z = 0;
            distance.y = 0;
            if (distance.magnitude < 0.3f)
            {
                moveDestination = Vector3.zero;
                movingLanes = false;
                damageInvunerability = false;
                disableInput = false;
                forceMoveSpeed = runSpeed;
            }
            else
            {
                //speed will be smoothed
                forceMoveSpeed = Mathf.Lerp(forceMoveSpeed, walkSpeed, Time.deltaTime * 1f);
                Vector3 moveDirection = distance.normalized * Time.deltaTime * forceMoveSpeed;
                animator.SetFloat("Speed", forceMoveSpeed / runSpeed);
                Quaternion rotation = Quaternion.LookRotation(distance.normalized);
                transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);
                moveDirection.y = gravityAccelOnGround;

                cc.Move(moveDirection);

                //keeps the lookoffset at the starting lane!
                if (lookOffsetXLocked)
                {
                    Vector3 look = camScript.Lookoffset;
                    look.x -= moveDirection.x;
                    camScript.Lookoffset = look;
                }
                
            }


        }


    }

    void SwitchDirection(float v)
    {
        if (!switchDirection)
            return;

        Vector3 lookOffsetTraget = camScript.Lookoffset;

        if (v > 0)
        {

            lookOffsetTraget.z = Mathf.Abs(camLookOffsetStart.z);
            lookOffsetTraget.z = Mathf.Lerp(camScript.Lookoffset.z, lookOffsetTraget.z, switchDirectionLerp * Time.deltaTime);
            camScript.Lookoffset = lookOffsetTraget;
        }
        else if (v < 0)
        {
            lookOffsetTraget.z = -Mathf.Abs(camLookOffsetStart.z);
            lookOffsetTraget.z = Mathf.Lerp(camScript.Lookoffset.z, lookOffsetTraget.z, switchDirectionLerp * Time.deltaTime);
            camScript.Lookoffset = lookOffsetTraget;
        }
    }

    public void Leap()
    {
        launchSpeed = new Vector3(0f, jumpSpeed, 0f);
        jumpAmount++;
        cc.Move(launchSpeed * Time.deltaTime);
        animator.SetBool("Leap", false);
    }

    public void PlayLeapSound()
    {
        audioSource.PlayOneShot(leapSound); 
    }
    public void PlayStepSound()
    {
        if (!cc.isGrounded)
            return;

        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                audioSource.PlayOneShot(step1Sound);
                break;
            case 1:
                audioSource.PlayOneShot(step2Sound);
                break;
            case 2:
                audioSource.PlayOneShot(step3Sound);
                break;
            case 3:
                audioSource.PlayOneShot(step4Sound);
                break;
        }

    }

    public void PlayLandSound()
    {
        audioSource.PlayOneShot(landSound);

    }

    public void PlayGotHitSound()
    {
        audioSource.PlayOneShot(gotHitSound);

    }

    public void PlayCollectedCrystalSound()
    {
        audioSource.PlayOneShot(collectedCrystalSound);
    }

    public void PlayRespawnedSound()
    {
        audioSource.PlayOneShot(respawnedSound);
    }


    public void IdleOver()
    {
        idleCounter++;
        if (idleCounter >= idleCounterTarget)
        {
            idleCounter = 0;
            idleCounterTarget = Random.Range(2, 4);
            animator.SetTrigger("idleSpecial");
        }
    }
    public void ImpulseLeapEnd()
    {
        animator.SetBool("ImpulseLeap", false);
        impulseLeap = false;
    }

    public void OutOfBounds()
    {
        health -= outOfBoundsDamage;
        healthSlider.value = health / maxHealth;
        //respawn
        Respawn();

    }

    public void TakeDamage(GameObject enemy, float damage)
    {
        if ((damageInvunerability && health > 0) || respawning)
        {
            return;
        }

        health -= damage;
        healthSlider.value = health / maxHealth;

        if (health <= 0)
        {
            //death animation
            animator.SetBool("Dead", true);

            StartCoroutine("DeathCountdown", enemy.transform.position);
        }
        else
        {
            StartCoroutine("GotHit", enemy.transform.position);
        }

    }

    public void Respawn()
    {
        ClearState();
        PlayRespawnedSound();
        //Physics.IgnoreLayerCollision(8, 9, true);
        if (health > 0)
        {
            transform.position = checkpoints[checkpoints.Count - 1].position;
            transform.localRotation = checkpoints[checkpoints.Count - 1].lookDirection;
        }
        else
        {
            transform.position = checkpoints[0].position;
            transform.localRotation = checkpoints[0].lookDirection;
            // respawn with full health
            health = maxHealth;
            healthSlider.value = health;

        }

        StartCoroutine("Respawning");
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
            PlayCollectedCrystalSound();
            Destroy(other.gameObject);
        }
    }

    void CheckGround()
    {
        if (!cc.isGrounded)
        {
            //clear the moving platform we were on
            transform.parent = null;
            cam.gameObject.transform.parent = null;
            currentPlatform = null;

            return;
        }

        if ((disableInput && damageInvunerability) && !movingLanes && !respawning && !dead)
        {
            disableInput = false;
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
                //Debug.DrawRay(start, direction * raycastLength, Color.red);
                impulseLeap = true;
                // shoot the character up by impulse speed
                launchSpeed = Vector3.up * impulsePlatformSpeed;
                // cannot jump while being impulsed
                jumpAmount = maxJumpAmount;
                //animate the leap
                animator.SetBool("ImpulseLeap", true);
            }
            else
            {
                //Debug.DrawRay(start, direction * raycastLength, Color.green);
            }

            // activate dissapearing platform script when jumping on a dissapearing platform
            // could be optimized (rn checking every frame)
            if (cc.isGrounded && other.tag == "DissapearingPlatform")
            {
                TimedDissapearing timedScript = other.GetComponent<TimedDissapearing>();

                // if the timedscript was destroyed, or is in respawning mode
                if (timedScript == null || (timedScript.enabled == true && timedScript.IsRespawning == true))
                {
                    timedScript = other.gameObject.AddComponent<TimedDissapearing>();
                    timedScript.enabled = true;
                }
                else
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
                    cam.gameObject.transform.parent = currentPlatform.transform;
                }
            }



        }
    }

    void CheckMovingPlatform()
    {
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * cc.height;
        Vector3 direction = Vector3.down;

        bool foundMovingPlatform = false;

        float length = cc.height + 0.1f;


        // spherecast toward the ground every frame to find the moving platform
        if (Physics.SphereCast(start, cc.radius, direction, out hit, length, groundRaycastLayer))
        {
            Debug.DrawRay(start, direction * length, Color.red);

            GameObject other = hit.collider.gameObject;
            if (currentPlatform == null)
            {
                currentPlatform = other.GetComponent<MovingPlatform>();
                // if the gameobject we are standing on has a moving platform script
                if (currentPlatform)
                {
                    // we parent the player to the moving platform to move the player with the moving platform / ovo ti ne radi
                    //transform.parent = currentPlatform.transform;
                }


            }
        }
        else
        {
            Debug.DrawRay(start, direction * length, Color.green);

        }

        if (!foundMovingPlatform)
        {
            transform.parent = null;
            cam.gameObject.transform.parent = null;
            currentPlatform = null;
        }


    }

    void LaunchAwayFrom(Vector3 impactPosition)
    {
        //direction from impact toward player
        Vector3 dir = transform.position - impactPosition;
        // see if we are launching player forward or backward
        int forward = dir.z > 0 ? 1 : -1;

        disableInput = true;

        //launch player
        launchSpeed = hitRelativeImpulseSpeed;

        //orient launch away from impact
        launchSpeed.z = hitRelativeImpulseSpeed.z * forward;

        // play the hit animation
        animator.SetTrigger("Hit");
        //start the invincibility
        damageInvunerability = true;

        cc.Move(launchSpeed * Time.deltaTime);
    }


    IEnumerator GotHit(Vector3 impactPosition)
    {
        //SoundManager.Instance.PlayGotHit();

        LaunchAwayFrom(impactPosition);

        Physics.IgnoreLayerCollision(8, 9, true);


        float timer = hitInvincibilityTime;
        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();

        // alternate enabling/disabling the mest renderer
        while (timer > 0)
        {
            TogglePlayerVisibility();

            yield return new WaitForSeconds(invincibilityFlashingInterval);
            timer -= invincibilityFlashingInterval;
        }

        ShowPlayer();

        Physics.IgnoreLayerCollision(8, 9, false);

        damageInvunerability = false;
    }


    void ClearState()
    {
        StopCoroutine("GotHit");
        StopCoroutine("DeathCountdown");

        camScript.Lookoffset = camLookOffsetStart;
        animator.SetBool("Dead", false);
        dead = false;

        transform.parent = null;
        cam.gameObject.transform.parent = null;
        currentPlatform = null;


        Physics.IgnoreLayerCollision(8, 9, false);

        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].enabled = true;
        }

        for (int i = 0; i < smr.Length; i++)
        {
            smr[i].enabled = true;
        }

        damageInvunerability = false;

        launchSpeed = Vector3.zero;
    }

    IEnumerator Respawning()
    {
        HidePlayer();
        disableInput = true;
        damageInvunerability = true;
        respawning = true;
        yield return new WaitForSeconds(respawnTime);
        respawning = false;
        disableInput = false;
        damageInvunerability = false;
        ShowPlayer();
    }


    void TogglePlayerVisibility()
    {
        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].enabled = !mr[i].enabled;
        }

        for (int i = 0; i < smr.Length; i++)
        {
            smr[i].enabled = !smr[i].enabled;
        }
    }

    void HidePlayer()
    {
        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].enabled = false;
        }

        for (int i = 0; i < smr.Length; i++)
        {
            smr[i].enabled = false;
        }
    }

    void ShowPlayer()
    {

        MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].enabled = true;
        }

        for (int i = 0; i < smr.Length; i++)
        {
            smr[i].enabled = true;
        }
    }

    public void MoveToLane(Vector3 destination, MoveLanes mlScript)
    {
        moveDestination = destination;
        disableInput = true;
        damageInvunerability = true;
        movingLanes = true;
        forceMoveSpeed = runSpeed;
        lookOffsetXLocked = mlScript.lockCameraDistance;

    }

    IEnumerator DeathCountdown(Vector3 impactPosition)
    {
        dead = true; 

        LaunchAwayFrom(impactPosition);

        Physics.IgnoreLayerCollision(8, 9, true);

        disableInput = true;
        damageInvunerability = true;
        //respawning = true;

        yield return new WaitForSeconds(deathWaitTime);

        animator.SetBool("Dead", false);
        animator.Play("New State", 2);
        Physics.IgnoreLayerCollision(8, 9, false);
        Respawn();

    }

}
