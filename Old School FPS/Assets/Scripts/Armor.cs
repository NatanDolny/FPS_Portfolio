using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    GameObject player;
    PlayerStats playerStats;
    GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        body = transform.Find("Body").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        body.transform.LookAt(player.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerStats.armor <= 80)
        {
            playerStats.armor += 20;
            playerStats.armorSound.Play();
            Destroy(this.gameObject);
        }
    }
}
