using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public bool playerDead = false;
    public int health = 100;
    public int armor = 0;
    public int ammo = 1; 
    public AudioSource keySound;
    public AudioSource armorSound;
    public AudioSource ammoSound;
    public bool canProceed = false;
    public float endTimer = 0;
    public float endMax = 2;

    // Start is called before the first frame update
    void Start()
    {
        keySound = transform.Find("PickUp").GetComponent<AudioSource>();
        armorSound = transform.Find("Armor").GetComponent<AudioSource>();
        ammoSound = transform.Find("Ammo").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            PlayerDead();
        }
    }

    public void PlayerDead()
    {
        playerDead = true;
        endTimer += Time.deltaTime;
        if (endTimer > endMax)
        {
            SceneManager.LoadScene("DeathMenu");
        }
    }
}
