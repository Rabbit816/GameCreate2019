using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleControl : MonoBehaviour
{
    private Vector3 buttonDownPosR, buttonUpPosR, buttonDownPosL, buttonUpPosL;    // マウスの座標

    [SerializeField, Tooltip("ターン制限を決めるボタン")]
    private List<Button> settingButtons = new List<Button>();

    [SerializeField, Tooltip("タイトルに表示されるカードオブジェクト")]
    private GameObject cardObjL, cardObjR;
    private Vector3 objStartPosL, objStartPosR;    // オブジェクトのスタート時の座標

    private bool updateFlagL = true, updateFlagR = true;    // updateの動作を管理するフラグ
    private bool moveEndL = false, moveEndR = false;    // オブジェクトの移動の完了を検知するフラグ

    private void Awake()
    {
        objStartPosL = cardObjL.transform.localPosition;
        objStartPosR = cardObjR.transform.localPosition;
        foreach(Button button in settingButtons)
        {
            button.enabled = false;
        }
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
                    cardObjR.transform.DOLocalMove(new Vector3(Screen.width / 2 + 300, 0, 0), 1.5f).OnStart(() => { updateFlagR = false; }).OnComplete(() => { moveEndR = true; ObjMoveEnd(false); });

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
                    cardObjL.transform.DOLocalMove(new Vector3((Screen.width / 2 + 300) * -1, 0, 0), 1.5f).OnStart(() => { updateFlagL = false; }).OnComplete(() => { moveEndL = true; ObjMoveEnd(true); });
                }
            }
        }

        // オブジェクトの移動完了を検知したら実行
        if(moveEndL && moveEndR)
        {
            AllObjMoveEnd();
        }
    }

    /// <summary>
    /// ルール設定ボタンを押したら実行される処理を付与する
    /// </summary>
    public void ButtonActionSet()
    {
        for (int i = 0; i < settingButtons.Count; i++)
        {
            int num = i;
            settingButtons[i].onClick.AddListener(() => GameMaster.Instance.GameStartButton((GameMaster.GameMode)num));
            settingButtons[i].onClick.AddListener(() => Action());
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
        }
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

    private void AllObjMoveEnd()
    {
        foreach(Button button in settingButtons)
        {
            button.enabled = true;
        }
    }
}
