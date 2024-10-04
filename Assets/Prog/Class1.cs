using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float speed = 3.0f;

    void Update()
    {
        // Wキー（前方移動）
        if (Input.GetKey(KeyCode.O))
        {
            transform.position += speed * transform.forward * Time.deltaTime;
        }

        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.L))
        {
            transform.position -= speed * transform.forward * Time.deltaTime;
        }

        // Dキー（右移動）
        if (Input.GetKey(KeyCode.P))
        {
            transform.position += speed * transform.right * Time.deltaTime;
        }

        // Aキー（左移動）
        if (Input.GetKey(KeyCode.K))
        {
            transform.position -= speed * transform.right * Time.deltaTime;
        }
    }
}