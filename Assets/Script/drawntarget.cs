using Photon.Pun;
using UnityEngine;

public class drawntarget : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject killer;
    [SerializeField] private GameObject target;
    public MeshRenderer meshRenderer;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float hideDistance = 25f;

    private bool currentMeshState = true; // 現在の表示状態を保持

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        killer = GameObject.FindGameObjectWithTag("Killer");
        target = GameObject.Find("drawn");
    }

    private void Update()
    {
        if (killer == null || target == null) return;

        float distanceToKiller = Vector3.Distance(transform.position, killer.transform.position);
        float distanceToTarget = Vector3.Distance(killer.transform.position, target.transform.position);

        // Killer に近ければ target へ移動、遠ければ Killer に移動
        if (distanceToKiller < hideDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, killer.transform.position, moveSpeed * Time.deltaTime);
        }

        // Y座標固定
        transform.position = new Vector3(transform.position.x, 16.45f, transform.position.z);

        // MeshRenderer の表示状態を判定
        bool shouldBeVisible = distanceToTarget >= hideDistance;

        // 状態が変わったときだけ RPC で全クライアントに送る
        if (shouldBeVisible != currentMeshState)
        {
            photonView.RPC("SetMeshRenderer", RpcTarget.AllBuffered, shouldBeVisible);
            currentMeshState = shouldBeVisible;
        }
    }

    [PunRPC]
    void SetMeshRenderer(bool enabledState)
    {
        meshRenderer.enabled = enabledState;
    }
}
