using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontrol : MonoBehaviourPun
{
    [SerializeField] private GameObject myCameraObject; // プレハブ内のCameraオブジェクト
    [SerializeField] private AudioListener myAudioListener; // カメラについているAudioListener

    // Start is called before the first frame update
    void Start()
    {
        // photonViewが取得できない場合の安全策
        if (photonView == null) return;

        if (photonView.IsMine)
        {
            // 自分のキャラの場合：カメラと耳（AudioListener）を有効化
            if (myCameraObject != null) myCameraObject.SetActive(true);
            if (myAudioListener != null) myAudioListener.enabled = true;
        }
        else
        {
            // 他人のキャラの場合：カメラと耳を無効化（他人の視界にならないようにする）
            if (myCameraObject != null) myCameraObject.SetActive(false);
            if (myAudioListener != null) myAudioListener.enabled = false;
        }
    }
}
