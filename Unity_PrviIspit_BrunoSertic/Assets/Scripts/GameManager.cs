using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    int score;
    int totalScore;
    public float timeLeft;

    public GameObject scoreText;
    public GameObject timeLeftText;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        timeLeft = GameObject.FindGameObjectsWithTag("Coin").Length * 45;
        totalScore = GameObject.FindGameObjectsWithTag("Coin").Length;
        scoreText.GetComponent<Text>().text = "COINS: " + score + "/" + totalScore;


        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        timeLeftText.GetComponent<Text>().text = "TIME: " + timeLeft.ToString("0");
    }


    public void AddCoin()
    {
        score++;
        scoreText.GetComponent<Text>().text = "COINS: " + score + "/" + totalScore;
    }
}
