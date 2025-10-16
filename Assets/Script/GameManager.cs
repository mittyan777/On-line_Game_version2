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
                rolesConfirmed = true;
                timerLabel.text = $"まもなく、スタートです";

                if (PhotonNetwork.IsMasterClient)
                {
                    Hashtable props = new Hashtable { ["RolesAssigned"] = true };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    StartCoroutine(WaitAndConfirmRoles());
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

    private System.Collections.IEnumerator WaitAndConfirmRoles()
    {
        yield return new WaitForSeconds(2f);

        // 各プレイヤーのRequestedRoleが設定済みか確認
        bool allRolesSet = false;
        while (!allRolesSet)
        {
            allRolesSet = true;
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (!p.CustomProperties.ContainsKey("RequestedRole"))
                    allRolesSet = false;
            }
            yield return null;
        }
        ConfirmRolesAndStart();
    }

    private void ConfirmRolesAndStart()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        List<Player> killerCandidates = new List<Player>();
        List<Player> survivorCandidates = new List<Player>();

        // --- 希望ロール収集 ---
        foreach (var p in players)
        {
            if (p.CustomProperties.TryGetValue("RequestedRole", out object requestedRoleObj))
            {
                string requestedRole = requestedRoleObj as string;
                if (requestedRole == "killer")
                    killerCandidates.Add(p);
                else
                    survivorCandidates.Add(p);
            }
            else
            {
                survivorCandidates.Add(p);
            }
        }

        // --- 役割決定 ---
        Player killer = killerCandidates.Count > 0
            ? killerCandidates[0]
            : players[Random.Range(0, players.Count)];

        List<Player> survivors = new List<Player>(players);
        survivors.Remove(killer);
        Player Survivor1 = survivors.Count > 0 ? survivors[Random.Range(0, survivors.Count)] : null;
        survivors.Remove(Survivor1);
        Player Survivor2 = survivors.Count > 0 ? survivors[0] : null;

        // --- ロールを付与 ---
        foreach (var p in players)
        {
            string finalRole;
            string color = "clear";

            if (p == killer)
            {
                finalRole = "killer";
            }
            else if (p == Survivor1)
            {
                finalRole = "survivor1";
                color = "red";
            }
            else if (p == Survivor2)
            {
                finalRole = "survivor2";
                color = "blue";
            }
            else
            {
                finalRole = "survivor";
            }

            Hashtable props = new Hashtable {
            { "Role", finalRole },
            { "Color", color }
        };
            p.SetCustomProperties(props);

            Debug.Log($"{p.NickName} の最終ロール: {finalRole} / 色: {color}");
        }

        StartCoroutine(LoadSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
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

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected: {cause}");
    }

}