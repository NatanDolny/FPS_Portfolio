using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAmmo : MonoBehaviour
{
    GameObject player;
    PlayerStats playerStats;
    GameObject body;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        body = transform.Find("Body").gameObject;
    }

    void Update()
    {
        body.transform.LookAt(player.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStats.ammoSound.Play();
            Gun rL =  player.GetComponent<PlayerController>().rocketLauncher.GetComponent<Gun>();
            rL.reloaded = true; 
            ref int rocketAmmo = ref rL.currentAmmo;
            if (rocketAmmo < 10)
            {
                rocketAmmo = rocketAmmo <= 5 ? rocketAmmo + 5 : 10;
                Destroy(this.gameObject);
            }
        }
    }
}
