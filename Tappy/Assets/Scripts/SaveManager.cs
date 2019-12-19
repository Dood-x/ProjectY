using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveState state;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }

        Load();
        Debug.Log(Utility.Serialize<SaveState>(state));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            state = Utility.Deserialize<SaveState>(PlayerPrefs.GetString("Save"));
            GameManager.Instance.SetStats();
        }
        else
        {
            Debug.Log("No save, creating one");
            state = new SaveState();
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Save", Utility.Serialize<SaveState>(state));
    }
}
