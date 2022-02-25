using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Controller script for handling showcase room video screen
/// </summary>
public class VideoRoomController : MonoBehaviour
{
    #region Members
    // Not best practice, but it works for this purpose
    private static int maxPlayingVideos = 4;
    private static int playingVideos = 0;

    // Object references
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    [Tooltip("Must include file extension in filename!")]
    public string videoFile = "";
    public AudioClip videoSound;
    public GameObject roomObjects;

    private bool playerExited = false;
    #endregion

    #region Unity functions
    void Awake()
    {
        // Simple error checking / preemptive prevention
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (videoFile == "" || videoFile == null)
        {
            Debug.LogWarning("Video file for room " + name + " does not exist.");
        }

        // Set video URL
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);

        // Make sure video player is setup correctly
        videoPlayer.source = VideoSource.Url;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        // TODO: Clean up if time is available
        //videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = true;

        // Setup room audio
        audioSource.clip = videoSound;
        audioSource.loop = true;

        // I need to understand WTF is going on with this syntax
        videoPlayer.loopPointReached += CheckVideoOver;
    }

    void Start()
    {
        // Assign first child object as room object parent by default
        if(roomObjects == null)
        {
            if(transform.childCount > 0)
            {
                roomObjects = transform.GetChild(0).gameObject;
            } 
        }

        if(roomObjects != null)
        {
            roomObjects.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Show in room objects
        if(roomObjects != null)
        {
            roomObjects.SetActive(true);
        }

        if (other.tag == "Player")
        {
            // Load and play video when entering the room area
            if(videoPlayer.url != null && videoPlayer.url != "" && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
                audioSource.Play();
                playerExited = false;
                playingVideos += 1;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Hide in room objects
        if(roomObjects != null)
        {
            roomObjects.SetActive(false);
        }

        if (other.tag == "Player")
        {
            // Unload if too many videos are already playing and set to stop video once the player is out
            if(playingVideos > maxPlayingVideos)
            {
                videoPlayer.Stop();
                audioSource.Stop();
                playingVideos -= 1;
            }
            playerExited = true;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Checks if player is out of the area and stops video accordingly
    /// </summary>
    /// <param name="vp"></param>
    private void CheckVideoOver(VideoPlayer vp)
    {
        if(playerExited)
        {
            videoPlayer.Stop();
            audioSource.Stop();
            playingVideos -= 1;
        }
    }
    #endregion
}
