using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : MonoBehaviour
{
    private int limitTurn;    // ゲーム終了までのターン数
    public int LimitTurn { set { limitTurn = value; } }

    [SerializeField, Tooltip("ゲーム終了までのターン数を表示するTextオブジェクト")]
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

    /// <summary>
    /// カウンターの表示、非表示の管理
    /// </summary>
    /// <param name="b">true=表示, false=非表示</param>
    public void CounterActive(bool b)
    {
        text.gameObject.SetActive(b);
    }
}
