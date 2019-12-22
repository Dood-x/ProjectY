using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool paused;
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        paused = !paused;
        pauseMenu.SetActive(paused);

        if (paused)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
        }
    }
}
