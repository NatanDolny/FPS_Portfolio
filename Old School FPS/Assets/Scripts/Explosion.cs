using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    GameObject player; 

    public int damage = 6;
    int radius = 4;
    float delay = 0.25f;
    float duration = 1.5f;
    float timer = 0;
    bool exploded = false;
    SphereCollider explosion;
    AudioSource audioSource;
    public AudioClip explosionAudio; 
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 

        explosion = transform.GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = explosionAudio;
    }

    // Update is called once per frame
    void Update()
    {
        if (!exploded)
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                exploded = true;
                //explosion.enabled = true;
                //explosion.radius = radius;
                audioSource.Play();
                timer = 0;

                Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.name == "Torso")
                    {
                        hitCollider.transform.parent.parent.GetComponent<EnemyController>().EnemyHit(damage);
                    }
                    if (hitCollider.CompareTag("Player"))
                    {
                        hitCollider.GetComponent<PlayerController>().PlayerHit();
                        hitCollider.GetComponent<PlayerStats>().health -= 20;
                    }
                    if (hitCollider.CompareTag("PowerSource"))
                    {
                        hitCollider.transform.GetComponent<AudioSource>().Play();
                        hitCollider.transform.GetComponent<CapsuleCollider>().enabled = false;
                        hitCollider.transform.Find("Body").gameObject.SetActive(false);
                    }
                    if (hitCollider.CompareTag("Destructible"))
                    {
                        hitCollider.transform.GetComponent<AudioSource>().Play();
                        hitCollider.transform.GetComponent<BoxCollider>().enabled = false;
                        hitCollider.transform.Find("Body").gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            transform.LookAt(player.transform);
            timer += Time.deltaTime;
            if (timer > duration)
            {
                Destroy(this.gameObject);
            }
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.transform.parent.parent.GetComponent<EnemyController>().EnemyHit(damage);
        }
    }*/
}
