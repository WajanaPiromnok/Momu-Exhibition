using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterCreation : MonoBehaviour
{
    [SerializeField] string ReferenceSaver; //This does absolutely nothing. This is only to trick GIT.

    [Header("UI References")]
    [SerializeField] Texture skinsHolder;
    public int skinIndex = 0;
    [SerializeField] GameObject headsHolder;
    public int headsIndex = 0;
    [SerializeField] GameObject topsHolder;
    public int topsIndex = 0;
    [SerializeField] GameObject pantsHolder;
    public int pantsIndex = 0;
    [SerializeField] GameObject shoesHolder;
    public int shoesIndex = 0;

    [Header("Sprite Resources")]
    [SerializeField] List<Texture> skins;
    [SerializeField] List<GameObject> heads;
    [SerializeField] List<GameObject> tops;
    [SerializeField] List<GameObject> pants;
    [SerializeField] List<GameObject> shoes;

    internal void LoadData(PlayerData playerData)
    {
        skinIndex = playerData.character.skin;
        skinsHolder = skins[skinIndex];

        headsIndex = playerData.character.head;
        headsHolder = heads[headsIndex];

        topsIndex = playerData.character.top;
        topsHolder = tops[topsIndex];

        pantsIndex = playerData.character.pants;
        pantsHolder = pants[pantsIndex];

        shoesIndex = playerData.character.shoes;
        shoesHolder = shoes[shoesIndex];
    }

    public void CycleSkinForward(bool status)
    {
        //Instantiate
        if(status)
        {
            if (skinIndex == skins.Count - 1)
                skinIndex = -1;

            if (skinIndex < skins.Count - 1)
            {
                skinIndex++;
                skinsHolder = skins[skinIndex];
            }
        }
        else
        {
            if (skinIndex == 0)
                skinIndex = skins.Count;

            if (skinIndex > 0)
            {
                skinIndex--;
                skinsHolder = skins[skinIndex];
            }
        }
    }

    public void CycleHeadForward(bool status)
    {
        if (status)
        {
            if (headsIndex == heads.Count - 1)
                headsIndex = -1;

            if (headsIndex < heads.Count - 1)
            {
                headsIndex++;
                headsHolder = heads[headsIndex];
            }
        }
        else
        {
            if (headsIndex == 0)
                headsIndex = heads.Count;

            if (headsIndex > 0)
            {
                headsIndex--;
                headsHolder = heads[headsIndex];
            }
        }
    }

    public void CycleTopForward(bool status)
    {
        if (status)
        {
            if (topsIndex == tops.Count - 1)
                topsIndex = -1;

            if (topsIndex < tops.Count - 1)
            {
                topsIndex++;
                topsHolder = tops[topsIndex];
            }
        }
        else
        {
            if (topsIndex == 0)
                topsIndex = tops.Count;

            if (topsIndex > 0)
            {
                topsIndex--;
                topsHolder = tops[topsIndex];
            }
            Debug.Log(topsIndex.ToString());
        }
    }

    public void CyclePantsForward(bool status)
    {
        if (status)
        {
            if (pantsIndex == pants.Count - 1)
                pantsIndex = -1;

            if (pantsIndex < pants.Count - 1)
            {
                pantsIndex++;
                pantsHolder = pants[pantsIndex];
            }
        }
        else
        {
            if (pantsIndex == 0)
                pantsIndex = pants.Count;

            if (pantsIndex > 0)
            {
                pantsIndex--;
                pantsHolder = pants[pantsIndex];
            }
        }
    }

    public void CycleShoesForward(bool status)
    {
        if (status)
        {
            if (shoesIndex == shoes.Count - 1)
                shoesIndex = -1;

            if (shoesIndex < shoes.Count - 1)
            {
                shoesIndex++;
                shoesHolder = shoes[shoesIndex];
            }
        }
        else
        {
            if (shoesIndex == 0)
                shoesIndex = shoes.Count;

            if (shoesIndex > 0)
            {
                shoesIndex--;
                shoesHolder = shoes[shoesIndex];
            }
        }
    }

    public void CreateCharacter()
    {
        GameManager.instance.CreateNewPlayerData(new Character(skinIndex, headsIndex, topsIndex, pantsIndex, shoesIndex));
    }

    public void ClearData()
    {
        if (GameManager.instance.playerData != null)
            GameManager.instance.playerData = null;

#if UNITY_EDITOR && UNITY_STANDALONE
        if(SaveLoad.DoesPlayerDataExist()) SaveLoad.RemoveSavedData();
#elif UNITY_WEBGL
        if (SaveLoad.DoesPlayerPrefsExist()) SaveLoad.RemoveSavedPlayerPrefs();
#endif
        SceneManager.LoadScene(0);
    }
}
