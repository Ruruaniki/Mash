using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    public AudioSource se;
    public GameObject tile;         //タイルのオブジェクト
    public GameObject mashA;        //青色のマシュのオブジェクト
    public GameObject mashB;        //緑色のマシュのオブジェクト
    public GameObject bar;          //バーのオブジェクト
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
        GameObject bar;             //バーのオブジェクト
        GameObject[,] objBoard;     //盤面のオブジェクトデータ
        Stack<GameObject> barList;  //バーのオブジェクトデータ
        List<Vector2Int> mashPos;   //マシュの座標データのリスト
        Material fade;              //選択中のフェード処理
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
            if (((x ^ y) & 1) == 1) {                                               //xかyがどちらか片方だけ0のとき
                if (rootBoard[posX, posY] + 2 * x + y == 0) {                       //バーを撤去するとき
                    rootBoard[posX, posY] = 0;
                    posX += x;
                    posY += y;
                    Destroy(barList.Pop());
                } else {
                    //マシュの効果
                    if (tileBoard[posX, posY] >= 2) {
                        x *= (rootBoard[posX, posY] + tileBoard[posX, posY]) % 2;
                        y *= (rootBoard[posX, posY] + tileBoard[posX, posY] + 1) % 2;
                    }
                    if (rootBoard[posX + x, posY + y] == 0) {                       //新しくバーを設置するとき
                        posX += x;
                        posY += y;
                        //バーオブジェクトを生成
                        Vector3 pos = new(1.0f * (posX - 0.5f * x - boardX / 2), 0.04f, 1.0f * (posY - 0.5f * y - boardY / 2));
                        Quaternion rot = Quaternion.Euler(0, 90f * x, 0);
                        barList.Push(Instantiate(bar, pos, rot));
                        rootBoard[posX, posY] = 2 * x + y;                          //上:-1、下:1、左:-2、右:2
                    }
                }
                //フェードタイル変更
                fade.SetColor("_EmissionColor", new Color(0, 0, 0));
                fade = objBoard[posX, posY].GetComponentsInChildren<Renderer>()[1].material;
                fade.EnableKeyword("_EMISSION");
                //クリア判定
                if (posX == startX && posY == startY) {
                    int flag = 0;
                    foreach (Vector2Int v in mashPos) {
                        if (rootBoard[v.x, v.y] != 0) flag++;
                    }
                    Debug.Log(flag);
                    if (flag == mashPos.Count) return 1;
                }
                //デバッグ用
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
                    //タイルの配置
                    pos = new Vector3(1.0f * (x - boardX / 2), 0.0f, 1.0f * (y - boardY / 2));
                    objBoard[x, y] = Instantiate(tile, pos, Quaternion.identity);
                    //マシュの配置
                    if (tileBoard[x, y] >= 2) {
                        pos = new Vector3(1.0f * (x - boardX / 2), 0.051f, 1.0f * (y - boardY / 2));
                        Instantiate(tileBoard[x, y] == 2 ? mashA : mashB, pos, Quaternion.identity);
                    }
                }
            }
            //選択中のタイルをフェードさせるための処理
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
