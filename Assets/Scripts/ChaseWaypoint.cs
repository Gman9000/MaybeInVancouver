using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseWaypoint : MonoBehaviour
{
    public float newSpeed;
    [SerializeField] private GameObject thirdBoss;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ThirdBoss"))
        {
            other.GetComponent<ThirdBoss>().AdjustSpeed(newSpeed);
        }
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, thirdBoss.transform.position) < 2.0f) thirdBoss.GetComponent<ThirdBoss>().AdjustSpeed(newSpeed);
    }
}
