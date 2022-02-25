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
    public Canvas loginGui;
    public Canvas blockCanvas;
    public Text blockCanvasText;
    public ShowBadge showBadge;
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

    public GameObject customizeCharacterUI;
    public Dialogue npcDialogue;
    public website npcWebsite101;
    public website npcWebsite102;
    public website npcWebsite103;
    public website npcWebsite104;
    public website npcWebsite201;
    public website npcWebsite202;
    public website npcWebsite203;
    public website npcWebsite301;
    public website npcWebsite302;
    public website npcWebsite303;
    public website npcWebsiteSponsored;


    public static bool IsLoadTestMode() {
        if(WtfUtils.GetArg("-loadTest") == "true")
            return true;

        if(mInstance == null)
            return false;

        if(WtfUtils.IsEditor())
            return Instance.onEditorLoadTest;
        else
            return false;
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
        if(PlaygroundMaster.mInstance != null)
            return;

        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        mInstance = this;
        DontDestroyOnLoad(this);
        ShowBlockCanvas("Starting server connection");

        if(!WtfUtils.IsEditor() && (WtfUtils.IsWebGL() || WtfUtils.IsOSX() || WtfUtils.IsWindows())) {
            // if(!WebLibCustom.IsMobileBrowser())
            Destroy(joystick.gameObject);
        }
    }

    void Start()
    {
        InvokeRepeating(nameof(CheckAndSetCursorLockAllowance), 0, 0.5f);

        if(WtfUtils.IsEditor()) {
            #if UNITY_EDITOR
            if(!ClonesManager.IsClone()) {
                if(onMainEditorAsClient)
                    InitPlayFab();
                else
                    InitNetwork();
            } else {
                InitPlayFab();
            }
            #endif
        } else if(WtfUtils.IsServerBuild()) {
            InitNetwork();
        } else {
            InitPlayFab(); // init network inside after login success
        }
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
        Debug.Log("Initializing network");
        if(WtfUtils.IsEditor()) {
            #if UNITY_EDITOR
            if (onEditorRemoteHost)
                SetAddress(REMOTE_HOST_URL);
            else
                SetAddress(LOCAL_HOST_URL);

            if (onEditorWithSsl)
                YesSsl();
            else
                NoSsl();

            if (ClonesManager.IsClone())
                network.StartClient(); //assume that clone is always client
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
            #endif
        } else { // build as client
            YesSsl();
            SetAddress(REMOTE_HOST_URL);
            if(WtfUtils.IsServerBuild()) {
                Debug.Log("Initializing as server");
                Debug.Log("Args = " + String.Join(" ", System.Environment.GetCommandLineArgs()));
                string wsPort = WtfUtils.GetArg("-port");
                string maxPlayer = WtfUtils.GetArg("-maxPlayer");
                Debug.Log("WS Port = " + wsPort);
                Debug.Log("Max Player = " + maxPlayer);
                if(wsPort != null)
                    FindObjectOfType<SimpleWebTransport>().port = ushort.Parse(wsPort);

                if(maxPlayer != null)
                    network.maxConnections = int.Parse(maxPlayer);

                network.StartServer();
            } else {
                network.StartClientConnectionToOneOfListedServers();
            }
        }
    }

    public void InitPlayFab()
    {
        if(!WtfUtils.IsEditor() && !WtfUtils.IsWebGL()) {
            HideLoginGui();
            PlayFabMaster.Instance.Login("anonymous", WtfUtils.GenerateRandomDigitsStr(32));
            return;
        }

        string debugCookie = "";

        if(WtfUtils.IsEditor())
            debugCookie = "yes";
        if(!WtfUtils.IsEditor() && WtfUtils.IsWebGL())
            debugCookie = WebLibCustom.ReadCookies("debug");
        
        if(debugCookie == "yes")
            ShowLoginIdGuiWhichHideOnSubmit();
        else 
        {
            HideLoginGui();

            string authMode = WebLibCustom.ReadLocalStorage("authMode");
            string accessTokenOrUserId = WebLibCustom.ReadLocalStorage("userId");
               
            if(String.IsNullOrEmpty(authMode)){
                ShowBlockCanvas("Authentication has not been applied, please re-login. Press ESC to show cursor.");
                return;
            }
                     
            if (authMode == "anonymous" && String.IsNullOrEmpty(accessTokenOrUserId))
                accessTokenOrUserId = WtfUtils.GenerateRandomDigitsStr(32); // lazy generation without unique checking
            
            PlayFabMaster.Instance.Login(authMode, accessTokenOrUserId);
        }
    }

    public void ShowLoginIdGuiWhichHideOnSubmit()
    {
        loginGui.gameObject.SetActive(true);
    }

    public void HideLoginGui()
    {
        loginGui.gameObject.SetActive(false);
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

    public bool IsShowingAvatarGUI() {
        return customizeCharacterUI.activeSelf;
    }

    public void ShowAvatarGUI() {
        customizeCharacterUI.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void HideAvatarGUI() {
        customizeCharacterUI.SetActive(false);
    }

    private bool IsAnyInteractableGuiActive() {
        return
            customizeCharacterUI.activeSelf ||
            npcDialogue.gameObject.activeSelf ||
            npcWebsite101.gameObject.activeSelf ||
            npcWebsite102.gameObject.activeSelf ||
            npcWebsite103.gameObject.activeSelf ||
            npcWebsite104.gameObject.activeSelf ||
            npcWebsite201.gameObject.activeSelf ||
            npcWebsite202.gameObject.activeSelf ||
            npcWebsite203.gameObject.activeSelf ||
            npcWebsite301.gameObject.activeSelf ||
            npcWebsite302.gameObject.activeSelf ||
            npcWebsite303.gameObject.activeSelf ||
            npcWebsiteSponsored.gameObject.activeSelf;
    }
    private void CheckAndSetCursorLockAllowance() {
        if(WtfUtils.IsEditor())
            return;

        if(IsAnyInteractableGuiActive()) {
            // ban cursor locking
            // unlock cursor if currently locked
            if(WtfUtils.IsWebGL()) {
                WebLibCustom.DisallowCursorLock();
                WebLibCustom.UnlockCursor();
            }
        } else {
            // allow cursor locking
            if(WtfUtils.IsWebGL())
                WebLibCustom.AllowCursorLock();
        }
    }

    public void ShowNpcDialogue() {
        npcDialogue.gameObject.SetActive(true);
        npcDialogue.Reset();
        npcDialogue.NextSentences();
        WebLibCustom.UnlockCursor();
    }

    public void HideNpcDialogue() {
        npcDialogue.gameObject.SetActive(false);
        npcDialogue.Reset();
    }

    public void ShowNpcWebsite101() {
        npcWebsite101.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite102()
    {
        npcWebsite102.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite103()
    {
        npcWebsite103.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite104()
    {
        npcWebsite104.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite201()
    {
        npcWebsite201.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite202()
    {
        npcWebsite202.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite203()
    {
        npcWebsite203.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite301()
    {
        npcWebsite301.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite302()
    {
        npcWebsite302.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsite303()
    {
        npcWebsite303.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void ShowNpcWebsiteSponsored()
    {
        npcWebsiteSponsored.gameObject.SetActive(true);
        WebLibCustom.UnlockCursor();
    }

    public void HideNpcWebsite() {
        npcWebsite101.gameObject.SetActive(false);
        npcWebsite102.gameObject.SetActive(false);
        npcWebsite103.gameObject.SetActive(false);
        npcWebsite104.gameObject.SetActive(false);
        npcWebsite201.gameObject.SetActive(false);
        npcWebsite202.gameObject.SetActive(false);
        npcWebsite203.gameObject.SetActive(false);
        npcWebsite301.gameObject.SetActive(false);
        npcWebsite302.gameObject.SetActive(false);
        npcWebsite303.gameObject.SetActive(false);
        npcWebsiteSponsored.gameObject.SetActive(false);
    }

    public bool IsShowingNpcDialogue() {
        return npcDialogue.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite101() {
        return npcWebsite101.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite102()
    {
        return npcWebsite102.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite103()
    {
        return npcWebsite103.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite104()
    {
        return npcWebsite104.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite201()
    {
        return npcWebsite201.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite202()
    {
        return npcWebsite202.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite203()
    {
        return npcWebsite203.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite301()
    {
        return npcWebsite301.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite302()
    {
        return npcWebsite302.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsite303()
    {
        return npcWebsite303.gameObject.activeSelf;
    }

    public bool IsShowingNpcWebsiteSponsored()
    {
        return npcWebsiteSponsored.gameObject.activeSelf;
    }
}
