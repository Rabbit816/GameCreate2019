using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : CardData
{
    private Image cardImage;

    private void Awake()
    {
        cardImage = gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// カードを表示する処理
    /// </summary>
    public void OutputData()
    {
        cardImage.sprite = CardSpriteData;
    }

    /// <summary>
    /// カードを裏返す処理
    /// </summary>
    public void ReturnCard()
    {
        cardImage.sprite = MainSpriteData;
    }

    /// <summary>
    /// カードを非表示にする処理
    /// </summary>
    public void RemoveCard()
    {
        gameObject.SetActive(false);
    }
}
