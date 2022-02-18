using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody2D theRB; 
    public float smoothTime; //the amount of time the camera takes to catch up to the player. 0.025f seems to be the sweet spot for this

    // Start is called before the first frame update
    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.Instance != null)
        {
            //Debug.Log("PLAYER EXISTS!");
            Vector2 currentVelocity = theRB.velocity;
            theRB.position = Vector2.SmoothDamp(theRB.position, PlayerMovement.Instance.gameObject.transform.position, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
            //Debug.Log("POSITION: " + theRB.position);
        }
    }
}
