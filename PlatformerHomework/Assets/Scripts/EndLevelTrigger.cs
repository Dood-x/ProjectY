using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndLevelTrigger : MonoBehaviour
{
    public GameObject endLevelScreen;
    public GameObject darkScreen;
    public Text crystalText;
    public Text timeText;
    public Text scoreText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(FadeInLevel());

            CalculateScore();
        }
    }

    private IEnumerator FadeInLevel()
    {
        while (darkScreen.GetComponent<Image>().color.a < 1)
        {
            var color1 = darkScreen.GetComponent<Image>().color;
            color1.a += .13f;
            darkScreen.GetComponent<Image>().color = color1;

            yield return new WaitForSeconds(.05f);
        }

        if (darkScreen.GetComponent<Image>().color.a >= 1)
        {
            endLevelScreen.SetActive(true);
            StartCoroutine(FadeOutLevel());
        }

    }
    private IEnumerator FadeOutLevel()
    {
        while (darkScreen.GetComponent<Image>().color.a > 0)
        {
            var color1 = darkScreen.GetComponent<Image>().color;
            color1.a -= .13f;
            darkScreen.GetComponent<Image>().color = color1;

            yield return new WaitForSeconds(.03f);
        }

        if (darkScreen.GetComponent<Image>().color.a <= 0)
        {
            darkScreen.SetActive(false);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    private void CalculateScore()
    {
        Player player = FindObjectOfType<Player>();
        float score = player.GetHealth() * player.GetCrystals();
        score = score / (player.GetTimeAlive() * .3f);

        crystalText.text = "Crystals: " + player.GetCrystals();
        timeText.text = "Time: " + player.GetTimeAlive().ToString("F2");
        scoreText.text = "Score: " + Convert.ToInt16(score);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
