using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave_room : MonoBehaviourPunCallbacks
{
    private void Start()
    {
  
    }
    // ゲーム終了時に呼ぶ
    public void LeaveRoomAndClose()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("ルームから切断します...");
            PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.LocalPlayer.TagObject = null;
            PhotonNetwork.LeaveRoom(); // 切断開始 -> 完了するとOnLeftRoomが呼ばれる
        }
        else
        {
            // すでに切断されている場合は直接移動
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }

    // ルーム退出完了時に呼ばれるコールバック
    public override void OnLeftRoom()
    {
        // タイトル画面などに遷移
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
    public void RoomEXT()
    {
        PhotonNetwork.LeaveRoom();
        Title_control.errorWindow = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }




}
