using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagtarget : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // ���E�̓��� (A/D �܂��� ��/�� �L�[)
        float moveY = Input.GetAxis("Vertical");   // �㉺�̓��� (W/S �܂��� ��/�� �L�[)
        if (moveX != 0)
        {
            moveY = 0;
        }
        Vector2 move = new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
