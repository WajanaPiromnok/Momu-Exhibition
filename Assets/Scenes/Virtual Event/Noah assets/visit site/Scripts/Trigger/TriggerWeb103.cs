using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class TriggerWeb103 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (MainNetworkPlayer.Main.gameObject == other.gameObject)
        {
            PlaygroundMaster.Instance.ShowNpcWebsite103();
        }
    }

    private void OnTriggerExit(Collider exit)
    {
        if (MainNetworkPlayer.Main.gameObject == exit.gameObject)
        {
            PlaygroundMaster.Instance.HideNpcWebsite();
        }
    }
}
