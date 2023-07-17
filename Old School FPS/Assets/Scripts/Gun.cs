using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject gunObject;
    public GameObject gameManager;
    public GameObject bulletPrefab; 

    public int maxAmmo = 10;
    public int currentAmmo;

    public GameObject shootingPoint; 

    public enum GunType
    {
        KNIFE = 0,
        AR = 1, 
        SNIPER = 2, 
        ROCKET = 3, 
        GRENADELAUNCHER = 4,
        BOW = 5, 
        LASER = 6
    }

    public GunType gunType = GunType.KNIFE;

    private void Awake()
    {
        shootingPoint = transform.Find("ShootingPoint").gameObject; 
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
        transform.rotation =  Quaternion.Euler(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Attack()
    {
        bool collided = false;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * 15, Color.yellow, 30, true);
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit, 15))
        {
            if (hit.transform.CompareTag("PowerSource"))
            {
                hit.transform.GetComponent<AudioSource>().Play();
                hit.transform.GetComponent<CapsuleCollider>().enabled = false;
                hit.transform.Find("Body").gameObject.SetActive(false);
            }
            if (hit.transform.CompareTag("Destructible"))
            {
                hit.transform.GetComponent<AudioSource>().Play();
                hit.transform.GetComponent<BoxCollider>().enabled = false;
                hit.transform.Find("Body").gameObject.SetActive(false);
            }
            if (hit.transform.name == "Head")
            {
                collided = true;
                hit.transform.parent.parent.GetComponent<EnemyController>().hitPoints -= 3;  
            }
            else if (hit.transform.name == "Torso")
            {
                collided = true;
                hit.transform.parent.parent.GetComponent<EnemyController>().hitPoints--;
            }
            if (collided)
            {
                if (hit.transform.parent.parent.GetComponent<EnemyController>().hitPoints <=0)
                {
                    Debug.Log("hit object of name: " + hit.transform.name + "Hitpoints " + 
                        hit.transform.parent.parent.GetComponent<EnemyController>().hitPoints);
                    hit.transform.parent.parent.GetComponent<EnemyController>().EnemyDied();
                }
                else
                {
                    Debug.Log("Hitpoints: " + 
                        hit.transform.parent.parent.GetComponent<EnemyController>().hitPoints);
                }
            }
                
        }
        else
        {
            collided = false;
        }
        return collided; 
    }
}
