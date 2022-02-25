using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentType
{
    SkinTone = 0,
    Hairstyle = 1,
    HairColor = 2,
    TopStyle = 3,
    TopColor = 4,
    PantStyle = 5,
    PantColor = 6,
    ShoeStyle = 7,
    ShoeColor = 8
}

public class OutlookChangeHandler : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinsHolder, faceHolder;
    [SerializeField] SkinnedMeshRenderer hair1Holder, hair2Holder, hair3Holder, hair4Holder, hair5Holder;
    [SerializeField] SkinnedMeshRenderer top1Holder, top2Holder, top3Holder;
    [SerializeField] SkinnedMeshRenderer pant1Holder, pant2Holder, pant3Holder;
    [SerializeField] SkinnedMeshRenderer shoe1Holder, shoe2Holder, shoe3Holder, shoe4Holder;

    public List<Material> skins;
    public List<Material> hairColorImages, topColorImages, pantColorImages, shoeColorImages;
    public List<GameObject> hairs, shirts, pants, shoes;
    public GameObject hair, shirt, pant, shoe;
    public Material mainMat;
    public Material hairMat;
    public Animator playerAnimator;

    public int skinIndex, hairIndex, hairColorIndex, topIndex, topColorIndex, pantIndex, pantColorIndex, shoeIndex, shoeColorIndex;
    
    public void AssignSkin(int id)
    {
        if(id >= skins.Count) return;
        skinIndex = id;
        skinsHolder.material = skins[skinIndex];
        faceHolder.material = skins[skinIndex];
    }
    public void AssignHair(int id)
    {
        if(id >= hairs.Count) return;
        hairIndex = id;
        foreach (var hair in hairs)
            hair.SetActive(false);
            
        hairs[hairIndex].SetActive(true);
    }

    public void AssignHairColor(int id)
    {
        if (id >= hairColorImages.Count) return;
        hairColorIndex = id;
        hair1Holder.material = hairColorImages[hairColorIndex];
        hair2Holder.material = hairColorImages[hairColorIndex];
        hair3Holder.material = hairColorImages[hairColorIndex];
        hair4Holder.material = hairColorImages[hairColorIndex];
        hair5Holder.material = hairColorImages[hairColorIndex];
    }

    public void AssignShirt(int id)
    {
        if(id >= shirts.Count) return;
        topIndex = id;
        foreach (var shirt in shirts)
            shirt.SetActive(false);
            
        shirts[topIndex].SetActive(true);
    }

    public void AssignShirtColor(int id)
    {
        if (id >= topColorImages.Count) return;
        topColorIndex = id;
        top1Holder.material = topColorImages[topColorIndex];
        top2Holder.material = topColorImages[topColorIndex];
        top3Holder.material = topColorImages[topColorIndex];
    }

    public void AssignPant(int id)
    {
        if(id >= pants.Count) return;
        pantIndex = id;
        foreach (var pant in pants)
            pant.SetActive(false);
        pants[pantIndex].SetActive(true);
    }

    public void AssignPantColor(int id)
    {
        if (id >= pantColorImages.Count) return;
        pantColorIndex = id;
        pant1Holder.material = pantColorImages[pantColorIndex];
        pant2Holder.material = pantColorImages[pantColorIndex];
        pant3Holder.material = pantColorImages[pantColorIndex];
    }

    public void AssignShoes(int id)
    {
        if(id >= shoes.Count) return;
        shoeIndex = id;
        foreach (var shoe in shoes)
            shoe.SetActive(false);
        shoes[shoeIndex].SetActive(true);
    }

    public void AssignShoeColor(int id)
    {
        if (id >= shoeColorImages.Count) return;
        shoeColorIndex = id;
        shoe1Holder.material = shoeColorImages[shoeColorIndex];
        shoe2Holder.material = shoeColorImages[shoeColorIndex];
        shoe3Holder.material = shoeColorImages[shoeColorIndex];
        shoe4Holder.material = shoeColorImages[shoeColorIndex];
    }

    public void AssignSkinMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(0, id);
    }
    public void AssignHairMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(1, id);
    }
    public void AssignHairColorMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(2, id);
    }

    public void AssignShirtMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(3, id);
    }

    public void AssignShirtColorMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(4, id);
    }


    public void AssignPantMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(5, id);
    }

    public void AssignPantColorMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(6, id);
    }

    public void AssignShoesMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(7, id);
    }

    public void AssignShoesColorMultiPlayer(int id)
    {
        MainNetworkPlayer.UpdateMainPlayerOutlook(8, id);
    }


    public int GetCurrentEquipment(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.SkinTone:
                return skinIndex;
            case EquipmentType.Hairstyle:
                return hairIndex;
            case EquipmentType.HairColor:
                return hairColorIndex;
            case EquipmentType.TopStyle:
                return topIndex;
            case EquipmentType.TopColor:
                return topColorIndex;
            case EquipmentType.PantStyle:
                return pantIndex;
            case EquipmentType.PantColor:
                return pantColorIndex;
            case EquipmentType.ShoeStyle:
                return shoeIndex;
            case EquipmentType.ShoeColor:
                return shoeColorIndex;
            default:
                return skinIndex;
        }
    }

    public void AssignEquipment(EquipmentType type, int id)
    {
        if (type == EquipmentType.Hairstyle || type == EquipmentType.HairColor)
            playerAnimator.SetTrigger("LookAt");

        switch (type)
        {
            case EquipmentType.SkinTone:
                AssignSkinMultiPlayer(id);
                break;
            case EquipmentType.Hairstyle:
                AssignHairMultiPlayer(id);
                break;
            case EquipmentType.HairColor:
                AssignHairColorMultiPlayer(id);
                break;
            case EquipmentType.TopStyle:
                AssignShirtMultiPlayer(id);
                break;
            case EquipmentType.TopColor:
                AssignShirtColorMultiPlayer(id);
                break;
            case EquipmentType.PantStyle:
                AssignPantMultiPlayer(id);
                break;
            case EquipmentType.PantColor:
                AssignPantColorMultiPlayer(id);
                break;
            case EquipmentType.ShoeStyle:
                AssignShoesMultiPlayer(id);
                break;
            case EquipmentType.ShoeColor:
                AssignShoesColorMultiPlayer(id);
                break;
        }
    }

    public void OnAvatarOutlookChanged(AvatarOutlook oldOutlook, AvatarOutlook newOutlook) {
        AssignSkin(newOutlook.values[0]);
        AssignHair(newOutlook.values[1]);
        AssignHairColor(newOutlook.values[2]);
        AssignShirt(newOutlook.values[3]);
        AssignShirtColor(newOutlook.values[4]);
        AssignPant(newOutlook.values[5]);
        AssignPantColor(newOutlook.values[6]);
        AssignShoes(newOutlook.values[7]);
        AssignShoeColor(newOutlook.values[8]);
    }
}
