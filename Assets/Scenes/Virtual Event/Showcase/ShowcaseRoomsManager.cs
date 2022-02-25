using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowcaseRoomsManager : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Transform camPivot;
    public Text labelText;
    // public Transform camTarget;
    private GameObject roomsParent;
    private List<Transform> rooms = new List<Transform>();
    private int currentRoomIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        roomsParent = GameObject.Find("room_screens");
        if(roomsParent != null) {
            for(int i = 0; i < roomsParent.transform.childCount; i++) {
                rooms.Add(roomsParent.transform.GetChild(i));
            }
        }
        ApplyNewIndex();
    }

    // Update is called once per frame
    public void Next() {
        currentRoomIndex++;
        currentRoomIndex %= rooms.Count;
        ApplyNewIndex();
    }

    public void Prev() {
        currentRoomIndex--;
        currentRoomIndex = currentRoomIndex < 0 ? currentRoomIndex + rooms.Count : currentRoomIndex;
        ApplyNewIndex();
    }

    private void ApplyNewIndex() {
        camPivot.transform.position = rooms[currentRoomIndex].position;
        labelText.text = rooms[currentRoomIndex].gameObject.name;
    }
}
