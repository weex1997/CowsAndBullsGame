using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine.SceneManagement;

public class AdmobAdsManager : MonoBehaviour
{

#if UNITY_ANDROID
    public string appId = "ca-app-pub-3940256099942544~3347511713";// "ca-app-pub-4418846178940302~8445668203";
    string bannerId = "ca-app-pub-3940256099942544/6300978111";//"ca-app-pub-4418846178940302/7132586538";
    string interId = "ca-app-pub-3940256099942544/1033173712";//"ca-app-pub-4418846178940302/6455312570";
    string rewardedId = "ca-app-pub-3940256099942544/5224354917";//"ca-app-pub-4418846178940302/1011414205";

#elif UNITY_IPHONE
    public string appId = "ca-app-pub-3940256099942544~3347511713";// "ca-app-pub-4418846178940302~1132701692";
    string bannerId = "ca-app-pub-3940256099942544/2934735716";//"ca-app-pub-4418846178940302/2279715106";
    string interId = "ca-app-pub-3940256099942544/4411468910";//"ca-app-pub-4418846178940302/3001769834";
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";//"ca-app-pub-4418846178940302/1688688166";

#elif UNITY_EDITOR
    public string appId = "ca-app-pub-3940256099942544~3347511713";// "ca-app-pub-4418846178940302~8445668203";
    string bannerId = "ca-app-pub-3940256099942544/6300978111";//"ca-app-pub-4418846178940302/7132586538";
    string interId = "ca-app-pub-3940256099942544/1033173712";//"ca-app-pub-4418846178940302/6455312570";
    string rewardedId = "ca-app-pub-3940256099942544/5224354917";//"ca-app-pub-4418846178940302/1011414205";

#endif



    // Singleton instance.
    public static AdmobAdsManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;


    private void Start()
    {
        if (HintText != null)
            HintText.text = PlayerPrefs.GetInt("Hint").ToString();
        if (HintRemoveText != null)
            HintRemoveText.text = PlayerPrefs.GetInt("HintRemove").ToString();

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {

            print("Ads Initialised !!");

        });
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    #region Banner

    public void LoadBannerAd()
    {
        //create a banner
        CreateBannerView();

        //listen to banner events
        ListenToBannerEvents();

        //load the banner
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        print("Loading banner Ad !!");
        bannerView.LoadAd(adRequest);//show the banner on the screen
    }
    void CreateBannerView()
    {

        if (bannerView != null)
        {
            DestroyBannerAd();
        }
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }
    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Banner view paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    public void DestroyBannerAd()
    {

        if (bannerView != null)
        {
            print("Destroying banner Ad");
            bannerView.Destroy();
            bannerView = null;
        }
    }
    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Interstitial ad failed to load" + error);
                return;
            }

            print("Interstitial ad loaded !!" + ad.GetResponseInfo());

            interstitialAd = ad;
            InterstitialEvent(interstitialAd);

        });

    }
    public void ShowInterstitialAd()
    {

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            print("Intersititial ad not ready!!");
        }
    }
    public void InterstitialEvent(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Interstitial ad paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion

    #region Rewarded

    [SerializeField] TMP_Text HintText;
    [SerializeField] TMP_Text HintRemoveText;

    public void LoadRewardedAd()
    {

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Rewarded failed to load" + error);
                return;
            }

            print("Rewarded ad loaded !!");
            rewardedAd = ad;
            RewardedAdEvents(rewardedAd);
        });
    }
    public void ShowRewardedAd(bool itsHint, bool itsHintRemove, bool itsAddAttempet, bool justAdd)
    {

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                print("Give reward to player !!");
                LoadRewardedAd();
                Time.timeScale = 1;

                if (justAdd)
                {
                    if (itsHint)
                    {
                        int hint = PlayerPrefs.GetInt("Hint") + 1;
                        PlayerPrefs.SetInt("Hint", hint);
                        PlayerPrefs.Save();
                        HintText.text = PlayerPrefs.GetInt("Hint").ToString();
                    }
                    else
                    {
                        int hintRemove = PlayerPrefs.GetInt("HintRemove") + 1;
                        PlayerPrefs.SetInt("HintRemove", hintRemove);
                        PlayerPrefs.Save();
                        HintRemoveText.text = PlayerPrefs.GetInt("HintRemove").ToString();
                    }
                }
                else if (!justAdd && !itsAddAttempet)
                {
                    if (itsHint)
                        GameButtonsManager.Instance.ShowHint();
                    else
                        GameButtonsManager.Instance.ShowHintRemoveNumbers();
                }
                else if (itsAddAttempet)
                {
                    // Grant a reward.
                    GameManager.Instance.AttemptUI(1);

                    bool val = true;
                    PlayerPrefs.SetInt("haveAttemptAds", val ? 1 : 0);
                    PlayerPrefs.Save();

                    SceneManager.UnloadSceneAsync("Masseges");
                }

            });
        }
        else
        {
            print("Rewarded ad not ready");
        }
    }
    public void RewardedAdEvents(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Rewarded ad paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion

}