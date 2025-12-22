using UnityEngine;
using Photon.Pun;

public class RandomColor : MonoBehaviourPun
{
    int collar;
    Material material;

    [SerializeField] Material[] referenceMaterials;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        // MasterClient だけがランダムを決定
        if (PhotonNetwork.IsMasterClient)
        {
            collar = Random.Range(1, 5);

            // 全クライアントに送信
            photonView.RPC(nameof(SetColor), RpcTarget.AllBuffered, collar);
        }
    }

    [PunRPC]
    void SetColor(int value)
    {
        collar = value;

        switch (collar)
        {
            case 1:
                gameObject.tag = "red";
                material.color = referenceMaterials[0].color;
                break;

            case 2:
                gameObject.tag = "blue";
                material.color = referenceMaterials[1].color;
                break;

            case 3:
                gameObject.tag = "purple";
                material.color = referenceMaterials[2].color;
                break;

            case 4:
                gameObject.tag = "white";
                material.color = referenceMaterials[3].color;
                break;
        }
    }
}
