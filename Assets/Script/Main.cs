using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Main : MonoBehaviour {
    public AudioSource se;
    public AudioClip click;
    public AudioClip prev;
    public AudioClip ops;
    public GameObject tile;         //�^�C���̃I�u�W�F�N�g
    public GameObject mashA;        //�F�̃}�V���̃I�u�W�F�N�g
    public GameObject mashB;        //�ΐF�̃}�V���̃I�u�W�F�N�g
    public GameObject bar;          //�o�[�̃I�u�W�F�N�g
    public GameObject mark;         //�i���p�̃I�u�W�F�N�g
    public GameObject noteA;
    public GameObject noteC;
    public GameObject noteE;
    public GameObject noteL;
    public GameObject noteR;
    public TextMeshPro text;
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
        int undoCount;              //�߂�����
        int missCount;              //�~�X������
        float startTime;            //�n�߂�����
        GameObject bar;             //�o�[�̃I�u�W�F�N�g
        GameObject[,] objBoard;     //�Ֆʂ̃I�u�W�F�N�g�f�[�^
        Stack<GameObject> barList;  //�o�[�̃I�u�W�F�N�g�f�[�^
        List<Vector2Int> mashPos;   //�}�V���̍��W�f�[�^�̃��X�g
        Material fade;              //�I�𒆂̃t�F�[�h����
        AudioSource se;
        AudioClip click;
        AudioClip prev;
        AudioClip ops;
        public Mash(int n) {
            //counter = 0;
            startTime = Time.time;
            undoCount = 0;
            missCount = 0;
            boardX = 5;
            boardY = 5;
            tileBoard = new int[boardX, boardY];
            rootBoard = new int[boardX, boardY];
            objBoard = new GameObject[boardX, boardY];
            barList = new Stack<GameObject>();
            mashPos = new List<Vector2Int>();
            stageSelect = n;
            stageData = new string[] {
                "0000000200000000010000000",
                "0000001020000000202000000",
                "0002000020001000000000020",
                "2000202020000000202020102",
                "0000003000000000001000000",
                "0030000000300030000000100",
                "0000003000301030300300000",
                "0300000103030303000000030",
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
        public int Main() {
            int key, move;
            //�N���A����
            if (posX == startX && posY == startY && CheckGoal() == 1) return 1;
            key = InputKey();
            //Debug.Log(key);
            if (key != 0) {
                move = MoveBoard(key);
                if (move == 0) {
                    se.PlayOneShot(ops);                 //�����ĂȂ��Ƃ�
                    missCount++;
                }
                if (move == 1) se.PlayOneShot(click);    //�������Ƃ�
                if (move == 2) {
                    se.PlayOneShot(prev);                //�߂����Ƃ�
                    undoCount++;
                }
            }
            return 0;
            //fade.SetColor("_EmissionColor", Color.HSVToRGB(0.0f, 0.0f, Mathf.Abs(Mathf.Sin(Time.time * 3.0f))));
            //counter++;
        }
        int InputKey() {
            int flag = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) flag += 1;
            if (Input.GetKeyDown(KeyCode.DownArrow)) flag += 2;
            if (Input.GetKeyDown(KeyCode.RightArrow)) flag += 4;
            if (Input.GetKeyDown(KeyCode.UpArrow)) flag += 8;
            return flag;
        }
        int MoveBoard(int key) {
            int x = 0, y = 0;
            if ((key & 1) == 1 && posX > 0) x = -1;
            if ((key & 2) == 2 && posY > 0) y = -1;
            if ((key & 4) == 4 && posX < boardX - 1) x = 1;
            if ((key & 8) == 8 && posY < boardY - 1) y = 1;
            if (((x ^ y) & 1) == 1) {                                               //x��y���ǂ��炩�Е�����0�̂Ƃ�
                if (rootBoard[posX, posY] + 2 * x + y == 0) {                       //�o�[��P������Ƃ�
                    rootBoard[posX, posY] = 0;
                    posX += x;
                    posY += y;
                    Destroy(barList.Peek(), barList.Peek().GetComponent<Fade>().Wait());
                    barList.Pop().GetComponent<Fade>().DestInit();
                    return 2;
                }
                //�}�V���̌���
                if (tileBoard[posX, posY] >= 2) {
                    x *= (rootBoard[posX, posY] + tileBoard[posX, posY]) % 2;
                    y *= (rootBoard[posX, posY] + tileBoard[posX, posY] + 1) % 2;
                }
                if (rootBoard[posX + x, posY + y] == 0) {                       //�V�����o�[��ݒu����Ƃ�
                    posX += x;
                    posY += y;
                    //�o�[�I�u�W�F�N�g�𐶐�
                    Vector3 pos = new(1.0f * (posX - 0.5f * x - boardX / 2), -0.1f, 1.0f * (posY - 0.5f * y - boardY / 2));
                    //Vector3 pos = new(1.0f * (posX - boardX / 2), -0.1f, 1.0f * (posY - boardY / 2));
                    Quaternion rot = Quaternion.Euler(0, 90f * x, 0);
                    //Quaternion rot = Quaternion.identity;
                    barList.Push(Instantiate(bar, pos, rot));
                    //pos = objBoard[posX, posY].transform.position;
                    //pos = new(pos.x, pos.y - 0.05f, pos.z);
                    //objBoard[posX, posY].transform.position = pos;
                    rootBoard[posX, posY] = 2 * x + y;                          //��:-1�A��:1�A��:-2�A�E:2
                    return 1;
                }
                //�t�F�[�h�^�C���ύX
                //fade.SetColor("_EmissionColor", new Color(0, 0, 0));
                //fade = objBoard[posX, posY].GetComponentsInChildren<Renderer>()[1].material;
                //fade.EnableKeyword("_EMISSION");
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
        int CheckGoal() {
            int flag = 0;
            foreach (Vector2Int v in mashPos)
            {
                if (rootBoard[v.x, v.y] != 0) flag++;
            }
            //Debug.Log(flag);
            if (flag == mashPos.Count) return 1;
            return 0;
        }
        public void MakeBoard(GameObject tile, GameObject mashA, GameObject mashB) {
            Vector3 pos;
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    if (tileBoard[x, y] >= 2) {
                        //�}�V���̔z�u
                        pos = new Vector3(1.0f * (x - boardX / 2), 0.0f, 1.0f * (y - boardY / 2));
                        objBoard[x, y] = Instantiate(tileBoard[x, y] == 2 ? mashA : mashB, pos, Quaternion.identity);
                    } else {
                        //�^�C���̔z�u
                        pos = new Vector3(1.0f * (x - boardX / 2), -0.05f, 1.0f * (y - boardY / 2));
                        objBoard[x, y] = Instantiate(tile, pos, Quaternion.identity);
                    }
                }
            }
            //�I�𒆂̃^�C�����t�F�[�h�����邽�߂̏���
            //fade = objBoard[startX, startY].GetComponentsInChildren<Renderer>()[1].material;
            //fade.EnableKeyword("_EMISSION");
        }
        public void DelBoard() {
            for (int x = 0; x < boardX; x++) {
                for (int y = 0; y < boardY; y++) {
                    Destroy(objBoard[x, y], objBoard[x, y].GetComponent<Fade>().Wait());
                    objBoard[x, y].GetComponent<Fade>().DestInit();
                }
            }
            for (; barList.Count > 0; ) {
                Destroy(barList.Peek(), barList.Peek().GetComponent<Fade>().Wait());
                barList.Pop().GetComponent<Fade>().DestInit();
            }
        }
        public void BarSet(GameObject barObj) {
            bar = barObj;
        }
        public void AudioSet(AudioSource lis, AudioClip se1, AudioClip se2, AudioClip se3) {
            se = lis;
            click = se1;
            prev = se2;
            ops = se3;
        }
        public int StageGet() {
            return stageData.Length;            //�X�e�[�W����Ԃ�
        }
        public int MissGet() {
            return missCount;                   //�~�X�񐔂�Ԃ�
        }
        public int UndoGet() {
            return undoCount;                   //��蒼���񐔂�Ԃ�
        }
        public float TimeGet() {
            return Time.time - startTime;       //�o�ߎ��Ԃ�Ԃ�
        }
    }

    Mash mash;
    List<GameObject> markList;
    List<GameObject> clearList;
    GameObject markObj;
    int f;                              //�t���O
    int stage;                          //���̃X�e�[�W
    int stageCount;                     //�X�e�[�W��
    int miss, undo;                     //�~�X�񐔁A��蒼����
    float time;                         //�o�ߎ���
    void Start() {
        f = 0;
        stage = 0;
        miss = 0;
        undo = 0;
        time = 0;
        MashInit(stage);
        stageCount = mash.StageGet();
        markList = new List<GameObject>();
        for (int i = 0; i < stageCount; i++) {
            markList.Add(Instantiate(tile, new(-5.0f + 10.0f * i / (stageCount - 1), 0, 4.0f), Quaternion.identity));
        }
        markObj = Instantiate(mark, new(-5.0f + 10.0f * stage / (stageCount - 1), 0, 4.0f), Quaternion.identity);
        //Debug.Log("" + -1 + "%" + 2 + "=" + (-1 % 2));
        //for (int x = -1; x <= 1; x++)
        //    for (int y = -1; y <= 1; y++)
        //        Debug.Log("" + x + "^" + y + "&1=" + ((x ^ y) & 1));
    }

    void Update() {
        if (f == 0) {
            if (mash.Main() == 1) {
                f = 1;
                miss += mash.MissGet();
                undo += mash.UndoGet();
                time += mash.TimeGet();
                mash.DelBoard();
                Destroy(markObj, markObj.GetComponent<Fade>().Wait());
                markObj.GetComponent<Fade>().DestInit();
                //�S�N���A�����Ƃ�
                if (stage == stageCount - 1) {
                    for (int i = 0; i < stageCount; i++) {
                        Destroy(markList[i], markList[i].GetComponent<Fade>().Wait());
                        markList[i].GetComponent<Fade>().DestInit();
                    }
                    clearList = new List<GameObject>();
                    string[] putStr = {"PutC", "PutL", "PutE", "PutA", "PutR" };
                    for (int i = 0; i < putStr.Length; i++) {
                        Invoke(putStr[i], 0.2f + 0.1f * i);
                    }
                    Invoke("DelClear", 5.0f);
                    Invoke("PutText", 6.0f);
                }
            }
        } else {
            if (stage < stageCount - 1) {
                f = 0;
                MashInit(++stage);
                markObj = Instantiate(mark, new(-5.0f + 10.0f * stage / (stageCount - 1), 0, 4.0f), Quaternion.identity);
            }
        }
    }
    void MashInit(int n) {
        mash = new(n);
        mash.MakeBoard(tile, mashA, mashB);
        mash.BarSet(bar);
        mash.AudioSet(se, click, prev, ops);
    }
    void PutC() {clearList.Add(Instantiate(noteC, new(-5.5f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutL() {clearList.Add(Instantiate(noteL, new(-3.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutE() {clearList.Add(Instantiate(noteE, new(-1.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutA() {clearList.Add(Instantiate(noteA, new(1.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutR() {clearList.Add(Instantiate(noteR, new(3.5f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void DelClear() {
        for (int i = 0; i < clearList.Count; i++) {
            Destroy(clearList[i], clearList[i].GetComponent<Fade>().Wait());
            clearList[i].GetComponent<Fade>().DestInit();
        }
    }
    void TextPut()
    {
        int score = 1919;
        text.text = "Miss:" + miss + ",Undo:" + undo + "\nTime:" + time + "\n\tScore:" + score;
    }
}
