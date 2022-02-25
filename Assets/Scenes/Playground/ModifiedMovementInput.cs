
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class ModifiedMovementInput : NetworkBehaviour
{

    public float Velocity;
    [Space]

    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float triggerSpeed = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isDataApplied = false;
    private static ModifiedMovementInput mainPlayer = null;
    public static ModifiedMovementInput Main { get { return mainPlayer; } }

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;
    public float verticalVel;
    private Vector3 moveVector;
    public bool clickToResetPos = false;

	[SyncVar] public int baseModel=0;
	[SyncVar] public int top=0;
	[SyncVar] public int body=0;
	[SyncVar] public int bottom=0;

    void Start()
    {
        cam = Camera.main;
        anim = this.GetComponent<Animator>();
        controller = this.GetComponent<CharacterController>();

        if (isLocalPlayer)
        {
            ApplyData();
            ThirdPersonCamSet cam = FindObjectOfType<ThirdPersonCamSet>();
            cam.SetTarget(this.transform);
            mainPlayer = this;
        }
    }

	[Command]
	public void CmdSetOutlook(int _baseModel, int _top, int _body, int _bottom){
		baseModel = _baseModel;
		top = _top;
		body = _body;
		bottom = _bottom;
	}

    void ApplyData()
    {
		Debug.Log("ApplyData");
        PlayerLastPosition lastPos = PlayFabMaster.Instance.playerPositionData;
		Debug.Log(lastPos.IsDefault());

        if (!lastPos.IsDefault())
        {
            Vector3 targetPos = new Vector3(lastPos.x, 5, lastPos.z);
			Vector3 targetRot = new Vector3(lastPos.rx, lastPos.ry, lastPos.rz);
            SetPositionRotation(targetPos, targetRot);
        }

		AvatarOutlook outlook = PlayFabMaster.Instance.outlookData;
		CmdSetOutlook(outlook.baseModel, outlook.top, outlook.body, outlook.bottom);
		isDataApplied = true;
        //need to apply avatar too
    }
    void SetPositionRotation(Vector3 newPos, Vector3 newRot)
    {
        controller.enabled = false;
        transform.position = newPos;
		transform.rotation = Quaternion.Euler(newRot);
		Debug.Log("SetPositionRotation");
        controller.enabled = true;
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (!PlayFabMaster.Instance.isLoggedIn) return;

        if (clickToResetPos == true)
        {
            clickToResetPos = false;
            SetPositionRotation(new Vector3(0, 5, 0), Vector3.zero);
        }

        if (CheckSpeed())
        {
            PlayerMoveAndRotation();
            anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
        }

        FallWithAcceleration();

        PlayFabMaster.Instance.SetLastPlayerPositionForUpdateIfLoggedIn(transform.position, transform.rotation, new AvatarOutlook(baseModel, top, body, bottom));
    }
    void FallWithAcceleration()
    {
        verticalVel = controller.isGrounded ? 0 : verticalVel - 1;
        float ySpeed = verticalVel * 0.2f * Time.deltaTime;
        controller.Move(new Vector3(0, ySpeed, 0));
    }

    bool CheckSpeed()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;
        return Speed > triggerSpeed;
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

	static public void ChangeOutlookValues(InputField iBaseModel, InputField iTop, InputField iBody, InputField iBottom){
		ModifiedMovementInput.mainPlayer.CmdSetOutlook(int.Parse(iBaseModel.text), int.Parse(iTop.text), int.Parse(iBody.text), int.Parse(iBottom.text));
	}
}
