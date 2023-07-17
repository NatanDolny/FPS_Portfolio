using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject powerSourceA;
    public GameObject powerSourceB;

    public bool openDoor = false;
    public Animator animator;

    private void Awake()
    {
        animator = transform.Find("Body").GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckOpenState();
    }

    public void CheckOpenState()
    {
        if (!openDoor)
        {
            if (powerSourceA.transform.Find("Body").gameObject.activeSelf == false &&
                powerSourceB.transform.Find("Body").gameObject.activeSelf == false)
            {
                openDoor = true;
                animator.SetTrigger("Open");
            }
        }

    }
}
