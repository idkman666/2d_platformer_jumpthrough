using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    string gameId = "3786979";
    string rewardedVideoId = "rewardedVideo";
    string videoAdId = "video";


    public bool isTestAd = false;

    float gameAudioBeforeAd;
    [Header("Parent")]
    [SerializeField] GameObject parent;
    GameManager gameManager;
    private void Start()
    {
        gameManager = parent.GetComponent<GameManager>();
        Advertisement.AddListener(this);
        IniializeAdvertisement();
    }

    void IniializeAdvertisement()
    {
        Advertisement.Initialize(gameId, isTestAd);
    }

    public bool PlayRewardedVideoAd()
    {
        ShutDownAudio();
        if (!Advertisement.IsReady(rewardedVideoId))
        {
            gameManager.ShowAdsError();
            TurnOnAudio();
            return false;
        }
        Advertisement.Show(rewardedVideoId);
        return true;
    }

    public void PlayVideoAd()
    {
        ShutDownAudio();
        if (!Advertisement.IsReady(videoAdId))
        {
            gameManager.ShowAdsError();
            TurnOnAudio();
            return;
        }
        Advertisement.Show(videoAdId);
    }
    void IUnityAdsListener.OnUnityAdsDidError(string message)
    {
        //show error
        gameManager.ShowAdsError();
        TurnOnAudio();
    }

    void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                GameManager.deathsToVideoAd = 0;
                TurnOnAudio();
                break;
            case ShowResult.Skipped:
                if (placementId == videoAdId)
                {
                    GameManager.deathsToVideoAd = 0;                                      
                    TurnOnAudio();
                }
                if (placementId == rewardedVideoId)
                {
                    //reward                    
                    TurnOnAudio();
                }                
                break;
            case ShowResult.Finished:
                if (placementId == rewardedVideoId)
                {
                    //reward                    
                    TurnOnAudio();
                }
                if (placementId == videoAdId)
                {
                    GameManager.deathsToVideoAd = 0;
                    //video ad finished.
                    TurnOnAudio();
                }
                break;
        }
    }

    void IUnityAdsListener.OnUnityAdsDidStart(string placementId)
    {
        if (placementId == rewardedVideoId || placementId == videoAdId)
        {            
            if (AudioListener.volume > 0.0f)
            {
                AudioListener.volume = 0.0f;               
            }
        }
    }

    void IUnityAdsListener.OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    void ShutDownAudio()
    {
        gameAudioBeforeAd = AudioListener.volume;       
        if (AudioListener.volume >= 0.0f)
        {
            AudioListener.volume = 0f;
        }
    }

    void TurnOnAudio()
    {
        AudioListener.volume = gameAudioBeforeAd;
    }
}
