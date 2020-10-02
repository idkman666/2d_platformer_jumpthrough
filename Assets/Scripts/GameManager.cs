using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int PLAYER_HEALTH = 3;
    CinemachineVirtualCamera cinemachineVirtual;
    PlayerController player;

    [SerializeField] float zoomedOutValue = 68f;
    bool isZoomOutPressed = false;
    float smoothingRate = 2.5f;

    [SerializeField]
    GameObject globalMusic;

    //audio
    [Header("Audio Btn")]
    [SerializeField] Sprite audioOff;
    [SerializeField] Sprite audioOn;
    [SerializeField] Button audioBtn;
    Image audioSprite;

    [Header("Player Stop Btn")]
    [SerializeField] Button stopPlayerBtn;
    [SerializeField] Sprite ads;
    [SerializeField] Sprite stopPlayer;
    Image stopPlayerSprite;

    [Header("Player Run Btn")]
    [SerializeField] Button runBtn;
    [SerializeField] Sprite runSprite;
    [SerializeField] Sprite jumpSprite;
    Image runJumpBtnSprite;

    [Header("Error Panel")]
    [SerializeField] GameObject errorPanel;

    //ads
    AdManager adManager;
    public static int deathsToVideoAd = 0;
    public static int rewardVideoAdCount = 1;

    //camera shake
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    float shakeTime = 0.5f;

    public static bool isPausePressed = false;
    private void Awake()
    {
        cinemachineVirtual = GetComponent<CinemachineVirtualCamera>();
        errorPanel.SetActive(false);
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        GameObject musicHolder = GameObject.Find("GlobalMusic(Clone)");
        adManager = GetComponent<AdManager>();
        if (musicHolder == null)
        {
            Instantiate(globalMusic);
        }

        audioSprite = audioBtn.GetComponent<Image>();
        stopPlayerSprite = stopPlayerBtn.GetComponent<Image>();
        runJumpBtnSprite = runBtn.GetComponent<Image>();
        if (AudioListener.volume <= 0.0f)
        {
            audioSprite.sprite = audioOff;
        }
        else
        {
            audioSprite.sprite = audioOn;
        }
        cinemachineBasicMultiChannelPerlin = cinemachineVirtual.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void OnEnable()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        cinemachineVirtual.Follow = player.transform;
        cinemachineVirtual.LookAt = player.transform;
    }

    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }

    public void StopPlayer()
    {
        if (isPausePressed == true)
        {            
            bool adActive = adManager.PlayRewardedVideoAd();
            if (adActive)
            {
                isPausePressed = false;
                stopPlayerSprite.sprite = stopPlayer;

                if (rewardVideoAdCount > 0)
                {
                    rewardVideoAdCount--;
                }
                else if (rewardVideoAdCount < 1)
                {
                    rewardVideoAdCount = 0;
                    stopPlayerBtn.interactable = false;
                }
            }

        }
        else if (isPausePressed == false)
        {
            isPausePressed = true;
            stopPlayerSprite.sprite = ads;
            player.StopPlayer();
        }
    }
    private void FixedUpdate()
    {
        if (isZoomOutPressed)
        {
            cinemachineVirtual.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineVirtual.m_Lens.OrthographicSize, 68f, Time.deltaTime * smoothingRate);
        }
        else
        {
            if (cinemachineVirtual.m_Lens.OrthographicSize >= 28f)
            {
                cinemachineVirtual.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineVirtual.m_Lens.OrthographicSize, 28f, Time.deltaTime * smoothingRate);
            }
        }

        if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain > 0)
        {
            shakeTime -= Time.deltaTime;
            if (shakeTime <= 0)
            {
                shakeTime = 0.5f;
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }

        if (player.isStart == true)
        {
            runJumpBtnSprite.sprite = jumpSprite;

        }
        else
        {
            runJumpBtnSprite.sprite = runSprite;
        }
    }
    public void ZoomOut()
    {
        isZoomOutPressed = true;
    }

    public void ZoomOutOff()
    {
        isZoomOutPressed = false;
    }

    public void MuteAudio()
    {
        if (AudioListener.volume >= 1.0f)
        {
            AudioListener.volume = 0.0f;
            audioSprite.sprite = audioOff;
        }
        else
        {
            AudioListener.volume = 1.0f;
            audioSprite.sprite = audioOn;
        }

    }

    public void ShakeCamera()
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 3;
    }
    public void PlayVideoAd()
    {
        adManager.PlayVideoAd();
    }
    public void PlayerJumpOrRun()
    {
        player.JumpOrRun();
    }

    public void ShowAdsError()
    {
        errorPanel.SetActive(true);
    }

    public void HideAdsError()
    {
        errorPanel.SetActive(false);
    }

    public void ResetStopButton()
    {
        rewardVideoAdCount = 1;
        stopPlayerSprite.sprite = stopPlayer;
        stopPlayerBtn.interactable = true;
    }
}


