using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<Sprite> backgrounds;
    public List<Sprite> muteIcons;
    public List<Sprite> btnSkins;
    public bool bckg1Active;
    public Image background;
    public Button muteButton;
    public Button skinButton;
    public bool muted;
    public bool colorSkinActive;
    public Button mainButton;

    // Start is called before the first frame update
    void Start()
    {
        bckg1Active = true;
    }

    public void ToggleBackground()
    {
        if (bckg1Active)
        {
            background.sprite = backgrounds[1];
            bckg1Active = false;
        }
        else
        {
            background.sprite = backgrounds[0];
            bckg1Active = true;
        }
    }

    public void ToggleMute()
    {
        if (muted)
        {
            GameManager.Instance.UnMuteSound();
            muteButton.image.sprite = muteIcons[0];
            muted = false;
        }
        else
        {
            GameManager.Instance.MuteSound();
            muteButton.image.sprite = muteIcons[1];
            muted = true;
        }
    }

    public void ToggleSkin()
    {
        if (colorSkinActive)
        {
            mainButton.image.sprite = btnSkins[0];
            colorSkinActive = false;
        }
        else
        {
            mainButton.image.sprite = btnSkins[1];
            colorSkinActive = true;
        }
    }
}
