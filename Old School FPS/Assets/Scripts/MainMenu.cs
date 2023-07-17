using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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
        SceneManager.LoadScene("Manual");
    }    

    public void QuitPressed()
    {
        Debug.Log("QUIT                                     QUIT");
        Application.Quit();
    }

    public void MenuPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
