using System;
using System.IO;
using UnityEngine;
using SpiritHatchers.Core;

namespace SpiritHatchers.Save
{
    public class SaveManager : MonoBehaviour
    {
        public PlayerSaveData CurrentSaveData { get; private set; }

        public string SaveFilePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, GameConstants.SaveFileName);
            }
        }

        public PlayerSaveData LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.Log($"No save file found at {SaveFilePath}. Creating a new save.");
                CurrentSaveData = PlayerSaveData.CreateNewSave();
                SaveGame(CurrentSaveData);
                return CurrentSaveData;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.LogWarning("Save file was empty. Creating a new save.");
                    CurrentSaveData = PlayerSaveData.CreateNewSave();
                    SaveGame(CurrentSaveData);
                    return CurrentSaveData;
                }

                CurrentSaveData = JsonUtility.FromJson<PlayerSaveData>(json);

                if (CurrentSaveData == null)
                {
                    Debug.LogWarning("Save file could not be read. Creating a new save.");
                    CurrentSaveData = PlayerSaveData.CreateNewSave();
                    SaveGame(CurrentSaveData);
                    return CurrentSaveData;
                }

                CurrentSaveData.EnsureListsAreValid();
                CurrentSaveData.lastLoginTime = DateTime.UtcNow.ToString("o");

                Debug.Log($"Loaded save data from {SaveFilePath}.");
                return CurrentSaveData;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load save data. Creating a new save. Error: {exception.Message}");
                CurrentSaveData = PlayerSaveData.CreateNewSave();
                SaveGame(CurrentSaveData);
                return CurrentSaveData;
            }
        }

        public void SaveGame()
        {
            SaveGame(CurrentSaveData);
        }

        public void SaveGame(PlayerSaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("SaveGame was called with null save data.");
                return;
            }

            try
            {
                saveData.EnsureListsAreValid();

                string directoryPath = Path.GetDirectoryName(SaveFilePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SaveFilePath, json);

                CurrentSaveData = saveData;
                Debug.Log($"Saved game data to {SaveFilePath}.");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to save game data. Error: {exception.Message}");
            }
        }

        public void DeleteSaveForTesting()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log($"Deleted save file at {SaveFilePath}.");
            }
            else
            {
                Debug.Log($"No save file exists to delete at {SaveFilePath}.");
            }

            CurrentSaveData = null;
        }
    }
}
