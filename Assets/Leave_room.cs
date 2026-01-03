using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave_room: MonoBehaviourPunCallbacks
{
    // ゲーム終了時に呼ぶ
    public void LeaveRoomAndClose()
    {

        // ルーム退出
        PhotonNetwork.LeaveRoom();
    }

    // ルーム退出完了時に呼ばれるコールバック
    public override void OnLeftRoom()
    {
        // タイトル画面などに遷移
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

}
