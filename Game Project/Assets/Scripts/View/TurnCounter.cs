using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : MonoBehaviour
{
    private int limitTurn;    // ゲーム終了までのターン数
    public int LimitTurn { set { limitTurn = value; } }

    [SerializeField]
    private Text text;

    // Update is called once per frame
    void Update()
    {
        if(limitTurn > 10)
        {
            text.text = "ゲーム終了まであと <size=75>" + limitTurn.ToString() + "</size> ターンです";
        }
        else if(limitTurn > 3)
        {
            text.text = "ゲーム終了まであと <size=75><color=yellow>" + limitTurn.ToString() + "</color></size> ターンです";
        }
        else
        {
            text.text = "ゲーム終了まであと <size=100><color=red>" + limitTurn.ToString() + "</color></size> ターンです";
        }
    }

    public void CounterOn()
    {
        text.gameObject.SetActive(true);
    }

    public void CounterOff()
    {
        text.gameObject.SetActive(false);
    }
}
