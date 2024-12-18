using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Playermove : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    private float distance = 0.5f;
    private Vector2 move;
    private Vector3 targetPos;
    private void Start()
    {
        targetPos = transform.position;
    }
    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
        if (move != Vector2.zero && transform.position == targetPos)
        {
            targetPos += new Vector3(move.x, move.y, 0) * distance;
        }
        Move(targetPos);

        RaycastHit hit;
    }
    private void Move(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition,
            _speed * Time.deltaTime);
    }
}
