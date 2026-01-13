using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game/Game Settings Data")]
public class GameSettingsSO : ScriptableObject
{
    [Header("デフォルト設定")]
    public float masterVolume = 1.0f;

    // セーブ用キー
    private const string KEY_VOLUME = "KEY_VOLUME";

    /// <summary>
    /// ゲーム起動時やオプション開始時にロードして適用する
    /// </summary>
    public void LoadAndApply()
    {
        masterVolume = PlayerPrefs.GetFloat(KEY_VOLUME, 1.0f);
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetFloat(KEY_VOLUME, masterVolume);
        PlayerPrefs.Save();
    }
}