using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forced_Logout: MonoBehaviourPunCallbacks
{


    // 誰かが抜けたときに呼ばれる
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player Left: {otherPlayer.NickName}");

        // マスタークライアントだけがシーン遷移を指示する
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ルームから切断します...");
            PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.LocalPlayer.TagObject = null;
            PhotonNetwork.LeaveRoom(); // 切断開始 -> 完了するとOnLeftRoomが呼ばれる
            Title_control.errorWindow2 = true;
            PhotonNetwork.LoadLevel("Title");


        }
    }
}

