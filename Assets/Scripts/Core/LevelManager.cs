using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Tools;

namespace Core
{
    public class LevelManager : SingletonMonoBehaviour<LevelManager>
    {
        [SerializeField] private List<LevelData> _levels;
        private const string LEVEL_KEY = "UnlockedLevel";

        public List<LevelData> Levels => _levels;

        public int GetUnlockedLevel()
        {
            return PlayerPrefs.GetInt(LEVEL_KEY, 1);
        }

        public void UnlockNextLevel(int currentLevelIndex)
        {
            int nextLevel = currentLevelIndex + 2; // Index starts at 0, level starts at 1
            int highestUnlocked = GetUnlockedLevel();
            
            if (nextLevel > highestUnlocked && nextLevel <= _levels.Count)
            {
                PlayerPrefs.SetInt(LEVEL_KEY, nextLevel);
                PlayerPrefs.Save();
            }
        }

        public void LoadLevel(int index)
        {
            // We can store the selected level index in a static variable to read it in the GameScene
            SelectedLevelIndex = index;
            SceneManager.LoadScene("SampleScene");
        }

        public static int SelectedLevelIndex = 0;
    }
}
