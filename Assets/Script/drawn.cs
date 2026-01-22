using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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
    [SerializeField] GameObject gameManager;

    public float distance;
    public float distance2;

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
        if (gameManager.GetComponent<MainGameManager>().Gamestart == true)
        {
            distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            distance2 = Vector3.Distance(GameObject.FindWithTag("Player2").transform.position, transform.position);
        }
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
        if (distance <= 10 || distance2 <= 10)
        {
            StoppingCountTime = 10f;
            MoveDisabled = true;
            SmokeParticle.SetActive(true);
            marker.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー追跡開始
        if (other.CompareTag("Drone Player Detection") && other.transform.parent.tag != "Killer")
        {
            if (!MoveDisabled)
            {
                photonView.RPC(nameof(rockon), RpcTarget.All, other.transform.parent.tag);
         
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
    [PunRPC]
    void rockon(string target)
    {
       GameObject a =  GameObject.FindWithTag(target);
        marker.SetActive(true);
        isChasing = true;
        siren = true;
        targetPlayer = a.transform;


        Debug.Log("追跡開始 -> " + a.tag);
    }
    private void OnTriggerExit(Collider other)
    {
        // プレイヤー追跡終了
        if (other.CompareTag("Drone Player Detection"))
        {
            photonView.RPC(nameof(rockof), RpcTarget.All, other.gameObject.tag);
          
        }
    }
    [PunRPC]
    void rockof(string target)
    {
        GameObject a = GameObject.FindWithTag(target);
        marker.SetActive(false);
        isChasing = false;
        siren = false;
        targetPlayer = null;
        Debug.Log("追跡終了 -> " + a.tag);
    }
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = 7.4f; // 固定したい高さ
        transform.position = pos;
    }

}
