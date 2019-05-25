using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    // カード情報
    private int cardNumber;
    public int CardNumber { set { cardNumber = value; } get { return cardNumber; } }
    private string cardMark;
    public string CardMark { set { cardMark = value; } get { return cardMark; } }
    private int cardId;
    public int CardId { set { cardId = value; } get { return cardId; } }

    private Sprite cardSprite;
    private Image cardImage;

    /// <summary>
    /// カードデータをもとにカードの表示をする
    /// </summary>
    public void OutputCard()
    {
        this.cardImage = this.gameObject.GetComponent<Image>();
        switch (cardMark)
        {
            case "スペード":
                this.cardSprite = CardControl.Instance.SpadeCards[this.cardNumber];
                break;
            case "クローバー":
                this.cardSprite = CardControl.Instance.CloverCards[this.cardNumber];
                break;
            case "ハート":
                this.cardSprite = CardControl.Instance.HeartCards[this.cardNumber];
                break;
            case "ダイヤ":
                this.cardSprite = CardControl.Instance.DiaCards[this.cardNumber];
                break;
            default:
                this.cardSprite = CardControl.Instance.CardMain;
                break;
        }
        this.cardImage.sprite = this.cardSprite;
    }
}
