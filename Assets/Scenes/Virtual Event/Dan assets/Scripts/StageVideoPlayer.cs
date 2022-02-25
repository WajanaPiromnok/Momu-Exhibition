using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Helper controller for setting stage screen video
/// </summary>
public class StageVideoPlayer : MonoBehaviour
{
    [HideInInspector]
    public string videoFile = "";
    public Material darkMaterial, lightMaterial;
    private VideoPlayer videoPlayer;
    private Renderer videoSurfaceRenderer;
    public Renderer pillarRenderer;

    void Awake()
    {
        // Setup references
        videoSurfaceRenderer = GetComponent<Renderer>();
        if (videoSurfaceRenderer == null)
        {
            Debug.LogError("Stage video player incorrectly applied to screen!");
        }

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("Screen lacks video player component!");
        }
    }

    void Start()
    {
        if(videoFile != null && videoFile != "")
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);
        }

        // Make sure video player is properly initialized
        videoPlayer.source = VideoSource.Url;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = false;
    }

    public bool IsReadyToPlayWithNoDelay() {
        return videoPlayer.isPrepared;
    }
    
    /// <summary>
    /// Set video URL for video player
    /// </summary>
    /// <param name="newURL"></param>
    public void SetVideoURL(string newURL)
    {
        videoPlayer.url = newURL;
        videoPlayer.Prepare();
    }

    /// <summary>
    /// Set video file for video player
    /// </summary>
    /// <param name="newVideoFile"></param>
    public void SetVideoFile(string newVideoFile)
    {
        videoFile = newVideoFile;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile);
        videoPlayer.Prepare();
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

    public void UseDarkMaterial() {
        // Debug.Log("Use Dark Mat on " + transform.parent.gameObject.name);
        // videoSurfaceRenderer.materials = new Material[] {darkMaterial};
        videoSurfaceRenderer.material = darkMaterial;
    }

    public void UseLightMaterial() {
        // Debug.Log("Use Light Mat on " + transform.parent.gameObject.name);
        // videoSurfaceRenderer.materials = new Material[] {lightMaterial};
        videoSurfaceRenderer.material = lightMaterial;
    }

    /// <summary>
    /// Hides video pillar
    /// </summary>
    public void HidePillar()
    {
        pillarRenderer.enabled = false;
    }

    /// <summary>
    /// Shows video pillar
    /// </summary>
    public void ShowPillar()
    {
        pillarRenderer.enabled = true;
    }

    /// <summary>
    /// Set transparecy of the screen and show or hide pillar behind
    /// </summary>
    /// <param name="opacity">value from [0,1]</param>
    /// <param name="hidePillar"></param>
    public void SetScreenTransparency(float opacity = 1.0f, bool hidePillar = true)
    {
        float alpha = Mathf.Clamp(opacity, 0.0f, 1.0f);

        if (pillarRenderer != null)
        {
            if(hidePillar)
            {
                HidePillar();
            }
            else
            {
                ShowPillar();
            } 
        }

        Debug.Log("Set video transparency to " + opacity);
        Color diffuse = videoSurfaceRenderer.material.GetColor("_BaseColor");
        videoSurfaceRenderer.material.SetColor("_BaseColor", new Color(diffuse.r, diffuse.g, diffuse.b, alpha));
        
        // Color diffuse = lightMaterial.GetColor("_BaseColor");
        // lightMaterial.SetColor("_BaseColor", new Color(diffuse.r, diffuse.g, diffuse.b, alpha));
        
        // diffuse = darkMaterial.GetColor("_BaseColor");
        // darkMaterial.SetColor("_BaseColor", new Color(diffuse.r, diffuse.g, diffuse.b, alpha));
    }
}
