using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Main : MonoBehaviour {
    public AudioSource se;          //オーディオソース
    public AudioClip click;         //移動時の効果音
    public AudioClip prev;          //やり直し時の効果音
    public AudioClip ops;           //ミス時の効果音
    public GameObject tile;         //タイルのオブジェクト
    public GameObject mashA;        //青色のマシュのオブジェクト
    public GameObject mashB;        //緑色のマシュのオブジェクト
    public GameObject bar;          //バーのオブジェクト
    public GameObject mark;         //進捗用のオブジェクト
    public GameObject start;        //スタートマーカーのオブジェクト
    public GameObject noteA;        //文字のオブジェクトデータ
    public GameObject noteC;        //文字のオブジェクトデータ
    public GameObject noteE;        //文字のオブジェクトデータ
    public GameObject noteL;        //文字のオブジェクトデータ
    public GameObject noteR;        //文字のオブジェクトデータ
    public TextMeshPro text;        //テキスト表示用の変数
    class Mash {
        //int counter;              //メイン関数を呼び出した回数
        int boardX;                 //盤面全体のXサイズ
        int boardY;                 //盤面全体のYサイズ
        int startX;                 //スタート地点のX座標
        int startY;                 //スタート地点のY座標
        int posX;                   //現在地点のX座標
        int posY;                   //現在地点のY座標
        int stageSelect;            //選択中のステージ
        string[] stageData;         //盤面の配置のデータ(テキスト)
        int[,] tileBoard;           //盤面の配置のデータ(0:無し、1:スタート地点、2以降:マシュ)
        int[,] rootBoard;           //盤面のルートのデータ
        int undoCount;              //戻った回数
        int missCount;              //ミスした回数
        float startTime;            //始めた時間
        GameObject bar;             //バーのオブジェクト
        GameObject[,] objBoard;     //盤面のオブジェクトデータ
        Stack<GameObject> barList;  //バーのオブジェクトデータ
        List<Vector2Int> mashPos;   //マシュの座標データのリスト
        //Material fade;            //選択中のフェード処理
        GameObject objStart;        //スタートマーカーのオブジェクトデータ
        AudioSource se;             //オーディオソース
        AudioClip click;            //移動時の効果音
        AudioClip prev;             //やり直し時の効果音
        AudioClip ops;              //ミス時の効果音
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
                "0000003000000000001000000",
                "0000001020000000202000000",
                "0030000000300030000000100",
                "2000202020000000202020102",
                "0000003000301030300300000",
                "0300000103030303000000030",
                "0003000000001032002003000",
                "0300000203021303000200030",
                "2033032003021033020022022",
            };
            for (int y = 0; y < boardY; y++) {
                for (int x = 0; x < boardX; x++) {
                    rootBoard[x, y] = 0;
                    tileBoard[x, y] = int.Parse(stageData[stageSelect].Substring(x + boardX * y, 1));
                    if (tileBoard[x, y] == 1) {
                        startX = x;
                        startY = y;//boardY - y - 1;
                        posX = startX;
                        posY = startY;
                    }
                    if (tileBoard[x, y] >= 2) {
                        mashPos.Add(new(x, y));
                    }
                }
            }
        }
        public int Main() {
            int key, move;
            //クリア判定
            if (posX == startX && posY == startY && rootBoard[posX, posY] != 0 && CheckGoal() == 1) return 1;
            if (posX != startX || posY != startY) {
                foreach (Vector2Int v in mashPos) {
                    //if (tileBoard[v.x, v.y] == 2) objBoard[v.x, v.y].GetComponent<Renderer>().material.color = new Color(0.5801887f, 0.6586151f, 1.0f);
                    //if (tileBoard[v.x, v.y] == 3) objBoard[v.x, v.y].GetComponent<Renderer>().material.color = new Color(1.0f, 0.5801887f, 0.5801887f);
                    objBoard[v.x, v.y].GetComponent<MatChg>().ChgStop();
                }
            }
            key = InputKey();
            //Debug.Log(key);
            if (key != 0) {
                move = MoveBoard(key);
                if (move == 0) {
                    se.PlayOneShot(ops);                 //動いてないとき
                    missCount++;
                }
                if (move == 1) se.PlayOneShot(click);    //動いたとき
                if (move == 2) {
                    se.PlayOneShot(prev);                //戻ったとき
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
            if (Input.GetKeyDown(KeyCode.UpArrow)) flag += 2;
            if (Input.GetKeyDown(KeyCode.RightArrow)) flag += 4;
            if (Input.GetKeyDown(KeyCode.DownArrow)) flag += 8;
            if (Input.GetKeyDown(KeyCode.RightControl)) flag += 16;
            return flag;
        }
        int MoveBoard(int key) {
            int x = 0, y = 0;
            if ((key & 1) == 1 && posX > 0) x = -1;
            if ((key & 2) == 2 && posY > 0) y = -1;
            if ((key & 4) == 4 && posX < boardX - 1) x = 1;
            if ((key & 8) == 8 && posY < boardY - 1) y = 1;
            if ((key & 16) == 16 && rootBoard[posX, posY] != 0) {
                x = -1 * (rootBoard[posX, posY] + 1) % 2;
                y = -1 * rootBoard[posX, posY] % 2;
            }
            if (((x ^ y) & 1) == 1) {                                               //xかyがどちらか片方だけ0のとき
                if (rootBoard[posX, posY] + 2 * x + y == 0) {                       //バーを撤去するとき
                    rootBoard[posX, posY] = 0;
                    posX += x;
                    posY += y;
                    Destroy(barList.Peek(), barList.Peek().GetComponent<Fade>().Wait());
                    barList.Pop().GetComponent<Fade>().DestInit();
                    return 2;
                }
                //マシュの効果
                if (tileBoard[posX, posY] >= 2) {
                    x *= (rootBoard[posX, posY] + tileBoard[posX, posY]) % 2;
                    y *= (rootBoard[posX, posY] + tileBoard[posX, posY] + 1) % 2;
                }
                if (rootBoard[posX + x, posY + y] == 0) {                           //新しくバーを設置するとき
                    posX += x;
                    posY += y;
                    //バーオブジェクトを生成
                    Vector3 pos = new(1.0f * (posX - 0.5f * x - boardX / 2), -0.1f, -1.0f * (posY - 0.5f * y - boardY / 2));
                    //Vector3 pos = new(1.0f * (posX - boardX / 2), -0.1f, 1.0f * (posY - boardY / 2));
                    Quaternion rot = Quaternion.Euler(0, 90f * x, 0);
                    //Quaternion rot = Quaternion.identity;
                    barList.Push(Instantiate(bar, pos, rot));
                    //pos = objBoard[posX, posY].transform.position;
                    //pos = new(pos.x, pos.y - 0.05f, pos.z);
                    //objBoard[posX, posY].transform.position = pos;
                    rootBoard[posX, posY] = 2 * x + y;                              //上:-1、下:1、左:-2、右:2
                    return 1;
                }
                //フェードタイル変更
                //fade.SetColor("_EmissionColor", new Color(0, 0, 0));
                //fade = objBoard[posX, posY].GetComponentsInChildren<Renderer>()[1].material;
                //fade.EnableKeyword("_EMISSION");
                //デバッグ用
                //string debug = "";
                //for (y = boardY - 1; y >= 0; y--) {
                //    for (x = 0; x < boardX; x++)
                //        debug += "" + tileBoard[x, y];
                //    debug += "\n";
                //}
                //Debug.Log(debug);
            }
            return 0;
        }
        int CheckGoal() {
            int flag = 0;
            //float f = Mathf.Sin(Time.time * 2) / 4 + 0.25f;
            //Color c = new(f, f, f);
            foreach (Vector2Int v in mashPos) {
                if (rootBoard[v.x, v.y] != 0) {
                    flag++;
                } else {
                    //objBoard[v.x, v.y].GetComponent<Renderer>().material.color = c;
                    //if (tileBoard[v.x, v.y] == 2) objBoard[v.x, v.y].GetComponent<Renderer>().material.color = new Color(0.5801887f, 0.6586151f, 1.0f) - c;
                    //if (tileBoard[v.x, v.y] == 3) objBoard[v.x, v.y].GetComponent<Renderer>().material.color = new Color(1.0f, 0.5801887f, 0.5801887f) - c;
                    //objBoard[v.x, v.y].GetComponent<Fade>().RotInit();
                    objBoard[v.x, v.y].GetComponent<MatChg>().ChgInit();
                }
            }
            //Debug.Log(flag);
            if (flag == mashPos.Count) return 1;
            return 0;
        }
        public void MakeBoard(GameObject tile, GameObject mashA, GameObject mashB, GameObject start) {
            Vector3 pos;
            for (int y = 0; y < boardY; y++) {
                for (int x = 0; x < boardX; x++) {
                    if (tileBoard[x, y] >= 2) {
                        //マシュの配置
                        pos = new Vector3(1.0f * (x - boardX / 2), 0, -1.0f * (y - boardY / 2));
                        objBoard[x, y] = Instantiate(tileBoard[x, y] == 2 ? mashA : mashB, pos, Quaternion.identity);
                    } else {
                        //タイルの配置
                        pos = new Vector3(1.0f * (x - boardX / 2), -0.05f, -1.0f * (y - boardY / 2));
                        objBoard[x, y] = Instantiate(tile, pos, Quaternion.identity);
                        if (tileBoard[x, y] == 1) {
                            pos = new Vector3(1.0f * (x - boardX / 2), 0.3f, -1.0f * (y - boardY / 2));
                            objStart = Instantiate(start, pos, Quaternion.identity);
                            objStart.GetComponent<Fade>().RotInit();
                        }
                    }
                }
            }
            //選択中のタイルをフェードさせるための処理
            //fade = objBoard[startX, startY].GetComponentsInChildren<Renderer>()[1].material;
            //fade.EnableKeyword("_EMISSION");
        }
        public void DelBoard() {
            for (int y = 0; y < boardY; y++) {
                for (int x = 0; x < boardX; x++) {
                    Destroy(objBoard[x, y], objBoard[x, y].GetComponent<Fade>().Wait());
                    objBoard[x, y].GetComponent<Fade>().DestInit();
                }
            }
            for (; barList.Count > 0; ) {
                Destroy(barList.Peek(), barList.Peek().GetComponent<Fade>().Wait());
                barList.Pop().GetComponent<Fade>().DestInit();
            }
            Destroy(objStart, objStart.GetComponent<Fade>().Wait());
            objStart.GetComponent<Fade>().DestInit();
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
        public int MissGet() {
            return missCount;                   //ミス回数を返す
        }
        public int UndoGet() {
            return undoCount;                   //やり直し回数を返す
        }
        public float TimeGet() {
            return Time.time - startTime;       //経過時間を返す
        }
    }

    Mash mash;
    Stack<GameObject> markList = new Stack<GameObject>();
    Stack<GameObject> clearList = new Stack<GameObject>();
    GameObject markObj;
    int f;                              //フラグ
    int stage;                          //今のステージ
    int stageCount;                     //ステージの個数
    int miss, undo;                     //ミス回数、やり直し回数
    float time;                         //経過時間
    float scene;                        //シーンチェンジ秒数
    void Start() {
        f = 2;
        stage = 0;
        stageCount = 10;
        GameInit();
        //Debug.Log("" + -1 + "%" + 2 + "=" + (-1 % 2));
        //for (int x = -1; x <= 1; x++)
        //    for (int y = -1; y <= 1; y++)
        //        Debug.Log("" + x + "^" + y + "&1=" + ((x ^ y) & 1));
    }

    void Update() {
        int debug = 0;
        if (f == 0 || f == 2) {
            for (int i = 0; i < 12; i++) {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
                    if (stage == stageCount + 1) {
                        if (i >= stageCount) break;
                        f = 2;
                        CancelInvoke();
                        DelClear();
                        GameInit();
                    }
                    stage = i == 0 ? 9 : i - 1;
                    debug = 1;
                    break;
                }
            }
        }
        switch (f) {
            case 0:                                                             //プレイ中
                //クリアしたとき
                if (mash.Main() == 1 || debug == 1) {
                    f = 1;
                    miss += mash.MissGet();
                    undo += mash.UndoGet();
                    time += mash.TimeGet();
                    mash.DelBoard();
                    Destroy(markObj, markObj.GetComponent<Fade>().Wait());
                    markObj.GetComponent<Fade>().DestInit();
                    //全てクリアしたとき
                    if (stage == stageCount) {
                        for (; markList.Count > 0;) {
                            Destroy(markList.Peek(), markList.Peek().GetComponent<Fade>().Wait());
                            markList.Pop().GetComponent<Fade>().DestInit();
                        }
                        string[] putStr = { "PutC", "PutL", "PutE", "PutA", "PutR" };
                        for (int i = 0; i < putStr.Length; i++) {
                            Invoke(putStr[i], 0.3f + 0.1f * i);
                        }
                        Invoke(nameof(DelClear), 3.0f);
                        Invoke(nameof(PutText), 3.5f);
                        stage++;
                    }
                }
                break;
            case 1:                                                             //ステージ移行
                scene += Time.deltaTime;
                if (scene > 0.4f) f = 2;
                break;
            case 2:                                                             //ステージ生成
                if (stage < stageCount) {
                    f = 0;
                    MashInit(stage);        //ステージ作成
                    stage++;
                }
                break;
            default:
                break;
        }
    }
    void GameInit() {
        miss = 0;
        undo = 0;
        time = 0;
        text.text = "";
        for (int i = 0; i < stageCount; i++) {
            markList.Push(Instantiate(tile, new(-5.0f + 10.0f * i / (stageCount - 1), 0, 4.0f), Quaternion.identity));
        }
    }
    void MashInit(int n) {
        scene = 0;
        mash = new(n);
        mash.MakeBoard(tile, mashA, mashB, start);
        mash.BarSet(bar);
        mash.AudioSet(se, click, prev, ops);
        markObj = Instantiate(mark, new(-5.0f + 10.0f * stage / (stageCount - 1), 0, 4.0f), Quaternion.identity);
    }
    void PutC() {clearList.Push(Instantiate(noteC, new(-5.5f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutL() {clearList.Push(Instantiate(noteL, new(-3.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutE() {clearList.Push(Instantiate(noteE, new(-1.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutA() {clearList.Push(Instantiate(noteA, new(1.0f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void PutR() {clearList.Push(Instantiate(noteR, new(3.3f, 0.5f, 0), Quaternion.Euler(-90.0f, 180.0f, 0))); }
    void DelClear() {
        for (; clearList.Count > 0;) {
            Destroy(clearList.Peek(), clearList.Peek().GetComponent<Fade>().Wait());
            clearList.Pop().GetComponent<Fade>().DestInit();
        }
    }
    void PutText() {
        int m = ScoreCalc(miss);
        int u = ScoreCalc(undo);
        int t = ScoreCalc((int)time / 4);
        text.text += "Ability\t\t" + m + "/100\n";
        text.text += "Agility\t\t" + t + "/100\n";
        text.text += "Accuracy\t" + u + "/100\n";
        text.text += "Score\t\t" + (m + u + t) + "/300\n\n";
        text.text += "\t   Thank you\n\t for playing!";
    }
    int ScoreCalc(int n) {
        float t = Mathf.Exp(-0.01f * n);
        return (int)(200.0f * (t / (t + 1)));
    }
}
