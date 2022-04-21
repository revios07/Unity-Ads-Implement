using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private static AdManager instance;

    #region Ads 
    private string _gameIdAndroid = "";
    private string _gameIdIos = "";

    private string _placementIdBannerAndroid = "";
    private string _placementIdBannerIos = "";

    [SerializeField]
    //<summary>
    //Use For Editor Or Test Phone
    //Not Show Real Ads
    //</summary>
    private bool _testMode = true;

    private bool _canShowFullScreenAdThisLevel = false;
    private bool _isBannerEnumWorks = false;

    #endregion

    private IEnumerator _showBanner;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static AdManager GetSingleton()
    {
        return instance;
    }

    private void Start()
    {
        //Open Ad For Every 3 Level Start
        _canShowFullScreenAdThisLevel = PlayerPrefs.GetInt("ShowFullScreenAd", 0) >= 3;
        PlayerPrefs.SetInt("ShowFullScreenAd", PlayerPrefs.GetInt("ShowFullScreenAd", 0) + 1);
        if (_canShowFullScreenAdThisLevel)
        {
            PlayerPrefs.SetInt("ShowFullScreenAd", 0);
        }

        //Hide Banner For Other Levels
        Advertisement.Banner.Hide();

        Advertisement.Initialize(_gameIdAndroid, _testMode);

        ShowAd();
    }

    //Call At When Player Dead 3times Or When 3Level Played
    public void ShowAd()
    {
        if (!_canShowFullScreenAdThisLevel)
            return;

        StartCoroutine(nameof(ShowFullScreenAd));
    }

    //Call At Every End Game When Turning Camera
    public void ShowBannerAd()
    {
        if (_showBanner != null || _isBannerEnumWorks)
            return;

        _isBannerEnumWorks = true;

        _showBanner = ShowBanner();
        StartCoroutine(_showBanner);
    }

    #region Need Dev Methods
    private IEnumerator ShowFullScreenAd()
    {
        while(!Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.1f);
        }

        Advertisement.Show();
    }

    private IEnumerator ShowBanner()
    {
        while (!Advertisement.IsReady(_placementIdBannerAndroid))
        {
            yield return new WaitForSeconds(0.25f);
        }

        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(_placementIdBannerAndroid);
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("Player Whatched the Ad");
                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("Player Didnt Fully Watch the Ad");
                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.Log("Player to Launch Ad? Internet?");
                    break;
                }
        }
    }
    #endregion
}
