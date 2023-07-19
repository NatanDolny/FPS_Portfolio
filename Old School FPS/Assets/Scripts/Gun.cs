using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject gunObject;
    public GameObject gameManager;
    public GameObject bulletPrefab;
    public GameObject rocketPrefab;
    Transform shootPoint;
    public int maxAmmo = 5;
    public int currentAmmo = 0;
    public float maxReload = 1.5f; 
    bool reload = false;
    public bool reloaded = false;
    float range;
    float reloadTimer = 0;
    AudioSource reloadAudio; 

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
        shootPoint = transform.Find("ShootingPoint");
        reloadAudio = shootPoint.GetComponent<AudioSource>();
        if (gunType == GunType.ROCKET)
        {
            range = 50;
        }
        else if (gunType == GunType.AR)
        {
            range = 18;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (currentAmmo >= 1)
            reloaded = true;
        else
            reloaded = false; 
    }

    void FixedUpdate()
    {
        if (reload)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > maxReload)
            {
                reloaded = true;
                reload = false;
                reloadTimer = 0;
                if (gunType == GunType.AR)
                {
                    currentAmmo = 1; 
                }
            }
        }
    }

    public void Reload()
    {
        reloadAudio.Play();
        reload = true;
    }

    public bool Attack()
    {
        currentAmmo--;
        bool collided = false;
        Ray ray = new Ray(shootPoint.position, transform.forward);
        Debug.DrawRay(shootPoint.position, transform.forward * 15, Color.yellow, 30, true);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
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
            if (gunType == GunType.AR)
            {
                if (hit.transform.name == "Head")
                {
                    collided = true;
                    int damage = hit.distance < 15 ? 3 : 1;
                    hit.transform.parent.parent.GetComponent<EnemyController>().EnemyHit(damage);
                }
                else if (hit.transform.name == "Torso")
                {
                    collided = true;
                    hit.transform.parent.parent.GetComponent<EnemyController>().EnemyHit(1);
                }
            }
            else if (gunType == GunType.ROCKET)
            {
                Vector3 explosionCoord = hit.point;
                Instantiate(rocketPrefab, explosionCoord, transform.rotation);
            }
        }
        else
        {
            collided = false;
        }
        return collided;
    }
}
