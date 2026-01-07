using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class SampleScene : MonoBehaviourPunCallbacks
{
    private int playerCount;
    const int maxPlayers = 3;
    public string[] playerPrefabName;
    public Transform[] spawnPoints;    // 各プレイヤーのスポーン位置

    //?????
    bool plugs = false;
    private IEnumerator Start()
    {
        // 1. もしオフライン（接続切れ）なら、少し待ってみる
        // シーンロード直後はステータス同期が一瞬遅れることがあるため
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("ルーム情報の同期を待機中...");
            // 最大3秒間、InRoomになるのを待つ
            float timeOut = 3.0f;
            while (!PhotonNetwork.InRoom && timeOut > 0)
            {
                timeOut -= Time.deltaTime;
                yield return null; // 1フレーム待つ
            }
        }

        // 2. 待った結果、部屋にいるなら生成
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogError("エラー：ルームに入っていません。");
            Debug.LogError($"現在の状態: {PhotonNetwork.NetworkClientState}");
        }
    }

    private void Update()
    {
        if (plugs == true)
        {




        }
    }

    // 生成処理の本体
    private void SpawnPlayer()
    {
        // 自分のプレイヤーID (1, 2, 3...) を取得
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // 配列の要素数を超えないように調整 (ID 1 -> index 0)
        // ※剰余演算(%)を使うことで、人数が増えてもエラーが出ないようにしています
        int spawnIndex = (actorNumber - 1) % spawnPoints.Length;
        int prefabIndex = (actorNumber - 1) % playerPrefabName.Length;

        // 生成するプレハブ名と場所を決定
        string prefabToSpawn = playerPrefabName[prefabIndex];
        Transform spawnPoint = spawnPoints[spawnIndex];

        // 生成実行
        GameObject myPlayer = PhotonNetwork.Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        Debug.Log($"プレイヤー生成完了: {myPlayer.name} (ActorNumber: {actorNumber})");
    }

    // // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    // public override void OnConnectedToMaster()
    // {
    //     // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j

    //     //for room1~3
    //     PhotonNetwork.JoinOrCreateRoom($"Room{Random.Range(1, 4)}", new RoomOptions(), TypedLobby.Default);
    // }

    // // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    // public override void OnJoinedRoom()
    // {
    //     playerCount = PhotonNetwork.PlayerList.Length; //���[���ɂ���l�����m�F

    //     if (playerCount <= maxPlayers)
    //     {
    //         PhotonNetwork.Instantiate(playerPrefabName[playerCount - 1], spawnPoints[playerCount - 1].position, Quaternion.identity);
    //     }

    // }
}