using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject controlsScreen;
    public Slider sensSlider;
    Canvas canvas;

    public bool paused = false;

    private void Awake()
    {
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        pauseScreen = canvas.transform.Find("PauseMenu").gameObject;
        controlsScreen = canvas.transform.Find("Controls").gameObject;
        sensSlider = controlsScreen.transform.Find("SensSlider").GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sensSlider.value = MouseSens.mouseSensitivity;

        sensSlider.onValueChanged.AddListener(delegate { ChangeSens(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        Initialise(false);
    }

    public void Controls()
    {
        pauseScreen.gameObject.SetActive(false);
        controlsScreen.gameObject.SetActive(true);
    }

    public void ChangeSens()
    {
        MouseSens.mouseSensitivity = sensSlider.value;
    }
    public void Pause()
    {
        pauseScreen.gameObject.SetActive(true);
        controlsScreen.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Initialise(bool toPause)
    {
        if (toPause)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            paused = true; 
            canvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Pause();
            canvas.gameObject.SetActive(false);
            paused = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
