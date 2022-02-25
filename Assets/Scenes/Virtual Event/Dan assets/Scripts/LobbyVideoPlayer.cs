using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Simple script to get run time URL for 
/// </summary>
public class LobbyVideoPlayer : MonoBehaviour
{
    public string videoFile = "";
    [SerializeField]
    private VideoPlayer videoPlayer;
    void Start()
    {
        // Init video URL
        if (videoFile != null && videoFile != "")
        {
            if(!WtfUtils.IsEditor() && !WtfUtils.IsWebGL())
                videoPlayer.url = "https://motionjam2021.momu.co/game/StreamingAssets/" + videoFile;
            else
                videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);
        }

        // Make sure video player is properly initialized
        videoPlayer.source = VideoSource.Url;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.playOnAwake = true;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = true;

        // Make sure play starts
        videoPlayer.Play();
    }
}

