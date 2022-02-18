using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBox : MonoBehaviour
{
    //Vector2 indicating the force of the wind current.
    public Vector2 windDirection;

    //When the player enters windbox, start affecting them with the wind.
    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            
            other.GetComponent<PlayerMovement>().setWindBool(true);
            other.GetComponent<PlayerMovement>().setWindDirection(windDirection);
        }
    }

    //When the player leaves the windbox, remove wind affects.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerMovement>().setWindBool(false);
            other.GetComponent<PlayerMovement>().setWindDirection(new Vector2(0,0));
        }
    }
}
