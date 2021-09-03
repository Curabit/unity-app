// Will have to download video clips from server and add them to videoClipList

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PlaybackScript : MonoBehaviour
{
    //Raw Image to Show Video Images
    public RawImage image;
    //public List<VideoClip> videoClipList;
    //public List<VideoPlayer> videoPlayerList;
    //private int videoIndex = 0;
    public RenderTexture rt;
    public int updateCount = 0;
    public JSONClasses response;
    public Dictionary<string, VideoPlayer> videoPlayerDict = new Dictionary<string, VideoPlayer>();
    public int totalVideoCount;

    void Start()
    {
        StartCoroutine(PlayVideo());
    }

    void Update()
    {
        updateCount++;

        // Checking state every 600 frames
        if (updateCount % 100 == 0)
        {
            StartCoroutine(CheckState());
        }
    }

    IEnumerator CheckState()
    {
        yield return StartCoroutine(JSONRequest());
        //Debug.Log("JSONRequest() within CheckState()");
        if (response.isStop == true)
        {
            List<VideoPlayer> dictValueList = new List<VideoPlayer>(videoPlayerDict.Values);
            foreach (VideoPlayer vp in dictValueList)
            {
                vp.Stop();
                yield return null;
            //    Debug.LogWarning("Stop signal RECEIVED");
            }
            //EditorApplication.isPlaying = false;
        }
    }

    IEnumerator JSONRequest()
    {
        string url = "https://app.curabit.in/api/json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        Debug.Log("Request sent");

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            yield break;
        }
        else
        {
            response = JsonConvert.DeserializeObject<JSONClasses>(request.downloadHandler.text);
            //Debug.Log(response.current.FileName);
            yield return response;
        }
    }

    IEnumerator PlayVideo(bool firstRun = true)
    {
        //Init videoPlayerList first time this function is called
        if (firstRun)
        {
            yield return StartCoroutine(JSONRequest());
            totalVideoCount = response.videos.Count;
            
            //Debug.Log(totalVideoCount);
            if (totalVideoCount == 0)
            {
                Debug.LogError("Video count 0 received from JSON");
                yield break;
            }

            string path;
            for (int i = 0; i < totalVideoCount; i++)
            {
                //Setting the path to load video based on JSON response
                path = @"Videos/" + response.videos[i];

                VideoClip vc = Resources.Load<VideoClip>(path);

                if (vc == null)
                {
                    Debug.Log("Video number " + (i+1) + " not found");
                    break;
                }

                //videoClipList.Add(vc);

                //For each video, add a videoPlayer component to GameObject and set targetTexture
                VideoPlayer videoPlayer = gameObject.AddComponent<VideoPlayer>();
                rt = Resources.Load<RenderTexture>("Textures/VideoRenderTexture");
                videoPlayer.targetTexture = rt;
                //videoPlayerList.Add(videoPlayer);

                videoPlayerDict.Add(response.videos[i], videoPlayer);
                Debug.Log(response.videos[i]);

                //Disable Play on Awake
                videoPlayer.playOnAwake = false;

                //We want to play from video clip not from url
                videoPlayer.source = VideoSource.VideoClip;
                //videoPlayerList[i].clip = videoClipList[i];
                videoPlayer.clip = vc;
            }
        }

        //Make sure that the NEXT VideoPlayer index is valid
        //if (videoIndex >= totalVideoCount)
        //    yield break;

        string currentFileName = response.current.FileName;
        List<Next> nextQueue = response.next; // List for Next in JSON
        //Debug.LogWarning(nextQueue[0].FileName);

        //Prepare video
        videoPlayerDict[currentFileName].Prepare();
        //videoPlayerList[videoIndex].Prepare();

        //Wait until this video is prepared
        while (!videoPlayerDict[currentFileName].isPrepared)
        {
            Debug.Log("Preparing video: " + currentFileName);
            yield return null;
        }

        Debug.LogWarning("Done Preparing current Video: " + currentFileName);

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayerDict[currentFileName].texture;

        //Play video in JSON current
        //videoPlayerList[videoIndex].Play();

        videoPlayerDict[currentFileName].Play();

        //Wait while the current video is playing
        bool reachedHalfWay = false;

        //int nextIndex = (videoIndex + 1);
        while (videoPlayerDict[currentFileName].isPlaying)
        {
            //Commented Debug.Log because it is expensive
            //Debug.Log("Playing time: " + videoPlayerList[videoIndex].time + " INDEX: " + videoIndex);

            if (currentFileName != response.current.FileName)
            {
                Debug.LogWarning("Video changed, currentFileName != response.current.FileName");
                videoPlayerDict[currentFileName].Stop();
                StartCoroutine(PlayVideo(false));
            }

            if (response.isPaused == true)
            {
                videoPlayerDict[currentFileName].Pause();
                while (videoPlayerDict[currentFileName].isPaused)
                {
                    if (response.isPaused == false)
                    {
                        break;
                    }
                    yield return null;
                }
                videoPlayerDict[currentFileName].Play();
            }

            //(Check if we have reached half way)
            if (!reachedHalfWay && videoPlayerDict[currentFileName].time >= (videoPlayerDict[currentFileName].length / 2))
            {
                reachedHalfWay = true; //Set to true so that we don't evaluate this again

                //Make sure that the NEXT VideoPlayer index is valid otherwise exit since this is the end
                //if (nextIndex >= totalVideoCount)
                //{
                //    Debug.LogWarning("End of All Videos: " + videoIndex);
                //    yield break;
                //}

                //Prepare the NEXT videos
                for (int i = 0; i < nextQueue.Count; i++)
                {
                    videoPlayerDict[nextQueue[i].FileName].Prepare();
                    Debug.LogWarning("Ready to Prepare NEXT Video: " + nextQueue[i].FileName);
                }
            }
            yield return null;
        }

        Debug.Log("Done Playing current Video: " + currentFileName);

        //Wait until NEXT video is prepared
        for (int i = 0; i < nextQueue.Count; i++)
        {
            while (!videoPlayerDict[nextQueue[i].FileName].isPrepared)
            {
                Debug.Log("Preparing NEXT Video: " + nextQueue[i].FileName);
                yield return null;
            }
            Debug.LogWarning("Done Preparing NEXT Video: " + nextQueue[i].FileName);
            yield return null;
        }
        

        //while (!videoPlayerList[nextIndex].isPrepared) 
        //{
        //    Debug.Log("Preparing NEXT Video Index: " + nextIndex);
        //    yield return null;
        //}

        //Releasing resources
        //videoPlayerList[videoIndex].Stop(); // IMPORTANT
        
        //Increment Video index
        //videoIndex++;

        //Play next prepared video. Pass false to it so that some codes are not executed at-all
        StartCoroutine(PlayVideo(false));
    }
}