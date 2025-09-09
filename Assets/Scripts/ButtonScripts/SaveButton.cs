using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_InputField nameInputField;
    public TextMeshProUGUI currentScoreText;

    private HighscoreList highscoreList = new HighscoreList();
    public void SaveHighscore()
    {
        string json = PlayerPrefs.GetString("HighscoreTable", "");
        if (!string.IsNullOrEmpty(json))
        {
            highscoreList = JsonUtility.FromJson<HighscoreList>(json);
        }
        else
        {
            highscoreList = new HighscoreList();
        }

        SceneTransitionManager stm=FindAnyObjectByType<SceneTransitionManager>();
        int currentScore = int.Parse(currentScoreText.text);
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty!");
            return;
        }

        // Create new entry
        HighscoreEntry entry = new HighscoreEntry { playerName = playerName, score = currentScore };
        highscoreList.entries.Add(entry);

        // Sort descending (highest first)
        highscoreList.entries.Sort((a, b) => b.score.CompareTo(a.score));

        // (Optional) keep only top 10
        if (highscoreList.entries.Count > 10)
            highscoreList.entries.RemoveRange(10, highscoreList.entries.Count - 10);

        // Save as JSON
        string jsonEnd = JsonUtility.ToJson(highscoreList);
        PlayerPrefs.SetString("HighscoreTable", jsonEnd);
        PlayerPrefs.Save();

        stm.LoadMenu();

        Debug.Log("Saved highscore: " + playerName + " - " + currentScore);
    }
}
