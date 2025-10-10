using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraControl : MonoBehaviour
{
    [SerializeField] GameObject PlayerObject;
    [SerializeField] List<GameObject> FloorCameras;
    bool useFixed = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            useFixed = !useFixed;

            if (useFixed)
            {
                // 定点カメラへ移動
                transform.SetParent(null);
                transform.position = FloorCameras[0].transform.position;
                transform.rotation = FloorCameras[0].transform.rotation;
            }
            else
            {
                // プレイヤーに戻る
                transform.SetParent(PlayerObject.transform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
