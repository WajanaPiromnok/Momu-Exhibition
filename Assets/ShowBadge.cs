using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBadge : MonoBehaviour
{
    public Image image;
    public RectTransform rectTransform;
    private bool hiding = true;
    // Start is called before the first frame update
    private void Update() {
        if(!hiding){
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(0, 0), Time.deltaTime*2);
            image.color = new Color(1,1,1,Mathf.Lerp(image.color.a, 1, Time.deltaTime*2));
        } else {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(-rectTransform.sizeDelta.x, 0), Time.deltaTime*2);
            image.color = new Color(1,1,1,Mathf.Lerp(image.color.a, 0, Time.deltaTime*2));
        }
    }
    public void SetSpriteAndShow(Sprite sprite) {
        if(sprite == null) {
            Hide();
            return;
        }
        hiding = false;
        image.sprite = sprite;
    }

    public void Hide() {
        hiding = true;
    }
}
