using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests
{
    public static IEnumerator Get(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri)) {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    UnityEngine.Debug.LogError("Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    UnityEngine.Debug.Log(request.downloadHandler.text);
                    break;
            }
        }
    }

    /*public static IEnumerator Upload(string uri, string profile)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(uri) {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(profile);
        }
    }*/


}
