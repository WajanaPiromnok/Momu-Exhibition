using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;
using Mirror;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using ParrelSync;
#endif

public class MainNetworkPlayer : NetworkBehaviour
{
    public static MainNetworkPlayer Main { get { return mainPlayer;  } }
    static public bool createdAsMainPlayer = false;

    [SyncVar] public string playfabIdOfThisPlayer;
    public GameObject renderingContainerObject;
    public SpriteRenderer maskSprite;
    public bool isDataApplied = false;
    public Collider geometryCollider;
    public OutlookChangeHandler outlookChangeHandler;
    public float creationTime = 0;
    public float smallScale = 0.5f;
    public float largeScale = 0.7f;

    private static MainNetworkPlayer mainPlayer = null;
    
    [SyncVar(hook = nameof(OnAvatarChanged))] public string avatarOutlookJson;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdController;
    private CharacterController charController;
    private CinemachineVirtualCamera virtualCam;
    private PlayerInput playerInput;
    private UICanvasControllerInput joystick;
    private Transform roomScreenParent;
    private SkinnedMeshRenderer skinnedMesh;
    
    private void ShowSkinnedMesh() {
        skinnedMesh.enabled =true;
    }
    
    [Command]
    private void CmdSetPlayfabIdAndKickDuplicate(string pid) {
        playfabIdOfThisPlayer = pid;
        
        Dictionary<string, MainNetworkPlayer> dict = new Dictionary<string, MainNetworkPlayer>();

        foreach(MainNetworkPlayer player in FindObjectsOfType<MainNetworkPlayer>()) {
            string playfabId = player.playfabIdOfThisPlayer;
            if(dict.ContainsKey(playfabId)) {
                MainNetworkPlayer anotherPlayer = dict[playfabId];
                if(player.creationTime <= anotherPlayer.creationTime) {
                    player.connectionToClient.Disconnect();
                } else {
                    dict[playfabId] = player;
                    anotherPlayer.connectionToClient.Disconnect();
                }
            } else {
                dict.Add(playfabId, player);
            }
        }
    }

    public override void OnStopClient(){
        if(hasAuthority)
            PlaygroundMaster.Instance.ShowBlockCanvas("You see this message becuase either of the following reasons\n- The same account tried to login twice -\n- You're disconnected due to inactivity -\n- The connected server closed connection -\n\nPlease try again, press ESC to show cursor");
    }

    public override void OnStartClient()
    {
        if(hasAuthority) {
            createdAsMainPlayer = true;
            WtfUtils.SetLayerRecursively(renderingContainerObject, LayerMask.NameToLayer("MainPlayer"));
            if (PlayFabMaster.Instance.isNewlyCreatedPlayer)
            {
                PlaygroundMaster.Instance.ShowAvatarGUI();
            }
        }
        PlaygroundMaster.Instance.HideBlockCanvas();
    }

    public override void OnStartServer()
    {
        Debug.Log("[Start] Current connections: " + NetworkServer.connections.Count.ToString());
    }

    public override void OnStopServer()
    {
        Debug.Log("[Stop ] Current connections: " + NetworkServer.connections.Count.ToString());
    }

    private void Start() {
        if(isLocalPlayer)
            gameObject.name = "Local Player";

        creationTime = Time.time;
        thirdController = GetComponent<ThirdPersonController>();
        charController = GetComponent<CharacterController>();
        virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        playerInput = GetComponent<PlayerInput>();
        joystick = FindObjectOfType<UICanvasControllerInput>();
        if(GameObject.Find("room_screens") != null)
            roomScreenParent = GameObject.Find("room_screens").transform;
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        
        if(!isLocalPlayer) {
            if(isServer) {
                Destroy(geometryCollider);
            }

            skinnedMesh.enabled = false;
            Invoke(nameof(ShowSkinnedMesh),3);
            Destroy(thirdController);
            Destroy(charController);
            Destroy(playerInput);
            Destroy(starterAssetsInputs);
            maskSprite.color = Color.white;
            return;
        }

        CmdSetPlayfabIdAndKickDuplicate(PlayFabMaster.Instance.GetPlayfabId());

        ApplyPlayfabData();
        mainPlayer = this;

        // #if UNITY_EDITOR
        // #else
        if(joystick != null)
            joystick.starterAssetsInputs = starterAssetsInputs;
        else
        {
            playerInput.SwitchCurrentControlScheme("KeyboardMouse");
        }
        #if UNITY_EDITOR
        // if(ParrelSync.ClonesManager.IsClone()){
            playerInput.SwitchCurrentControlScheme("KeyboardMouse");
        // }
        #endif
        // #endif

        // if(!PlaygroundMaster.IsLoadTestMode())
            virtualCam.Follow = transform.Find("PlayerCameraRoot");

        // if(PlaygroundMaster.IsTestMode()) 
        //     StartCoroutine(RepeatGenerateTestInput());
    }
    bool CheckInScreenRoom() {
        if(roomScreenParent == null)
            return false;

        for(int i = 0; i < roomScreenParent.childCount; i++) {
            Transform screen = roomScreenParent.GetChild(i);
            float distance = Vector3.Distance(transform.position, screen.position);
            if(distance <= 2.7) {
                return true;
            }
        }

        return false;
    }

