using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; protected set; }
    [SerializeField]
    private GameObject playerGO;

    [SerializeField]
    private GameObject firstBossGO;

    [SerializeField]
    private GameObject thirdBossGO;

    [SerializeField]
    private List<GameObject> Checkpoints;

    private int currentSpawn = 0;

    public Flowchart storyFlowChart;
    void Awake()
    {
        Instance = this;
    }

    //Fungus has a hard time with parameters and therefore two functions are needed to enable and disable the player
    public void EnablePlayer()
    {
        PlayerMovement.Instance.enabled = true;
    }
    public void Disableplayer()
    {
        PlayerMovement.Instance.enabled = false;
    }
    //When called, brings player back to last checkpoint.
    public void ResetPlayer()
    {

        if (currentSpawn == 5)
        {
            firstBossGO.GetComponent<FirstBoss>().ResetBoss();
        }

        else if (currentSpawn == 9)
        {
            thirdBossGO.GetComponent<ThirdBoss>().Reset(); ;
        }

        playerGO.transform.position = Checkpoints[currentSpawn].transform.position;
        playerGO.GetComponent<PlayerMovement>().ResetHealth();
    }

    public void SetCurrentSpawn(int newSpawn, bool firstBoss)
    {
        currentSpawn = newSpawn;

        if (firstBoss)
        {
            firstBossGO.GetComponent<FirstBoss>().StartBattle();
        }
    }

    public int GetCurrentSpawn()
    {
        return currentSpawn;
    }
}
