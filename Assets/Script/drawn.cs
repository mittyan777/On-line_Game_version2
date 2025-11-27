using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class drawn : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private float chaseDistance = 10f; // プレイヤーを追いかける距離

    [SerializeField]
    private float wanderRadius = 20f; // 徘徊エリアの半径

    [SerializeField]
    private float wanderInterval = 10f; // 徘徊ポイントを変更する間隔

    [SerializeField]
    private float StoppingTime = 10f; // ドローン停止時間

    private float StoppingCountTime = 0f;

    private bool isChasing = false;
    private float wanderTimer;
    private bool siren = false;
    private bool MoveDisabled = false;

    private Transform targetPlayer; // 追跡対象

    [SerializeField] private GameObject marker;
    [SerializeField] GameObject SmokeParticle;

    void Start()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        SmokeParticle.SetActive(false);
        wanderTimer = wanderInterval;
        SetNewWanderDestination();
    }

    void Update()
    {
        // 停止中の処理
        if (MoveDisabled)
        {
            _navMeshAgent.isStopped = true;
            StoppingCountTime -= Time.deltaTime;
            if (StoppingCountTime <= 0f)
            {
                MoveDisabled = false; // ドローン再始動
                SmokeParticle.SetActive(false);
            }
        }
        else
        {
            _navMeshAgent.isStopped = false;
        }

        // 追跡 or 徘徊
        if (isChasing && targetPlayer != null)
        {
            _navMeshAgent.SetDestination(targetPlayer.position);
        }
        else
        {
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= wanderInterval ||
                (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance))
            {
                SetNewWanderDestination();
                wanderTimer = 0f;
            }
        }
    }

    private void SetNewWanderDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
        {
            _navMeshAgent.SetDestination(navHit.position);
        }
    }

    public void Call_Stop_Drone()
    {
        photonView.RPC(nameof(Stop_Drone), RpcTarget.All);
    }

    [PunRPC]
    void Stop_Drone()
    {
        StoppingCountTime = 5f;
        MoveDisabled = true;
        SmokeParticle.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー追跡開始
        if (other.CompareTag("Drone Player Detection") && !other.CompareTag("Killer"))
        {
            if (!MoveDisabled)
            {
                marker.SetActive(true);
                isChasing = true;
                siren = true;
                targetPlayer = other.transform;
                Debug.Log("追跡開始 -> " + other.tag);
            }
        }

        // ドローン停止
        if (other.CompareTag("DroneStopper") && !other.CompareTag("Killer"))
        {
            MoveDisabled = true;
            StoppingCountTime = StoppingTime;
            Debug.Log("停止エリア侵入 -> " + other.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // プレイヤー追跡終了
        if (other.CompareTag("Drone Player Detection"))
        {
            marker.SetActive(false);
            isChasing = false;
            siren = false;
            targetPlayer = null;
            Debug.Log("追跡終了 -> " + other.tag);
        }
    }
}
