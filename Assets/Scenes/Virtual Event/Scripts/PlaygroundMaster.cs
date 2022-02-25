using UnityEngine;
using Mirror;
using Mirror.SimpleWeb;
using System;
using UnityEngine.UI;
using StarterAssets;
#if UNITY_EDITOR
using ParrelSync;
#endif

public class PlaygroundMaster : MonoBehaviour
{
    public MyNetworkManager network;
    public Canvas canvas;
    public Canvas blockCanvas;
    public Text blockCanvasText;
    public bool onMainEditorAsClient = false;
    public bool onEditorWithSsl = false;
    public bool onEditorRemoteHost = false;
    public bool onEditorAsClientWithHost = false;
    public bool onEditorLoadTest = false;
    public Text serverText;
    public string REMOTE_HOST_URL = "twnz.dev";
    private const string LOCAL_HOST_URL = "localhost";
    static PlaygroundMaster mInstance;
    public UICanvasControllerInput joystick;

    public static bool IsLoadTestMode() {
        if(WtfUtils.GetArg("-loadTest") == "true")
            return true;

        if(mInstance == null)
            return false;

        #if UNITY_EDITOR
            return Instance.onEditorLoadTest;
        #else
            return false;
        #endif
    }
    
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
            ShowBlockCanvas("Starting server connection");

            #if UNITY_WEBGL
                #if !UNITY_EDITOR
                    bool jsTellsMobile = WebLibCustom.IsMobileBrowser();
                    if(!jsTellsMobile)
                        Destroy(joystick.gameObject);

                    REMOTE_HOST_URL = WebLibCustom.ReadRemoteHostURL();
                #endif
            #endif
        }
        else
        {
            return;
        }
    }

    void Start()
    {
#if UNITY_EDITOR
        if(!ClonesManager.IsClone()) {
            if(onMainEditorAsClient)
                InitPlayFab();
            else
                InitNetwork();
        } else {
            InitPlayFab();
        }
#elif UNITY_SERVER
        InitNetwork();
#else
        InitPlayFab(); // init network inside
#endif
    }

    void SetAddress(string s) { network.networkAddress = s; }
    void YesSsl()
    {
        SimpleWebTransport webTransport = network.gameObject.GetComponent<SimpleWebTransport>();
        webTransport.clientUseWss = true;
        webTransport.sslEnabled = true;
    }

    void NoSsl()
    {
        SimpleWebTransport webTransport = network.gameObject.GetComponent<SimpleWebTransport>();
        webTransport.clientUseWss = false;
        webTransport.sslEnabled = false;
    }
    public void InitNetwork()
    {
#if UNITY_EDITOR
        // BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        if (onEditorRemoteHost)
            SetAddress(REMOTE_HOST_URL);
        else
            SetAddress(LOCAL_HOST_URL);

        if (onEditorWithSsl)
            YesSsl();
        else
            NoSsl();

        if (ClonesManager.IsClone())
        {
            network.StartClient(); //assume that clone is always client
        }
        else
        {
            if (onMainEditorAsClient){
                if(onEditorAsClientWithHost){
                    SetAddress(LOCAL_HOST_URL);
                    network.StartHost();
                }
                else{
                    network.StartClientConnectionToOneOfListedServers();
                }
            }
            else
                network.StartServer();
        }
#else // build as client
    YesSsl();
    SetAddress(REMOTE_HOST_URL);
#if UNITY_SERVER
    string wsPort = WtfUtils.GetArg("-port");
    string maxPlayer = WtfUtils.GetArg("-maxPlayer");
    Debug.Log(wsPort);
    Debug.Log(maxPlayer);
    Debug.Log( String.Join(" ", System.Environment.GetCommandLineArgs()));
    if(wsPort != null) {
        FindObjectOfType<SimpleWebTransport>().port = ushort.Parse(wsPort);;
        Debug.Log("using ws port " + wsPort);
    }

    if(maxPlayer != null) {
        network.maxConnections = int.Parse(maxPlayer);
        Debug.Log("max player set at " + maxPlayer);
    }

    network.StartServer();
#else
    network.StartClientConnectionToOneOfListedServers();
#endif
#endif
    }

    public void InitPlayFab()
    {
        if(onEditorLoadTest) {
            PlayFabMaster.Instance.Login("anonymous", WtfUtils.GenerateRandomDigitsStr(32));
            return;
        }

        #if UNITY_EDITOR
            string debugCookie = "yes";
        #elif UNITY_WEBGL
            string debugCookie = WebLibCustom.ReadCookies("debug");
        #else
            string debugCookie = "yes";
        #endif
        
        if(debugCookie == "yes")
        {
            if(IsLoadTestMode()) {
                PlayFabMaster.Instance.Login("anonymous", WtfUtils.GenerateRandomDigitsStr(32));
            } else {
                ShowUserIdGuiThatWillLoginAndHideOnSubmit();
            }
        }
        else 
        {
            HideUserIdGui();

            string authMode = WebLibCustom.ReadLocalStorage("authMode");
            string accessTokenOrUserId = WebLibCustom.ReadLocalStorage("userId");
               
            if(String.IsNullOrEmpty(authMode)){
                ShowBlockCanvas("Authentication has not been applied, please re-login");
                return;
            }
                     
            if (authMode == "anonymous" && String.IsNullOrEmpty(accessTokenOrUserId))
                accessTokenOrUserId = WtfUtils.GenerateRandomDigitsStr(32); // lazy generation without unique checking
            
            PlayFabMaster.Instance.Login(authMode, accessTokenOrUserId);
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

    public void ShowBlockCanvas(string text=null) {
        blockCanvas.gameObject.SetActive(true);
        if(text != null) {
            blockCanvasText.text = text;
        }
    }

    public void HideBlockCanvas() {
        blockCanvas.gameObject.SetActive(false);
    }

    public bool IsShowingBlock() {
        return blockCanvas.gameObject.activeSelf;
    }

    public void SetServerTextNumber(int i) {
        serverText.text = "Server #" + i.ToString();
    }
}
