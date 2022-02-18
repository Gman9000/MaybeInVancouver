using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; protected set; } //instance of PlayerMovement that can be accessed by other scripts easily
    public float playerSpeed;                           //horizontal player speed.
    public float jumpSpeed;                             //player jump speed.
    public float timeToGlide;                           //amount of time it takes for the player to be able to glide after jumping.
    private float glideTime = 0;                        //the specific moment that the player will be able to glide (determine upon jumping).
    public float glideForce;                            //how much the player will be able to slow themselves down when glididng.
    private bool gliding = false;                       //boolean that represents whether the player is gliding or not.
    public float playerWidth;                           //width of the player
    private bool isHitting = false;                     //boolean representing whether or not the player is currently attacking.
    public float hitTime;                               //the amount of time a single hit takes.
    private bool inWind = false;                        //boolean representing whether or not the player is in a wind current.
    private Vector2 windDirection = new Vector2(0, 0);  //the direction of wind affecting the player if they are in a wind current.
    private int playerHealth;                           //health of player.
    public int MAXHEALTH;                               //maximum health of the player.
    private bool hitState = false;                      //is the player's invincibility frames active.
    private int collectibleCount = 0;
    public Animator DKanim;
    public int collectibleCountMax;
    [SerializeField] private bool grounded = false;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private List<GameObject> playerHealthUnits;
    [SerializeField] private GameObject playerHealthUI;
    [SerializeField] private Text collectibleCountText;
    [SerializeField] private GameObject collectibleUI;

    void Awake()
    {
        Instance = this;
        playerHealth = MAXHEALTH;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Update player horizontal speed.
        rigidBody.velocity = new Vector2(Input.GetAxis("Horizontal")*playerSpeed*Time.deltaTime, rigidBody.velocity.y);
        DKanim.SetFloat("velocity", Mathf.Abs(rigidBody.velocity.x));
        //Update the direction the player is facing.
        FaceCheck();

        //Check if player is grounded and adjust grounded bool as such.
        grounded = GroundCheck();
        DKanim.SetBool("jump", !grounded);

        //First check if the player is in a wind current and not grounded.
        if (inWind && !grounded)
        {
            //If the player is gliding within the windbox, increase their speed in the wind direction.
            if (gliding)
            {
                rigidBody.AddForce(windDirection * 2);
            }

            else rigidBody.AddForce(windDirection);
        }

        //Otherwise, check if the player is gliding.
        else if (gliding)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -1 / glideForce);
        }

    }

    void Update()
    {
        //If the game is unpaused allow inputs for jumping and attacking.
        if (!pauseMenu.GetComponent<PauseMenu>().Paused())
        {
            //Update the player vertical speed.
            JumpCheck();

            //Check if the player is pressing the hit button.
            HitCheck();
        }
 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.GetComponent<PauseMenu>().ActivateMenu();
        }
    }

    private void JumpCheck()
    {

        //If the player is pressing the jump button and is grounded, jump.
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidBody.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);

            //set glide time.
            glideTime = Time.time + timeToGlide;
            gliding = false;
            
            DKanim.SetBool("glide", gliding);
        }

        //Else if the player is in the air and able to glide, turn off gravity and start gliding.
        if(Input.GetButtonDown("Jump") && Time.time > glideTime && !grounded)
        {
            Physics.gravity = new Vector3(0, 0, 0);
            gliding = true;
            DKanim.SetBool("glide", gliding);

        }

        //Else if the player let's go of the jump button or they land on the ground, stop gliding.
        if (Input.GetButtonUp("Jump") || grounded)
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
            gliding = false;
            DKanim.SetBool("glide", gliding);
        }

    }

    private void FaceCheck()
    {
        Vector3 playerDirection = transform.localScale;
        if (Input.GetAxis("Horizontal") > 0f) playerDirection.x = playerWidth; 

        else if (Input.GetAxis("Horizontal") < 0f) playerDirection.x = -playerWidth;

        transform.localScale = playerDirection;
    }

    private bool GroundCheck()
    {
        bool isGrounded = false;

        //this is a debug to show the ground check in the scene.
        Debug.DrawRay(new Vector2(transform.position.x + 0.95f * transform.GetComponent<BoxCollider2D>().size.x/2, transform.position.y), 1.05f * transform.GetComponent<BoxCollider2D>().size.x * Vector2.down, Color.green, Time.fixedDeltaTime);
        Debug.DrawRay(new Vector2(transform.position.x - 0.95f * transform.GetComponent<BoxCollider2D>().size.x/2, transform.position.y), 1.05f * transform.GetComponent<BoxCollider2D>().size.x * Vector2.down, Color.green, Time.fixedDeltaTime);
        
        
        //check the collider on both sides to see if the player is touching the ground.
        RaycastHit2D hit01 = Physics2D.Raycast(new Vector2(transform.position.x + 0.95f * transform.GetComponent<BoxCollider2D>().size.x/2, transform.position.y), 
            Vector2.down, 1.05f * transform.GetComponent<BoxCollider2D>().size.x, groundLayer);

        RaycastHit2D hit02 = Physics2D.Raycast(new Vector2(transform.position.x - 0.95f * transform.GetComponent<BoxCollider2D>().size.x/2, transform.position.y), 
            Vector2.down, 1.05f * transform.GetComponent<BoxCollider2D>().size.x, groundLayer);

        if ((hit01.collider != null) || (hit02.collider != null)) isGrounded = true;

        return isGrounded;
    }

    private void HitCheck()
    {
        //If the player is clicking the mouse and the player character isn't already attacking, launch attack.
        if ((Input.GetMouseButtonDown(0)) && (!isHitting))
        {
            StartCoroutine(Hit());  //start hitting coroutine. 
            //if ()            if (grounded)
            /*{
                DKanim.SetBool("slash", true);
                DKanim.SetBool("glidingSlash", false);
                DKanim.SetBool("jumpingSlash", false);
            }*/
        }
    }

    private IEnumerator Hit()
    {
        if (grounded && !gliding) //you are slashing on the ground and are not in the air and are not gliding
        {
            DKanim.SetBool("slash", true);
            DKanim.SetBool("glidingSlash", false);
            DKanim.SetBool("jumpingSlash", false);
        }
        else if (!grounded && !gliding) //you are slashing while in the air and not gliding
        {
            DKanim.SetBool("slash", false);
            DKanim.SetBool("glidingSlash", false);
            DKanim.SetBool("jumpingSlash", true);
        }
        else if (!grounded && gliding) //you are slashing while in the air and gliding
        {
            DKanim.SetBool("slash", false);
            DKanim.SetBool("glidingSlash", true);
            DKanim.SetBool("jumpingSlash", false);
        }
        isHitting = true;       //set isHitting to true.

        hitBox.SetActive(true);                     //set hitbox to active.

        yield return new WaitForSeconds(hitTime);   //wait the established amount of seconds.

        hitBox.SetActive(false);                    //deactivate hitbox.

        isHitting = false;                          //set isHitting to false.
        DKanim.SetBool("slash", false);
        DKanim.SetBool("glidingSlash", false);
        DKanim.SetBool("jumpingSlash", false);

    }

    //Public methods called from the Windbox script whenever player enters or exits windbox.
    public void setWindBool(bool WindBool)
    {
        inWind = WindBool;
    }

    //sets the wind force affecting the player while in the wind box
    public void setWindDirection(Vector2 newWindDirection)
    {    
        windDirection = newWindDirection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LevelManager.Instance)
        {
            //if the player collides with checkpoint box, call level manager to set as current spawn point
            if (other.CompareTag("CheckPoint"))
            {
                LevelManager.Instance.SetCurrentSpawn(other.gameObject.GetComponent<CheckPoint>().getNum(), other.gameObject.GetComponent<CheckPoint>().IsFirstBoss());
            }

            //if the player collides with pitfall, reset player to last spawn point
            if (other.CompareTag("Pitfall"))
            {
                LevelManager.Instance.ResetPlayer();
            }

            if (other.CompareTag("Collectible"))
            { 
                collectibleCount++;
                collectibleCountText.text = collectibleCount.ToString() + "/12";

                if (collectibleCount == collectibleCountMax)
                {
                    StartCoroutine(TransitionToNextCutScene());
                }

                Destroy(other.gameObject);
            }

            if (other.CompareTag("Ending"))
            {
                StartCoroutine(Ending());
            }
        }
    }

    //activate invincibility frames after being hit
    private IEnumerator HitState()
    {
        hitState = true;

        yield return new WaitForSeconds(2f);

        hitState = false;
    }

    //reset player health to maximum
    public void ResetHealth()
    {
        playerHealth = MAXHEALTH;
        for (int i = 0; i < 3; i++)
        {
            playerHealthUnits[i].SetActive(true);
        }
    }

    public void DamagePlayer()
    {

        if (!hitState)
        {
            playerHealth--;

            for (int i = 0; i < 3 - playerHealth; i++)
            {
                playerHealthUnits[i].SetActive(false);
            }

            //if player health is depleted, call level manager to reset player
            if (playerHealth == 0) LevelManager.Instance.ResetPlayer();

            else StartCoroutine(HitState());
        }
        
    }

    public void SetUI(bool health, bool collectible)
    {
        if (health) playerHealthUI.SetActive(true);
        else playerHealthUI.SetActive(false);

        if (collectible) collectibleUI.SetActive(true);
        else collectibleUI.SetActive(false);
    }

    public IEnumerator TransitionToNextCutScene()
    {
        LevelManager.Instance.SetCurrentSpawn(9, false);
        this.enabled = false;
        SetUI(false, false);
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        LevelManager.Instance.ResetPlayer();
        //this is called only after level 2 so we can hardcode the block to activate here
        LevelManager.Instance.storyFlowChart.ExecuteBlock("Chapter3");
        //StoryProgressionHandler.Instance.SetChapterNumber(3); //this will always be set to 1 from the main menu
        //StoryProgressionHandler.Instance.EnableCutsceneChapter();

        yield return null;

    }

    public IEnumerator Ending()
    {
        LevelManager.Instance.SetCurrentSpawn(0, false);
        this.enabled = false;
        SetUI(false, false);
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        LevelManager.Instance.ResetPlayer();
        StoryProgressionHandler.Instance.SetChapterNumber(4); //this will always be set to 1 from the main menu
        StoryProgressionHandler.Instance.EnableCutsceneChapter();

        yield return null;

    }
}
