using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int id;
    public bool isActive;
    [Space]
    public GameObject back;
    public GameObject deselectedImage;
    public GameObject selectedImage;
    
    Button navButton;
    EquipmentType type;
    ButtonSection section;
    bool secondaryBar;

    private void Start()
    {
        navButton = GetComponent<Button>();
        navButton.onClick.AddListener(EquipElement);
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (!isActive)
        {
            selectedImage.SetActive(true);
            deselectedImage.SetActive(false);
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!isActive)
        {
            selectedImage.SetActive(false);
            deselectedImage.SetActive(true);
        }
    }
    public void InitVariables(EquipmentType t, ButtonSection s, bool isSecondary)
    {
        type = t;
        section = s;
        secondaryBar = isSecondary;
    }
    public void HandleState(bool activate)
    {
        back.SetActive(activate);
        selectedImage.SetActive(activate);
        deselectedImage.SetActive(!activate);
        isActive = activate;

        if (!secondaryBar)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
    public void EquipElement()
    {
        section.ResetAllButtons();
        HandleState(true);
        MainNetworkPlayer.Main.GetComponent<OutlookChangeHandler>().AssignEquipment(type, id);
    }
}
