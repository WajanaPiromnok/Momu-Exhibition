using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a mainstage event that shows at a predefined time
/// </summary>
public class StageEvent : MonoBehaviour
{
    #region Members
    public string eventName = "Event";
    public string startTimeStr = "dd/mm/yyyy hh:mm:ss";
    public string endTimeStr = "dd/mm/yyyy hh:mm:ss";

    public System.DateTime startTime;
    public System.DateTime endTime;

    public GameObject eventModels;
    public Animation stageAnimator;
    public Animation lightFrameAnimator;
    [Header("Video URL should be relative path in streaming assets folder!")]
    public string stageAvideo = "";
    public string stageBvideo = "";
    public string stageCvideo = "";
    public string stageDvideo = "";
    public AudioClip stageAudio;

    [HideInInspector]
    public bool hasShown = false;

    // Culture info for parsing date and time settings
    private System.Globalization.CultureInfo culInf = new System.Globalization.CultureInfo("fr-FR");

    #endregion

    #region Unity Functions
    void Awake()
    {
        // Use gregorianCalendar for date time parsing
        culInf.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();

        // Parse and initialize event start and end time (with error checking)
        if (startTimeStr != null && startTimeStr != "")
        {
            startTime = System.DateTime.Parse(startTimeStr, culInf);
        }
        else
        {
            startTime = System.DateTime.Now;
            Debug.LogError("Event " + eventName + " start time not set.");
        }
            
        if (endTimeStr != null && endTimeStr != "")
        {
            endTime = System.DateTime.Parse(endTimeStr, culInf);
        }
        else
        {
            endTime = startTime.AddMinutes(2);
            Debug.LogError("Event " + eventName + " end time not set.");
        }

        // If time has already passed for event, set has shown to be true
        if(System.DateTime.Now > startTime || System.DateTime.Now > endTime)
        {
            hasShown = true;
        }
            
        Debug.Log("Event " + eventName + " set for show from " + startTime + " to " + endTime);

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
        if (stageAnimator != null)
        {
            stageAnimator.Play();
        }
        if (lightFrameAnimator != null)
        {
            lightFrameAnimator.Play();
        }
        hasShown = true;
    }

    /// <summary>
    /// Hide the event object content
    /// </summary>
    public void HideEvent()
    {
        eventModels.SetActive(false);
        if (stageAnimator != null)
        {
            stageAnimator.Stop();
        }
        if (lightFrameAnimator != null)
        {
            lightFrameAnimator.Stop();
        }
    }
    #endregion
}
