using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Tools;

namespace Core
{
    /// <summary>
    /// Quản lý quảng cáo Google AdMob (Rewarded Ad).
    /// Gắn vào một GameObject trong scene game.
    /// App ID: ca-app-pub-3732703191204945~6042380960
    /// </summary>
    public class AdManager : SingletonMonoBehaviour<AdManager>
    {
        // ===== CÀI ĐẶT AD UNIT ID =====
        // TODO: Thay bằng Rewarded Ad Unit ID thật lấy từ AdMob Console của bạn!
        // Hiện tại đang dùng ID TEST — quảng cáo test sẽ hiện nhưng không tính tiền.
#if UNITY_ANDROID
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917"; // Test Android
#elif UNITY_IOS
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/1712485313"; // Test iOS
#else
        private const string AD_UNIT_ID = "unused";
#endif

        private RewardedAd _rewardedAd;

        /// <summary>True nếu quảng cáo đã tải xong và sẵn sàng hiển thị.</summary>
        public bool IsAdReady => _rewardedAd != null && _rewardedAd.CanShowAd();

        protected override void Awake()
        {
            base.Awake();
            // Khởi tạo AdMob SDK rồi tải quảng cáo
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("[AdManager] Google Mobile Ads initialized.");
                LoadRewardedAd();
            });
        }

        // ─────────────────────────────────────────────
        // LOAD AD
        // ─────────────────────────────────────────────
        public void LoadRewardedAd()
        {
            // Huỷ ad cũ nếu còn tồn tại
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            Debug.Log("[AdManager] Loading rewarded ad...");
            RewardedAd.Load(AD_UNIT_ID, new AdRequest.Builder().Build(), OnRewardedAdLoaded);
        }

        private void OnRewardedAdLoaded(RewardedAd ad, LoadAdError loadError)
        {
            if (loadError != null || ad == null)
            {
                Debug.LogError("[AdManager] Rewarded ad failed to load: " + loadError?.GetMessage());
                return;
            }

            _rewardedAd = ad;

            // Đăng ký sự kiện
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("[AdManager] Ad closed — loading next ad.");
                LoadRewardedAd();
            };

            _rewardedAd.OnAdFullScreenContentFailed += (AdError showError) =>
            {
                Debug.LogError("[AdManager] Ad failed to show: " + showError.GetMessage());
                LoadRewardedAd();
            };

            Debug.Log("[AdManager] Rewarded ad ready.");
        }

        // ─────────────────────────────────────────────
        // SHOW AD
        // ─────────────────────────────────────────────
        /// <summary>
        /// Hiển thị quảng cáo rewarded.
        /// onRewarded: gọi khi người dùng xem xong và nhận thưởng.
        /// onFailed  : gọi nếu ad chưa tải xong hoặc lỗi.
        /// </summary>
        public void ShowRewardedAd(Action onRewarded, Action onFailed = null)
        {
            if (!IsAdReady)
            {
                Debug.LogWarning("[AdManager] Rewarded ad not ready yet.");
                onFailed?.Invoke();
                return;
            }

            _rewardedAd.Show(reward =>
            {
                Debug.Log($"[AdManager] User earned reward: {reward.Amount} {reward.Type}");
                onRewarded?.Invoke();
            });
        }
    }
}
