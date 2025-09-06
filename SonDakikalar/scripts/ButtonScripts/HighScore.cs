using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    [Header("UI References")]
    public GameObject entryTemplate;       // Prefab for a single row (inactive in hierarchy)
    public Transform entryContainer;
    public GameObject scrollView;
    // ScrollView content

    private HighscoreList highscoreList;


    // Call this from your button OnClick
    public void ShowHighscoresButton()
    {
        scrollView.SetActive(true);
        LoadHighscores();
        DisplayHighscores();
    }

    private void LoadHighscores()
    {
        string json = PlayerPrefs.GetString("HighscoreTable", "");
        Debug.Log(json);
        if (!string.IsNullOrEmpty(json))
        {
            highscoreList = JsonUtility.FromJson<HighscoreList>(json);
        }
        else
        {
            highscoreList = new HighscoreList(); // empty list if nothing saved
        }
    }

    private void DisplayHighscores()
    {
        // Clear previous entries except

        entryTemplate.SetActive(true);
        Debug.Log("A");
        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate.transform)
                Destroy(child.gameObject);
        }

        // Populate ScrollView
        foreach (HighscoreEntry entry in highscoreList.entries)
        {
            GameObject obj = Instantiate(entryTemplate, entryContainer);
            obj.SetActive(true); // Activate only the clone
            HighScoreUIEntry uiEntry = obj.GetComponent<HighScoreUIEntry>();
            if (uiEntry != null)
                uiEntry.SetEntry(entry.playerName, entry.score);
            obj.GetComponent<TextMeshProUGUI>().text=entry.playerName+" "+entry.score;
        }
    }
}