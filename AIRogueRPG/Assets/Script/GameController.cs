using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController instance = null;
    public static GameController Instance {
        get {
            if (instance == null) return null;
            return instance;
        }
    }

    private bool gameStarted;
    private float playTime = 0f;
    private LevelData levelData;

    [SerializeField]
    private GameObject gameEndPopUpUI;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        levelData = new LevelData();
    }

    private void Update() {
        if (gameStarted) {
            playTime += Time.deltaTime;
        }
    }

    public void GameEnd(bool isSuccess) {
        PlayerDataHandler.Instance.GetNextLevelData();
        PlayerStatManager.Instance.SetIsWorking(false);
        if (gameStarted) {
            ShowGameEndPopUp(isSuccess);
        }
        gameStarted = false;
        
    }
    public bool GetGameStarted() {
        return gameStarted;
    }
    public void SetGameStarted(bool started) {
        if (started) playTime = 0f;
        gameStarted = started;
    }

    private void ShowGameEndPopUp(bool isSuccess) {
        GameObject popUpUI = Instantiate(gameEndPopUpUI, FindAnyObjectByType<Canvas>().transform);
        TextMeshProUGUI resultText = popUpUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        resultText.text = isSuccess ? "Game Clear" : "Game Over";
        Transform timer = popUpUI.transform.GetChild(3);
        StartCoroutine(GameEndUITimer(timer.GetChild(0).GetComponent<Image>(), 3f));
    }

    IEnumerator GameEndUITimer(Image timerImg, float time) {
        bool timerWorking = true;
        float timerValue = time;
        while (timerWorking) {
            timerValue -= Time.deltaTime;
            timerImg.fillAmount = timerValue / time;
            yield return new WaitForSeconds(0.001f);

            if(timerValue <= 0f) {
                timerWorking = false;
            }
        }

        SceneManager.LoadScene("StartScene");
        
    }

    public float GetPlayTime() {
        return playTime;
    }

    public void SetLevelData(LevelData levelData) {
        if (levelData == null) return;
        this.levelData = levelData;
    }

    public LevelData GetLevelData() {
        return levelData;
    }
}
