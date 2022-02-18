using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player hits a bush, destroy bush.
        if (other.CompareTag("Bush"))
        {
            Destroy(other.gameObject);
        }

        //If the player hits an enemy, damage enemy.
        if (other.CompareTag("ChargerEnemy"))
        {
            other.GetComponent<ChargerEnemyScript>().TakeDamage();
        }

        if (other.CompareTag("FlyingEnemy"))
        {
            other.GetComponent<FlyingEnemy>().TakeDamage();
        }

        if (other.CompareTag("FirstBoss"))
        {
            other.GetComponent<FirstBoss>().TakeDamage();
        }
    }
}
