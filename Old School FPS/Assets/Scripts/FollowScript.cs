using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    EnemyController controller;
    GameObject player;
    
    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<EnemyController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
    }
}
