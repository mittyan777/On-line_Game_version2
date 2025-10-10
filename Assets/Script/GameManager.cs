using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private Text timerLabel;
    [SerializeField] private float selectTimer = 10f;
    private float countTimer;
    [SerializeField] private Text SurvivorCount_Label;
    [SerializeField] private Text KillerCount_Label;
    [SerializeField] private GameObject SelectButtons;


    int CurrentPlayerCount = 0;
    const int MaxPlayers = 3;
    private float ReloadInfoTimer = 1f;
    private float ReloadInfoCounter = 0f;
    private bool rolesConfirmed = false;
    private int lastSentTimer = -1;

    private void Start()
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
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom) return;

        // タイマーはマスタークライアントのみ管理
        if (PhotonNetwork.IsMasterClient)
        {
            ReloadInfoCounter -= Time.deltaTime;
            if (ReloadInfoCounter <= 0f)
            {
                ReloadInfoCounter = ReloadInfoTimer;
                DisplayRequestCounts();
            }
            if (CurrentPlayerCount != MaxPlayers)
            {
                countTimer = selectTimer;
            }
            else
            {
                countTimer -= Time.deltaTime;
                int countdown = Mathf.CeilToInt(countTimer);
                // 1秒ごとに送信
                if (countdown != lastSentTimer)
                {
                    lastSentTimer = countdown;
                    Hashtable timerProp = new Hashtable();
                    timerProp["SelectCountdown"] = countdown;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(timerProp);
                    Debug.Log($"カウントダウン: {countdown} 秒");
                }

                // 全員が準備できていたら即時開始
                if (!rolesConfirmed && (CheckAllReady() || countdown <= 0))
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "AllReady", true } });
                }

            }

        }
    }

    // RoomProperties が更新されたら全員に通知
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //投票数の更新
        if (propertiesThatChanged.ContainsKey("SurvivorRequest_Counts") &&
            propertiesThatChanged.ContainsKey("KillerRequest_Counts"))
        {
            Debug.Log("Received player request counts update.");
            int survivorCount = (int)propertiesThatChanged["SurvivorRequest_Counts"];
            int killerCount = (int)propertiesThatChanged["KillerRequest_Counts"];
            SurvivorCount_Label.text = $"サバイバー希望: {survivorCount}/2";
            KillerCount_Label.text = $"キラー希望: {killerCount}/1";
        }

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

            Hashtable props = new Hashtable
{
    { "SurvivorRequest_Counts", survivorCount },
    { "KillerRequest_Counts", killerCount }
};
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
}