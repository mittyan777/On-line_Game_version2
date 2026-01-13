using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OptionScript : MonoBehaviour
{
    [Header("設定データ(ScriptableObject)")]
    [SerializeField] private GameSettingsSO gameSettings; // ★ここに作成したSOをアタッチ

    [Header("オプション画面関連")]
    [SerializeField] GameObject Option_Window;
    [SerializeField] Slider MasterSound_Slider;

    [Space(10)]
    [Header("コントローラのフォーカス")]
    [SerializeField] GameObject Controller_Options;

    AudioScript audioScript;

    bool Opening_Options;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //シングルトンで管理するので、Findを実行する
        GameObject bgmObj = GameObject.Find("BGM");
        if (bgmObj != null)
        {
            audioScript = bgmObj.GetComponent<AudioScript>();
        }

        Close_OptionWindow();

        // ★ SOのデータをロードして適用
        gameSettings.LoadAndApply();

        // ★ UIの初期化（SOの値を入れる）
        InitializeUIValues();

        RegisterUIEvents();
    }

    /// <summary>
    /// ScriptableObjectの値をUIコンポーネントに反映させる
    /// </summary>
    private void InitializeUIValues()
    {
        MasterSound_Slider.value = gameSettings.masterVolume;

        // 音量初期反映
        if (audioScript != null)
        {
            audioScript.Set_MasterParameter(gameSettings.masterVolume);
        }
    }

    /// <summary>
    /// UI操作時のコールバック登録
    /// </summary>
    private void RegisterUIEvents()
    {
        // 音量
        MasterSound_Slider.onValueChanged.AddListener((val) =>
        {
            gameSettings.masterVolume = val;
            if (audioScript != null) audioScript.Set_MasterParameter(val);
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    //オプション画面を開いているか
    public bool Get_IsOpening_Options() { return Opening_Options; }

    private void SwitchWindow(GameObject activeWindow, GameObject focusTarget)
    {
        Option_Window.SetActive(activeWindow == Option_Window);

        if (focusTarget != null)
        {
            EventSystem.current.SetSelectedGameObject(focusTarget);
        }
    }

    /*ボタンイベント*/
    public void Option_Function()
    {
        if (Opening_Options) Close_OptionWindow();
        else Open_Options();
    }

    private void Open_Options()
    {
        Opening_Options = true;
        SwitchWindow(Option_Window, Controller_Options);
    }


    //オプション画面を閉じる
    private void Close_OptionWindow()
    {
        Opening_Options = false;

        Option_Window.SetActive(false);

        // 閉じる時にもセーブ
        gameSettings.Save();
    }

    void OnDisable()
    {
        // タイトルに戻る時など、シーン遷移時に確実にセーブする
        if (gameSettings != null) gameSettings.Save();
    }
    void OnApplicationQuit()
    {
        if (gameSettings != null) gameSettings.Save();
    }
}
