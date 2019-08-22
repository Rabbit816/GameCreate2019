using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TitleControl : MonoBehaviour
{
    private Vector3 buttonDownPosR, buttonUpPosR, buttonDownPosL, buttonUpPosL;    // マウスの座標

    [SerializeField, Tooltip("ターン制限を決めるボタン")]
    private Button[] settingButtons = new Button[4];

    [SerializeField, Tooltip("タイトルに表示されるカードオブジェクト")]
    private GameObject cardObjL, cardObjR;
    private Vector3 objStartPosL, objStartPosR;    // オブジェクトのスタート時の座標

    private bool updateFlagL = true, updateFlagR = true;    // Updateの動作を管理するフラグ
    private bool moveEndL = false, moveEndR = false;    // オブジェクトの移動の完了を検知するフラグ

    [SerializeField, Tooltip("タイトル文字のオブジェクト")]
    private GameObject titleTextObj;
    [SerializeField, Tooltip("ゲーム終了ボタン")]
    private GameObject exitButtonObj;
    [SerializeField, Tooltip("カードオブジェクトを移動させるボタン")]
    private GameObject moveButtonObjL, moveButtonObjR;
    [SerializeField, Tooltip("ガイドテキスト")]
    private Text startText, ruleText, hintText;
    [SerializeField]
    private EventTrigger[] eventTriggers;    // ルール設定ボタン用のイベントトリガー

    private void Awake()
    {
        // ゲーム実行したら初期化する要素
        objStartPosL = cardObjL.transform.localPosition;
        objStartPosR = cardObjR.transform.localPosition;
        buttonDownPosL = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        buttonUpPosL = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        buttonDownPosR = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        buttonUpPosR = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        eventTriggers = new EventTrigger[settingButtons.Length];
        ruleText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        CloseRuleText();
    }

    void Update()
    {
        if(Input.mousePosition.x > Screen.width / 2 && updateFlagR)
        {
            if (Input.GetMouseButtonDown(0))
            {
                buttonDownPosR = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                buttonUpPosR = Input.mousePosition;
                if(buttonUpPosR.x - buttonDownPosR.x > 700)
                {
                    // 初期化
                    buttonDownPosR = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    buttonUpPosR = new Vector3(Screen.width / 2, Screen.height / 2, 0);

                    // 実行する処理
                    CardObjMoveR();
                }
            }
        }

        if(Input.mousePosition.x < Screen.width / 2 && updateFlagL)
        {
            if (Input.GetMouseButtonDown(0))
            {
                buttonDownPosL = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                buttonUpPosL = Input.mousePosition;
                if(buttonDownPosL.x - buttonUpPosL.x > 700)
                {
                    // 初期化
                    buttonDownPosL = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    buttonUpPosL = new Vector3(Screen.width / 2, Screen.height / 2, 0);

                    // 処理
                    CardObjMoveL();
                }
            }
        }

        // オブジェクトの移動完了を検知したら実行
        if(moveEndL && moveEndR)
        {
            moveEndL = false;
            moveEndR = false;
            AllObjMoveEnd();
        }
    }

    /// <summary>
    /// ルール設定ボタンに処理を付与する
    /// </summary>
    public void ButtonActionSet()
    {
        for (int i = 0; i < settingButtons.Length; i++)
        {
            int num = i;
            settingButtons[i].enabled = false;
            eventTriggers[i] = settingButtons[i].GetComponent<EventTrigger>();
            settingButtons[i].onClick.AddListener(() => GameMaster.Instance.GameStartButton((GameMaster.GameMode)num));
            settingButtons[i].onClick.AddListener(() => Action());
            var eve1 = new EventTrigger.Entry();
            eve1.eventID = EventTriggerType.PointerEnter;
            eve1.callback.AddListener((data) => OutputRuleText((GameMaster.GameMode)num));
            var eve2 = new EventTrigger.Entry();
            eve2.eventID = EventTriggerType.PointerExit;
            eve2.callback.AddListener((data) => CloseRuleText());
            eventTriggers[i].triggers.Add(eve1);
            eventTriggers[i].triggers.Add(eve2);
        }
    }

    /// <summary>
    /// ルール設定ボタンを押したら実行される処理
    /// </summary>
    private void Action()
    {
        foreach(Button button in settingButtons)
        {
            button.gameObject.SetActive(false);
            button.enabled = false;
        }

        // タイトルのオブジェクトを非表示
        titleTextObj.SetActive(false);
        exitButtonObj.SetActive(false);
        ruleText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        CloseRuleText();
    }

    /// <summary>
    /// カードオブジェクトの移動が完了したらオブジェクトをもとの位置に戻して、非表示にする
    /// </summary>
    private void ObjMoveEnd(bool objType)
    {
        if (objType)
        {
            cardObjL.SetActive(false);
            cardObjL.transform.localPosition = objStartPosL;
        }
        else
        {
            cardObjR.SetActive(false);
            cardObjR.transform.localPosition = objStartPosR;
        }
    }

    /// <summary>
    /// 全てのカードオブジェクトに移動が完了したら実行
    /// </summary>
    private void AllObjMoveEnd()
    {
        hintText.gameObject.SetActive(true);
        foreach(Button button in settingButtons)
        {
            button.enabled = true;
        }
    }

    /// <summary>
    /// タイトルオブジェクトの配置をリセットする
    /// </summary>
    public void ReturnTitle()
    {
        cardObjL.SetActive(true);
        cardObjR.SetActive(true);
        titleTextObj.SetActive(true);
        exitButtonObj.SetActive(true);
        foreach(Button button in settingButtons)
        {
            button.gameObject.SetActive(true);
        }
        updateFlagL = true;
        updateFlagR = true;
        moveButtonObjL.SetActive(true);
        moveButtonObjR.SetActive(true);
        startText.gameObject.SetActive(true);
    }

    /// <summary>
    /// カードオブジェクトを左へ移動させる処理
    /// </summary>
    public void CardObjMoveL()
    {
        updateFlagL = false;
        moveButtonObjL.SetActive(false);
        moveEndL = true;
        if (moveEndR)
        {
            startText.gameObject.SetActive(false);
            ruleText.gameObject.SetActive(true);
        }
        cardObjL.transform.DOLocalMove(new Vector3((Screen.width / 2 + 300) * -1, 0, 0), 1.5f).OnComplete(() => { ObjMoveEnd(true); });
    }

    /// <summary>
    /// カードオブジェクトを右へ移動させる処理
    /// </summary>
    public void CardObjMoveR()
    {
        updateFlagR = false;
        moveButtonObjR.SetActive(false);
        moveEndR = true;
        if (moveEndL)
        {
            startText.gameObject.SetActive(false);
            ruleText.gameObject.SetActive(true);
        }
        cardObjR.transform.DOLocalMove(new Vector3(Screen.width / 2 + 300, 0, 0), 1.5f).OnComplete(() => { ObjMoveEnd(false); });
    }

    /// <summary>
    /// ゲームのルールを決めるときに表示するテキスト処理
    /// </summary>
    /// <param name="gameMode"></param>
    private void OutputRuleText(GameMaster.GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMaster.GameMode.limit50:
                ruleText.text = "<color=red><size=60>50</size></color>ターン以内にクリアせよ";
                break;
            case GameMaster.GameMode.limit75:
                ruleText.text = "<color=red><size=60>75</size></color>ターン以内にクリアせよ";
                break;
            case GameMaster.GameMode.limit100:
                ruleText.text = "<color=red><size=60>100</size></color>ターン以内にクリアせよ";
                break;
            default:
                ruleText.text = "制限ターンなしの<color=yellow><size=60>タイムアタックモード</size></color>";
                break;
        }
    }

    /// <summary>
    /// ボタンからカーソルが離れたら実行
    /// </summary>
    private void CloseRuleText()
    {
        ruleText.text = "";
    }
}
