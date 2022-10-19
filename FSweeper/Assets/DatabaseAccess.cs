using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using MongoDB.Bson.Serialization;
using System.Text;
using Unity.VisualScripting;

public class DatabaseAccess
{
    private static string url;

    private class DatabaseAccessMonoBehaviour : MonoBehaviour { }

    private static DatabaseAccessMonoBehaviour databaseAccessMonoBehaviour;

    private static void Init()
    {
        if (databaseAccessMonoBehaviour == null)
        {
            GameObject gameObject = new GameObject("DatabaseAccess");
            databaseAccessMonoBehaviour = gameObject.AddComponent<DatabaseAccessMonoBehaviour>();
        }
    }

    public static void SetUrl(string _url)
    {
        url = _url;
    }
    public static void GetPlayer(string name, Action<string> onError, Action<Player> onSuccess)
    {
        Init();
        databaseAccessMonoBehaviour.StartCoroutine(GetCoroutine(name, onError, onSuccess));
    }

    public static void GetAllPlayers(Action<string> onError, Action<List<Player>> onSuccess)
    {
        Init();
        databaseAccessMonoBehaviour.StartCoroutine(GetCoroutine(onError, onSuccess));
    }

    public static void UpdatePlayer(Player player, Action<string> onError, Action<string> onSuccess)
    {
        Init();
        databaseAccessMonoBehaviour.StartCoroutine(PatchCoroutine(player, onError, onSuccess));
    }

    public static void AddPlayer(Player player, Action<string> onError, Action<string> onSuccess)
    {
        Init();
        databaseAccessMonoBehaviour.StartCoroutine(PostCoroutine(player, onError, onSuccess));  
    }

    private static IEnumerator GetCoroutine(Action<string> onError, Action<List<Player>> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    onError("Connection Error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError("DataProcessing Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    onError("Protocol Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onSuccess(JsonConvert.DeserializeObject<PlayerList>(request.downloadHandler.text).list);
                    break;
            }
        }
    }

    private static IEnumerator GetCoroutine(string name, Action<string> onError, Action<Player> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url + $"/{name}"))
        {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    onError("Connection Error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError("DataProcessing Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    onError("Protocol Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onSuccess(JsonConvert.DeserializeObject<Player>(request.downloadHandler.text));
                    break;
            }
        }
    }

    private static IEnumerator PatchCoroutine(Player player, Action<string> onError, Action<string> onSuccess)
    {
        string bodyData = JsonConvert.SerializeObject(player);
        Debug.Log("BodyData : " + bodyData);
        using (UnityWebRequest request = UnityWebRequest.Put(url, bodyData))
        {
            request.method = "PATCH";
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    onError("Connection Error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError("DataProcessing Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    onError("Protocol Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onSuccess(request.downloadHandler.text);
                    break;
            }
        }
    }   

    private static IEnumerator PostCoroutine(Player player, Action<string> onError, Action<string> onSuccess)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            string bodyData = JsonConvert.SerializeObject(player);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData));
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    onError("Connection Error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError("DataProcessing Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    onError("Protocol Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    onSuccess(request.downloadHandler.text);
                    break;
            }
        }
        
    }
}
