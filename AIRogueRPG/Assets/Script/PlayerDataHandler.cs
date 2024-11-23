using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerData {
    public float playtime;
}

[System.Serializable]
public class LevelData {
    public string difficulty;
    public int enemy_count;
}

public class PlayerDataHandler : MonoBehaviour
{
    private static PlayerDataHandler instance = null;
    public static PlayerDataHandler Instance {
        get {
            if (instance == null) return null;
            return instance;
        }
    }

    private string sendUrl = "http://localhost:5000/process_playtime";

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void GetNextLevelData() {
        float playTime = GameController.Instance.GetPlayTime();
        StartCoroutine(SendData(playTime));
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
                GameController.Instance.SetLevelData(
                    JsonUtility.FromJson<LevelData>(newLevelJson));
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
