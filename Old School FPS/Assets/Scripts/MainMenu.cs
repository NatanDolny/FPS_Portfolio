using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public float timer = 0;
    string wantedLevel = "Level1";
    public Slider sensSlider;
    public TMP_Dropdown levelSelect;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Controls")
            sensSlider = transform.Find("SensSlider").GetComponent<Slider>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
            levelSelect = transform.Find("LevelSelect").GetComponent<TMP_Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (SceneManager.GetActiveScene().name == "Controls")
        {
            sensSlider.onValueChanged.AddListener(delegate { ChangeSens(); });
            sensSlider.value = MouseSens.mouseSensitivity;
        }
        if (SceneManager.GetActiveScene().name == "MainMenu")
            levelSelect.onValueChanged.AddListener(delegate { ChangeLevel(); });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Manual")
        {
            timer += Time.deltaTime;
            if (timer > 5)
            {
                SceneManager.LoadScene("Level1");
            }
        }
    }

    public void PlayPressed()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" && wantedLevel == "Level1")
            SceneManager.LoadScene("Manual");
        else
            SceneManager.LoadScene(wantedLevel);
    }    
    public void ControlsPressed()
    {
        SceneManager.LoadScene("Controls");
    }
    public void QuitPressed()
    {
        Application.Quit();
    }
    public void ChangeSens()
    {
        MouseSens.mouseSensitivity = sensSlider.value;
    }
    public void ChangeLevel()
    {
        if (levelSelect.value == 0)
            wantedLevel = "Level1";
        else if (levelSelect.value == 1)
            wantedLevel = "Level2";
        else
            wantedLevel = "Level3";
    }
    public void MenuPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
