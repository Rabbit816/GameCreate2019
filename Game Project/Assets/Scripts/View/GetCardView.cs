using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCardView : MonoBehaviour
{
    [SerializeField]
    private List<Image> getCards;
    private int cardCounter = 0;

    /// <summary>
    /// 獲得カードリストのカードを非表示にする
    /// </summary>
    public void ResetGetCard()
    {
        cardCounter = 0;

        foreach(Image image in getCards)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }

    /// <summary>
    /// 獲得したカードをリストに表示する
    /// </summary>
    /// <param name="sprite">表示するカードのSprite</param>
    public void OutputGetCard(Sprite spriteData)
    {
        var card = getCards[cardCounter];
        card.sprite = spriteData;
        card.color = new Color(card.color.r, card.color.g, card.color.b, 1);
        cardCounter++;
    }

    /// <summary>
    /// 獲得したカードリストのオンオフ
    /// </summary>
    /// <param name="b">true=表示, false=非表示</param>
    public void GetCardListActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
