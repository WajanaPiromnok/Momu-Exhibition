using UnityEngine;
using Mirror;
using Mirror.SimpleWeb;
using System;
using UnityEngine.UI;
using UnityEditor;

public class PlaygroundMaster : MonoBehaviour
{
    static PlaygroundMaster mInstance;

    public static PlaygroundMaster Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType<PlaygroundMaster>();
            }
            return mInstance;
        }
    }
    void Awake()
    {
        if (PlaygroundMaster.mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this);
        
            // the only case that ssl applies is when run as windows on editor
            // #if UNITY_EDITOR
            //     Debug.Log("UNITY_EDITOR true");
            //     BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

            //     if (asClient)
            //         if (buildTarget != BuildTarget.StandaloneWindows64)
            //             YesSsl();

            // #elif UNITY_SERVER // build as server
            //     #if UNITY_STANDALONE_WIN // assume that windows means local development
            //                     NoSsl();
            //     #else
            //                     YesSsl();
            //     #endif

            // #else // build as client
            //         YesSsl();
            // #endif
        } else {
            return;
        }
    }
    // public PlayfabMaster playfab;
    public NetworkManager network;
    public Canvas canvas;
    public bool asClient = false;

    void Start()
    {
#if UNITY_EDITOR
        InitPlayFab(); // init network inside
#elif UNITY_SERVER
        InitNetwork();
#else
        InitPlayFab(); // init network inside
#endif
    }


    void YesSsl()
    {
        SimpleWebTransport webTransport = network.gameObject.GetComponent<SimpleWebTransport>();
        Debug.Log("Yes SSL");
        webTransport.clientUseWss = true;
        webTransport.sslEnabled = true;
        network.networkAddress = "twnz.dev";
    }

    void NoSsl()
    {
        SimpleWebTransport webTransport = network.gameObject.GetComponent<SimpleWebTransport>();
        Debug.Log("No SSL");
        webTransport.clientUseWss = false;
        webTransport.sslEnabled = false;
        network.networkAddress = "localhost";
    }
    public void InitNetwork()
    {
        // the only case that ssl applies is when run as windows on editor
        // start position
#if UNITY_EDITOR
        Debug.Log("UNITY_EDITOR true");
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        if (asClient)
        {
            if (buildTarget == BuildTarget.StandaloneWindows64)
            {
                // as set
            }
            else
                YesSsl();
            network.StartClient();
        }
        else
        {
            // anyhow is fine
            network.StartHost();
        }

#elif UNITY_SERVER // build as server
#if UNITY_STANDALONE_WIN // assume that windows means local development
                NoSsl();
#else
                YesSsl();
#endif

            network.StartServer();
#else // build as client
            YesSsl();
            network.StartClient();
#endif
    }

    public void InitPlayFab()
    {
        //check if there's userToken/Id provided
        string cookies = null;
        if (cookies != null)
        {
            string loginMode = "get from cookies";
            string accessTokenOrUserId = "get from cookies";

            HideUserIdGui();
            if (loginMode == "anonymous" && String.IsNullOrEmpty(accessTokenOrUserId))
            {
                // lazily generate and assume it will be unique
                accessTokenOrUserId = "";
                for (int i = 0; i < 6; i++)
                    accessTokenOrUserId += UnityEngine.Random.Range(0, 9999).ToString("0000");
            }

            PlayFabMaster.Instance.Login(loginMode, accessTokenOrUserId);
        }
        else
        {
            ShowUserIdGuiThatWillLoginAndHideOnSubmit();
        }
    }
    public void ShowUserIdGuiThatWillLoginAndHideOnSubmit()
    {
        canvas.gameObject.SetActive(true);
    }

    public void HideUserIdGui()
    {
        canvas.gameObject.SetActive(false);
    }

    public void OnSubmitUserId(InputField inputField)
    {
        PlayFabMaster.Instance.Login("anonymous", inputField.text);
    }
}
