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
    private float chaseDistance = 10f; // �v���C���[��ǂ������鋗��

    [SerializeField]
    private float wanderRadius = 20f; // �p�j�G���A�̔��a

    [SerializeField]
    private float wanderInterval = 10f; // �p�j�|�C���g��ύX����Ԋu

    private bool isChasing = false;
    private float wanderTimer;
    private bool siren = false;

    private Transform targetPlayer; // �ǐՑΏ�

    [SerializeField]GameObject marker;

    void Start()
    {
        // NavMeshAgent �������擾
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
            // �v���C���[�̈ʒu����ɒǔ�
            _navMeshAgent.SetDestination(targetPlayer.position);
        }
        else
        {
            // �p�j
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= wanderInterval ||
                (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance))
            {
                SetNewWanderDestination();
                wanderTimer = 0f;
            }
        }
    }

    // NavMesh ��̃����_���Ȝp�j�|�C���g���擾���Đݒ�
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
                Debug.Log("�ǐՒ� -> " + other.tag);
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
                Debug.Log("�ǐՒ� -> " + other.tag);
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
            Debug.Log("�ǐՏI�� -> " + other.tag);
            isChasing = false;
            siren = false;
            targetPlayer = null;
        }
    }
}
