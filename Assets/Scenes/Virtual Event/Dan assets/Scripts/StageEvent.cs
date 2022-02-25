using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a mainstage event that shows at a predefined time
/// </summary>
public class StageEvent : MonoBehaviour
{
    const int NEXT_UP_THRESHOLD = 10;
    #region Members
    public string eventName = "Event";
    public int durationSeconds = 0;
    public GameObject eventModels;
    public List<Animation> stageAnimators;
    public Animation lightFrameAnimator;
    public Sprite showInfoSprite;
    public bool hasLightAnimation = true;
    public bool exitWithBlackout = true;
    public float brightInDuration = 0.1f;
    public float blackoutDuration = 5f;
    public float cooldownDuration = 5f;
    
    [Header("Video URL should be relative path in streaming assets folder!")]
    public string stageAvideo = "";
    public string stageBvideo = "";
    public string stageCvideo = "";
    public string stageDvideo = "";
    public AudioClip stageAudio;
    public bool hasTransparentScreens = false;
    public float screenAlpha = 0.5f;

    // Culture info for parsing date and time settings
    private System.Globalization.CultureInfo culInf = new System.Globalization.CultureInfo("fr-FR");

    #endregion

    #region Unity Functions
    void Awake()
    {
        // Use gregorianCalendar for date time parsing
        culInf.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();

        // Hide event content by default
        HideEvent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Show the event object contents
    /// </summary>
    public void ShowEvent()
    {
        eventModels.SetActive(true);
        PlaygroundMaster.Instance.showBadge.SetSpriteAndShow(showInfoSprite);

        if (stageAnimators != null)
        {
            foreach(Animation ani in stageAnimators)
            {
                ani.Play();
            }
        }
        if (lightFrameAnimator != null)
        {
            lightFrameAnimator.Play();
        }
    }

    /// <summary>
    /// Hide the event object content
    /// </summary>
    public void HideEvent()
    {
        eventModels.SetActive(false);
        PlaygroundMaster.Instance.showBadge.Hide();
        if (stageAnimators != null)
        {
            foreach (Animation ani in stageAnimators)
            {
                ani.Stop();
            }
        }
        if (lightFrameAnimator != null)
        {
            lightFrameAnimator.Stop();
        }
    }
    #endregion
}
