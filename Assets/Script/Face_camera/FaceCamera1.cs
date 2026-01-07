using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;

public class FaceCamera1 : MonoBehaviourPunCallbacks
{
    GameObject []Target;
    [SerializeField]GameObject manager;
    [SerializeField] string target_name;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.GetComponent<MainGameManager>().Gamestart == true)
        {
           
            
                Target = GameObject.FindGameObjectsWithTag(target_name);
                transform.SetParent(Target[0].GetComponent<PlayerController>().Mermaid_Face.transform);
                transform.localPosition = new Vector3(0f, 0.1f, 0.463f);
                transform.LookAt(Target[0].GetComponent<PlayerController>().Mermaid_Face.transform);
            
        }
    }
}
