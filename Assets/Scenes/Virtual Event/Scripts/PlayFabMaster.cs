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
    public bool isNewlyCreatedPlayer = false;
    public ServerDetails serverDetails = null;
    [SerializeField] public AvatarOutlook outlookData = null;
    [SerializeField] public PlayerLastPosition playerPositionData = null;
    void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start() {
        StartCoroutine(UpdateUserDataCoroutine());
    }

    IEnumerator UpdateUserDataCoroutine(){
        while(true){
            while(!isLoggedIn || !PlaygroundMaster.Instance.network.isNetworkActive) {
                yield return new WaitForSeconds(10);
            }

            while(isLoggedIn && PlaygroundMaster.Instance.network.isNetworkActive) {
                yield return new WaitForSeconds(10);
                if(isLoggedIn && PlaygroundMaster.Instance.network.isNetworkActive) {
                    if(!playerPositionData.IsDefault() && MainNetworkPlayer.Main != null && MainNetworkPlayer.Main.isDataApplied) {
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

            case "google":
                var googleLoginRequest = new LoginWithFacebookRequest { AccessToken=userId, CreateAccount = true };
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
        Debug.Log(error.GenerateErrorReport());
        PlaygroundMaster.Instance.ShowBlockCanvas("Authentication failed, please retry");
    }
    void OnLoginResult(LoginResult result)
    {
        PlaygroundMaster.Instance.HideUserIdGui();
        this.isNewlyCreatedPlayer = result.NewlyCreated;
        this.playFabId = result.PlayFabId;
        Debug.Log("got playfab id " + this.playFabId);

        List<string> userDataKeys = new List<string>(){AvatarOutlook.key, PlayerLastPosition.key};
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
                    this.outlookData = AvatarOutlook.FromPlayFab(dataResult.Data);
                    this.playerPositionData = PlayerLastPosition.FromPlayFab(dataResult.Data);
                } else {
                    // the values doesn't exist so create default locally and update straight away
                    this.outlookData = new AvatarOutlook();
                    this.playerPositionData = new PlayerLastPosition();
                    UpdateAllUserData();
                }

                AfterUserDataLoaded();
            },
            dataError =>
            {
                PlaygroundMaster.Instance.ShowBlockCanvas("Cannot access database, please try again");
            }
        );
        #if !UNITY_EDITOR
        #if UNITY_WEBGL
        
        string email = WebLibCustom.ReadLocalStorage("userEmail");
        Debug.Log("Updating email : " + email);
        if(!String.IsNullOrEmpty(email)) {
            AddOrUpdateContactEmailRequest emailRequest = new AddOrUpdateContactEmailRequest{EmailAddress=email};
            PlayFabClientAPI.AddOrUpdateContactEmail(emailRequest, result => {
                // Debug.Log(result.ToJson());
            }, error => {
                Debug.Log("updating email failed");
                Debug.Log(error.GenerateErrorReport());
            });
        }
        #endif
        #endif
    }

    void AfterUserDataLoaded()
    {
        LoadServerListThenInitNetwork();
    }

    void LoadServerListThenInitNetwork() {
        GetTitleDataRequest titleDataRequest = new GetTitleDataRequest { Keys = new List<string>(){"serverDetails"} };
        PlayFabClientAPI.GetTitleData(titleDataRequest, 
        result => {
            this.serverDetails = ServerDetails.FromPlayFab(result.Data);
            PlaygroundMaster.Instance.InitNetwork();
        }, 
        error => {
            PlaygroundMaster.Instance.ShowBlockCanvas("Cannot load server connection details, please try again");
        });
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
                {AvatarOutlook.key, JsonUtility.ToJson(this.outlookData)},
                {PlayerLastPosition.key, JsonUtility.ToJson(this.playerPositionData)}
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

    public void UpdatePlayerDataBuffer(Vector3 currentPos, Quaternion currentRot, string outlookJson)
    {
        this.playerPositionData = new PlayerLastPosition(currentPos, currentRot.eulerAngles);
        this.outlookData = JsonUtility.FromJson<AvatarOutlook>(outlookJson);
    }

    public bool isLoggedIn{get {return PlayFabClientAPI.IsClientLoggedIn();}}
}
