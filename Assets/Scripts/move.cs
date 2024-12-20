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

    //Mapの縦横
    public int columns = 8;
    public int rows = 8;

    //生成するアイテムの個数
    public Count Wallcount = new Count(3, 7);
    public Count foodcount = new Count(1, 3);
    public Count enemycount = new Count(1, 2);


    //MAPの材料
    public GameObject exit;
    public GameObject floor;
    public GameObject Wall;
    public GameObject OuterWall;
    public GameObject enemy;
    public GameObject food;

    //変換用
    private Transform boardHolder;

    //6×6のマスでobjectがない場所を管理する
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

                // プレイヤーを "d-exit" の位置に即時転移
                transform.position = target.transform.position;

                // 移動中フラグをリセット
                isMoving = false;
            }
        }
        else if (collision.tag == "d-enter")
        {
            GameObject target = GameObject.FindGameObjectWithTag("enter");
            if (target != null)
            {
                StopAllCoroutines();

                // プレイヤーを "enter" の位置に即時転移
                transform.position = target.transform.position;

                // 移動中フラグをリセット
                isMoving = false;
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "enter")
        {
            // すでに処理済みの場合は何もしない
            if (hasEntered) return;

            // 一度処理したことを記録
            hasEntered = true;

            // マップをリセットする
            ResetMap();

            // リストを初期化
            InitialiseList();

            // ボードの生成
            BoardSetup();

            // ランダムに壁を生成
            LayoutObjectAtRandom(Wall, Wallcount.minmum, Wallcount.maximum);

            // ランダムに食べ物を生成
            LayoutObjectAtRandom(food, foodcount.minmum, foodcount.maximum);

            // ランダムに敵を生成
            LayoutObjectAtRandom(enemy, enemycount.minmum, enemycount.maximum);

            // 出口を配置
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
        }
    }

    void ResetMap()
    {
        if (boardHolder != null)
        {
            // boardHolderの子オブジェクトを全て削除
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
        //Boardをインスタンス化してboardHolderに設定
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //床を設置してインスタンス化の準備
                GameObject toInsutantiate = floor;

                //8×8マス外は外壁を設置してインスタンス化の準備
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInsutantiate = OuterWall;
                }

                //toInsutantiateに設定されたものをインスタンス化
                GameObject instance =
                    Instantiate(toInsutantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                //インスタンス化された床or外壁の親要素をboardHolderに設定
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    void InitialiseList()
    {
        //リストをクリアする
        gridPositons.Clear();

        //6×6のますをリストに取得する
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                //gridPositionsにx,yの値をいれる
                gridPositons.Add(new Vector3(x, y, 0));
            }
        }

    }

    //gridPositionsからランダムな位置を取得する
    Vector3 RandomPosition()
    {
        //randomIndexを宣言して、gridPositionsの数から数値をランダムで入れる
        int randomIndex = Random.Range(0, gridPositons.Count);

        //randomPositionを宣言して、gridPositionsのrandomIndexに設定する
        Vector3 randomPosition = gridPositons[randomIndex];

        //使用したgridPositionsの要素を削除
        gridPositons.RemoveAt(randomIndex);

        return randomPosition;
    }

    //Mapにランダムで引数のものを配置する(敵、壁、アイテム)
    void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum)
    {
        //生成するアイテムの個数を最小最大値からランダムに決め、objectCountに設定する
        int objectCount = Mathf.Min(Random.Range(minimum, maximum), gridPositons.Count);

        //設置するオブジェクトの数分ループで回す
        for (int i = 0; i < objectCount; i++)
        {
            //現在オブジェクトが置かれていない、ランダムな位置を取得
            Vector3 randomPosition = RandomPosition();

            //生成
            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }
}
