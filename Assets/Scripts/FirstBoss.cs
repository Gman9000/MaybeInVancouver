using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    private bool battleStarted = false;
    public float attackSpeedNormal;
    public float attackSpeedFast;
    private float attackTime = 0f;
    private int shotCount = 0;
    private bool lowHP = false;
    public int lowHPPoint;
    private bool isCharging = false;
    private int currentSide = 1;
    public float enemyWidth;
    public float jumpSpeed;
    public float chargeSpeed;
    public float bossHealth;
    private bool hitState = false;
    public Animator FBanim;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<GameObject> leftWaypoints;
    [SerializeField] private List<GameObject> rightWaypoints;
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private BoxCollider2D collider;
    [SerializeField] private GameObject firePosition;
    [SerializeField] private GameObject arenaGate;
    [SerializeField] private LayerMask groundLayer;

    // Update is called once per frame
    void FixedUpdate()
    {
        //if the battle has started, enough time has past since last attack and the boss isn't currently charging, make next move.
        if ((battleStarted) && (Time.time > attackTime) && (!isCharging))
        {
            //if we haven't shot enough times, shoot a projectile.
            if (shotCount < 3)
            {
                ShootProjectile();
            }

            //otherwise, begin charging.
            else
            {
                StartCoroutine(Charge());
            }
        }
    }

    //Start the battle once the player has reached the appropriate checkpoint.
    public void StartBattle()
    {
        battleStarted = true;
        arenaGate.SetActive(true);
    }

    //Fire a single projectile at the player.
    private void ShootProjectile()
    {
        StartCoroutine(ShootingAnimation());
        //acquire the player's current position and rotate towards it, then instantiate a bullet prefab with said rotation.
        Vector3 vectorToTarget = (playerTarget.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(vectorToTarget.x, vectorToTarget.y) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        GameObject projectileGO = Instantiate(projectilePrefab, firePosition.transform.position, rot) as GameObject;
        projectileGO.GetComponent<Projectile>().SetDirectionAndVelocity(vectorToTarget);
        projectileGO.transform.localScale = new Vector3(2,2,2);

        shotCount++;

        if (lowHP) attackTime = Time.time + attackSpeedFast;
        else attackTime = Time.time + attackSpeedNormal;
    }

    //Depending on a random integer between one and 3, choose a random elevation to jump to and then charge across the screen.
    private IEnumerator Charge()
    {
        isCharging = true;

        List<GameObject> targetWaypoints;
        List<GameObject> currentSideWaypoints;

        //Depending on which side of the arena the boss is on, assign one set of waypoints as the target and the other as the current.
        if (currentSide == 1) 
        {
            targetWaypoints = leftWaypoints;
            currentSideWaypoints = rightWaypoints;
            currentSide = 0;
        }
        else
        {
            targetWaypoints = rightWaypoints;
            currentSideWaypoints = leftWaypoints;
            currentSide = 1;
        }

        //randomly select a target waypoint from the other side to represent eleveation.
        int targetWaypoint = Random.Range(0, 2);


        //if the target waypoint is not at the bottom, jump into the air until the desired elevation has been reached.
        if (targetWaypoint != 0)
        {
            rigidBody.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            FBanim.SetBool("isJumping", true);
            while (transform.position.y < currentSideWaypoints[targetWaypoint].transform.position.y)
            {
                yield return null;
            }
        }

        //turn off gravity for boss cube.
        rigidBody.bodyType = RigidbodyType2D.Static;
        collider.isTrigger = true;
        FBanim.SetBool("isCharging", true);
        Vector3 directionOfTravel = targetWaypoints[targetWaypoint].transform.position - transform.position;
        directionOfTravel.Normalize();

        //if the target has not finished clearing the arena, move them forward.
        while (Vector2.Distance(transform.position, targetWaypoints[targetWaypoint].transform.position) > 1f)
        {
            this.transform.Translate(
                directionOfTravel.x * chargeSpeed * Time.deltaTime, 0, 0, Space.World);

            yield return null;
        }

        FBanim.SetBool("isCharging", true);

        //turn gravity back on.
        rigidBody.bodyType = RigidbodyType2D.Dynamic;

        //turn the boss around.
        Vector3 enemyDirection = transform.localScale;

        if (currentSide == 0)
        {
            enemyDirection.x = -enemyWidth;
        }

        else if (currentSide == 1)
        {
            enemyDirection.x = enemyWidth;
        }

        transform.localScale = enemyDirection;

        FBanim.SetBool("isCharging", false);
        isCharging = false; //set isCharging to false
        collider.isTrigger = false;
        shotCount = 0;      //set shotCount to 0.



        if (lowHP) attackTime = Time.time + attackSpeedFast;
        else attackTime = Time.time + attackSpeedNormal;
    }

    //enemy is hit with player attack and takes damage
    public void TakeDamage()
    {
        if (!hitState)
        {
            bossHealth--;

            if (bossHealth == 0) StartCoroutine(TransitionToNextCutScene());
            if (bossHealth == lowHPPoint) lowHP = true;

            StartCoroutine(HitState());

        }

    }

    public IEnumerator TransitionToNextCutScene()
    {
        FBanim.SetBool("isDead", true);
        LevelManager.Instance.SetCurrentSpawn(6, false);
        battleStarted = false;
        playerTarget.GetComponent<PlayerMovement>().enabled = false;
        playerTarget.GetComponent<PlayerMovement>().SetUI(false, false);
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        LevelManager.Instance.ResetPlayer();
        LevelManager.Instance.storyFlowChart.ExecuteBlock("Chapter2");
        //StoryProgressionHandler.Instance.SetChapterNumber(2); //this will always be set to 1 from the main menu
        //StoryProgressionHandler.Instance.EnableCutsceneChapter();
        
        //StartCoroutine(CutsceneHandler.Instance.FadeBlackOutSquare(false));
        //player.GetComponent<PlayerMovement>().enabled = true;
        yield return null;
        //Destroy(this.gameObject);
    }

    //activate invincibilitie frames for enemy upon being hit
    private IEnumerator HitState()
    {
        hitState = true;

        yield return new WaitForSeconds(0.2f);

        hitState = false;
    }

    private IEnumerator ShootingAnimation()
    {
        FBanim.SetBool("isShooting", true);
        yield return new WaitForSeconds(0.3f);
        FBanim.SetBool("isShooting", false);
    }

    private IEnumerator GroundCheck()
    {
        RaycastHit2D hit01 = Physics2D.Raycast(new Vector2(transform.position.x + 0.95f * transform.GetComponent<BoxCollider2D>().size.x / 2, transform.position.y),
            Vector2.down, 1.05f * transform.GetComponent<BoxCollider2D>().size.x, groundLayer);

        RaycastHit2D hit02 = Physics2D.Raycast(new Vector2(transform.position.x - 0.95f * transform.GetComponent<BoxCollider2D>().size.x / 2, transform.position.y),
            Vector2.down, 1.05f * transform.GetComponent<BoxCollider2D>().size.x, groundLayer);

        while ((hit01.collider != null) || (hit02.collider != null)) yield return null;

        FBanim.SetBool("isJumping", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player")) && (isCharging))
        {
            other.GetComponent<PlayerMovement>().DamagePlayer();
        }
    }

    public void ResetBoss()
    {
        
        StopAllCoroutines();
        currentSide = 1;
        transform.position = rightWaypoints[0].transform.position;
        isCharging = false;
        collider.isTrigger = false;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        shotCount = 0;
        attackTime = Time.time + attackSpeedNormal;
        Vector3 enemyDirection = transform.localScale;
        enemyDirection.x = enemyWidth;
        transform.localScale = enemyDirection;
        
    }
}
