using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    public AudioSource se;
    public GameObject tile;         //�^�C���̃I�u�W�F�N�g
    public GameObject mashA;        //�F�̃}�V���̃I�u�W�F�N�g
    public GameObject mashB;        //�ΐF�̃}�V���̃I�u�W�F�N�g
    public GameObject bar;          //�o�[�̃I�u�W�F�N�g
    class Mash {
        //int counter;              //���C���֐����Ăяo������
        int boardX;                 //�ՖʑS�̂�X�T�C�Y
        int boardY;                 //�ՖʑS�̂�Y�T�C�Y
        int startX;                 //�X�^�[�g�n�_��X���W
        int startY;                 //�X�^�[�g�n�_��Y���W
        int posX;                   //���ݒn�_��X���W
        int posY;                   //���ݒn�_��Y���W
        int stageSelect;            //�I�𒆂̃X�e�[�W
        string[] stageData;         //�Ֆʂ̔z�u�̃f�[�^(�e�L�X�g)
        int[,] tileBoard;           //�Ֆʂ̔z�u�̃f�[�^(0:�����A1:�X�^�[�g�n�_�A2�ȍ~:�}�V��)
        int[,] rootBoard;           //�Ֆʂ̃��[�g�̃f�[�^
        GameObject bar;             //�o�[�̃I�u�W�F�N�g
        GameObject[,] objBoard;     //�Ֆʂ̃I�u�W�F�N�g�f�[�^
        Stack<GameObject> barList;  //�o�[�̃I�u�W�F�N�g�f�[�^
        List<Vector2Int> mashPos;   //�}�V���̍��W�f�[�^�̃��X�g
        Material fade;              //�I�𒆂̃t�F�[�h����
        public Mash() {
            //counter = 0;
            boardX = 5;
            boardY = 5;
            tileBoard = new int[boardX, boardY];
            rootBoard = new int[boardX, boardY];
            objBoard = new GameObject[boardX, boardY];
            barList = new Stack<GameObject>();
            mashPos = new List<Vector2Int>();
            stageSelect = 0;
            stageData = new string[] {
                "0002000020001000000000020",
                "0000003000301030300300000",
                "0003000000001032002003000",
                "0300000203021303000200030",
                "2033032003021033020022022",
            };
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    rootBoard[x, y] = 0;
                    tileBoard[x, y] = int.Parse(stageData[stageSelect].Substring(x * boardX + y, 1));
                    if (tileBoard[x, y] == 1) {
                        startX = x;
                        startY = y;
                        posX = x;
                        posY = y;
                    }
                    if (tileBoard[x, y] >= 2) {
                        mashPos.Add(new(x, y));
                    }
                }
            }
        }
        public void Main() {
            InputKey();
            fade.SetColor("_EmissionColor", Color.HSVToRGB(0.0f, 0.0f, Mathf.Abs(Mathf.Sin(Time.time * 3.0f))));
            //counter++;
        }
        public int InputKey() {
            int x = 0, y = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow) && posX > 0) x = -1;
            if (Input.GetKeyDown(KeyCode.DownArrow) && posY > 0) y = -1;
            if (Input.GetKeyDown(KeyCode.RightArrow) && posX < boardX - 1) x = 1;
            if (Input.GetKeyDown(KeyCode.UpArrow) && posY < boardY - 1) y = 1;
            if (((x ^ y) & 1) == 1) {                                               //x��y���ǂ��炩�Е�����0�̂Ƃ�
                if (rootBoard[posX, posY] + 2 * x + y == 0) {                       //�o�[��P������Ƃ�
                    rootBoard[posX, posY] = 0;
                    posX += x;
                    posY += y;
                    Destroy(barList.Pop());
                } else {
                    //�}�V���̌���
                    if (tileBoard[posX, posY] >= 2) {
                        x *= (rootBoard[posX, posY] + tileBoard[posX, posY]) % 2;
                        y *= (rootBoard[posX, posY] + tileBoard[posX, posY] + 1) % 2;
                    }
                    if (rootBoard[posX + x, posY + y] == 0) {                       //�V�����o�[��ݒu����Ƃ�
                        posX += x;
                        posY += y;
                        //�o�[�I�u�W�F�N�g�𐶐�
                        Vector3 pos = new(1.0f * (posX - 0.5f * x - boardX / 2), 0.04f, 1.0f * (posY - 0.5f * y - boardY / 2));
                        Quaternion rot = Quaternion.Euler(0, 90f * x, 0);
                        barList.Push(Instantiate(bar, pos, rot));
                        rootBoard[posX, posY] = 2 * x + y;                          //��:-1�A��:1�A��:-2�A�E:2
                    }
                }
                //�t�F�[�h�^�C���ύX
                fade.SetColor("_EmissionColor", new Color(0, 0, 0));
                fade = objBoard[posX, posY].GetComponentsInChildren<Renderer>()[1].material;
                fade.EnableKeyword("_EMISSION");
                //�N���A����
                if (posX == startX && posY == startY) {
                    int flag = 0;
                    foreach (Vector2Int v in mashPos) {
                        if (rootBoard[v.x, v.y] != 0) flag++;
                    }
                    Debug.Log(flag);
                    if (flag == mashPos.Count) return 1;
                }
                //�f�o�b�O�p
                //string debug = "";
                //for (y = boardY - 1; y >= 0; y--) {
                //    for (x = 0; x < boardX; x++)
                //        debug += "" + (rootBoard[x, y] != 0 ? (rootBoard[x, y] + 3) : 0);
                //    debug += "\n";
                //}
                //Debug.Log(debug);
            }
            return 0;
        }
        public void MakeBoard(GameObject tile, GameObject mashA, GameObject mashB) {
            Vector3 pos;
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    //�^�C���̔z�u
                    pos = new Vector3(1.0f * (x - boardX / 2), 0.0f, 1.0f * (y - boardY / 2));
                    objBoard[x, y] = Instantiate(tile, pos, Quaternion.identity);
                    //�}�V���̔z�u
                    if (tileBoard[x, y] >= 2) {
                        pos = new Vector3(1.0f * (x - boardX / 2), 0.051f, 1.0f * (y - boardY / 2));
                        Instantiate(tileBoard[x, y] == 2 ? mashA : mashB, pos, Quaternion.identity);
                    }
                }
            }
            //�I�𒆂̃^�C�����t�F�[�h�����邽�߂̏���
            fade = objBoard[startX, startY].GetComponentsInChildren<Renderer>()[1].material;
            fade.EnableKeyword("_EMISSION");
        }
        public void BarSet(GameObject barObj) {
            bar = barObj;
        }
    }

    Mash mash;

    void Start() {
        mash = new();
        mash.MakeBoard(tile, mashA, mashB);
        mash.BarSet(bar);
        //
        //Debug.Log("" + -1 + "%" + 2 + "=" + (-1 % 2));
        //for (int x = -1; x <= 1; x++)
        //    for (int y = -1; y <= 1; y++)
        //        Debug.Log("" + x + "^" + y + "&1=" + ((x ^ y) & 1));
    }

    void Update() {
        mash.Main();
    }
}
