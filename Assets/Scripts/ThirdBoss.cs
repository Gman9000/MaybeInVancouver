using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBoss : MonoBehaviour
{
    public float startSpeed;
    public float speed;
    private bool chaseStarted = false;
    [SerializeField] private GameObject startPoint;

    void FixedUpdate()
    {
        if (chaseStarted)
            this.transform.Translate(
                speed * Time.deltaTime, 0, 0, Space.World);
    }

    public void StartChase()
    {
        chaseStarted = true;
        speed = startSpeed;
    }

    public void AdjustSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void Reset()
    {
        transform.position = startPoint.transform.position;
        speed = startSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Collider2D>().CompareTag("Player"))
        {
            Reset();
            LevelManager.Instance.ResetPlayer();
        }
    }
}
