using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SampleScene_test : MonoBehaviourPunCallbacks
{
    private int playerCount;
    const int maxPlayers = 3;
    public string[] playerPrefabName;
    public Transform[] spawnPoints;

    public string selectedRoomName = "Room1"; // デフォルトは Room1

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // UI ボタンから呼び出す
    public void JoinRoom1() => TryJoinRoom("Room1");
    public void JoinRoom2() => TryJoinRoom("Room2");
    public void JoinRoom3() => TryJoinRoom("Room3");

    private void TryJoinRoom(string roomName)
    {
        selectedRoomName = roomName;
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.PlayerList.Length;

        // 🔍 現在のルーム情報をデバッグ出力
        Debug.Log($"現在のルーム名: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"現在のプレイヤー数: {playerCount}");
        Debug.Log($"最大プレイヤー数: {PhotonNetwork.CurrentRoom.MaxPlayers}");

        if (playerCount <= maxPlayers)
        {
            PhotonNetwork.Instantiate(
                playerPrefabName[playerCount - 1],
                spawnPoints[playerCount - 1].position,
                Quaternion.identity
            );
        }
    }

}