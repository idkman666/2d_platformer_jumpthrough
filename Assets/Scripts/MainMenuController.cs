using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject levelPanel;
    public GameObject btnPlay;

    //Level controller
    private int LevelsUnlocked = 1;
    public List<Button> levelBtns;
    [SerializeField] GameObject gameCompleteImage;
    int maxLevel = 10;
    //death counter
    [SerializeField] Text deathCounter;

    //global music
    public GameObject globalMusic;

    private void Awake()
    {
        deathCounter.text = PlayerPrefs.GetInt("TotalDeaths").ToString();
        levelPanel.SetActive(false);
        btnPlay.SetActive(true);
        for (int i = 1; i < levelBtns.Count; i++)
        {
            levelBtns[i].interactable = false;
        }
        GameObject musicHolder = GameObject.Find("GlobalMusic(Clone)");
        
        if (musicHolder == null)
        {
            Instantiate(globalMusic);
        }
    }
    private void Start()
    {
        LevelsUnlocked = PlayerPrefs.GetInt("LevelPlayed");  
        if(LevelsUnlocked >= 11)
        {
            gameCompleteImage.SetActive(true);
        }
        for (int i = 0; i <  (LevelsUnlocked>maxLevel ? maxLevel : LevelsUnlocked); i++)
        {
            levelBtns[i].interactable = true;
        }
    }

    public void LoadLevelsPanel()
    {
        btnPlay.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void LoadLevel(int lvlIndex)
    {
        switch (lvlIndex)
        {
            case 1:
                SceneManager.LoadScene(1);
                break;
            case 2:
                SceneManager.LoadScene(2);
                break;
            case 3:
                SceneManager.LoadScene(3);
                break;
            case 4:
                SceneManager.LoadScene(4);
                break;
            case 5:
                SceneManager.LoadScene(5);
                break;
            case 6:
                SceneManager.LoadScene(6);
                break;
            case 7:
                SceneManager.LoadScene(7);
                break;
            case 8:
                SceneManager.LoadScene(8);
                break;
            case 9:
                SceneManager.LoadScene(9);
                break;
            case 10:
                SceneManager.LoadScene(10);
                break;
        }
    }
}
