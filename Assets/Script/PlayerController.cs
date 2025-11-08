using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Photon.Realtime;
using Unity.Burst.CompilerServices;
using static Unity.Burst.Intrinsics.X86.Sse4_2;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] int gameManager;
    [SerializeField] GameObject camera_Object;
    [SerializeField] Canvas playerCanvas;
    [Header("アウトライン")]
    [SerializeField] Outline outline_Script;
    float side;
    float ver;
    float MoveSpeed = 5;

    bool Is_PlayMode = false;

    Rigidbody rb;
    Ray ray;
    [SerializeField] GameObject camera;

    [SerializeField] private float rayDistance = 0.01f;
    [SerializeField] private GameObject rayObject;

    [SerializeField] Text select;
    string collar = "";
    [SerializeField] private GameObject Manager;
    GameObject OFObject;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
       
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            Is_PlayMode = true;
           
            //Roll Check
            Invoke("test", 5);
        }
       

    }
    void test()
    {

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            string role = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

            // 全員に共有する
            photonView.RPC("SetRole", RpcTarget.AllBuffered, role);

            if (this.gameObject.tag == "Killer")
            {
                GameObject.Find("killerImage").SetActive(true);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(false);
            }
            else if (this.gameObject.tag == "Player")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(true);
                GameObject.Find("Player2Image").SetActive(false);
            }
            else if (this.gameObject.tag == "Player2")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(true);
            }
        }

    }

    [PunRPC]
    void SetRole(string role)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            if (role == "killer")
            {
                gameObject.layer = LayerMask.NameToLayer("Killer");
                gameObject.tag = "Killer";
                Debug.Log("あなたは Killer です！");
            }
            else if (role == "survivor")
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (players.Length == 0)
                {
                    gameObject.tag = "Player";

                }
                else if (players.Length == 1)
                {
                    gameObject.tag = "Player2";

                }

                Debug.Log("あなたは Survivor です！");
            }
            else
            {
                Debug.Log("ロールが設定されていません。");
            }
        }
    }



    // Update is called once per frame

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float x = 0f;
            float z = 0f;

            // 入力取得
            if (Input.GetKey("w")) 
            {
                z += 1f;
                animator.SetBool("forward_walk", true);
            }
            else
            {
                animator.SetBool("forward_walk", false);
            }
            if (Input.GetKey("s")) 
            {
                z -= 1f;
                animator.SetBool("back_walk", true);
            }
            else
            {
                animator.SetBool("back_walk", false);
            }
            if (Input.GetKey("a")) 
            {
                x -= 1f;
                animator.SetBool("left_walk", true);
            }
            else
            {
                animator.SetBool("left_walk", false);
            }
            if (Input.GetKey("d")) 
            {
                x += 1f;
                animator.SetBool("right_walk", true);
            }
            else
            {
                animator.SetBool("right_walk", false);
            }
            
            
            
            

            // 移動方向ベクトル
            Vector3 move = (transform.forward * z + transform.right * x).normalized;

            // 実際の移動
            transform.position += move * MoveSpeed * Time.deltaTime;
        }
    }
    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            if (select == null)
            {
                select = GameObject.FindGameObjectWithTag("selectUI").GetComponent<Text>();
            }
        }

        // Ray作成
        ray = new Ray(rayObject.transform.position, rayObject.transform.forward);
        // Sceneビューで赤い線を描画

        if (photonView.IsMine)
        {
            playerCanvas.enabled = true;
            //Rayのエラーのため、無効化
            //photonView.RPC("SetRay", RpcTarget.AllBuffered);

            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            side += h;
            ver += v;
            ver = Mathf.Clamp(ver, -50f, 90f);
           // side = Mathf.Clamp(side, -90, 90f);
            camera.transform.rotation = Quaternion.Euler(-ver, side, camera.transform.eulerAngles.z);

            transform.rotation = Quaternion.Euler(0f, side, 0f);
          

            RaycastHit hit;

            // Ray飛ばす
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("DOA"))
                {

                    Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                    if (animator.GetBool("open") == false)
                    {
                        select.text = "[F]開ける";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", true);
                        }
                    }
                    else if (animator.GetBool("open") == true)
                    {
                        select.text = "[F]閉める";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", false);
                        }
                    }


                }
                else if (hit.collider.CompareTag("Shelf"))
                {

                    Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                    if (animator.GetBool("open") == false)
                    {
                        select.text = "[F]開ける";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", true);
                        }
                    }
                    else if (animator.GetBool("open") == true)
                    {
                        select.text = "[F]閉める";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", false);
                        }
                    }


                }
                else if (hit.collider.CompareTag("tora"))
                {
                    select.text = "[F]回収";
                    if (Input.GetKeyDown("f") && GetComponent<ItemSelect>().tora == false)
                    {
                       GetComponent<ItemSelect>().tora = true;

                        photonView.RPC("DestroyObject", RpcTarget.MasterClient, hit.collider.gameObject.GetComponent<PhotonView>().ViewID);

                    }
                }
                else
                {
                    // DOAじゃないオブジェクトに当たった時は非表示
                    select.text = "";
                }
            }
            else if (Is_PlayMode)
            {
                // 何にも当たらなかったら非表示
                select.text = "";
            }



            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                MoveSpeed = 8;
                animator.SetBool("dash", true);
                if (camera.GetComponent<Camera>().fieldOfView < 80)
                {
                    camera.GetComponent<Camera>().fieldOfView += 80f * Time.deltaTime;
                }
            }
            else
            {
                animator.SetBool("dash", false);
                MoveSpeed = 5;
                if (camera.GetComponent<Camera>().fieldOfView > 60)
                {
                    camera.GetComponent<Camera>().fieldOfView -= 50f * Time.deltaTime;
                }
            }

        }
        else
        {
            playerCanvas.enabled = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.red);

        
        //error fix
        //camera_Object.transform.rotation = Quaternion.Euler(-ver, transform.eulerAngles.y, 0f);
    }

    [PunRPC]
    void DestroyObject(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }



}
