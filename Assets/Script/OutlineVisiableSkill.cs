using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OutlineVisiableSkill : MonoBehaviourPunCallbacks
{
    [SerializeField] float skillDuration = 5f; // 壁越しアウトラインの持続時間

    private int normalLayer;
    private int outlineLayer;

    void Start()
    {
        normalLayer = LayerMask.NameToLayer("PlayerNormal");
        outlineLayer = LayerMask.NameToLayer("OutlineVisible");
    }

    void Update()
    {
        // スキル発動キー例: Qキー
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC(nameof(RPC_ActivateOutlineSkill), RpcTarget.All);
        }
    }

    public void StartOutlineSkill()
    {
        photonView.RPC(nameof(RPC_ActivateOutlineSkill), RpcTarget.All);
    }

    [PunRPC]
    void RPC_ActivateOutlineSkill()
    {
        // スキル使用者の名前などを出力
        Debug.Log($"{photonView.Owner.NickName} がアウトラインスキルを発動！");

        // 自分以外のプレイヤーを対象に壁越し可視化
        foreach (var playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = playerObj.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != photonView.Owner)
            {
                StartCoroutine(ShowOutlineThroughWalls(playerObj, skillDuration));
            }
        }
    }

    IEnumerator ShowOutlineThroughWalls(GameObject targetPlayer, float duration)
    {
        SetLayerRecursively(targetPlayer, outlineLayer);
        yield return new WaitForSeconds(duration);
        SetLayerRecursively(targetPlayer, normalLayer);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
