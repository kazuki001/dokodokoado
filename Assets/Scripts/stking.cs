using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class stking : MonoBehaviour
{
    // �L�����N�^�[�i�Ǐ]����Ώہj���w�肷�邽�߂̕ϐ�
    public Transform target;

    // �J�����ƃL�����N�^�[�̊Ԃ̃I�t�Z�b�g�i�ʒu�̍��j���w�肷��ϐ�
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
        // �L�����N�^�[�̈ʒu + �I�t�Z�b�g���J�����̈ʒu�ɐݒ�
        transform.position = target.position + offset;
    }
}
