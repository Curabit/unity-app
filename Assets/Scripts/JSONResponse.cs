//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class JSONResponse : MonoBehaviour
//{
//    private string url = "https://app.curabit.in/api/test/get-json";

//    public void Start()
//    {
//        Debug.Log("Starting script JSONResponse.");
//        StartCoroutine(MakeJSONRequest());
//    }
//    IEnumerator MakeJSONRequest()
//    {
//        UnityWebRequest request = UnityWebRequest.Get(url);
//        Debug.Log("Sending request");
//        yield return request.SendWebRequest();

//        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
//        {
//            Debug.Log(request.error);
//            yield break;
//        }
//        else
//        {
//            JSONClasses response = JsonConvert.DeserializeObject<JSONClasses>(request.downloadHandler.text);
//            yield return null;
//            Debug.Log(response.next[1].FileName.GetType());
//        }
//    }
//}
