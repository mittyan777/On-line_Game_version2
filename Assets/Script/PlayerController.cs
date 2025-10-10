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

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] int gameManager;
    [SerializeField] GameObject camera_Object;
    float side;
    float ver;

    Rigidbody rb;
    Ray ray;
    [SerializeField] GameObject camera;

    [SerializeField] private float rayDistance = 0.01f;
    [SerializeField] private GameObject rayObject;

    [SerializeField] Text select;
    string collar = "red";
    // Start is called before the first frame update
    void Start()
    {
        select = GameObject.FindGameObjectWithTag("selectUI").GetComponent<Text>();
        //Roll Check
        Invoke("test", 5);

    }
    void test()
    {
        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            string role = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

            // 全員に共有する
            photonView.RPC("SetRole", RpcTarget.AllBuffered, role);


        }
    }

    [PunRPC]
    void SetRole(string role)
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



    // Update is called once per frame

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey("w"))
            {
                transform.position += transform.forward * 5 * Time.deltaTime;

            }
            if (Input.GetKey("s"))
            {
                transform.position -= transform.forward * 5 * Time.deltaTime;

            }
            if (Input.GetKey("a"))
            {
                transform.position -= transform.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey("d"))
            {
                transform.position += transform.right * 5 * Time.deltaTime;
            }
        }
    }
    void Update()
    {
        // Ray作成
        ray = new Ray(rayObject.transform.position, rayObject.transform.forward);
        // Sceneビューで赤い線を描画

        if (photonView.IsMine)
        {
            photonView.RPC("SetRay", RpcTarget.AllBuffered);

            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            side += h;
            ver += v;
            ver = Mathf.Clamp(ver, -90f, 90f);
            transform.rotation = Quaternion.Euler(0f, side, 0f);
            camera.transform.rotation = Quaternion.Euler(-ver, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);
            if (gameObject.layer == 7)
            {
                // gameObject.tag = ("Killer");
            }
            else if (gameObject.layer == 6)
            {
                // gameObject.tag = ("Player");
            }

            //if (this.gameObject.tag == "Killer")
            //{
            //    GameObject.Find("killerImage").SetActive(true);
            //    GameObject.Find("PlayerImage").SetActive(false);
            //    GameObject.Find("Player2Image").SetActive(false);
            //}
            //else if (this.gameObject.tag == "Player")
            //{
            //    GameObject.Find("killerImage").SetActive(false);
            //    GameObject.Find("PlayerImage").SetActive(true);
            //    GameObject.Find("Player2Image").SetActive(false);
            //}
            //else if (this.gameObject.tag == "Player2")
            //{
            //    GameObject.Find("killerImage").SetActive(false);
            //    GameObject.Find("PlayerImage").SetActive(false);
            //    GameObject.Find("Player2Image").SetActive(true);
            //}


            RaycastHit hit;
            string sceneName = SceneManager.GetActiveScene().name;

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
                else
                {
                    // DOAじゃないオブジェクトに当たった時は非表示
                    select.text = "";
                }
            }
            else if (sceneName == "main")
            {
                // 何にも当たらなかったら非表示
                select.text = "";
            }





        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.red);

        if (Input.GetKeyDown("c"))
        {
            if (collar == "red")
            {
                photonView.RPC("ChangeColor", RpcTarget.AllBuffered, "blue");
            }
            else if (collar == "blue")
            {
                photonView.RPC("ChangeColor", RpcTarget.AllBuffered, "red");
            }
        }

        GameObject[] Barrier_red = GameObject.FindGameObjectsWithTag("red");
        GameObject[] Barrier_blue = GameObject.FindGameObjectsWithTag("blue");
        if (collar == "red")
        {
            GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.red);
            foreach (GameObject obj in Barrier_red)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
            foreach (GameObject obj in Barrier_blue)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }
            }

        }
        if (collar == "blue")
        {
            GetComponent<Outline>().outlineFillMaterial.SetColor("_OutlineColor", UnityEngine.Color.blue);
            foreach (GameObject obj in Barrier_blue)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
            foreach (GameObject obj in Barrier_red)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }
            }
        }



        //error fix
        //camera_Object.transform.rotation = Quaternion.Euler(-ver, transform.eulerAngles.y, 0f);
    }
    [PunRPC]
    void ChangeColor(string newColor)
    {
        collar = newColor;
        Debug.Log($"色変更同期：{newColor}");
    }

}
