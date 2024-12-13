using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Map������script���L�q����
public class BoardManager : MonoBehaviour
{
    //Map��Ƀ����_����������A�C�e���ŏ��l�A�ő�l�����߂�class
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

    //Map�̏c��
    public int columns = 8;
    public int rows = 8;

    //��������A�C�e���̌�
    public Count Wallcount = new Count(3, 9);
    public Count foodcount = new Count(1, 5);

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
        //Map�̐���
        BoardSetup();

        //gridPositions�̃N���A�ƍĎ擾
        InitialiseList();

        //�����_���ɕǂ𐶐�
        LayoutObjectAtRandom(Wall, Wallcount.minmum, Wallcount.maximum);

        //�H�ו�����
        LayoutObjectAtRandom(food, foodcount.minmum, foodcount.maximum);

        //�o���̐ݒu
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
    }

    //�t�B�[���h����
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
    //gridPositions���N���A����
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
        int objectCount = Random.Range(minimum, maximum);

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