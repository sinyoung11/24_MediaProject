using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData {
    public float playtime;
}

[System.Serializable]
public class LevelData {
    public string difficulty;
    public int enemy_count;
}

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
    GameEndPopUpUI gameEndPopUPUIScript;
    private float lostHealth = 0;
    private int shootWater = 0;
    private int hitWater = 0;
    private bool isWaitingGrade = false;
    private LevelData levelData;
    private string sendUrl = "http://localhost:5000/process_playtime";

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
        PlayerStatManager.Instance.SetIsWorking(false);
        if (gameStarted) {
            ShowGameEndPopUp(isSuccess);
        }
        gameStarted = false;
        isWaitingGrade = true;
        StartCoroutine(SendData(playTime));

    }
    public bool GetGameStarted() {
        return gameStarted;
    }
    public void SetGameStarted(bool started) {
        if (started) {
            playTime = 0f;
            lostHealth = 0f;
            shootWater = 0;
            hitWater = 0;
        }
        gameStarted = started;
    }

    private void ShowGameEndPopUp(bool isSuccess) {
        GameObject popUpUI = Instantiate(gameEndPopUpUI, FindAnyObjectByType<Canvas>().transform);
        gameEndPopUPUIScript = popUpUI.GetComponent<GameEndPopUpUI>();
        TextMeshProUGUI resultText = gameEndPopUPUIScript.resultText;
        resultText.text = isSuccess ? "Game Clear" : "Game Over";
        gameEndPopUPUIScript.StartDesc(playTime, lostHealth, shootWater, hitWater);
        StartCoroutine(GameEndUITimer(gameEndPopUPUIScript.timerImage, 3f));
    }

    IEnumerator GameEndUITimer(Image timerImg, float time) {
        float stepTimer = 0.0f;
        while(stepTimer <= 10.0f){
            // if(!isWaitingGrade) break;
            stepTimer += Time.deltaTime;
            yield return null;
        }
        
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

    public void AddLostHealth(float damage){
        lostHealth += damage;
    }

    public void AddShooWater(){
        shootWater++;
    }

    public void AddHitWater(){
        hitWater++;
    }

    public float GetLostHealth(){
        return lostHealth;
    }

    public int GetShootWater(){
        return shootWater;
    }

    public int GethitWater(){
        return hitWater;
    }

    public float GetPlayTime() {
        return playTime;
    }


    public void SetLevelData(LevelData levelData) {
        if (levelData == null) return;
        this.levelData = levelData;
        isWaitingGrade = false;
        gameEndPopUPUIScript.SetGrade(levelData.difficulty);
    }

    public LevelData GetLevelData() {
        return levelData;
    }


    IEnumerator SendData(float time) {

        PlayerData data = new PlayerData { playtime = time };
        Debug.Log($"Send play time data: {data.playtime}");
        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(sendUrl, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        switch (request.result) {
            case UnityWebRequest.Result.Success:
                string newLevelJson = request.downloadHandler.text;
                Debug.Log($"Success to get new level data: {newLevelJson}");
                SetLevelData(JsonUtility.FromJson<LevelData>(newLevelJson));
                break;
            case UnityWebRequest.Result.ConnectionError:
                Debug.Log("ConnectionError: " + request.error);
                break;
            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log("DataProcessingError: " + request.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.Log("ProtocolError: " + request.error);
                break;
        }

    }
}
