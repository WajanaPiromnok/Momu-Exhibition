using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Stage event manager that handles showing event objects at certain times of day
/// </summary>
public class StageEventManager : MonoBehaviour
{
    #region Members
    public bool debug_Enabled = false;
    private StageEvent debug_PreviouslyPlayedEvent = null;
    public StageVideoPlayer stage_a_video;
    public StageVideoPlayer stage_b_video;
    public StageVideoPlayer stage_c_video;
    public StageVideoPlayer stage_d_video;
    public AudioSource stageAudioPlayer;
    public Light mainLight;
    public float mainLightIntensity = 2.0f;
    public Light secondaryLight;
    public float secondaryLightIntensity = 0.7f;
    public Volume postProcessingVolume;
    public float postExposureLightValue = 0.3f;
    public float postExposureDarkValue = -3.0f;

    [Header("Child event objects will be loaded by default")]
    public List<StageEvent> events;

    private bool runEventCheck = true;
    #endregion

    #region Unity functions
    void Awake()
    {
        // Load all child events by default
        foreach(Transform evtObj in transform)
        {
            if(evtObj.GetComponent<StageEvent>())
            {
                StageEvent evt = evtObj.GetComponent<StageEvent>();
                events.Add(evt);
                Debug.Log("Loaded event " + evt.eventName + " for " + evt.startTimeStr + " to " + evt.endTimeStr);
            }
        }

        if(events.Count == 0)
        {
            Debug.LogWarning("No events have been setup.");
        }
    }

    void Start()
    {
        // Start checking event schedule
        // Done as coroutine to reduce background processing
        StartCoroutine(EventScheduleCheck());
    }

    #endregion

    #region Methods
    /// <summary>
    /// Setup and show StageEvent and dim the lights
    /// </summary>
    /// <param name="evt"></param>
    public void SetupAndShowEvent(StageEvent evt)
    {
        StartCoroutine(SetLightIntensityOverTime(mainLight, 0.0f, 1.0f));
        StartCoroutine(SetLightIntensityOverTime(secondaryLight, 0.0f, 1.0f));
        StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureDarkValue, 1.0f));

        stage_a_video.SetVideoFile(evt.stageAvideo);
        stage_b_video.SetVideoFile(evt.stageBvideo);
        stage_c_video.SetVideoFile(evt.stageCvideo);
        stage_d_video.SetVideoFile(evt.stageDvideo);
        
        evt.ShowEvent();

        stage_a_video.PlayVideo();
        stage_b_video.PlayVideo();
        stage_c_video.PlayVideo();
        stage_d_video.PlayVideo();

        stageAudioPlayer.PlayOneShot(evt.stageAudio);

        Debug.Log("Showing " + evt.eventName);
    }

    /// <summary>
    /// Hide StageEvent object and return lights back to normal
    /// </summary>
    /// <param name="evt"></param>
    public void HideEvent(StageEvent evt)
    {
        stage_a_video.StopVideo();
        stage_b_video.StopVideo();
        stage_c_video.StopVideo();
        stage_d_video.StopVideo();

        stageAudioPlayer.Stop();

        evt.HideEvent();

        StartCoroutine(SetLightIntensityOverTime(mainLight, mainLightIntensity, 1.0f));
        StartCoroutine(SetLightIntensityOverTime(secondaryLight, secondaryLightIntensity, 1.0f));
        StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureLightValue, 1.0f));
        Debug.Log("Closing " + evt.eventName);
    }

    /// <summary>
    /// Check event schedule and show events as needed
    /// </summary>
    /// <returns></returns>
    public IEnumerator EventScheduleCheck()
    {
        if(debug_Enabled)
            yield break;

        while(runEventCheck)
        {
            if (events.Count > 0)
            {
                // Loop and check all events at current time and show if necessary
                DateTime currentTime = DateTime.Now;

                // Using LINQ apparently allows removing while iterating hence ToList() -> https://stackoverflow.com/questions/1582285/how-to-remove-elements-from-a-generic-list-while-iterating-over-it
                foreach (StageEvent evt in events.ToList())
                {
                    // If the event has not started before and hasn't already passed
                    if (!evt.hasShown)
                    {
                        // If event hasn't started and it's time to start
                        if (currentTime >= evt.startTime && currentTime < evt.endTime)
                        {
                            SetupAndShowEvent(evt);
                        }
                    }
                    else
                    {   
                        // Hide shown event and remove from the checking list
                        if (currentTime >= evt.endTime && currentTime >= evt.startTime)
                        {
                            HideEvent(evt);
                            events.Remove(evt);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(.5f);
        }
    }
    
    /// <summary>
    /// Set a light to transition to a certain intensity over a certain time
    /// </summary>
    /// <param name="targetLight"></param>
    /// <param name="toIntensity"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator SetLightIntensityOverTime(Light targetLight, float toIntensity, float duration)
    {
        float counter = 0;
        float startIntensity = targetLight.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(startIntensity, toIntensity, counter / duration);
            yield return null;
        }
    }

    /// <summary>
    /// Set post processing post exposure to a certain value over a certain time
    /// </summary>
    /// <param name="targetVolume"></param>
    /// <param name="toIntensity"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator SetPostExposureOverTime(Volume targetVolume, float toValue, float duration)
    {
        ColorAdjustments colAdj;
        if (!targetVolume.profile.TryGet<ColorAdjustments>(out colAdj))
        {
            Debug.LogError("No color adjustment post process is setup!");
        }

        float counter = 0;
        float startValue = colAdj.postExposure.value;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            colAdj.postExposure.value = Mathf.Lerp(startValue, toValue, counter / duration);
            yield return null;
        }
    }

    public void DebugPlay(int index) {
        if(!debug_Enabled)
            return;

        if(debug_PreviouslyPlayedEvent != null) {
            debug_PreviouslyPlayedEvent.HideEvent();
        }

        events[index].ShowEvent();
        debug_PreviouslyPlayedEvent = events[index];
    }
    #endregion
}
