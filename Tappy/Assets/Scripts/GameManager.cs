using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Sprite> mainMenuIcons;
    public static GameManager Instance;
    public SaveManager saveManager;
    public ParticleSystem buttonPressedFX;
    public GameObject buttonHolder;
    public Button mainButton;
    public Button menuButton;
    public bool menuActive;

    public float size = 0.8f;
    private int count;
    public Text textCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseCount(int amount)
    {
        if (!menuActive)
        {
            buttonPressedFX.Play();
            count += amount;
            saveManager.state.count = count;
            textCount.text = count.ToString();
            saveManager.Save();
            textCount.fontSize = 56;
            StartCoroutine(ScaleCount());
        }
    }

    public IEnumerator ScaleCount()
    {
        do
        {
            textCount.fontSize += 2;
            yield return new WaitForEndOfFrame();
        } while (textCount.fontSize < 85);
    }

    public void SetStats()
    {
        count = saveManager.state.count;
        textCount.text = count.ToString();
    }

    public void ToggleMenu()
    {
        Debug.Log("Clicked");
        menuActive = !menuActive;
        buttonHolder.SetActive(menuActive);
        mainButton.interactable = !menuActive;

        if (menuActive)
        {
            menuButton.gameObject.SetActive(true);
            menuButton.image.sprite = mainMenuIcons[0];
        }
        else
        {
            menuButton.gameObject.SetActive(false);

            menuButton.image.sprite = mainMenuIcons[1];
            buttonHolder.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
