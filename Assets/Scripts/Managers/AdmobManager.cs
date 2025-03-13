using UnityEngine;
using System;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public bool IsTestMode; // TestMode ��, �ν����Ϳ��� Ŭ��
    private RewardedAd _rewardedAd;
    private BannerView _bannerView;
    private string _adRewardUnitId;
    private string _adBannerUnitId;

    void Awake()
    {
        Instance = this;

        Init();
    }

    public void Init()
    {

        if (IsTestMode)
        {
            // �׽�Ʈ�� ID
            _adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";
            _adBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
        }
        else
        {
#if UNITY_ANDROID
            // ���� ID
            _adRewardUnitId = "ca-app-pub-5906820670754550/6581913731";
            _adBannerUnitId = "";
#elif UNITY_IPHONE
            // �׽�Ʈ�� ID
            _adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
            _adBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            _adRewardUnitId = "unused";
            _adBannerUnitId = "unused";
#endif
        }

        MobileAds.Initialize((InitializationStatus initStatus) => { });
    }

    public void Release()
    {

    }

    #region Banner
    //���� �ε�, ��� �� ȣ��
    public void LoadBannerAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    //���� �����ֱ�
    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyAd();
        }

        _bannerView = new BannerView(_adBannerUnitId, AdSize.Banner, AdPosition.Top);

        //������ ���(���� ������)
        //AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        //_bannerView = new BannerView(_adBannerUnitId, adaptiveSize, AdPosition.Bottom);
    }

    //���� ǥ��
    public void ShowAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Show banner ad.");
            _bannerView.Show();
        }
        else
        {
            LoadBannerAd();
        }
    }

    //���� �����
    public void HideAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hide banner ad.");
            _bannerView.Hide();
        }
    }

    //���� ����
    private void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        _bannerView.OnAdFullScreenContentOpened += (null);
        {
            Debug.Log("Banner view full screen content opened.");
        };
        _bannerView.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    #endregion

    #region Reward
    //��� ��, ȣ��
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adRewardUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
                ShowRewardedAd();
            });
    }

    private void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // ���� ���� �Է�       
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
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

            // ���� â�� ���� ��, ������ ����
            SceneManager.LoadScene("GameScene");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            LoadRewardedAd();
        };
    }
    #endregion
}