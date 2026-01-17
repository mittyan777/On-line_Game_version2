using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class MainGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] float GameTime = 120f;
    [SerializeField] TextMeshProUGUI TimerLabel;

    [SerializeField] public bool GameStart_trigger = false;

    float CountTimer;
    bool gameEnd = false;
    float timerSendInterval = 0.2f;
    float timerSendCounter = 0f;

    public int sabaiba_count = 0;
    public int killer_count = 0;

    [Header("キャラクター設定")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Player2;
    [SerializeField] GameObject killer;
    [SerializeField] GameObject dummy_Player;
    [SerializeField] GameObject dummy_Player_switch;
    [SerializeField] GameObject dummy_Player_switch2;
    [SerializeField] drawn[] Drone_Objects;
    [SerializeField] GameObject Direction_right;
    [SerializeField] GameObject[] kakuho_UI;
    [SerializeField] TextMeshProUGUI[] memo;

    [SerializeField] GameObject[] CollarImage;

    [SerializeField] GameObject effect;

    [SerializeField] GameObject jail_doa;

    [SerializeField] public bool blue = false;
    [SerializeField] public bool red = false;
    public bool Gamestart = false;
    bool isLeaving = false;
    public bool hasCalledGameOver = false;

    int Game_completer;
    public static string[] Game_completer_name;
    static public string[] Player_GameObject_name;
    int i = 0;
    [SerializeField] int DesPlayer;
    static public string[] Player_name;


    [SerializeField] GameObject[] face_camera;

    [SerializeField] OptionScript optionScript;
    AudioScript audioScript;

    public AudioClip BGM_File;

    bool Start_BGM＿trigger = false;
    [SerializeField] public GameObject optionwindow;
    [SerializeField] GameObject cursor_control;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        CountTimer = GameTime;

        TrySetRoleLabel(PhotonNetwork.LocalPlayer);

        Cursor.lockState = CursorLockMode.None;

    }
    void Awake()
    {
        Game_completer_name = new string[2]; // クリア人数分
        Player_GameObject_name = new string[2];
    }

    void Update()
    {
        if (Gamestart == false) return;
        if (Start_BGM＿trigger == false)
        {
            //シングルトンで管理するので、Findを実行する
            audioScript = GameObject.Find("BGM").GetComponent<AudioScript>();
            audioScript.Change_PlayAudio(BGM_File);
            Start_BGM＿trigger = true;
        }

        if (Player_GameObject_name[0] == null)
        {
            Player_GameObject_name[0] = GameObject.FindWithTag("Player").name;
        }
        if (Player_GameObject_name[1] == null)
        {
            Player_GameObject_name[1] = GameObject.FindWithTag("Player2").name;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionScript.Option_Function();
        }

        while (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            //Null発生
            Player.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.white);
            Player.transform.position = new Vector3(210f, 2f, -31f);

        }
        while (Player2 == null)
        {
            Player2 = GameObject.FindGameObjectWithTag("Player2");
            //Null発生
            Player2.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.white);
            Player2.transform.position = new Vector3(210f, 2f, -31f);
        }
        while (killer == null)
        {
            killer = GameObject.FindGameObjectWithTag("Killer");
            killer.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", new Color32(0, 0, 0, 0));
            killer.transform.position = new Vector3(246f, 2f, -31f);
        }
        while (dummy_Player == null)
        {
            dummy_Player = GameObject.FindGameObjectWithTag("dummy_Player");
            //Null発生
            dummy_Player.GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.white);
        }

        Debug.Log(Game_completer_name[0]);
        Debug.Log(Game_completer_name[1]);

        if (killer != null)
        {

            Direction_right.transform.position = killer.transform.position;
        }

        //アウトライン、コライダー設定
        if (blue == true && red == false)
        {
            CollarImage[0].SetActive(true);
            CollarImage[1].SetActive(false);
            CollarImage[2].SetActive(false);
            CollarImage[3].SetActive(false);

            GameObject[] blues = GameObject.FindGameObjectsWithTag("blue");
            GameObject[] reds = GameObject.FindGameObjectsWithTag("red");
            GameObject[] purple = GameObject.FindGameObjectsWithTag("purple");
            GameObject[] white = GameObject.FindGameObjectsWithTag("white");
            Collider killer = GameObject.FindGameObjectWithTag("Killer").GetComponent<Collider>();
            Collider playerCol = Player.GetComponent<Collider>();
            Collider player2Col = Player2.GetComponent<Collider>();

            // Player と Blue/Red の衝突を無効化
            foreach (var b in blues)
            {
                Collider col = b.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, true);
                Physics.IgnoreCollision(player2Col, col, true);
                Physics.IgnoreCollision(killer, col, true);
            }

            foreach (var r in reds)
            {
                Collider col = r.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var p in purple)
            {
                Collider col = p.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var w in white)
            {
                Collider col = w.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.blue;
            Player2.GetComponent<Outline>().OutlineColor = UnityEngine.Color.blue;
            dummy_Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.blue;
            dummy_Player_switch.GetComponent<Animator>().SetBool("switch", true);
            dummy_Player_switch2.GetComponent<Animator>().SetBool("switch", false);
        }
        if (red == true && blue == false)
        {
            CollarImage[0].SetActive(false);
            CollarImage[1].SetActive(true);
            CollarImage[2].SetActive(false);
            CollarImage[3].SetActive(false);



            GameObject[] blues = GameObject.FindGameObjectsWithTag("blue");
            GameObject[] reds = GameObject.FindGameObjectsWithTag("red");
            GameObject[] purple = GameObject.FindGameObjectsWithTag("purple");
            GameObject[] white = GameObject.FindGameObjectsWithTag("white");
            Collider killer = GameObject.FindGameObjectWithTag("Killer").GetComponent<Collider>();
            Collider playerCol = Player.GetComponent<Collider>();
            Collider player2Col = Player2.GetComponent<Collider>();

            // Player と Blue/Red の衝突を無効化
            foreach (var b in blues)
            {
                Collider col = b.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }

            foreach (var r in reds)
            {
                Collider col = r.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, true);
                Physics.IgnoreCollision(player2Col, col, true);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var p in purple)
            {
                Collider col = p.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var w in white)
            {
                Collider col = w.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.red;
            Player2.GetComponent<Outline>().OutlineColor = UnityEngine.Color.red;
            dummy_Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.red;
            dummy_Player_switch.GetComponent<Animator>().SetBool("switch", false);
            dummy_Player_switch2.GetComponent<Animator>().SetBool("switch", true);
        }
        if (blue == false && red == false)
        {
            CollarImage[0].SetActive(false);
            CollarImage[1].SetActive(false);
            CollarImage[2].SetActive(true);
            CollarImage[3].SetActive(false);



            GameObject[] blues = GameObject.FindGameObjectsWithTag("blue");
            GameObject[] reds = GameObject.FindGameObjectsWithTag("red");
            GameObject[] purple = GameObject.FindGameObjectsWithTag("purple");
            GameObject[] white = GameObject.FindGameObjectsWithTag("white");
            Collider killer = GameObject.FindGameObjectWithTag("Killer").GetComponent<Collider>();
            Collider playerCol = Player.GetComponent<Collider>();
            Collider player2Col = Player2.GetComponent<Collider>();

            // Player と Blue/Red の衝突を無効化
            foreach (var b in blues)
            {
                Collider col = b.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }

            foreach (var r in reds)
            {
                Collider col = r.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var p in purple)
            {
                Collider col = p.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var w in white)
            {
                Collider col = w.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, true);
                Physics.IgnoreCollision(player2Col, col, true);
                Physics.IgnoreCollision(killer, col, true);
            }

            Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.white;
            Player2.GetComponent<Outline>().OutlineColor = UnityEngine.Color.white;
            dummy_Player.GetComponent<Outline>().OutlineColor = UnityEngine.Color.white;
            dummy_Player_switch.GetComponent<Animator>().SetBool("switch", false);
            dummy_Player_switch2.GetComponent<Animator>().SetBool("switch", false);
        }
        if (blue == true && red == true)
        {
            CollarImage[0].SetActive(false);
            CollarImage[1].SetActive(false);
            CollarImage[2].SetActive(false);
            CollarImage[3].SetActive(true);


            GameObject[] blues = GameObject.FindGameObjectsWithTag("blue");
            GameObject[] reds = GameObject.FindGameObjectsWithTag("red");
            GameObject[] purple = GameObject.FindGameObjectsWithTag("purple");
            GameObject[] white = GameObject.FindGameObjectsWithTag("white");
            Collider killer = GameObject.FindGameObjectWithTag("Killer").GetComponent<Collider>();
            Collider playerCol = Player.GetComponent<Collider>();
            Collider player2Col = Player2.GetComponent<Collider>();

            // Player と Blue/Red の衝突を無効化
            foreach (var b in blues)
            {
                Collider col = b.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }

            foreach (var r in reds)
            {
                Collider col = r.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var p in purple)
            {
                Collider col = p.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, true);
                Physics.IgnoreCollision(player2Col, col, true);
                Physics.IgnoreCollision(killer, col, true);
            }
            foreach (var w in white)
            {
                Collider col = w.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCol, col, false);
                Physics.IgnoreCollision(player2Col, col, false);
                Physics.IgnoreCollision(killer, col, true);
            }
            Player.GetComponent<Outline>().OutlineColor = new UnityEngine.Color(0.5f, 0f, 0.5f, 1f);
            Player2.GetComponent<Outline>().OutlineColor = new UnityEngine.Color(0.5f, 0f, 0.5f, 1f);
            dummy_Player.GetComponent<Outline>().OutlineColor = new UnityEngine.Color(0.5f, 0f, 0.5f, 1f);
            dummy_Player_switch.GetComponent<Animator>().SetBool("switch", true);
            dummy_Player_switch2.GetComponent<Animator>().SetBool("switch", true);
        }

        //photonView.RPC(nameof(ChangeColor), RpcTarget.AllBuffered);
        if (PhotonNetwork.IsMasterClient || !gameEnd)
        {
            if (GameStart_trigger == true)
            {
                TimerLabel.gameObject.SetActive(true);
                CountTimer -= Time.deltaTime;
            }
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

        if (Game_completer == 2 && !isLeaving)
        {
            isLeaving = true;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Game_Clear");
            }
            photonView.RPC(nameof(Clear_load), RpcTarget.All);

        }

    }

    public void jail_doa_control()
    {
        DoorController door = jail_doa.GetComponent<DoorController>();
        if (door != null)
        {
            door.SetOpen(false);
        }

    }
    [PunRPC]
    void Clear_load()
    {

    }
    [PunRPC]
    void Game_over_load()
    {
        SceneManager.LoadScene("Game_over");
        // PhotonNetwork.LeaveRoom();
    }


    private void TrySetRoleLabel(Player player)
    {
        if (player.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            string role = roleObj as string;
            if (role == "killer")
            {

                player.NickName = "Killer";
            }
            else if (role == "survivor")
            {

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
            if (Game_completer > 0 && !isLeaving)
            {
                isLeaving = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel("Game_Clear");
                }

            }
            if (Game_completer == 0 && !isLeaving)
            {
                isLeaving = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel("Game_over");
                }
            }
            //if (Game_completer == 1 && !isLeaving)
            //{
            //    isLeaving = true;
            //    photonView.RPC(nameof(Clear_load), RpcTarget.All);
            //
            //}
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
        if (blue == false)
        {
            blue = true;
        }
        else if (blue == true)
        {
            blue = false;
        }
        Debug.Log("blueON");
    }
    [PunRPC]
    void SyncColor2(bool colorState)
    {
        if (red == false)
        {
            red = true;
        }
        else if (red == true)
        {
            red = false;
        }
        Debug.Log("redON");

    }

    public void playercontrol()
    {
        photonView.RPC(nameof(SyncColor), RpcTarget.All, blue);

    }
    public void player2control()
    {
        photonView.RPC(nameof(SyncColor2), RpcTarget.All, red);

    }
    public void killer_skill()
    {
        photonView.RPC(nameof(RPC_ActivateOutlineSkill), RpcTarget.All);

    }
    public void killer_skillOF()
    {
        photonView.RPC(nameof(RPC_DeactivateOutlineSkill), RpcTarget.All);

    }
    public void Game_Clear(string a)
    {
        photonView.RPC(nameof(Clear), RpcTarget.All, a);

    }
    public void Game_over()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (hasCalledGameOver) return;
        hasCalledGameOver = true;
        photonView.RPC(nameof(Game_over_count), RpcTarget.All);
    }
    public void Game_over_of()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!hasCalledGameOver) return;
        hasCalledGameOver = false;
        photonView.RPC(nameof(Game_over_count2), RpcTarget.All);
    }
    public void name_Record(string name, string target)
    {
        photonView.RPC(nameof(RPC_name_Record), RpcTarget.All, name, target);
    }
    [PunRPC]
    void RPC_name_Record(string name, string target_name)
    {
        // 不可視文字を除去
        string cleanName = name
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace("\u200B", "") // ゼロ幅スペース
            .Trim();

        if (target_name == "Player")
        {
            Player_name[0] = cleanName;
            if (Player_name[0] != null)
            {
                GameObject.FindGameObjectWithTag(target_name).GetComponent<PlayerController>().my_name.richText = false;

                GameObject.FindGameObjectWithTag(target_name)
                    .GetComponent<PlayerController>().my_name.text = Player_name[0];
            }
        }
        if (target_name == "Player2")
        {
            Player_name[1] = cleanName;
            if (Player_name[1] != null)
            {
                GameObject.FindGameObjectWithTag(target_name).GetComponent<PlayerController>().my_name.richText = false;
                GameObject.FindGameObjectWithTag(target_name)
                    .GetComponent<PlayerController>().my_name.text = Player_name[1];
            }
        }
        if (target_name == "Killer")
        {
            Player_name[2] = cleanName;
            if (Player_name[2] != null)
            {
                GameObject.FindGameObjectWithTag(target_name).GetComponent<PlayerController>().my_name.richText = false;
                GameObject.FindGameObjectWithTag(target_name)
                    .GetComponent<PlayerController>().my_name.text = Player_name[2];
            }
        }
    }



    public void StopDrone_Ability()
    {
        for (int num = 0; num < Drone_Objects.Length; num++)
        {
            Drone_Objects[num].Call_Stop_Drone();
        }
    }

    public void Face_swap(string target)
    {
        photonView.RPC(nameof(RPC_Face_swap), RpcTarget.All, target);
    }
    public void Face_swapOF(string target)
    {
        photonView.RPC(nameof(RPC_Face_swapOF), RpcTarget.All, target);
    }
    [PunRPC]
    void RPC_Face_swap(string target_name)
    {
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.GetComponent<ExpressionController>().preset = GameObject.FindWithTag(target_name).GetComponent<PlayerController>().preset;
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.GetComponent<ExpressionController>()._value = 1f;
    }
    [PunRPC]
    void RPC_Face_swapOF(string target_name)
    {
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.GetComponent<ExpressionController>()._value = 0f;
    }

    [PunRPC]
    void RPCcaught(string target_name)
    {
        StartCoroutine(caught(target_name));

    }
    public void killer_Securing(string a)
    {
        photonView.RPC(nameof(RPCcaught), RpcTarget.All, a);
    }
    IEnumerator caught(string target_name)
    {
        Debug.Log("確認OK");
        photonView.RPC(nameof(RPCkiller_Securing1), RpcTarget.All, target_name);
        yield return new WaitForSeconds(3f);
        photonView.RPC(nameof(RPCkiller_Securing2), RpcTarget.All, target_name);
        yield return new WaitForSeconds(1f);

        Game_over();
    }
    [PunRPC]
    void RPCkiller_Securing1(string target_name)
    {
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Trap_trigger = true;
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().fade_trigger = true;
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.SetActive(false);
        Instantiate(effect, new Vector3(GameObject.FindWithTag(target_name).transform.position.x, GameObject.FindWithTag(target_name).transform.position.y + 2, GameObject.FindWithTag(target_name).transform.position.z), Quaternion.identity);
    }
    [PunRPC]
    void RPCkiller_Securing2(string target_name)
    {
        GameObject.FindWithTag(target_name).transform.position = new Vector3(82, 7, 10);
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().fade_trigger = false;
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Trap_trigger = false;
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.SetActive(true);
    }

    public void killer_skin(string target)
    {
        photonView.RPC(nameof(killer_skin_change), RpcTarget.All, target);
    }

    [PunRPC]
    void killer_skin_change(string target_name)
    {
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().Mermaid.SetActive(false);
        GameObject.FindWithTag(target_name).GetComponent<PlayerController>().killer_skin.SetActive(true);
    }


    [PunRPC]
    void Clear(string target)
    {

        Game_completer += 1;
        GameObject.FindWithTag(target).GetComponent<PlayerController>().Mermaid.SetActive(false);
        GameObject.FindWithTag(target).GetComponent<PlayerController>().Ghost_skin.SetActive(true);
        GameObject.FindWithTag(target).gameObject.layer = 20;
        GameObject.FindWithTag(target).transform.position = new Vector3(100, 7, 4);

        if (target == "Player")
        {
            Game_completer_name[0] = target;
        }
        if (target == "Player2")
        {
            Game_completer_name[1] = target;
        }


    }
    [PunRPC]
    void Game_over_count()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("回数確認");
        DesPlayer++;

        Debug.Log($"[Master] DesPlayer = {DesPlayer}");

        if (DesPlayer >= 2)
        {
            photonView.RPC(nameof(Game_over_load), RpcTarget.All);
        }
        else
        {
            hasCalledGameOver = false;
        }
    }

    [PunRPC]
    void Game_over_count2()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        DesPlayer -= 1;
    }
    //アウトライン透過能力
    [PunRPC]
    void RPC_ActivateOutlineSkill()
    {

        // スキル使用者の名前などを出力
        Debug.Log($"{photonView.Owner.NickName} がアウトラインスキルを発動！");

        // 自分以外のプレイヤーを対象に壁越し可視化
        // 全てのプレイヤーオブジェクトを捜索

        Player.GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_On;
        Player2.GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_On;
        GameObject.FindGameObjectWithTag("dummy_Player").GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_On;



    }
    [PunRPC]
    void RPC_DeactivateOutlineSkill()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_Off;
        }
        if (GameObject.FindGameObjectWithTag("Player2") != null)
        {
            GameObject.FindGameObjectWithTag("Player2").GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_Off;
        }
        if (GameObject.FindGameObjectWithTag("dummy_Player") != null)
        {
            GameObject.FindGameObjectWithTag("dummy_Player").GetComponent<Outline>().OutlineMode = Outline.Mode.Skill_Off;
        }
    }
    [PunRPC]
    void RPCkakuho(string target)
    {
        if (target == "Player")
        {
            kakuho_UI[0].SetActive(true);
        }
        else if (target == "Player2")
        {
            kakuho_UI[1].SetActive(true);
        }
    }
    public void kakuho(string target)
    {
        photonView.RPC(nameof(RPCkakuho), RpcTarget.All, target);
    }
    [PunRPC]
    void RPCkakuhoOF(string target)
    {
        if (target == "Player")
        {
            kakuho_UI[0].SetActive(false);
        }
        else if (target == "Player2")
        {
            kakuho_UI[1].SetActive(false);
        }
    }
    public void kakuhoOF(string target)
    {
        photonView.RPC(nameof(RPCkakuhoOF), RpcTarget.All, target);
    }
    public void cursor_controlON()
    {
        cursor_control.SetActive(true);
    }
    [PunRPC]
    void RPCMEMO(string target,string name)
    {
        if (name == "number1")
        {
            memo[0].text = ($"{target}");
        }
        if (name == "number2")
        {
            memo[1].text = ($"{target}");
        }
        if (name == "number3")
        {
            memo[2].text = ($"{target}");
        }
        if (name == "number4")
        {
            memo[3].text = ($"{target}");
        }
    }
    public void MEMO(string target, string name)
    {
        photonView.RPC(nameof(RPCMEMO), RpcTarget.All, target,name);
    }
}
