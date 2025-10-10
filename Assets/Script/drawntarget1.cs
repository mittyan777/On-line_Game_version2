using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class drawntarget1: MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] _player;
    [SerializeField] float distance;
    [SerializeField] float distance2;
    [SerializeField] GameObject []target;
    public MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer =GetComponent<MeshRenderer>();
    
    }

    // Update is called once per frame
    void Update()
    {
        _player = GameObject.FindGameObjectsWithTag("Killer");
        target[0] = GameObject.Find("drawn2");
        if (photonView.IsMine)
        {
          
            distance = Vector3.Distance(transform.position, _player[0].transform.position);
            distance2 = Vector3.Distance(_player[0].transform.position, target[0].transform.position);

            if (distance < 25)
            {
             
                transform.position = Vector3.MoveTowards(transform.position, target[0].transform.position, 10 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _player[0].transform.position, 10 * Time.deltaTime);
            }

            if (distance2 < 25)
            {
                meshRenderer.enabled = false;
            }
            else
            {
                meshRenderer.enabled = true;
            }

            transform.position = new Vector3(transform.position.x, 16.45f, transform.position.z);
        }
    }
}
