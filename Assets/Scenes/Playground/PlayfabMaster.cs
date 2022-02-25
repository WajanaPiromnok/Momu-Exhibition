using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabMaster : MonoBehaviour
{
    static PlayFabMaster mInstance;
 
    public static PlayFabMaster Instance
    {
        get
        {
            if (mInstance == null){
                GameObject go = new GameObject("PlayFab Singleton");
                mInstance = go.AddComponent<PlayFabMaster>();
            }
            return mInstance;
        }
    }

    private string playFabId = null;
    [SerializeField] public AvatarOutlook outlookData = null;
    [SerializeField] public PlayerLastPosition playerPositionData = null;
    void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start() {
        StartCoroutine(UpdateUserDataCoroutine());
    }

    IEnumerator UpdateUserDataCoroutine(){
        Debug.Log("Start update player data coroutine");
        while(true){
            Debug.Log("First part while true");

            while(!isLoggedIn || !PlaygroundMaster.Instance.network.isNetworkActive) {
                yield return new WaitForSeconds(10);
            }

            Debug.Log("Second part while true");
            while(isLoggedIn && PlaygroundMaster.Instance.network.isNetworkActive) {
                yield return new WaitForSeconds(10);
                Debug.Log("Before Update Check");
                if(isLoggedIn && PlaygroundMaster.Instance.network.isNetworkActive) {
                    if(!playerPositionData.IsDefault() && ModifiedMovementInput.Main != null && ModifiedMovementInput.Main.isDataApplied) {
                        UpdateAllUserData();
                    }
                }
            }
            yield return new WaitForSeconds(10);
        }
    }
    public void Login(string authMode, string userId)
    {
        Debug.Log("logging in " + authMode + ": " + userId);

        switch (authMode)
        {
            case "facebook":
                var fbLoginRequest = new LoginWithFacebookRequest { AccessToken = userId, CreateAccount = true };
                PlayFabClientAPI.LoginWithFacebook(fbLoginRequest, OnLoginResult, OnLoginError);
                break;

            case "email":
                var googleLoginRequest = new LoginWithGoogleAccountRequest { PlayerSecret=userId, CreateAccount = true };
                PlayFabClientAPI.LoginWithGoogleAccount(googleLoginRequest, OnLoginResult, OnLoginError);
                break;

            case "anonymous":
                var customLoginRequest = new LoginWithCustomIDRequest { CustomId = userId, CreateAccount = true };
                PlayFabClientAPI.LoginWithCustomID(customLoginRequest, OnLoginResult, OnLoginError);
                break;

            default:
                Debug.LogError("auth mode doesn't match any implementation : " + authMode);
                throw new NotImplementedException();
        }
    }

    void OnLoginError(PlayFabError error) {
        throw new NotImplementedException();
        // show alert popup and clear the field
        // TODO
    }
    void OnLoginResult(LoginResult result)
    {
        PlaygroundMaster.Instance.HideUserIdGui();
        this.playFabId = result.PlayFabId;
        Debug.Log("got playfab id " + this.playFabId);

        List<string> userDataKeys = new List<string>();
        userDataKeys.AddRange(AvatarOutlook.keys);
        userDataKeys.AddRange(PlayerLastPosition.keys);
        Debug.Log("KEYS");
        Debug.Log(WtfUtils.ListToString(userDataKeys));
        GetUserDataRequest getDataRequest = new GetUserDataRequest { PlayFabId = playFabId, Keys = userDataKeys };
        PlayFabClientAPI.GetUserData(getDataRequest,
            dataResult =>
            {
                bool hasAllKeys = true;

                foreach(string expectedKey in userDataKeys) {
                    if(!dataResult.Data.ContainsKey(expectedKey)) {
                        hasAllKeys = false;
                        break;
                    }
                }
                
                if(hasAllKeys){
                    this.outlookData = new AvatarOutlook(dataResult.Data);
                    this.playerPositionData = new PlayerLastPosition(dataResult.Data);
                } else {
                    OnGetUserDataErrorOrDataIncomplete();
                }

                AfterGetUserData(result.NewlyCreated);
            },
            dataError =>
            {
                Debug.Log(dataError.GenerateErrorReport());
                OnGetUserDataErrorOrDataIncomplete();
                AfterGetUserData(result.NewlyCreated);
            }
        );
    }

    void AfterGetUserData(bool isNewlyCreated)
    {
        PlaygroundMaster.Instance.InitNetwork();
        // apply pos/avatar first time within control script start/awake

        if (isNewlyCreated)
        {
            //show avater selection GUI
        }
    }

    void OnGetUserDataErrorOrDataIncomplete(){
        // the values doesn't exist so create default locally and update straight away
        this.outlookData = new AvatarOutlook();
        this.playerPositionData = new PlayerLastPosition();
        UpdateAllUserData();
    }
    void UpdateAllUserData()
    {
        if (String.IsNullOrEmpty(playFabId))
        {
            Debug.LogError("playfabId not set but attempt to update data");
            return;
        }

        UpdateUserDataRequest request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {"outlook_baseModel", this.outlookData.baseModel.ToString()},
                {"outlook_top", this.outlookData.top.ToString()},
                {"outlook_body", this.outlookData.body.ToString()},
                {"outlook_bottom",this.outlookData.bottom.ToString()},
                {"position_x", this.playerPositionData.x.ToString()},
                {"position_z", this.playerPositionData.z.ToString()},
                {"rotation_x", this.playerPositionData.rx.ToString()},
                {"rotation_y", this.playerPositionData.ry.ToString()},
                {"rotation_z", this.playerPositionData.rz.ToString()},
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => { Debug.Log("Update user data successfully"); },
            error => { Debug.LogError(error.GenerateErrorReport()); }
        );
    }

    public string GetPlayfabId()
    {
        return playFabId;
    }

    public void SetLastPlayerPositionForUpdateIfLoggedIn(Vector3 currentPos, Quaternion currentRot, AvatarOutlook outlook)
    {
        if(isLoggedIn){
            this.playerPositionData = new PlayerLastPosition(currentPos.x, currentPos.z, currentRot.eulerAngles);
            this.outlookData = outlook;
        }
    }

    public bool isLoggedIn{get {return PlayFabClientAPI.IsClientLoggedIn();}}
}
