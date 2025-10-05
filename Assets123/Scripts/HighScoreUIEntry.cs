using TMPro;
using UnityEngine;

public class HighScoreUIEntry : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void SetEntry(string playerName, int score)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
