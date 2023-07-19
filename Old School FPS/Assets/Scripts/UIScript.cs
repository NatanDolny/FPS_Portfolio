using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    PlayerStats playerStats;
    public Image manImage;
    public Image keycardImage;
    public TMP_Text armorTmp;
    public TMP_Text healthTmp;
    public TMP_Text ammoTmp;
    public Sprite healthy;
    public Sprite damaged;
    public Sprite critical;
    // Start is called before the first frame update
    void Start()
    {
        manImage = transform.Find("Man").GetComponent<Image>();
        keycardImage = transform.Find("Keycard").GetComponent<Image>();
        healthTmp = transform.Find("Health").GetComponent<TMP_Text>();
        armorTmp = transform.Find("Armor").GetComponent<TMP_Text>();
        ammoTmp = transform.Find("Ammo").GetComponent<TMP_Text>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        armorTmp.text = "" + playerStats.armor;
        healthTmp.text = "" + playerStats.health;
        ammoTmp.text = "" + playerStats.ammo;

        if (playerStats.canProceed)
        {
            keycardImage.enabled = true;
        }
        if (playerStats.health > 60)
        {
            manImage.sprite = healthy;
        }
        else if(playerStats.health <= 60 && playerStats.health > 20)
        {
            manImage.sprite = damaged;
        }
        else 
        {
            manImage.sprite = critical;
        }
    }
}
