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
    public List<TimeRangeStrings> triggerRanges = new List<TimeRangeStrings>(); 
    public StageVideoPlayer stage_a_video;
    public StageVideoPlayer stage_b_video;
    public StageVideoPlayer stage_c_video;
    public StageVideoPlayer stage_d_video;
    public AudioSource stageAudioPlayer;
    public GameObject stageLights;
    public Light mainLight;
    public float mainLightIntensity = 2.0f;
    public Light secondaryLight;
    public float secondaryLightIntensity = 0.7f;
    public Volume postProcessingVolume;
    public float postExposureLightValue = 0.3f;
    public float postExposureDarkValue = -3.0f;
    public float postExposureBlackoutValue = -8.0f;

    [Header("Child event objects will be loaded by default")]
    public List<StageEvent> events;
    
    private bool runEventCheck = true;
    private StageEvent debug_currentEvent = null;
    private StageEvent debug_nextEvent = null;
    private List<StageVideoPlayer> videoPlayers;
    #endregion

    #region Unity functions
    void Awake()
    {
        videoPlayers = new List<StageVideoPlayer>{
            stage_a_video,
            stage_b_video,
            stage_c_video,
            stage_d_video
        };

        //     triggerRanges.Clear();
        // for(int i = 0; i < 24; i++) {
        //     TimeRangeStrings newObj = new TimeRangeStrings();
        //     newObj.startTimeStr = i.ToString("00") + ":00:00";
        //     newObj.endTimeStr = i.ToString("00") + ":20:00"; 
        //     triggerRanges.Add(newObj);
            
        //     TimeRangeStrings newObj2 = new TimeRangeStrings();
        //     newObj2.startTimeStr = i.ToString("00") + ":30:00";
        //     newObj2.endTimeStr = i.ToString("00") + ":50:00"; 
        //     triggerRanges.Add(newObj2);
        // }
        foreach(StageEvent evt in GetComponentsInChildren<StageEvent>())
            if(evt.gameObject.activeSelf)
                events.Add(evt);

        if(events.Count == 0)
            Debug.LogWarning("No events have been setup.");
    }

    void Start()
    {
        // Done as coroutine to reduce background processing
        StartCoroutine(
            debug_Enabled ? 
            DebugCoroutine() : 
            EventScheduleCheck()
        );
    }

    #endregion

    #region Methods
    private TimeRangeStrings CheckTimeTrigger() {
        foreach(TimeRangeStrings timeRange in triggerRanges)
            if(timeRange.IsInRange())
                return timeRange;
        return null;
    }
    private void PrepareStageForSeries() {
        StartCoroutine(SetLightIntensityOverTime(mainLight, 0.0f, 1.0f));
        StartCoroutine(SetLightIntensityOverTime(secondaryLight, 0.0f, 1.0f));
        StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureDarkValue, 1.0f));
    }
    private Coroutine InitiateBlackout(float transitionTime) {
        return StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureBlackoutValue, transitionTime));
    }
    private Coroutine GetLightBack(float transitionTime) {
        return StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureDarkValue, transitionTime));
    }
    private void SetStageLightsActive(bool show) {
        if(stageLights != null)
            stageLights.gameObject.SetActive(show);
    }

    /// <summary>
    /// Returns true if all using video players are prepared
    /// and ready to be played first frame with no starting delay
    /// </summary>
    /// <param name="evt"></param>
    private bool IsReadyToPlayVideos(StageEvent evt) {
        if(String.IsNullOrEmpty(evt.stageAvideo))
            if(!stage_a_video.IsReadyToPlayWithNoDelay())
                return false;

        if(String.IsNullOrEmpty(evt.stageBvideo))
            if(!stage_b_video.IsReadyToPlayWithNoDelay())
                return false;
                
        if(String.IsNullOrEmpty(evt.stageCvideo))
            if(!stage_c_video.IsReadyToPlayWithNoDelay())
                return false;
                
        if(String.IsNullOrEmpty(evt.stageDvideo))
            if(!stage_d_video.IsReadyToPlayWithNoDelay())
                return false;

        return true;
    }
    private void StopVideoAndAudio() {
        // Stop video and audio
        stage_a_video.StopVideo();
        stage_b_video.StopVideo();
        stage_c_video.StopVideo();
        stage_d_video.StopVideo();
        stageAudioPlayer.Stop();
    }

    private void TearDownStageAfterSeries() {
        stage_a_video.UseLightMaterial();
        stage_b_video.UseLightMaterial();
        stage_c_video.UseLightMaterial();
        stage_d_video.UseLightMaterial();
        
        StopVideoAndAudio();

        // Reset screens
        stage_a_video.SetScreenTransparency(1.0f, false);
        stage_b_video.SetScreenTransparency(1.0f, false);
        stage_c_video.SetScreenTransparency(1.0f, false);
        stage_d_video.SetScreenTransparency(1.0f, false);

        SetStageLightsActive(true);
        StartCoroutine(SetLightIntensityOverTime(mainLight, mainLightIntensity, 1.0f));
        StartCoroutine(SetLightIntensityOverTime(secondaryLight, secondaryLightIntensity, 1.0f));
        StartCoroutine(SetPostExposureOverTime(postProcessingVolume, postExposureLightValue, 1.0f));
    }

    /// <summary>
    /// Check event schedule and show events as needed
    /// </summary>
    /// <returns></returns>
    private IEnumerator EventScheduleCheck()
    {
        while(runEventCheck)
        {
            TimeRangeStrings trigger = CheckTimeTrigger();
            if(trigger != null)
            {
                PrepareStageForSeries();
                int secondsSinceStart = trigger.SecondsSinceStart();

                foreach(StageEvent evt in events) {
                    // skip some events in the past
                    if(secondsSinceStart >= evt.durationSeconds) {
                        //ignoring detailed timing such as lighing transition
                        secondsSinceStart -= evt.durationSeconds;
                        continue;
                    }
                    
                    secondsSinceStart = 0;

                    yield return StartCoroutine(SetupStageForEvent(evt));
                    evt.ShowEvent();
                    GetLightBack(evt.brightInDuration);
                    yield return new WaitForSeconds(evt.durationSeconds);
                    if(evt.exitWithBlackout)
                        yield return InitiateBlackout(evt.blackoutDuration);
                    evt.HideEvent();
                    StopVideoAndAudio();
                    yield return new WaitForSeconds(evt.cooldownDuration);
                }

                TearDownStageAfterSeries();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator SetupStageForEvent(StageEvent evt) {
        Debug.Log("Setting up : " + evt.eventName);

        // Setup stage lights
        SetStageLightsActive(!evt.hasLightAnimation);

        // Init the video
        stage_a_video.SetVideoFile(evt.stageAvideo);
        stage_b_video.SetVideoFile(evt.stageBvideo);
        stage_c_video.SetVideoFile(evt.stageCvideo);
        stage_d_video.SetVideoFile(evt.stageDvideo);


        // Wait until video players can play without delay
        yield return new WaitWhile(() => !IsReadyToPlayVideos(evt));
        
        stage_a_video.UseDarkMaterial();
        stage_b_video.UseDarkMaterial();
        stage_c_video.UseDarkMaterial();
        stage_d_video.UseDarkMaterial();
        
        // Setup transparent screens if necessary
        if(evt.hasTransparentScreens)
        {
            stage_a_video.SetScreenTransparency(evt.screenAlpha, true);
            stage_b_video.SetScreenTransparency(evt.screenAlpha, true);
            stage_c_video.SetScreenTransparency(evt.screenAlpha, true);
            stage_d_video.SetScreenTransparency(evt.screenAlpha, true);
        }

        // Start the video and audio (with null checks for special cases)
        if(!String.IsNullOrEmpty(evt.stageAvideo))
            stage_a_video.PlayVideo();

        if(!String.IsNullOrEmpty(evt.stageBvideo))
            stage_b_video.PlayVideo();

        if(!String.IsNullOrEmpty(evt.stageCvideo))
            stage_c_video.PlayVideo();

        if(!String.IsNullOrEmpty(evt.stageDvideo))
            stage_d_video.PlayVideo();

        if(evt.stageAudio != null)
            stageAudioPlayer.PlayOneShot(evt.stageAudio);
    }
    
    private IEnumerator DebugCoroutine()
    {
        PrepareStageForSeries();
                    
        while(true) {
            if(debug_nextEvent != null) {
                if(debug_currentEvent != null) {
                    debug_currentEvent.HideEvent();
                    StopVideoAndAudio();
                }
                yield return StartCoroutine(SetupStageForEvent(debug_nextEvent));
                debug_nextEvent.ShowEvent();
                debug_currentEvent = debug_nextEvent;
                debug_nextEvent = null;
            }
            yield return null;
        }
    }
    
    /// <summary>
    /// Set a light to transition to a certain intensity over a certain time
    /// </summary>
    /// <param name="targetLight"></param>
    /// <param name="toIntensity"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator SetLightIntensityOverTime(Light targetLight, float toIntensity, float duration)
    {
        float counter = 0;
        float startIntensity = targetLight.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(startIntensity, toIntensity, counter / duration);
            yield return null;
        }
        targetLight.intensity = toIntensity;
    }

    /// <summary>
    /// Set post processing post exposure to a certain value over a certain time
    /// </summary>
    /// <param name="targetVolume"></param>
    /// <param name="toIntensity"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator SetPostExposureOverTime(Volume targetVolume, float toValue, float duration)
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
        colAdj.postExposure.value = toValue;
    }

    /// <summary>
    /// Debug function
    /// </summary>
    /// <param name="index"></param>
    public void DebugPlay(int index) {
        if(!debug_Enabled)
            return;

        debug_nextEvent = events[index];
    }
    #endregion
}
