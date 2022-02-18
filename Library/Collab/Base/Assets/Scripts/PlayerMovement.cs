using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private bool grounded = false;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject hitBox;
    
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

        //Update the direction the player is facing.
        FaceCheck();

        //Check if player is grounded and adjust grounded bool as such.
        grounded = GroundCheck();

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
        //Update the player vertical speed.
        JumpCheck();

        //Check if the player is pressing the hit button.
        HitCheck();
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
        }

        //Else if the player is in the air and able to glide, turn off gravity and start gliding.
        if(Input.GetButtonDown("Jump") && Time.time > glideTime && !grounded)
        {
            Physics.gravity = new Vector3(0, 0, 0);
            gliding = true;
        }

        //Else if the player let's go of the jump button or they land on the ground, stop gliding.
        if (Input.GetButtonUp("Jump") || grounded)
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
            gliding = false;
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
            
        }
    }

    private IEnumerator Hit()
    {
        isHitting = true;       //set isHitting to true.

        hitBox.SetActive(true);                     //set hitbox to active.

        yield return new WaitForSeconds(hitTime);   //wait the established amount of seconds.

        hitBox.SetActive(false);                    //deactivate hitbox.

        isHitting = false;                          //set isHitting to false.
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
        //if the player collides with checkpoint box, call level manager to set as current spawn point
        if (other.tag == "CheckPoint")
        {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().SetCurrentSpawn(other.gameObject.GetComponent<CheckPoint>().getNum());
        }

        //if the player collides with pitfall, reset player to last spawn point
        if (other.tag == "Pitfall")
        {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().ResetPlayer();
        }

        //if player collides with enemy, deal damage to player
        if ((other.tag == "Enemy") && (!hitState))
        {
            Debug.Log("Test");
            playerHealth--;

            //if player health is depleted, call level manager to reset player
            if (playerHealth == 0) GameObject.Find("LevelManager").GetComponent<LevelManager>().ResetPlayer();

            else StartCoroutine(HitState());
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
    }
}
