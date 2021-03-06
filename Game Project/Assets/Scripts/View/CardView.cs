﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardView : CardData
{
    [SerializeField]
    private Image cardImage;
    private bool isCardTurning;
    public bool IsCardTurning { set { isCardTurning = value; } get { return isCardTurning; } }
    [SerializeField]
    private Vector3 cardSetPos;
    public Vector3 CardSetPos { set { cardSetPos = value; } }


    private void Update()
    {
        if(CardSpriteData != null && cardImage.sprite != CardSpriteData)
        {
            cardImage.sprite = CardSpriteData;
        }
    }

    /// <summary>
    /// カードを返す処理
    /// </summary>
    /// <param name="b">true=数字の面を表示, false=裏に戻す</param>
    public void TurnCard(bool b)
    {
        StartCoroutine(CardAction(b));
    }

    private IEnumerator CardAction(bool turnMode)
    {
        isCardTurning = true;
        float angle = 0f;
        float speed = 500f;
        bool flag = true;

        // SE再生
        GameMaster.Instance.PlaySE();

        if (turnMode)
        {
            // カードをめくる処理
            while(angle < 180)
            {
                angle += speed * Time.deltaTime;
                transform.eulerAngles = new Vector3(0, angle, 0);
                if(angle >= 90 && flag)
                {
                    NumberSprite(turnMode);
                    flag = false;
                }
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            angle = 180f;
            // めくったカードを戻す処理
            while (angle > 0)
            {
                angle -= speed * Time.deltaTime;
                transform.eulerAngles = new Vector3(0, angle, 0);
                if (angle <= 90 && flag)
                {
                    NumberSprite(turnMode);
                    flag = false;
                }
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        isCardTurning = false;
    }

    /// <summary>
    /// カードの数字イラストのオンオフ
    /// </summary>
    /// <param name="mode">true=ON, false=OFF</param>
    private void NumberSprite(bool mode)
    {
        if (mode)
        {
            cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, 1);
        }
        else
        {
            cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, 0);
        }
    }

    /// <summary>
    /// カードを指定の位置に移動させる
    /// </summary>
    public void SetCardPosition()
    {
        if(transform.localPosition != cardSetPos)
        {
            transform.DOLocalMove(cardSetPos, 0.75f).OnComplete(() => { GameMaster.Instance.Card.CardSecondMoveCount++; });
        }
        else
        {
            GameMaster.Instance.Card.CardSecondMoveCount++;
        }
    }

    /// <summary>
    /// カードの表示をゲーム開始時の状態にする
    /// </summary>
    public void ResetCard()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        NumberSprite(false);
        gameObject.SetActive(true);
    }
}
