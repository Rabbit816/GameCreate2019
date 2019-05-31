using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [SerializeField]
    private int cardId;                // カードのID番号
    public int CardId { set { cardId = value; } get { return cardId; } }
    [SerializeField]
    private int cardNumber;            // カードに描かれた数字の情報
    public int CardNumber { set { cardNumber = value; } get { return cardNumber; } }
    [SerializeField]
    private string cardMark;           // カードに描かれたマークの情報
    public string CardMark { set { cardMark = value; } }
    [SerializeField]
    private Sprite cardSpriteData;     // 表示するカードのスプライト画像データ
    public Sprite CardSpriteData { set { cardSpriteData = value; } get { return cardSpriteData; } }
}
