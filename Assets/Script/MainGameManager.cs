using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Drawing;

public class MainGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] float GameTime = 120f;
    [SerializeField] Text TimerLabel;
    [SerializeField] Text CurrentRoll_Label;

    float CountTimer;
    bool gameEnd = false;
    float timerSendInterval = 0.2f;
    float timerSendCounter = 0f;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject Player2;
    [SerializeField] GameObject killer;
    bool a = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        CountTimer = GameTime;

        TrySetRoleLabel(PhotonNetwork.LocalPlayer);

       
    }

    void Update()
    {

        while (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
           
            
            Player.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.blue);
            
        }
        while(Player2 == null)
        {
            Player2 = GameObject.FindGameObjectWithTag("Player2");
            Player2.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.red);
           
        }
        while (killer == null)
        {
            killer = GameObject.FindGameObjectWithTag("Killer");
            killer.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", new Color32(0, 0, 0, 0));
        }

        //photonView.RPC(nameof(ChangeColor), RpcTarget.AllBuffered);
        if (!PhotonNetwork.IsMasterClient || gameEnd) return;

        CountTimer -= Time.deltaTime;
        if (CountTimer <= 0f)
        {
            CountTimer = 0f;
            gameEnd = true;

            // ゲーム終了同期
            Hashtable props = new Hashtable { ["GameEnd"] = true };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        // タイマー同期は間隔を開けて送る
        timerSendCounter += Time.deltaTime;
        if (timerSendCounter >= timerSendInterval)
        {
            timerSendCounter = 0f;
            Hashtable timerProp = new Hashtable { ["GameTimer"] = CountTimer };
            PhotonNetwork.CurrentRoom.SetCustomProperties(timerProp);
        }

        TimerLabel.text = $"{(int)CountTimer}";

      
    }

    private void TrySetRoleLabel(Player player)
    {
        if (player.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            string role = roleObj as string;
            if (role == "killer")
            {
                CurrentRoll_Label.text = "あなたは Killer です！";
                player.NickName = "Killer";
            }
            else if (role == "survivor")
            {
                CurrentRoll_Label.text = "あなたは Survivor です！";
                player.NickName = "Survivor";
            }
        }
        else
        {
            Debug.Log("ロール未設定 (後で反映されます)");
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // タイマー更新
        if (propertiesThatChanged.ContainsKey("GameTimer"))
        {
            CountTimer = System.Convert.ToSingle(propertiesThatChanged["GameTimer"]);
        }

        // ゲーム終了判定
        if (propertiesThatChanged.ContainsKey("GameEnd"))
        {
            gameEnd = (bool)propertiesThatChanged["GameEnd"];
            if (gameEnd)
                Debug.Log("ゲーム終了 (同期)");
        }

        TimerLabel.text = $"{(int)CountTimer}";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("Role"))
        {
            TrySetRoleLabel(targetPlayer);
        }
    }
    [PunRPC]
    void SyncColor(bool colorState)
    {
        a = colorState;
       

        Player.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.blue);
            Player2.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.red);
       
    }
    [PunRPC]
    void SyncColor2(bool colorState)
    {
        a = colorState;

            Player.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.red);
            Player2.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.blue);
        
    }
    public void playercontrol()
    {
        photonView.RPC(nameof(SyncColor), RpcTarget.All, a);
    }
    public void player2control()
    {
        photonView.RPC(nameof(SyncColor2), RpcTarget.All, a);
    }

}
