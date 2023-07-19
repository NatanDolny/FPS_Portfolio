using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameObject player;
    PlayerStats playerStats;
    public bool keyCardCollected = false;

    public int weakEnemiesLeft;
    public int bossEnemiesLeft;
    List<EnemyController> bosses = new List<EnemyController>();
    public GameObject[] doors;
    public GameObject finalDoor;
    public bool hasKey = false;
    bool finalOpened = false;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    private void Start()
    {
        foreach (Transform boss in transform.Find("BossEnemies"))
        {
            EnemyController bossController;
            boss.TryGetComponent(out bossController);
            bossController.forceIdle = true;
        }
    }

    private void FixedUpdate()
    {
        UpdateLists();
        if (!keyCardCollected)
        {
            if (playerStats.canProceed)
            {
                keyCardCollected = true;
                OpenDoors();
            }
            if (weakEnemiesLeft <= 0)
            {
                transform.Find("Body").gameObject.SetActive(false);
            }
        }
        else
        {
            if (bossEnemiesLeft <= 0 && !finalOpened)
            {
                OpenFinal();
            }
        }
    }

    private void UpdateLists()
    {
        weakEnemiesLeft = transform.Find("WeakEnemies").childCount;
        bossEnemiesLeft = transform.Find("BossEnemies").childCount;
    }

    void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<DoorScript>().OpenDoor(); 
        }
        foreach (Transform boss in transform.Find("BossEnemies"))
        {
            EnemyController bossController;
            boss.TryGetComponent(out bossController);
            bossController.forceIdle = false;
        }
    }

    public void OpenFinal()
    {
        finalDoor.GetComponent<DoorScript>().OpenDoor();
        finalOpened = true;
    }
}
