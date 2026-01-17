using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class drawntarget : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject killer;
    [SerializeField] float distance;
    [SerializeField] float distance2;
    [SerializeField] GameObject[] target;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(killer.transform.position.x, transform.position.y, killer.transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        killer = GameObject.FindGameObjectWithTag("Killer");
        target[0] = GameObject.Find("drawn");
        if (photonView.IsMine)
        {
            Vector3 dir = target[0].transform.position - transform.position;
            dir.y = 0f;

            transform.rotation =
                Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);

            distance = Vector3.Distance(transform.position, killer.transform.position);
            distance2 = Vector3.Distance(killer.transform.position, target[0].transform.position);

            if (distance < 25)
            {

                transform.position = Vector3.MoveTowards(transform.position, target[0].transform.position, 10 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, killer.transform.position, 10 * Time.deltaTime);
            }

            if (distance2 < 25)
            {
                spriteRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = true;
            }

            transform.position = new Vector3(transform.position.x, 16.45f, transform.position.z);
        }
    }
}
