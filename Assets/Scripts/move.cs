using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float moveTime = 0.1f;
    public bool isMoving = false;
    public float speed = 1f; // ˆÚ“®‘¬“x‚ğ’²®‚·‚é•Ï”

    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            ATMove(horizontal, vertical);
        }
    }

    public void ATMove(int horizontal, int vertical)
    {
        RaycastHit2D hit;

        bool canMove = Move(horizontal, vertical, out hit);

        if (hit.transform == null)
        {
            return;
        }
    }

    public bool Move(int horizontal, int vertical, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(horizontal, vertical);
        boxCollider.enabled = false;

        hit = Physics2D.Linecast(start, end, blockingLayer);

        boxCollider.enabled = true;

        if (!isMoving && hit.transform == null)
        {
            StartCoroutine(Movement(end));
            return true;
        }

        return false;
    }

    IEnumerator Movement(Vector3 end)
    {
        isMoving = true;
        float remainingDistance = (transform.position - end).sqrMagnitude;

        while (remainingDistance > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime); // ‘¬“x‚ğ”½‰f
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Exit")
        {
            Invoke("Restart", 1f);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
