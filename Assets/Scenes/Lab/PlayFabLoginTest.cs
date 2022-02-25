using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLoginTest : MonoBehaviour
{
    private string customId;
 public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
        }

        customId = "GettingStartedGuide#" + Random.Range(1, 1000).ToString("0000");
        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        Debug.Log(result.ToJson());

        var request = new GetAccountInfoRequest {PlayFabId=customId};
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
        Debug.Log("Get Account Info Success");
        Debug.Log(result.ToJson());
    }

    private void OnGetAccountInfoFailure(PlayFabError error) {
        Debug.Log("GetAccountInfo error");
        Debug.Log(error.GenerateErrorReport());
    }
}
