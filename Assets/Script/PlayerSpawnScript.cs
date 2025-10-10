using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private string[] playerPrefabNames; // Player1, Player2, Player3 ...
    [SerializeField] private Transform[] spawnPoints;    // 各プレイヤーのスポーン位置

    public override void OnEnable()
    {
        base.OnEnable(); // 重要！Photonコールバックを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable(); // 重要！Photonコールバックを登録
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady) return;

        // すでに生成済みなら何もしない
        if (PhotonNetwork.LocalPlayer.TagObject != null) return;

        // ActorNumber ではなく、Roomのプレイヤーリスト順でインデックス決定
        Player[] playerList = PhotonNetwork.PlayerList;
        int localIndex = System.Array.IndexOf(playerList, PhotonNetwork.LocalPlayer);

        localIndex = Mathf.Clamp(localIndex, 0, playerPrefabNames.Length - 1);
        int spawnIndex = localIndex % spawnPoints.Length;

        string prefabName = playerPrefabNames[localIndex];
        Vector3 spawnPos = spawnPoints[spawnIndex].position;

        GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);

        PhotonNetwork.LocalPlayer.TagObject = player;
        Debug.Log($"Spawned {prefabName} at {spawnPos}");
    }
}
