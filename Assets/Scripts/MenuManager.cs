using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject TitleSection, ControlsSection, LevelSection, SpecialGroup;
    public UnityEngine.UI.Button[] buttons;
    public TextAsset[] MapFiles;
    public static TextAsset MapFile;
    public static int MapNum;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        if (PlayerPrefs.GetInt("Clear") <= 0) PlayerPrefs.SetInt("Clear", 1);
        if(PlayerPrefs.GetInt("Clear") >= 21) SpecialGroup.gameObject.SetActive(true);
        else SpecialGroup.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Length && i < MapFiles.Length && i < PlayerPrefs.GetInt("Clear"); i++)
        {
            int val = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(val));
            buttons[i].interactable = true;
        }
        if (PlayerPrefs.GetInt("Clear") >= 21)
        {
            for (int i = 21; i < buttons.Length && i < MapFiles.Length; i++)
            {
                int val = i;
                buttons[i].onClick.AddListener(() => OnButtonClick(val));
                buttons[i].interactable = true;
            }
        }
        if(PlayerPrefs.GetInt("FromGame") == 1)
        {
            StartPressed();
        }
    }
    public void StartPressed()
    {
        TitleSection.gameObject.SetActive(false);
        LevelSection.gameObject.SetActive(true);
    }

    public void ControlsPressed()
    {
        TitleSection.gameObject.SetActive(false);
        ControlsSection.gameObject.SetActive(true);
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Application Quit");
    }

    public void BackPressed()
    {
        LevelSection.gameObject.SetActive(false);
        ControlsSection.gameObject.SetActive(false);
        TitleSection.gameObject.SetActive(true);
    }

    public void OnButtonClick(int value)
    {
        Debug.Log("Button value " + value);
        MapFile = MapFiles[value];
        MapNum = value;
        PlayerPrefs.SetInt("Stage", value+1);
        PlayerPrefs.SetInt("NowDeath", 0);
        SceneManager.LoadScene(1);
    }

    public void DevButton()
    {
        PlayerPrefs.SetInt("Clear", 21);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FromGame", 0);
    }
}
