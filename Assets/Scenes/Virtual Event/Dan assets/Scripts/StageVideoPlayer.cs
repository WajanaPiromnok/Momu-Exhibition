using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Helper controller for setting stage screen video
/// </summary>
public class StageVideoPlayer : MonoBehaviour
{
    public string videoFile = "";
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if(videoFile != null && videoFile != "")
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);
        }

        // Make sure video player is properly initialized
        videoPlayer.source = VideoSource.Url;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        // TODO: Clean up if time is available
        //videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = false;
    }

    /// <summary>
    /// Set video URL for video player
    /// </summary>
    /// <param name="newURL"></param>
    public void SetVideoURL(string newURL)
    {
        videoPlayer.url = newURL;
    }

    /// <summary>
    /// Set video file for video player
    /// </summary>
    /// <param name="newVideoFile"></param>
    public void SetVideoFile(string newVideoFile)
    {
        videoFile = newVideoFile;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);
    }

    /// <summary>
    /// Stop the video player
    /// </summary>
    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    /// <summary>
    /// Start the video player
    /// </summary>
    public void PlayVideo()
    {
        videoPlayer.Play();
    }
}
