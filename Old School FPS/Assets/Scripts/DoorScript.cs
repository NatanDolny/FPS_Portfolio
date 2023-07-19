using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    PlayerStats playerStats;

    public GameObject powerSourceA;
    public GameObject powerSourceB;

    public bool openDoor = false;
    public bool ignorePowerSource = false;
    public Animator animator;
    public AudioSource audioSource;
    public Transform lastEnemiesParent;

    private void Awake()
    {
        animator = transform.Find("Body").GetComponent<Animator>();
        audioSource = transform.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void CheckOpenState()
    {
        if (!openDoor)
        {
            if ((ignorePowerSource && playerStats.canProceed) || (powerSourceA.transform.Find("Body").gameObject.activeSelf == false &&
                powerSourceB.transform.Find("Body").gameObject.activeSelf == false && 
                playerStats.canProceed))
            {
                if (lastEnemiesParent.childCount == 0)
                    OpenDoor();
            }
        }
    }

    public void OpenDoor()
    {
        openDoor = true;
        audioSource.Play();
        playerStats.keySound.Play();
        animator.SetTrigger("Open");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckOpenState();
        }
    }
}
