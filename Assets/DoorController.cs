using Photon.Pun;
using UnityEngine;

public class DoorController : MonoBehaviourPun
{
    [SerializeField] Animator animator;

    [PunRPC]
    void RPC_SetOpen(bool isOpen)
    {
        animator.SetBool("open", isOpen);
        GetComponent<AudioSource>().Play();
    }

    public void SetOpen(bool isOpen)
    {
        photonView.RPC(nameof(RPC_SetOpen), RpcTarget.AllBuffered, isOpen);
    }
}
