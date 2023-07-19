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

    private AudioSource shotSource;
    private AudioSource painSource;

    public AudioClip[] painGrunts;
    public AudioClip deathGrunt;
    public AudioClip gunAudio;
    public AudioClip rocketAudio;


    public GameObject gameManager;
    public GameObject gun;
    public Animator gunAnimator;
    public GameObject rocketLauncher;
    public Animator rocketAnimator;
    public GameObject rLAnimator;

    public bool canShoot = true;
    public float reloadTimer = 0;
    public float reloadDuration = 1.5f;
    public Gun equippedGun;


    public bool rocketEquipped = false;
    public int rocketAmmo = 0;

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

    PauseMenu pauseMenu;
    [HideInInspector] public bool paused;

    private void Awake()
    {
        playerMovement = new PlayerMovement();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        cameraObject = transform.Find("Main Camera").gameObject;
        gun = cameraObject.transform.Find("Gun").gameObject;
        rocketLauncher = cameraObject.transform.Find("RocketLauncher").gameObject;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>(); 
        playerMovement.Movement.Attack.performed += LeftClick;
        playerMovement.Movement.Jump.performed += Jump;
        playerMovement.Movement.Swap.performed += Swap;
        playerMovement.Movement.Pause.performed += Pause;
        pauseMenu = transform.Find("PauseMenu").GetComponent<PauseMenu>();
    }

    private void OnEnable()
    {
        playerMovement.Enable(); 
    }

    private void OnDisable()
    {
        playerMovement.Disable(); 
    }

    void Start()
    {
        Invoke("SetRotation", 0.05f);
        playerStats = GetComponent<PlayerStats>();
        shotSource = transform.Find("ShotSource").GetComponent<AudioSource>();
        painSource = transform.Find("PainSource").GetComponent<AudioSource>();
        gunAnimator = gun.transform.Find("Body").GetComponent<Animator>();
        rocketAnimator = rocketLauncher.transform.Find("Body").GetComponent<Animator>();
        rocketEquipped = false;
        rocketLauncher.transform.Find("Body").gameObject.SetActive(false);
        equippedGun = gun.GetComponent<Gun>();
    }


    void Update()
    {
        Movement(); 
        CameraControl();
    }
    private void FixedUpdate()
    {
        playerStats.ammo = equippedGun.currentAmmo;
    }

    public void CameraControl()
    {
        if (!playerStats.playerDead && !pauseMenu.paused)
        {
            mouseRotation = Mouse.current.delta.ReadValue();
            xRotation -= mouseRotation.y * Time.deltaTime * mouseXSens * MouseSens.mouseSensitivity;
            yRotation += mouseRotation.x * Time.deltaTime * mouseYSens * MouseSens.mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, -30, 60);
            cameraObject.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }

    public void Movement()
    {
        if (!playerStats.playerDead && !pauseMenu.paused)
        {
            Vector2 movement = playerMovement.Movement.Movement.ReadValue<Vector2>();
            var forward = cameraObject.transform.forward;
            var right = cameraObject.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            var desiredDirection = forward * movement.y + right * movement.x;
            rb.velocity = (forward * movement.y + right * movement.x) * moveVelocity;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Swap(InputAction.CallbackContext ctx)
    {
        if (!pauseMenu.paused)
        {
            if (equippedGun.gunType == Gun.GunType.AR)
            {
                rocketLauncher.transform.Find("Body").gameObject.SetActive(true);
                gun.transform.Find("Body").gameObject.SetActive(false);
                playerStats.ammo = rocketLauncher.GetComponent<Gun>().currentAmmo;
                equippedGun = rocketLauncher.GetComponent<Gun>();
            }
            else
            {
                rocketLauncher.transform.Find("Body").gameObject.SetActive(false);
                gun.transform.Find("Body").gameObject.SetActive(true);
                playerStats.ammo = gun.GetComponent<Gun>().currentAmmo;
                equippedGun = gun.GetComponent<Gun>();
            }
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {

    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        if (!pauseMenu.paused)
        {
            pauseMenu.paused = true;
        }
        else
        {
            Debug.Log("REPREESSED ESC");
            pauseMenu.paused = false;
        }
        paused = pauseMenu.paused; 
        Debug.Log("paused is: " + paused + " or pause pause " + pauseMenu.paused);
        pauseMenu.Initialise(pauseMenu.paused);
    }

    public void LeftClick(InputAction.CallbackContext ctx)
    {
        if (!playerStats.playerDead && equippedGun.reloaded && equippedGun.currentAmmo > 0 && !pauseMenu.paused)
        {
            if (equippedGun.gunType == Gun.GunType.ROCKET)
            {
                playerStats.ammo = rocketLauncher.GetComponent<Gun>().currentAmmo;
                rocketAnimator.SetTrigger("Attack");
                rocketLauncher.GetComponent<Gun>().Attack();
                shotSource.clip = rocketAudio;
                shotSource.Play();
                rocketLauncher.GetComponent<Gun>().Reload();
            }
            else if (equippedGun.gunType == Gun.GunType.AR)
            {
                playerStats.ammo = gun.GetComponent<Gun>().currentAmmo;
                gunAnimator.SetTrigger("Attack");
                gun.GetComponent<Gun>().Attack();
                shotSource.clip = gunAudio;
                shotSource.Play();
                gun.GetComponent<Gun>().Reload();
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                EnemyController controller;
                if (enemy.TryGetComponent<EnemyController>(out controller))
                {
                    if (Vector3.Distance(enemy.transform.position, transform.position) < controller.sightRange)
                    {
                        enemy.transform.LookAt(new Vector3(transform.position.x, controller.sprite.transform.position.y, transform.position.z));
                        if (controller.currentState != EnemyController.CurrentState.ATTACKING)
                            controller.currentState = EnemyController.CurrentState.IDLE;
                    }
                }
            }
        }
    }

    void SetRotation()
    {
        cameraObject.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void PlayerHit()
    {
        if (playerStats.health > 0)
        {
            painSource.clip = painGrunts[Random.Range(0, painGrunts.Length)];
        }
        else
        {
            painSource.clip = deathGrunt;
        }
        painSource.Play();
    }
}