    void FixedUpdate()
    {
        if(CheckInScreenRoom()){
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * smallScale, Time.deltaTime*3);
            CinemachineComponentBase componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
            Cinemachine3rdPersonFollow component = (componentBase as Cinemachine3rdPersonFollow);
            float currentDist = component.CameraDistance;
            component.CameraDistance = Mathf.Lerp(currentDist, 4.0f*smallScale/largeScale, Time.deltaTime*3);
            // virtualCam.m_Lens.FieldOfView = Mathf.Lerp(virtualCam.m_Lens.FieldOfView, 70, Time.deltaTime*3);
        }
        else {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * largeScale, Time.deltaTime*3);
            CinemachineComponentBase componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
            Cinemachine3rdPersonFollow component = (componentBase as Cinemachine3rdPersonFollow);
            float currentDist = component.CameraDistance;
            component.CameraDistance = Mathf.Lerp(currentDist, 4f, Time.deltaTime*3);
            // virtualCam.m_Lens.FieldOfView = Mathf.Lerp(virtualCam.m_Lens.FieldOfView, 40, Time.deltaTime*3);
        }

        if(!hasAuthority)
            return;

        if(PlayFabMaster.Instance.isLoggedIn == false)
            return;
        
        PlayFabMaster.Instance.UpdatePlayerDataBuffer(transform.position, transform.rotation, avatarOutlookJson);

        if(PlaygroundMaster.IsLoadTestMode()) {
            charController.Move(transform.forward*5*Time.deltaTime);
            this.transform.Rotate(Vector3.up*30*Time.deltaTime);
        }

        Vector3 position = transform.position;
        bool fell = position.y <= -5;
        bool inEventHall = position.z <= 27;
        bool outboundSpawnHall = !inEventHall && (position.z >= 66 || position.x <= -10 || position.x >= 10);
        bool outboundEventHall = inEventHall && (position.x <= -30 || position.x >= 30 || position.z <= -30);
        
        if(fell || outboundSpawnHall || outboundEventHall) {
            Transform spawnTrans = WtfUtils.GetRandomSpawnTransform();
            SetPosRotWithoutConflict(spawnTrans.position, spawnTrans.rotation.eulerAngles);
        }
    }

    void OnAvatarChanged(string oldJson, string newJson)
    {
        AvatarOutlook parsedOldObj = JsonUtility.FromJson<AvatarOutlook>(oldJson);
        AvatarOutlook parsedNewObj = JsonUtility.FromJson<AvatarOutlook>(newJson);
        outlookChangeHandler.OnAvatarOutlookChanged(parsedOldObj, parsedNewObj);
    }

    static public void SetMainPlayerOutlook(List<int> listOfValues){

        AvatarOutlook currentObj = JsonUtility.FromJson<AvatarOutlook>(mainPlayer.avatarOutlookJson);
        for(int i=0; i<listOfValues.Count; i++)
            currentObj.values[i] = listOfValues[i];
        mainPlayer.CmdSetOutlook(JsonUtility.ToJson(currentObj));
    }

    static public void UpdateMainPlayerOutlook(int index, int value)
    {
        AvatarOutlook currentObj = JsonUtility.FromJson<AvatarOutlook>(mainPlayer.avatarOutlookJson);
        currentObj.values[index] = value;
        mainPlayer.CmdSetOutlook(JsonUtility.ToJson(currentObj));
    }

	[Command]
	public void CmdSetOutlook(string jsonStr){
        avatarOutlookJson = jsonStr;
	}

    void ApplyPlayfabData()
    {
        if(!isLocalPlayer) return;

        PlayerLastPosition lastPos = PlayFabMaster.Instance.playerPositionData;

        if (!lastPos.IsDefault())
        {
            Vector3 targetPos = new Vector3(lastPos.pos.x, lastPos.pos.y, lastPos.pos.z);
			Vector3 targetRot = lastPos.rot;
            SetPosRotWithoutConflict(targetPos, targetRot);
        }

		CmdSetOutlook(JsonUtility.ToJson(PlayFabMaster.Instance.outlookData));
		isDataApplied = true;
    }
    void SetPosRotWithoutConflict(Vector3 newPos, Vector3 newRot)
    {
        charController.enabled = false;
        transform.position = newPos;
		transform.rotation = Quaternion.Euler(newRot);

        if(CheckInScreenRoom())
            transform.localScale = Vector3.one * 0.5f;

        if(isLocalPlayer)
            charController.enabled = true;
    }
}
