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
    [SerializeField]
    private GameObject turnCounterObj;

    // Update is called once per frame
    void Update()
    {
        if(limitTurn > 10)
        {
            text.text = "<size=75><color=black>" + limitTurn.ToString() + "</color></size>";
        }
        else if(limitTurn > 3)
        {
            text.text = "<size=75><color=yellow>" + limitTurn.ToString() + "</color></size>";
        }
        else
        {
            text.text = "<size=100><color=red>" + limitTurn.ToString() + "</color></size>";
        }
    }

    public void CounterOn()
    {
        turnCounterObj.SetActive(true);
    }

    public void CounterOff()
    {
        turnCounterObj.SetActive(false);
    }
}
