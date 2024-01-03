using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    int score = 0;

    public TextMeshProUGUI scoreText;
    public GameObject obstacle;
    public Transform spawnPointB;
    public Transform spawnPointT;
    public GameObject playButton;
    public GameObject player;
    public GameObject highscoreButton;
    public GameObject highscoreTable;
    public GameObject addScoreWindow;
    public GameObject difficultMenu;
    public bool hardDifficultLevel;

    private readonly float waitTime = 0.2f;

    private float timer = 0.0f;
    private float playerPosition;
    private GameObject[] obstacleArray = null;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (GameObject.FindWithTag("Obstacle") != null)
        {
            obstacleArray = GameObject.FindGameObjectsWithTag("Obstacle");
            playerPosition = player.transform.position.z;

            if (Array.Exists(obstacleArray, obstacleElement => Math.Round(obstacleElement.transform.position.z) == Math.Round(playerPosition)))
            {
                if (timer > waitTime)
                {
                    ScoreUp();
                    TimedDifficultyIncreaser();
                    timer = 0.0f;

                }
            }
        }
    }

    IEnumerator SpawnObstaclesBot()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(1f, 2f);

            yield return new WaitForSeconds(waitTime);

            Instantiate(obstacle, spawnPointB.position, Quaternion.identity);
        }
    }

    IEnumerator SpawnObstaclesTop()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(1.5f, 3f);

            yield return new WaitForSeconds(waitTime);

            Instantiate(obstacle, spawnPointT.position, Quaternion.identity);
        }
    }

    public void DifficultyLevelMenu()
    {
        playButton.SetActive(false);
        highscoreButton.SetActive(false);
        difficultMenu.SetActive(true);
    }

    public void EasyDifficultLevel()
    {
        hardDifficultLevel = false;
        difficultMenu.SetActive(false);
        GameStart();
    }

    public void HardDifficultLevel()
    {
        hardDifficultLevel = true;
        difficultMenu.SetActive(false);
        GameStart();
    }

    public void GameStart()
    {
        playButton.SetActive(false);
        highscoreButton.SetActive(false);

        StartCoroutine(SpawnObstaclesBot());

        if (hardDifficultLevel)
        {
            StartCoroutine(SpawnObstaclesTop());
        }

        Time.timeScale = 1;
    }

    public void ViewHighscoreTable()
    {
        highscoreTable.SetActive(true);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Game");
    }

    public void EnterNewScore()
    {
        addScoreWindow.SetActive(false);
        highscoreTable.SetActive(true);
    }

    private void ScoreUp()
    {
        score++;
        scoreText.text = score.ToString();
    }

    private void TimedDifficultyIncreaser()
    {
        if (score % 10 == 0)
        {
            Time.timeScale += 0.2f;
        }
    }
}
