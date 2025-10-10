using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class SampleScene : MonoBehaviourPunCallbacks
{
    private int playerCount;
    const int maxPlayers = 3;
    public string[] playerPrefabName;
    public Transform[] spawnPoints;    // 各プレイヤーのスポーン位置

    bool plugs = false;
    private void Start()
    {
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Update()
    {
        if (plugs == true)
        {




        }
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.PlayerList.Length; //���[���ɂ���l�����m�F

        if (playerCount <= maxPlayers)
        {
            PhotonNetwork.Instantiate(playerPrefabName[playerCount - 1], spawnPoints[playerCount - 1].position, Quaternion.identity);
        }

    }
}