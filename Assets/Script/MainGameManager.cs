using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MainGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] float GameTime = 120f;
    [SerializeField] Text TimerLabel;
    [SerializeField] Text CurrentRoll_Label;

    float CountTimer;
    bool gameEnd = false;
    float timerSendInterval = 1.0f;
    float timerSendCounter = 0f;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        CountTimer = GameTime;
    }

    void Update()
    {
        if (gameEnd) return;

        if (PhotonNetwork.IsMasterClient)
        {
            CountTimer -= Time.deltaTime;
            if (CountTimer <= 0f)
            {
                CountTimer = 0f;
                gameEnd = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { ["GameEnd"] = true });
            }

            timerSendCounter += Time.deltaTime;
            if (timerSendCounter >= timerSendInterval)
            {
                timerSendCounter = 0f;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { ["GameTimer"] = CountTimer });
            }
        }

        if (TimerLabel != null)
            TimerLabel.text = $"{(int)CountTimer}";
    }

    [PunRPC]
    public string TrySetRoleLabel(Player player)
    {
        if (player == null || !player.CustomProperties.ContainsKey("Role"))
        {
            Debug.Log("ロール未設定 (後で反映されます)");
            return "";
        }

        string role = (string)player.CustomProperties["Role"];

        // --- 色プロパティ設定 ---
        string color = "clear";
        if (role == "killer") color = "clear";
        else if (role == "survivor1") color = "blue";
        else if (role == "survivor2") color = "red";

        player.SetCustomProperties(new Hashtable { ["Color"] = color });

        // --- 自分だけUI更新 ---
        if (player == PhotonNetwork.LocalPlayer && CurrentRoll_Label != null)
        {
            CurrentRoll_Label.text = $"あなたは {role} です！";
        }

        return role;
    }

    //色変更（ローカル）
    public void ChangeColor(string newColor)
    {
        var player = PhotonNetwork.LocalPlayer;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["Color"] = newColor;
        player.SetCustomProperties(hash);
    }

    public string Get_ColorType(Player player)
    {
        if (player.CustomProperties.TryGetValue("Color", out object color))
            return color as string;
        return null;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameTimer"))
        {
            CountTimer = System.Convert.ToSingle(propertiesThatChanged["GameTimer"]);
        }

        if (propertiesThatChanged.ContainsKey("GameEnd"))
        {
            gameEnd = (bool)propertiesThatChanged["GameEnd"];
            if (gameEnd)
                Debug.Log("ゲーム終了 (同期)");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("Role"))
        {
            TrySetRoleLabel(targetPlayer);
        }
    }
}
