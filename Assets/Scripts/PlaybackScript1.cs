// Will have to download video clips from server and add them to videoClipList

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlaybackScript1 : MonoBehaviour
{
    //Raw Image to Show Video Images [Assigned from the Editor]
    public RawImage image;
    //Set from the Editor
    public List<VideoClip> videoClipList;
    public List<VideoPlayer> videoPlayerList;
    private int videoIndex = 0;
    public RenderTexture rt;

    void Start()
    {
        StartCoroutine(playVideo());
    }

    IEnumerator playVideo(bool firstRun = true)
    {
        if (videoClipList == null || videoClipList.Count <= 0)
        {
            Debug.LogError("Assign VideoClips from the Editor");
            yield break;
        }

        //Init videoPlayerList first time this function is called
        if (firstRun)
        {
            for (int i = 0; i < videoClipList.Count; i++)
            {
                //For each video, add a videoPlayer component to GameObject
                VideoPlayer videoPlayer = gameObject.AddComponent<VideoPlayer>();
                //rt = Resources.Load<RenderTexture>("Textures\\VideoRenderTexture.renderTexture");
                videoPlayer.targetTexture = rt;
                videoPlayerList.Add(videoPlayer);

                //Disable Play on Awake
                videoPlayerList[i].playOnAwake = false;

                //We want to play from video clip not from url
                videoPlayerList[i].source = VideoSource.VideoClip;

                //Set video Clip To Play 
                videoPlayerList[i].clip = videoClipList[i];
                //videoPlayerList[videoIndex].Prepare(); //TEMPORARY
            }
        }

        //Make sure that the NEXT VideoPlayer index is valid
        if (videoIndex >= videoPlayerList.Count)
            yield break;

        //Prepare video
        videoPlayerList[videoIndex].Prepare();

        //Wait until this video is prepared
        while (!videoPlayerList[videoIndex].isPrepared)
        {
            Debug.Log("Preparing Index: " + videoIndex);
            yield return null;
        }
        Debug.LogWarning("Done Preparing current Video Index: " + videoIndex);

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayerList[videoIndex].texture;

        //Play first video
        videoPlayerList[videoIndex].Play();

        //Wait while the current video is playing
        bool reachedHalfWay = false;
        int nextIndex = (videoIndex + 1);
        while (videoPlayerList[videoIndex].isPlaying)
        {
            //Commented Debug.Log because it is expensive
            //Debug.Log("Playing time: " + videoPlayerList[videoIndex].time + " INDEX: " + videoIndex);

            //(Check if we have reached half way)
            if (!reachedHalfWay && videoPlayerList[videoIndex].time >= (videoPlayerList[videoIndex].clip.length / 2))
            {
                reachedHalfWay = true; //Set to true so that we don't evaluate this again

                //Make sure that the NEXT VideoPlayer index is valid otherwise exit since this is the end
                if (nextIndex >= videoPlayerList.Count)
                {
                    Debug.LogWarning("End of All Videos: " + videoIndex);
                    yield break;
                }

                //Prepare the NEXT video
                Debug.LogWarning("Ready to Prepare NEXT Video Index: " + nextIndex);
                videoPlayerList[nextIndex].Prepare();
            }
            yield return null;
        }
        Debug.Log("Done Playing current Video Index: " + videoIndex);

        //Wait until NEXT video is prepared
        while (!videoPlayerList[nextIndex].isPrepared)
        {
            Debug.Log("Preparing NEXT Video Index: " + nextIndex);
            yield return null;
        }

        Debug.LogWarning("Done Preparing NEXT Video Index: " + videoIndex);

        //Releasing resources
        videoPlayerList[videoIndex].Stop(); // IMPORTANT
        
        //Increment Video index
        videoIndex++;

        //Play next prepared video. Pass false to it so that some codes are not executed at-all
        StartCoroutine(playVideo(false));
    }
}