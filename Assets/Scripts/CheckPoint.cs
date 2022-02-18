using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int num;         //Checkpoint number
    public bool firstBoss;  //Is this checkpoint before the first boss

    public int getNum()
    {
        return num;
    }

    public bool IsFirstBoss()
    {
        return firstBoss;
    }
}
