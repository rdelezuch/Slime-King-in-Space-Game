using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    public TMP_InputField enteredName;
    public TextMeshProUGUI addScore;
    public Button toggleButton;
    public GameObject easyModeText;
    public GameObject hardModeText;

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;
    private GameManager gameManager;
    private string highscoreTableKey;
    private bool hardMode = false;
    private bool refreshTable = false;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleBooleanValue);
    }

    private void Update()
    {
        HighscoreChange();
    }

    void Awake()
    {
        SetModeAfterGame();
        highscoreTableKey = hardMode ? "highscoreTableHard" : "highscoreTable";
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        int score = Convert.ToInt32(addScore.text);
        string enteredNameString = enteredName.text;

        entryTemplate.gameObject.SetActive(false);

        if (score != 0 && enteredNameString.Length != 0)
        {
            AddHighscoreEntry(score, enteredNameString);
        }

        string jsonString = PlayerPrefs.GetString(highscoreTableKey);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        // Remove unnecessary results
        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 100f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;
            case 1:
                rankString = rank + "ST"; break;
            case 2:
                rankString = rank + "ND"; break;
            case 3:
                rankString = rank + "RD"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set background easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        if (rank == 1)
        {
            // Highlight first
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.red;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.red;
            entryTransform.Find("posText").GetComponent<Text>().color = Color.red;
        }

        // Set tropy
        switch (rank)
        {
            default:
                entryTransform.Find("trophy").gameObject.SetActive(false);
                break;
            case 1:
                entryTransform.Find("trophy").GetComponent<Image>().color = new Color(1, 0.843f, 0);
                break;
            case 2:
                entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0.753f, 0.753f, 0.753f);
                break;
            case 3:
                entryTransform.Find("trophy").GetComponent<Image>().color = new Color(0.627f, 0.322f, 0.176f);
                break;
        }


        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name)
    {
        string json;

        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString(highscoreTableKey);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Create new highscore if not exist
        if (highscores == null)
        {
            highscoreEntryList = new List<HighscoreEntry>();
            json = JsonUtility.ToJson(highscoreEntryList);
            PlayerPrefs.SetString(highscoreTableKey, json);
            PlayerPrefs.Save();
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(highscoreTableKey, json);
        PlayerPrefs.Save();
    }

    private void ToggleBooleanValue()
    {
        hardMode = !hardMode;
        refreshTable = !refreshTable;
    }

    private void HighscoreChange()
    {
        if (hardMode)
        {
            hardModeText.SetActive(true);
            easyModeText.SetActive(false);
        }
        else
        {
            hardModeText.SetActive(false);
            easyModeText.SetActive(true);
        }

        RefreshTable(refreshTable);

        refreshTable = false;
    }

    private void RefreshTable(bool refresh)
    {
        if (refreshTable)
        {
            GameObject[] clones = GameObject.FindGameObjectsWithTag("highscoreEntryTemplate");

            foreach (GameObject clone in clones)
            {
                Destroy(clone);
            }

            ResetDataScore();
            Awake();
        }
    }

    private void ResetDataScore()
    {
        addScore.text = null;
        enteredName.text = null;
    }

    private void SetModeAfterGame()
    {
        if (addScore.text != "0" && enteredName.text.Length != 0)
        {
            gameManager = FindObjectOfType<GameManager>();
            hardMode = gameManager.hardDifficultLevel;
        }
        
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    // Represents a single High score entry
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }
}
