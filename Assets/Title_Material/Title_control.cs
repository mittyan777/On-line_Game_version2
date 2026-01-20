using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// MonoBehaviourPunCallbacksを継承することで、OnJoinedRoomなどが使えるようになります
public class Title_control : MonoBehaviourPunCallbacks
{
    // [SerializeField] GameObject camera;
    [SerializeField] GameObject StartButton_obj;
    [SerializeField] GameObject RoomSelect_obj;
    [SerializeField] GameObject fade;
    [SerializeField] AudioClip Title_BGM; 

    AudioScript audioScript;
    bool Start_trigger;
    bool isConnecting = false; // 重複して入室処理が走らないようにするフラグ
    float Speed = 10;
    int roomnum;
    [SerializeField] GameObject errorWindow_UI;
    [SerializeField] GameObject errorWindow_UI2;
    static public bool errorWindow = false;
    static public bool errorWindow2 = false;

    // Start is called before the first frame update
    void Start()
    {
        fade.SetActive(false);
        //RenderSettings.fogDensity = 0.05f;
        StartButton_obj.SetActive(true);
        RoomSelect_obj.SetActive(false);

        // ゲーム開始時にPhotonサーバーへ接続する
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        if (GameObject.Find("BGM") != null)
        {
            audioScript = GameObject.Find("BGM").GetComponent<AudioScript>();
        }
        audioScript.Change_PlayAudio(Title_BGM);
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (errorWindow == true)
        {
            errorWindow_UI.SetActive(true);
        }
        else
        {
            errorWindow_UI.SetActive(false);
        }
        if (errorWindow2 == true)
        {
            errorWindow_UI2.SetActive(true);
        }
        else
        {
            errorWindow_UI2.SetActive(false);
        }

        if (Start_trigger == false)
        {

        }


        if (Start_trigger)
        {
            fade.SetActive(true);
        }
        if (fade.GetComponent<Image>().color.a == 1)
        {
            ConnectRoom();
        }
    }
    public void error_ext()
    {
        errorWindow = false;
    }
    public void error_ext2()
    {
        errorWindow2 = false;
    }
    public void GameStart()
    {
        StartButton_obj.SetActive(false);
        RoomSelect_obj.SetActive(true);
    }

    public void Select_RoomNum(int num)
    {
        // まだスタートしておらず、かつPhotonサーバーに接続済みであれば進行可能
        if (Start_trigger == false)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                roomnum = num;
                Start_trigger = true;

                RoomSelect_obj.SetActive(false);
            }
            else
            {
                Debug.LogWarning("まだサーバーに接続できていません。少し待ってから押してください。");
            }
        }
    }

    public void ConnectRoom()
    {
        isConnecting = true; // 処理中フラグを立てる
        Debug.Log($"Room{roomnum} に入室を試みます...");

        // ルームオプションの設定（必要に応じてMaxPlayersなどを設定）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3; // 例: 最大3人

        PhotonNetwork.JoinOrCreateRoom($"Room{roomnum}", roomOptions, TypedLobby.Default);
    }

    // --- Photonのコールバック ---

    // ルーム入室に成功した時に自動的に呼ばれる関数
    public override void OnJoinedRoom()
    {
        Debug.Log("ルーム入室成功！ロビーシーンへ移動します。");

        // 入室が完了してからシーンを読み込む
        PhotonNetwork.LoadLevel("lobby");
    }

    // 入室に失敗した場合（念のため）
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"入室失敗: {message}");
        isConnecting = false; // フラグを戻して再試行できるようにする
    }
}