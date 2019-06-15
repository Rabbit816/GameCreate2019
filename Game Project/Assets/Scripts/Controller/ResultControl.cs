using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MonoBehaviour
{
    public static ResultControl Instance;

    [SerializeField]
    private GameObject thisGameObject;

    [SerializeField]
    private Text resultMainText;
    [SerializeField, Tooltip("経過ターンのテキスト")]
    private Text turnText;
    [SerializeField, Tooltip("経過時間のテキスト")]
    private Text timerText;
    [SerializeField, Tooltip("獲得カード枚数のテキスト")]
    private Text cardText;

    // ゲーム終了時の経過ターンの情報
    private int gameEndTurn;

    // ゲーム終了時の経過時間の情報
    private int minute;
    private int second;

    // ゲーム終了時の獲得したカードの枚数の情報
    private int getCardCount;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Update()
    {
        timerText.text = minute.ToString("00") + " : " + second.ToString("00");
    }

    /// <summary>
    /// リザルト情報の表示
    /// </summary>
    /// <param name="isClear"></param>
    public void OutputResult(bool isClear)
    {
        // ゲーム終了時のデータを取得
        var gameData = GameMaster.Instance;
        gameEndTurn = gameData.GameTurn;
        minute = (int)gameData.GameTime / 60;
        second = (int)gameData.GameTime % 60;
        getCardCount = gameData.GetCardCounter;

        // データをテキストに表示
        if (isClear)
        {
            resultMainText.text = "GAME CLEAR !!";
            turnText.text = gameEndTurn.ToString() + "ターン";
            cardText.text = "すべてのカードを獲得";
        }
        else
        {
            resultMainText.text = "GAME OVER";
            turnText.text = "クリアできませんでした";
            cardText.text = getCardCount.ToString() + "まい";
        }
        timerText.text = minute.ToString("00") + " : " + second.ToString("00");
        
    }

    /// <summary>
    /// ゲーム開始時に実行する処理
    /// </summary>
    public void GameStart()
    {
        thisGameObject.SetActive(false);
    }
    /// <summary>
    /// ゲーム終了時に実行する処理
    /// </summary>
    public void GameEnd()
    {
        thisGameObject.SetActive(true);
    }
}
