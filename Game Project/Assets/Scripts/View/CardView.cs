using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private int cardId;
    public int CardId { set { cardId = value; } }
    [SerializeField]
    private Image cardImage;
    
    public void CardColor()
    {
        switch (cardId)
        {
            case 0:
                cardImage.color = Color.red;
                break;
            case 1:
                cardImage.color = Color.blue;
                break;
            case 2:
                cardImage.color = Color.yellow;
                break;
            case 3:
                cardImage.color = Color.green;
                break;
            case 4:
                cardImage.color = Color.black;
                break;
        }
    }
}
