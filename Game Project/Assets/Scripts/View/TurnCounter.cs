using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : MonoBehaviour
{
    private int gameTurn;    // 現在のターン数
    public int GameTurn { set { gameTurn = value; } }

    private int limitTurn;    // ゲーム終了までのターン数
    public int LimitTurn { set { limitTurn = value; } }

    private Text text;

    private void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = gameTurn.ToString() + "  /  " + limitTurn.ToString();
    }

    public void CounterOn()
    {
        gameObject.SetActive(true);
    }

    public void CounterOff()
    {
        gameObject.SetActive(false);
    }
}
