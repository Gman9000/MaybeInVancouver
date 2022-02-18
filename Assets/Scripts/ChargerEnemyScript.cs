using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerEnemyScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;      //enemy rigidBody
    [SerializeField] private GameObject playerTarget;    //player character
    public float speed;                 //enemy speed
    public float distance;              //required distance between player and enemy before enemy charges
    public float enemyWidth;            //width of enemy, needed for when enemy turns around
    private int direction = 1;          //integer that can be either 1 or -1, determines direction of enemy travel
    public int enemyHealth;             //health of enemy unit
    private bool hitState = false;      //is the enemy's invincibility frames currently active
    public Vector2 damageForce;         //what is the force of the knockback when enemy is damaged
    public Animator chargerEnemyAnim;



    // Update is called once per frame
    void FixedUpdate()
    {
        chargerEnemyAnim.SetFloat("toadVelocity", Mathf.Abs(rigidBody.velocity.x));
        //if the player is close enough, adjust enemy direction and chase after player.
        if (Vector2.Distance(transform.position, playerTarget.transform.position) < distance)
        {
            Vector3 enemyDirection = transform.localScale;

            if (transform.position.x < playerTarget.transform.position.x)
            {
                enemyDirection.x = enemyWidth;
                direction = 1;
            }

            else if (transform.position.x > playerTarget.transform.position.x)
            {
                enemyDirection.x = -enemyWidth;
                direction = -1;
            }

            transform.localScale = enemyDirection;

            //If the enemy is not in a hitstate, move toward player. Otherwise, enemy is being knocked back.
            if (!hitState)
            {
                rigidBody.velocity = new Vector2(direction * speed * Time.deltaTime, rigidBody.velocity.y);
                chargerEnemyAnim.SetFloat("toadVelocity", Mathf.Abs(rigidBody.velocity.x));
            }
            
        }
        else
        {

            //If the enemy is not in a hitstate, remain still. Otherwise, enemy is being knocked back.
            if (!hitState)
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
                chargerEnemyAnim.SetFloat("toadVelocity", Mathf.Abs(rigidBody.velocity.x));
            }
            
        }

    }

    //enemy is hit with player attack and takes damage
    public void TakeDamage()
    {
        if (!hitState)
        {
            enemyHealth--;

            if (enemyHealth == 0) { StartCoroutine(DeathState()); } //subject to change.
            StartCoroutine(HitState());

            //if the enemy is still alive, knock them back the desired force
            rigidBody.AddForce(new Vector2(-direction * damageForce.x, damageForce.y), ForceMode2D.Impulse);
        }
        
    }

    private IEnumerator DeathState()
    {
        chargerEnemyAnim.SetTrigger("toadDeath");
        yield return new WaitForSeconds(0.5f);
        chargerEnemyAnim.ResetTrigger("toadDeath");
        Destroy(this.gameObject);
    }

    //activate invincibilitie frames for enemy upon being hit
    private IEnumerator HitState()
    {
        hitState = true;

        yield return new WaitForSeconds(0.2f);

        hitState = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Collider2D>().CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().DamagePlayer();
        }
    }
}
