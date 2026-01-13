using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Security;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private Text timerLabel;
    [SerializeField] private float selectTimer = 10f;
    private float countTimer;
    [SerializeField] private Text SurvivorCount_Label;
    [SerializeField] private Text KillerCount_Label;
    [SerializeField] private Text RoomNumber;
    [SerializeField] private GameObject SelectButtons;


    int CurrentPlayerCount = 0;
    const int MaxPlayers = 3;
    private float ReloadInfoTimer = 1f;
    private float ReloadInfoCounter = 0f;
    private bool rolesConfirmed = false;
    private bool isAllReadySent = false; // ★追加: 送信済みフラグ
    private int lastSentTimer = -1;

    // 通信量を減らすためのキャッシュ変数
    private int lastSentSurvivorCount = -1;
    private int lastSentKillerCount = -1;

    [SerializeField] OptionScript optionScript;
    AudioScript audioScript;

    public AudioClip BGM_File;

    private IEnumerator Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        countTimer = selectTimer;
        ReloadInfoCounter = ReloadInfoTimer;
        SupportLogger supportLogger = FindObjectOfType<SupportLogger>();

        if (supportLogger != null)
        {
            // Disable traffic statistics logging
            supportLogger.LogTrafficStats = false;

            // If you want to completely disable the SupportLogger component
            supportLogger.enabled = false;
        }

        // ★修正ポイント: ルーム情報が取得できるまで待機する
        // CurrentRoom が null ではない、かつ プロパティがロードされるのを待つ
        while (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
        {
            yield return null; // 1フレーム待つ
        }
        // 念のため、カスタムプロパティの中身が空（同期前）かもしれないので、少しだけ余裕を持たせる
        if (PhotonNetwork.CurrentRoom.CustomProperties.Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        RoomNumber.text = $"{PhotonNetwork.CurrentRoom.Name}";

        UpdateRequestUI(PhotonNetwork.CurrentRoom.CustomProperties);
        UpdatePlayerCount();

        //シングルトンで管理するので、Findを実行する
        audioScript = GameObject.Find("BGM").GetComponent<AudioScript>();
        audioScript.Change_PlayAudio(BGM_File);
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionScript.Option_Function();
        }

        // タイマーはマスタークライアントのみ管理
        if (PhotonNetwork.IsMasterClient)
        {
            ReloadInfoCounter -= Time.deltaTime;
            if (ReloadInfoCounter <= 0f)
            {
                ReloadInfoCounter = ReloadInfoTimer;
                DisplayRequestCounts();
            }
            if (CurrentPlayerCount < MaxPlayers)
            {
                countTimer = selectTimer;
            }
            else
            {
                countTimer -= Time.deltaTime;
                int countdown = Mathf.CeilToInt(countTimer);
                // 1秒ごとに送信
                if (countdown != lastSentTimer && countdown >= 0)
                {
                    lastSentTimer = countdown;
                    Hashtable timerProp = new Hashtable();
                    timerProp["SelectCountdown"] = countdown;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(timerProp);
                    Debug.Log($"カウントダウン: {countdown} 秒");
                }

                // 全員が準備できていたら即時開始
                if (!rolesConfirmed && !isAllReadySent && (CheckAllReady() || countdown <= 0))
                {
                    Debug.Log("全員準備完了、または時間切れ。ステータス更新を送信します。");
                    isAllReadySent = true; // ★一度だけ実行するためにフラグを立てる
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "AllReady", true } });
                }
            }
        }
    }

    // RoomProperties が更新されたら全員に通知
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //投票数の更新
        UpdateRequestUI(propertiesThatChanged);

        if (!rolesConfirmed)
        {
            if (propertiesThatChanged.ContainsKey("SelectCountdown"))
            {

                int countdown = (int)propertiesThatChanged["SelectCountdown"];
                timerLabel.text = $"ゲーム開始まで {countdown} 秒";
            }

            if (propertiesThatChanged.ContainsKey("AllReady"))
            {
                RollDecider rollDecider = GetComponent<RollDecider>();
                if (rollDecider != null)
                {
                    rollDecider.Disable_SelectButtons();
                }
                rolesConfirmed = true;
                timerLabel.text = "全員準備完了！";
                if (PhotonNetwork.IsMasterClient)
                {
                    Invoke(nameof(ConfirmRolesAndStart), 3f);
                }
            }
        }
    }

    private bool CheckAllReady()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("IsReady") || !(bool)p.CustomProperties["IsReady"])
                return false;
        }
        return true;
    }

    private void UpdateRequestUI(Hashtable props)
    {
        if (props.ContainsKey("SurvivorRequest_Counts") &&
                   props.ContainsKey("KillerRequest_Counts"))
        {
            Debug.Log("Received player request counts update.");
            int survivorCount = (int)props["SurvivorRequest_Counts"];
            int killerCount = (int)props["KillerRequest_Counts"];
            SurvivorCount_Label.text = $"サバイバー希望: {survivorCount}/2";
            KillerCount_Label.text = $"キラー希望: {killerCount}/1";
        }
    }

    private void ConfirmRolesAndStart()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        List<Player> killerCandidates = new List<Player>();
        List<Player> survivorCandidates = new List<Player>();

        foreach (var p in players)
        {
            if (p.CustomProperties.ContainsKey("RequestedRole"))
            {
                string requested = (string)p.CustomProperties["RequestedRole"];
                if (requested == "killer") killerCandidates.Add(p);
                else survivorCandidates.Add(p);
            }
            else
            {
                survivorCandidates.Add(p); // 未選択は Survivor
            }
        }

        // --- キラーを1人だけ決める ---
        Player killer;
        if (killerCandidates.Count >= 1)
        {
            // 候補が1人以上 → ランダムで1人をキラーにする
            killer = killerCandidates[Random.Range(0, killerCandidates.Count)];
        }
        else
        {
            // キラー希望が誰もいない場合 → 全プレイヤーからランダムで1人
            killer = players[Random.Range(0, players.Count)];
        }

        // --- 役割を設定 ---
        foreach (var p in players)
        {
            string finalRole = (p == killer) ? "killer" : "survivor";
            Hashtable props = new Hashtable { { "Role", finalRole } };
            p.SetCustomProperties(props);
            Debug.Log($"{p.NickName} のロール: {finalRole}");
        }

        // --- シーン遷移 ---
        PhotonNetwork.LoadLevel("main");
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            timerLabel.text = $"参加者を待っています {PhotonNetwork.CurrentRoom.PlayerCount}/3";
            CurrentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    private void DisplayRequestCounts()
    {
        if (PhotonNetwork.InRoom)
        {
            int survivorCount = 0;
            int killerCount = 0;

            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.CustomProperties.ContainsKey("RequestedRole"))
                {
                    string requested = (string)p.CustomProperties["RequestedRole"];
                    if (requested == "killer") killerCount++;
                    else survivorCount++;
                }
            }

            // 前回の値と同じなら通信しない（通信負荷軽減）
            if (survivorCount != lastSentSurvivorCount || killerCount != lastSentKillerCount)
            {
                lastSentSurvivorCount = survivorCount;
                lastSentKillerCount = killerCount;

                Hashtable props = new Hashtable
                {
                    { "SurvivorRequest_Counts", survivorCount },
                    { "KillerRequest_Counts", killerCount }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
    }
}