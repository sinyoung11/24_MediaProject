using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    public void OnGameOver(float playtime)
    {
        StartCoroutine(SendPlaytime(playtime));
    }

    IEnumerator SendPlaytime(float playtime)
    {
        string url = "http://localhost:5000/process_playtime"; // Flask 서버 주소
        PlaytimeData data = new PlaytimeData { playtime = playtime };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string newLevelJson = request.downloadHandler.text;
            ApplyNewLevel(newLevelJson);
        }
        else
        {
            Debug.LogError("Error sending playtime: " + request.error);
        }
    }

    void ApplyNewLevel(string newLevelJson)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(newLevelJson);
        Debug.Log("New Difficulty: " + levelData.difficulty);
        Debug.Log("Enemy Count: " + levelData.enemy_count);
    }

    [System.Serializable]
    public class PlaytimeData
    {
        public float playtime;
    }

    [System.Serializable]
    public class LevelData
    {
        public string difficulty;
        public int enemy_count;
    }
}