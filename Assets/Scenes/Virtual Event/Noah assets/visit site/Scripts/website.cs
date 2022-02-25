using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class website : MonoBehaviour
{
    public string MotionStudio;
    public string FashionBrand;

    public void motionStudio()
    {
        Application.OpenURL(MotionStudio);
    }

    public void fashionBrand()
    {
        Application.OpenURL(FashionBrand);
    }

    public void CloseThroughGameMaster()
    {
        PlaygroundMaster.Instance.HideNpcWebsite();
    }
}
