using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private int _levelIndex; // 0 cho Level 1, 1 cho Level 2...
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private GameObject _lockIcon; // Hình ổ khóa (nếu có)

        private void Awake()
        {
            if (_button == null) _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(OnLevelClick);
            }
        }

        private void Start()
        {
            UpdateStatus();
        }

        public void OnLevelClick()
        {
            Debug.Log("DA BAM NUT LEVEL: " + (_levelIndex + 1));
            LevelManager.SelectedLevelIndex = _levelIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        public void UpdateStatus()
        {
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            bool isUnlocked = (_levelIndex + 1) <= unlockedLevel;
            
            if (_button == null) _button = GetComponent<Button>();
            _button.interactable = isUnlocked;
            if (_lockIcon != null) _lockIcon.SetActive(!isUnlocked);
        }
    }
}
