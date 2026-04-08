using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools;
namespace Core
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private List<LevelData> _allLevels;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _moveText;
        [SerializeField] private TextMeshProUGUI _targetScoreText;
        [SerializeField] private UIManager _uiManager;

        private int _maxAllowedMove;
        private int _score;
        private int _targetScore;
        private Vector2Int _dimensions;
        private MatchableGrid _grid;
        private bool _isGameOver = false;
        private bool _hasUsedContinue = false; // Giới hạn xem quảng cáo 1 lần/màn

        public int TotalLevels => _allLevels != null ? _allLevels.Count : 0;
        public int CurrentScore => _score;
        public bool HasUsedContinue => _hasUsedContinue;

        protected override void Awake()
        {
            base.Awake();
            
            LevelData currentLevel = null;
            if (_allLevels != null && _allLevels.Count > 0)
            {
                int levelIdx = Mathf.Clamp(LevelManager.SelectedLevelIndex, 0, _allLevels.Count - 1);
                currentLevel = _allLevels[levelIdx];
            }

            if (currentLevel != null)
            {
                _dimensions = currentLevel.dimensions;
                _maxAllowedMove = currentLevel.maxMoves;
                _targetScore = currentLevel.targetScore;
            }
            else
            {
                _dimensions = new Vector2Int(8, 8);
                _maxAllowedMove = 30;
                _targetScore = 5000;
            }

            _grid = (MatchableGrid) MatchableGrid.Instance;
            
            _score = 0;
            _scoreText.text = _score.ToString("D5");
            _moveText.text = _maxAllowedMove.ToString();
            
            if (_targetScoreText != null)
                _targetScoreText.text = "Target: " + _targetScore.ToString();
        }

        private void Start()
        {
            if (_grid != null)
            {
                // Force grid to center to avoid "floating candies" issue
                _grid.transform.position = Vector3.zero;
                Debug.Log($"[GameManager] Loading Level {LevelManager.SelectedLevelIndex + 1} with size {_dimensions}");
                
                _grid.InitializeGrid(_dimensions);
                _grid.PopulateGrid();
            }
            else
            {
                Debug.LogError("[GameManager] MatchableGrid instance not found in Start!");
            }
        }

        public void IncreaseScore(int value)
        {
            if (_isGameOver) return;

            _score += value;
            if (_scoreText != null)
                _scoreText.text = _score.ToString("D5");
            else
                Debug.LogWarning("Score Text is NOT assigned in GameManager!");
            
            if (_score >= _targetScore)
            {
                WinGame();
            }
        }

        private void WinGame()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            Debug.Log("LEVEL COMPLETE!");
            
            // Save progress (Only if LevelManager exists)
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.UnlockNextLevel(LevelManager.SelectedLevelIndex);
            }
            else
            {
                Debug.LogWarning("LevelManager NOT found in scene. Progress will not be saved.");
            }
            
            if (_uiManager != null)
                _uiManager.ShowWinPanel(_score);
            else
                Debug.LogError("UIManager is NOT assigned in GameManager! Cannot show Win Panel.");
        }

        public bool CanMoveMatchables()
        {
            if (_maxAllowedMove <= 0 || _isGameOver)
                return false;
            return true;
        }

        public void DecreaseMove()
        {
            if (_isGameOver) return;

            _maxAllowedMove--;
            if (_moveText != null)
                _moveText.text = _maxAllowedMove.ToString();
            else
                Debug.LogWarning("Move Text is NOT assigned in GameManager!");
            
            if (_maxAllowedMove <= 0)
            {
                if (_score >= _targetScore)
                    WinGame();
                else
                    LoseGame();
            }
        }

        private void LoseGame()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            Debug.Log("GAME OVER!");
            
            if (_uiManager != null)
                _uiManager.ShowLosePanel(_score, !_hasUsedContinue);
            else
                Debug.LogError("UIManager is NOT assigned in GameManager! Cannot show Lose Panel.");
        }

        /// <summary>
        /// Tiếp tục chơi sau khi xem quảng cáo — cộng thêm nước đi, GIỮ nguyên điểm.
        /// </summary>
        public void ContinueWithExtraMoves(int extraMoves = 5)
        {
            _hasUsedContinue = true;
            _isGameOver = false;
            _maxAllowedMove += extraMoves;

            if (_moveText != null)
                _moveText.text = _maxAllowedMove.ToString();

            Debug.Log($"[GameManager] +{extraMoves} lượt được thưởng. Còn lại: {_maxAllowedMove}");

            if (_uiManager != null)
                _uiManager.HideLosePanel();
        }
    }
}


