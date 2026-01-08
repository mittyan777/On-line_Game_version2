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
    [SerializeField] GameObject MY_camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.GetComponent<MainGameManager>().Gamestart == true)
        {

            if (target_name != "Killer")
            {
                Target = GameObject.FindGameObjectsWithTag(target_name);
                if (Target[0].layer == LayerMask.NameToLayer("Player"))
                {
                    MY_camera.transform.SetParent(Target[0].GetComponent<PlayerController>().Mermaid_Face.transform);
                    MY_camera.transform.localPosition = new Vector3(0f, 0.1f, 0.6f);
                    MY_camera.transform.LookAt(Target[0].GetComponent<PlayerController>().Mermaid_Face.transform);
                }
                else
                {
                    MY_camera.transform.SetParent(Target[0].GetComponent<PlayerController>().Ghost_skin.transform);
                    MY_camera.transform.localPosition = new Vector3(0f, 0.65f, 0.8f);
                }
            }
            elseÅ@if(target_name == "Killer")
            {
                Target = GameObject.FindGameObjectsWithTag(target_name);
                MY_camera.transform.SetParent(Target[0].GetComponent<PlayerController>().killer_skin.transform);
                MY_camera.transform.localPosition = new Vector3(0f, 0.65f, 0.6f);
               // transform.LookAt(Target[0].GetComponent<PlayerController>().killer_skin.transform);
            }
          
            
        }
    }
}
