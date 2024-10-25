using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kabehantei : MonoBehaviour
{
    public float speed = 5f; // プレイヤーの移動速度
    private Vector2 moveDirection; // 移動方向
    private Rigidbody2D rb; // Rigidbody2Dの参照

    // 移動方向を制御するためのフラグ
    private bool canMoveUp = true;
    private bool canMoveDown = true;
    private bool canMoveLeft = true;
    private bool canMoveRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dの取得
    }

    // Update is called once per frame
    void Update()
    {
        // 入力を取得
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 移動方向を制限
        if ((moveX < 0 && !canMoveLeft) || (moveX > 0 && !canMoveRight))
        {
            moveX = 0;
        }
        if ((moveY < 0 && !canMoveDown) || (moveY > 0 && !canMoveUp))
        {
            moveY = 0;
        }

        moveDirection = new Vector2(moveX, moveY).normalized; // 正規化して移動方向を設定
    }

    void FixedUpdate()
    {
        // 移動処理
        rb.velocity = moveDirection * speed; // Rigidbody2Dの速度を設定
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁との衝突を検知
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            // 衝突した方向によって移動を制限
            if (collisionNormal == Vector2.up)
            {
                canMoveDown = false; // 下方向の移動を無効化
            }
            else if (collisionNormal == Vector2.down)
            {
                canMoveUp = false; // 上方向の移動を無効化
            }
            else if (collisionNormal == Vector2.left)
            {
                canMoveRight = false; // 右方向の移動を無効化
            }
            else if (collisionNormal == Vector2.right)
            {
                canMoveLeft = false; // 左方向の移動を無効化
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 壁から離れたときに移動制限を解除
        if (collision.gameObject.CompareTag("Wall"))
        {
            canMoveUp = true;
            canMoveDown = true;
            canMoveLeft = true;
            canMoveRight = true;
        }
    }
}
