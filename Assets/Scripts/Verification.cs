using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Verification : MonoBehaviour
{
    private string apiEndpoint = "https://app.curabit.in/api/endpoint";
    private RequestID requestID = new RequestID();
    private string tempCode;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PostRequest());    
    }

    IEnumerator PostRequest()
    {
        string therapistIDFileDataPath = Application.dataPath + @"/Scripts/T_ID";
        string therapistID = File.ReadAllText(therapistIDFileDataPath);
        Debug.LogWarning(therapistID);
        requestID.getUserSession = new GetUserSession();
        requestID.getUserSession.id = therapistID;

        // get_user_session Request
        string userSessionRequest = JsonConvert.SerializeObject(requestID.getUserSession);
        UnityWebRequest sessionRequest = UnityWebRequest.Put(apiEndpoint, userSessionRequest);
        sessionRequest.SetRequestHeader("Content-Type", "application/json");
        sessionRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return sessionRequest.SendWebRequest();

        Debug.LogWarning("SESSION REQUEST Response Code: " + sessionRequest.responseCode);

        if (sessionRequest.responseCode == 400)
        {
            while (true)
            {
                // set_pairing_code Request
                requestID.setPairingCode = new SetPairingCode();
                requestID.setPairingCode.code = RandomString(6);
                Debug.LogWarning("Pairing Code: " + requestID.setPairingCode.code);
                string pairingCodeRequest = JsonConvert.SerializeObject(requestID.setPairingCode);
                //Debug.LogWarning(pairingCodeRequest);
                UnityWebRequest pairingCodeWebRequest = UnityWebRequest.Put(apiEndpoint, pairingCodeRequest);
                pairingCodeWebRequest.SetRequestHeader("Content-Type", "application/json");
                yield return pairingCodeWebRequest.SendWebRequest();

                if (pairingCodeWebRequest.responseCode == 400)
                {
                    Debug.LogWarning("Code already exists.");
                }
                else if (pairingCodeWebRequest.responseCode == 201) // VERIFY RESPONSE CODE
                {
                    Debug.Log("Unique code generated");
                    tempCode = requestID.setPairingCode.code;
                    break;
                }
                else
                {
                    Debug.LogWarning("Error: " + pairingCodeWebRequest.responseCode);
                    break;
                }
                yield return null;
            }

            // get_id request
            while (true)
            {
                requestID.getID = new GetID();
                requestID.getID.code = tempCode;
                string therapistIDRequest = JsonConvert.SerializeObject(requestID.getID);
                UnityWebRequest therapistIDWebRequest = UnityWebRequest.Put(apiEndpoint, therapistIDRequest);
                therapistIDWebRequest.SetRequestHeader("Content-Type", "application/json");
                therapistIDWebRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return therapistIDWebRequest.SendWebRequest();

                Debug.Log(therapistIDWebRequest.downloadHandler.text);

                // Add therapist ID to file
                if (therapistIDWebRequest.responseCode == 200)
                {
                    Link2 therapistIDResponse = JsonConvert.DeserializeObject<Link2>(therapistIDWebRequest.downloadHandler.text);
                    Debug.LogWarning("Request successful");
                    therapistID = therapistIDResponse.resp;
                    Debug.LogWarning("Therapist ID: " + therapistID);
                    using (FileStream fs = File.Create(therapistIDFileDataPath))
                    {
                        // Add some text to file    
                        byte[] therapistIDToFile = new UTF8Encoding(true).GetBytes(therapistID);
                        fs.Write(therapistIDToFile, 0, therapistIDToFile.Length);
                    }
                    yield return null;
                    break;
                }
                else if (therapistIDWebRequest.responseCode == 400)
                {
                    Debug.LogWarning("get_id 400 BAD REQUEST, Code not paired yet");
                }
                else
                {
                    Debug.LogWarning("THERAPIST ID Error: " + therapistIDWebRequest.responseCode);
                }
                yield return null;
            }
        }
        else if (sessionRequest.responseCode != 200 && sessionRequest.responseCode != 400)
        {
            Debug.LogWarning("get_user_session Error: " + sessionRequest.responseCode + sessionRequest.error);
        }

        Link response = JsonConvert.DeserializeObject<Link>(sessionRequest.downloadHandler.text);
        string userStatus = response.session_details.status;
        Debug.Log(sessionRequest.downloadHandler.text);
        Debug.Log(userStatus);

        yield return null;
    }

    public string RandomString(int length)
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", "");
        return path.Substring(0, length);
    }
}
