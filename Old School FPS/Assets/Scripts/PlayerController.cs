using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private InputAction inputController;
    //private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    private Rigidbody rb; 

    private AudioSource audioSource;

    public GameObject gameManager;
    public GameObject gun;
    public Animator gunAnimator;
    public bool canShoot = true;
    public float reloadTimer = 0;
    public float reloadDuration = 1.5f;
    public GameObject upperBody;
    public GameObject cameraObject;
    //public Transform shootingPoint; 

    public float mouseXSens = 30;
    public float mouseYSens = 30;
    private float xRotation = 0;
    private float yRotation = 0; 
    public Vector2 mouseRotation; 

    public float moveVelocity = 10;
    public float jumpVelocity = 100; 

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        //inputController = GetComponent<InputAction>();
        //playerInput = GetComponent<PlayerInput>(); 
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        //shootingPoint.position = gun.transform.Find("ShootingPoint").position;
        //gun.GetComponent<Gun>().shootingPoint = shootingPoint; 
        //upperBody = transform.Find("PlayerBody").Find("UpperBody").gameObject;
        cameraObject = transform.Find("Main Camera").gameObject;
        gun = cameraObject.transform.Find("Gun").gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        rb = GetComponent<Rigidbody>(); 
        playerMovement.Movement.Attack.performed += LeftClick;
        playerMovement.Movement.Jump.performed += Jump;
    }

    private void OnEnable()
    {
        playerMovement.Enable(); 
    }

    private void OnDisable()
    {
        playerMovement.Disable(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetRotation", 0.05f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerStats = GetComponent<PlayerStats>();
        audioSource = transform.Find("AudioSource").GetComponent<AudioSource>();
        gunAnimator = gun.transform.Find("Body").GetComponent<Animator>();
        //upperBody.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement(); 
        CameraControl();
    }

    public void CameraControl()
    {
        if (!playerStats.playerDead)
        {
            mouseRotation = Mouse.current.delta.ReadValue();
            xRotation -= mouseRotation.y * Time.deltaTime * mouseXSens;
            yRotation += mouseRotation.x * Time.deltaTime * mouseYSens;
            xRotation = Mathf.Clamp(xRotation, -30, 60);
            cameraObject.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        
        //transform.Find("Texture").transform.rotation = cameraObject.transform.rotation;
    }

    public void Movement()
    {
        if (!canShoot)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadDuration)
            {
                canShoot = true;
                reloadTimer = 0;
            }
        }
        if (!playerStats.playerDead)
        {
            Vector2 movement = playerMovement.Movement.Movement.ReadValue<Vector2>();
            //Vector3 finalVelocities = new Vector3();
            var forward = cameraObject.transform.forward;
            var right = cameraObject.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            var desiredDirection = forward * movement.y + right * movement.x;
            rb.velocity = (forward * movement.y + right * movement.x) * moveVelocity;
            rb.angularVelocity = Vector3.zero;
            //transform.Translate(desiredDirection * moveVelocity * Time.deltaTime);
        }
        
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        //rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z); 

    }

    public void LeftClick(InputAction.CallbackContext ctx)
    {
        if (!playerStats.playerDead && canShoot)
        {
            canShoot = false;
            gunAnimator.SetTrigger("Attack");
            audioSource.Play();
            if (gun.GetComponent<Gun>().Attack())
            {
                Debug.Log("enemy hit");
            }
            else
            {
                Debug.Log("MISSED");
            }
        }
    }

    void SetRotation()
    {
        cameraObject.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
