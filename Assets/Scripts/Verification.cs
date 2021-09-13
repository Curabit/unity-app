using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;

public class Verification : MonoBehaviour
{
    private string apiEndpoint = "https://app.curabit.in/api/endpoint";
    private RequestID requestID = new RequestID();
    private string tempCode;
    private TMP_Text m_TextComponent;
    private Transform TMPTransform;
    private GameObject pairingText;
    public string userStatus;
    public static string session_id;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PostRequest());    
    }

    void Awake()
    {
        pairingText = GameObject.Find("Pairing Text");
        pairingText.SetActive(false);
        TMPTransform = pairingText.transform.Find("Text (TMP)");
        m_TextComponent = TMPTransform.GetComponent<TMP_Text>();
        // Loads the second Scene
        SceneManager.LoadScene("playback", LoadSceneMode.Additive);
    }

    IEnumerator PostRequest()
    {
        string therapistIDFileDataPath = Application.persistentDataPath + @"/T_ID";
        Debug.Log("Data path: " + therapistIDFileDataPath);
        string therapistID;
        if (File.Exists(therapistIDFileDataPath))
        {
            therapistID = File.ReadAllText(therapistIDFileDataPath);
            Debug.LogWarning(therapistID + "Therapist ID Generated");
            requestID.getUserSession = new GetUserSession();
            requestID.getUserSession.id = therapistID;
        }
        else
        {
            Debug.LogWarning("File does not exist yet");

            // ADD TO ELSE
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

                pairingText.SetActive(true);
                m_TextComponent.text = "Log in to app.curabit.in and enter headset linking code: " + requestID.setPairingCode.code;

                if (pairingCodeWebRequest.responseCode == 400)
                {
                    Debug.LogWarning("Code already exists.");
                }
                else if (pairingCodeWebRequest.responseCode == 201)
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

                    requestID.getUserSession = new GetUserSession();
                    requestID.getUserSession.id = therapistID;

                    pairingText.SetActive(false);

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


        while (true)
        {
            // get_user_session Request
            string userSessionRequest = JsonConvert.SerializeObject(requestID.getUserSession);
            UnityWebRequest sessionRequest = UnityWebRequest.Put(apiEndpoint, userSessionRequest);
            sessionRequest.SetRequestHeader("Content-Type", "application/json");
            sessionRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return sessionRequest.SendWebRequest();

            Debug.LogWarning("SESSION REQUEST Response Code: " + sessionRequest.responseCode);

            if (sessionRequest.responseCode != 200)
            {
                Debug.LogWarning("get_user_session Code: " + sessionRequest.responseCode + "Error: " +sessionRequest.error);
            }

            Link sessionResponse = JsonConvert.DeserializeObject<Link>(sessionRequest.downloadHandler.text);
            Debug.Log(sessionRequest.downloadHandler.text);
            userStatus = sessionResponse.session_details.status;
       
            Debug.Log(userStatus);

            if (userStatus != "on-standby")
            {
                session_id = userStatus;
                break;
            }

            Debug.LogWarning("SESSION ID RECEIVED = " + session_id);
        }

        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("playback"));

        //SceneManager.LoadSceneAsync("playback", LoadSceneMode.Additive);
        //Debug.LogWarning("Async request sent");
        //yield return new WaitForSecondsRealtime(10);
    }

    public string RandomString(int length)
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", "");
        return path.Substring(0, length);
    }
}
