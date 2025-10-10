using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

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

    private bool isChasing = false;
    private float wanderTimer;
    private bool siren = false;

    private Transform targetPlayer; // 追跡対象

    [SerializeField]GameObject marker;

    void Start()
    {
        // NavMeshAgent を自動取得
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        wanderTimer = wanderInterval;
        SetNewWanderDestination();
        


    }

    void Update()
    {
      
        if (isChasing && targetPlayer != null)
        {
            // プレイヤーの位置を常に追尾
            _navMeshAgent.SetDestination(targetPlayer.position);
        }
        else
        {
            // 徘徊
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= wanderInterval ||
                (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance))
            {
                SetNewWanderDestination();
                wanderTimer = 0f;
            }
        }
    }

    // NavMesh 上のランダムな徘徊ポイントを取得して設定
    private void SetNewWanderDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
        {
            _navMeshAgent.SetDestination(navHit.position);
        }
    }
   
    private void OnTriggerStay(Collider other)
    {

        if ((other.gameObject.tag == ("Player")))
        {
            //if (photonView.IsMine)
            //{
            if ((other.gameObject.tag == ("Player")))
            {
                marker.SetActive(true);
                Debug.Log("追跡中 -> " + other.tag);
                isChasing = true;
                siren = true;
                targetPlayer = other.transform;
            }
            //}
        }
        if ((other.gameObject.tag == ("Player2")))
        {
            //if (photonView.IsMine)
            //{
                marker.SetActive(true);
                Debug.Log("追跡中 -> " + other.tag);
                isChasing = true;
                siren = true;
                targetPlayer = other.transform;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            marker.SetActive(false);
            Debug.Log("追跡終了 -> " + other.tag);
            isChasing = false;
            siren = false;
            targetPlayer = null;
        }
    }
}
