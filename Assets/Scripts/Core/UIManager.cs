using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Core
{
    public class UIManager : MonoBehaviour
    {
        [Header("Win Panel")]
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private TextMeshProUGUI _winScoreText;
        [SerializeField] private Button _nextLevelButton;

        [Header("Lose Panel")]
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private TextMeshProUGUI _loseScoreText;
        [SerializeField] private Button _watchAdButton;       // Nút "+Lượt chơi (Xem QC)"
        [SerializeField] private TextMeshProUGUI _watchAdButtonText; // Text hiển thị trên nút

        public void ShowWinPanel(int score)
        {
            if (_winPanel == null) return;
            
            _winPanel.SetActive(true);
            if (_winScoreText != null) _winScoreText.text = "SCORE: " + score.ToString();

            // Chỉ hiện nút "Next" nếu còn màn tiếp theo
            if (_nextLevelButton != null)
            {
                bool hasNext = (LevelManager.SelectedLevelIndex + 1) < GameManager.Instance.TotalLevels;
                _nextLevelButton.gameObject.SetActive(hasNext);
            }
        }

        /// <param name="showAdButton">Hiện nút xem QC hay không (false nếu đã xem rồi)</param>
        public void ShowLosePanel(int score, bool showAdButton = true)
        {
            if (_losePanel == null) return;
            
            _losePanel.SetActive(true);
            if (_loseScoreText != null) _loseScoreText.text = "SCORE: " + score.ToString();

            // Hiện/ẩn nút xem quảng cáo
            if (_watchAdButton != null)
            {
                bool adReady = AdManager.Instance != null && AdManager.Instance.IsAdReady;
                _watchAdButton.gameObject.SetActive(showAdButton && adReady);

                if (_watchAdButtonText != null)
                    _watchAdButtonText.text = "▶ +5 Lượt chơi (Xem QC)";
            }
        }

        /// <summary>Chỉ HideLosePanel, được gọi từ GameManager sau khi tiếp tục.</summary>
        public void HideLosePanel()
        {
            if (_losePanel != null)
                _losePanel.SetActive(false);
        }

        // Gắn vào nút Restart
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Gắn vào nút Menu/Home
        public void GoToMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }

        // Gắn vào nút Next Level
        public void LoadNextLevel()
        {
            LevelManager.SelectedLevelIndex++;
            // Reload the same scene but with updated index
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ─────────────────────────────────────────────────────────────
        // NÚt XEM QUẢNG CÁO — gắn vào OnClick() của _watchAdButton
        // ─────────────────────────────────────────────────────────────
        public void OnWatchAdButtonClicked()
        {
            // Vô hiệu hoá nút tránh double-click
            if (_watchAdButton != null)
                _watchAdButton.interactable = false;

            if (AdManager.Instance == null)
            {
                Debug.LogError("[UIManager] AdManager.Instance is null!");
                ReEnableAdButton();
                return;
            }

            AdManager.Instance.ShowRewardedAd(
                onRewarded: () =>
                {
                    // Đã xem xong QC — cộng lượt và tiếp tục chơi
                    if (GameManager.Instance != null)
                        GameManager.Instance.ContinueWithExtraMoves(5);
                },
                onFailed: () =>
                {
                    Debug.LogWarning("[UIManager] Ad not ready — re-enabling button.");
                    ReEnableAdButton();
                }
            );
        }

        private void ReEnableAdButton()
        {
            if (_watchAdButton != null)
                _watchAdButton.interactable = true;
        }
    }
}
