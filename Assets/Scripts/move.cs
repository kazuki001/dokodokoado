using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class move : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float moveTime = 0.1f;
    public bool isMoving = false;
    public float speed = 1f;

    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;
    
    public class Count
    {
        public int minmum;
        public int maximum;

        public Count(int min, int max)
        {
            minmum = min;
            maximum = max;
        }
    }

    private bool hasEntered = false;

    //Map�̏c��
    public int columns = 8;
    public int rows = 8;

    //��������A�C�e���̌�
    public Count Wallcount = new Count(3, 7);
    public Count foodcount = new Count(1, 3);
    public Count enemycount = new Count(1, 2);


    //MAP�̍ޗ�
    public GameObject exit;
    public GameObject floor;
    public GameObject Wall;
    public GameObject OuterWall;
    public GameObject enemy;
    public GameObject food;

    //�ϊ��p
    private Transform boardHolder;

    //6�~6�̃}�X��object���Ȃ��ꏊ���Ǘ�����
    private List<Vector3> gridPositons = new List<Vector3>();

    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

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
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
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
            GameObject target = GameObject.FindGameObjectWithTag("d-exit");
            if (target != null)
            {
                StopAllCoroutines();

                // �v���C���[�� "d-exit" �̈ʒu�ɑ����]��
                transform.position = target.transform.position;

                // �ړ����t���O�����Z�b�g
                isMoving = false;
            }
        }
        else if (collision.tag == "d-enter")
        {
            GameObject target = GameObject.FindGameObjectWithTag("enter");
            if (target != null)
            {
                StopAllCoroutines();

                // �v���C���[�� "enter" �̈ʒu�ɑ����]��
                transform.position = target.transform.position;

                // �ړ����t���O�����Z�b�g
                isMoving = false;
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "enter")
        {
            // ���łɏ����ς݂̏ꍇ�͉������Ȃ�
            if (hasEntered) return;

            // ��x�����������Ƃ��L�^
            hasEntered = true;

            // �}�b�v�����Z�b�g����
            ResetMap();

            // ���X�g��������
            InitialiseList();

            // �{�[�h�̐���
            BoardSetup();

            // �����_���ɕǂ𐶐�
            LayoutObjectAtRandom(Wall, Wallcount.minmum, Wallcount.maximum);

            // �����_���ɐH�ו��𐶐�
            LayoutObjectAtRandom(food, foodcount.minmum, foodcount.maximum);

            // �����_���ɓG�𐶐�
            LayoutObjectAtRandom(enemy, enemycount.minmum, enemycount.maximum);

            // �o����z�u
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
        }
    }

    void ResetMap()
    {
        if (boardHolder != null)
        {
            // boardHolder�̎q�I�u�W�F�N�g��S�č폜
            foreach (Transform child in boardHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }



    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void BoardSetup()
    {
        //Board���C���X�^���X������boardHolder�ɐݒ�
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //����ݒu���ăC���X�^���X���̏���
                GameObject toInsutantiate = floor;

                //8�~8�}�X�O�͊O�ǂ�ݒu���ăC���X�^���X���̏���
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInsutantiate = OuterWall;
                }

                //toInsutantiate�ɐݒ肳�ꂽ���̂��C���X�^���X��
                GameObject instance =
                    Instantiate(toInsutantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                //�C���X�^���X�����ꂽ��or�O�ǂ̐e�v�f��boardHolder�ɐݒ�
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    void InitialiseList()
    {
        //���X�g���N���A����
        gridPositons.Clear();

        //6�~6�̂܂������X�g�Ɏ擾����
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                //gridPositions��x,y�̒l�������
                gridPositons.Add(new Vector3(x, y, 0));
            }
        }

    }

    //gridPositions���烉���_���Ȉʒu���擾����
    Vector3 RandomPosition()
    {
        //randomIndex��錾���āAgridPositions�̐����琔�l�������_���œ����
        int randomIndex = Random.Range(0, gridPositons.Count);

        //randomPosition��錾���āAgridPositions��randomIndex�ɐݒ肷��
        Vector3 randomPosition = gridPositons[randomIndex];

        //�g�p����gridPositions�̗v�f���폜
        gridPositons.RemoveAt(randomIndex);

        return randomPosition;
    }

    //Map�Ƀ����_���ň����̂��̂�z�u����(�G�A�ǁA�A�C�e��)
    void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum)
    {
        //��������A�C�e���̌����ŏ��ő�l���烉���_���Ɍ��߁AobjectCount�ɐݒ肷��
        int objectCount = Mathf.Min(Random.Range(minimum, maximum), gridPositons.Count);

        //�ݒu����I�u�W�F�N�g�̐������[�v�ŉ�
        for (int i = 0; i < objectCount; i++)
        {
            //���݃I�u�W�F�N�g���u����Ă��Ȃ��A�����_���Ȉʒu���擾
            Vector3 randomPosition = RandomPosition();

            //����
            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }
}
