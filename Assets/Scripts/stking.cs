using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class stking : MonoBehaviour
{
    // キャラクター（追従する対象）を指定するための変数
    public Transform target;

    // カメラとキャラクターの間のオフセット（位置の差）を指定する変数
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        // キャラクターの位置 + オフセットをカメラの位置に設定
        transform.position = target.position + offset;
    }
}
