using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class makeframe : MonoBehaviour
{
    enum ModeFlag
    {
        CREATE = 0,
        PAUSE,
        PLAY,
        CLEAR
    }
    public AudioSource MoveSE;
    public GameObject Frame;
    public GameObject Select;
    public GameObject Up;
    public GameObject Down;
    public GameObject Right;
    public GameObject Left;
    public GameObject Corner;
    public GameObject Straight;
    const int FieldX = 5;
    const int FieldY = 5;
    private int PosX;
    private int PosY;
    private int StartX;
    private int StartY;
    private ModeFlag Mode;
    private int Arrow;                              //1:ç∂,2:è„,3:âE,4:â∫
    private int fps = 0;
    private int[,] Field = new int[FieldX, FieldY];
    private int[,] Circle = new int[FieldX, FieldY];        //1:íºäp,2:íºê¸
    private Material SelCol;
    private string[] stage = { "0001000010000000000000010", "0000002000200020200200000", "0002000000000021001002000", "0200000102010202000100020", "1022021002010022010011011" };
    private int clearfg;
    private int move;
    private int stagenow = 0;
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        StartX = 2;//Random.Range(0, FieldX);
        StartY = 2;//Random.Range(0, FieldY);
        PosX = StartX;
        PosY = StartY;
        Field[PosX, PosY] = -5;
        Arrow = 0;
        Mode = ModeFlag.CREATE;
        MakeField();
    }

    void Update()
    {

        /*if (Mode == ModeFlag.CREATE)
        {
            if (Getkey(0))
            {
                DeleteAll();
                MakeField();
                MakeCircle();
            }
        }
        if (Mode == ModeFlag.PAUSE)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DeleteAll();
                SetStart();
                MakeField();
                MakeCircle();
                Mode = ModeFlag.PLAY;
            }
        }*/

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            stagenow = 0;
            Mode = ModeFlag.CREATE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            stagenow = 1;
            Mode = ModeFlag.CREATE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stagenow = 2;
            Mode = ModeFlag.CREATE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            stagenow = 3;
            Mode = ModeFlag.CREATE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            stagenow = 4;
            Mode = ModeFlag.CREATE;
        }
        if (Mode == ModeFlag.CREATE)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                move = 0;
                DeleteAll();
                LoadCircle(stagenow);
                if (stagenow <= 3)
                {
                    stagenow++;
                } else
                {
                    stagenow = 0;
                }
                SetStart();
                MakeField();
                MakeCircle();
                Mode = ModeFlag.PLAY;
            }
        }
        if (Mode == ModeFlag.PLAY)
        {
            if (Getkey(0))
            {
                DeleteAll();
                MakeField();
                MakeCircle();
                //FieldDebug();
            }
        }
        if (Mode == ModeFlag.CLEAR)
        {    
            Mode = ModeFlag.CREATE;
        }
        //SaveCircle();
        SelCol.SetColor("_EmissionColor", Color.HSVToRGB(0.0f, 0.0f, 3.0f * Mathf.Abs(Mathf.Sin(fps / 30.0f))));
        fps++;
    }
    bool Getkey(int press)            //1:ç∂,2:â∫,3:âE,4:è„
    {
        bool judge = false;
        if (Input.GetKeyDown(KeyCode.Z) || press == 5)
        {
            if (Arrow != 0)
            {
                int b = 1 << (Arrow - 1);
                Field[PosX, PosY] = (Field[PosX, PosY] == 5) ? 0 : Field[PosX, PosY];
                PosX += (b & 1) - ((b >> 2) & 1);
                PosY += ((b >> 1) & 1) - ((b >> 3) & 1);
                Arrow = 0;
                for (b = 1; b <= 8; b <<= 1)
                {
                    if (((b >> 2) & 1) <= PosX && PosX < FieldX - (b & 1) && ((b >> 3) & 1) <= PosY && PosY < FieldY - ((b >> 1) & 1))
                    {
                        if (1 << (Mathf.Abs(Field[PosX - ((b >> 2) & 1) + (b & 1), PosY - ((b >> 3) & 1) + ((b >> 1) & 1)]) - 1) == b)
                        {
                            Arrow = Mathf.Abs(Field[PosX - ((b >> 2) & 1) + (b & 1), PosY - ((b >> 3) & 1) + ((b >> 1) & 1)]);
                        }
                    }
                }
                move--;
                Field[PosX, PosY] = (Arrow == 0) ? -5 : 5;
            }
            return true;
        }
        for (int key = 1; key <= 4; key++)
        {
            judge = false;
            int b = 1 << (key - 1);
            if ((b & 1) <= PosX && PosX < FieldX - ((b >> 2) & 1) && ((b >> 1) & 1) <= PosY && PosY < FieldY - ((b >> 3) & 1))
            {
                if (Field[PosX + ((b >> 2) & 1) - (b & 1), PosY + ((b >> 3) & 1) - ((b >> 1) & 1)] <= 0)
                {
                    if (Field[PosX, PosY] >= 0 || (Field[PosX, PosY] < 0 && move == 0))
                    {
                        if (Mode == ModeFlag.CREATE || (Mode == ModeFlag.PLAY && ((Circle[PosX, PosY] == 0) || (Circle[PosX, PosY] + Arrow) % 2 == 1)))
                        {
                            if (key == 1 && Arrow != 3) judge = (press == 1) || Input.GetKeyDown(KeyCode.LeftArrow);
                            if (key == 3 && Arrow != 1) judge = (press == 3) || Input.GetKeyDown(KeyCode.RightArrow);
                        }
                        if (Mode == ModeFlag.CREATE || (Mode == ModeFlag.PLAY && ((Circle[PosX, PosY] == 0) || (Circle[PosX, PosY] + Arrow) % 2 == 0)))
                        {
                            if (key == 2 && Arrow != 4) judge = (press == 2) || Input.GetKeyDown(KeyCode.DownArrow);
                            if (key == 4 && Arrow != 2) judge = (press == 4) || Input.GetKeyDown(KeyCode.UpArrow);
                        }
                    }
                    if (judge)
                    {
                        move++;
                        MoveSE.Play();
                        Field[PosX, PosY] = (Field[PosX, PosY] == 5) ? key : -key;
                        if (Arrow != 0 && Mode == 0) Circle[PosX, PosY] = (Arrow % 2 == key % 2) ? 2 : 1;
                        Arrow = key;
                        PosX += ((b >> 2) & 1) - (b & 1);
                        PosY += ((b >> 3) & 1) - ((b >> 1) & 1);
                        if (Field[PosX, PosY] == 0)
                        {
                            Field[PosX, PosY] = 5;
                        }
                        else
                        {
                            if (Mode == ModeFlag.CREATE)
                            {
                                Circle[PosX, PosY] = (-Field[PosX, PosY] % 2 == key % 2) ? 2 : 1;
                                Mode = ModeFlag.PAUSE;
                            }
                            if (Mode == ModeFlag.PLAY)
                            {
                                clearfg = 0;
                                for (int x = 0; x < FieldX; x++)
                                {
                                    for (int y = 0; y < FieldY; y++)
                                    {
                                        if (Circle[x, y] != 0 && Field[x, y] == 0) clearfg = 1;
                                    }
                                }
                                if (clearfg == 0) Mode = ModeFlag.CLEAR;
                                Debug.Log("clearfg:" + clearfg);
                            }
                        }
                        break;
                    }
                }
            }
        }
        return judge;
    }

    void FieldDebug() {
        string t = "";
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                if (Field[x, y] == -1)
                {
                    t += "" + Field[x, y];
                } else
                {
                    t += " " + Field[x, y];
                }
            }
            t += "\n";
        }
        Debug.Log("Field:\n" + t);
    }
    void MakeField()
    {
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                if (Mathf.Abs(Field[x, y]) == 5)
                {
                    SelCol = Instantiate(Select, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity).GetComponentsInChildren<Renderer>()[1].material;
                    SelCol.EnableKeyword("_EMISSION");
                } else
                {
                    //Instantiate(Frame, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity).GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, -30.0f, 0.0f), ForceMode.Impulse);
                    Instantiate(Frame, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                    if (Mathf.Abs(Field[x, y]) == 1) Instantiate(Left, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                    if (Mathf.Abs(Field[x, y]) == 2) Instantiate(Down, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                    if (Mathf.Abs(Field[x, y]) == 3) Instantiate(Right, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                    if (Mathf.Abs(Field[x, y]) == 4) Instantiate(Up, new Vector3(1.0f * (x - FieldX / 2), 0.0f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                }
            }
        }
    }

    void MakeCircle()
    {
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                if (Circle[x, y] == 1)
                {
                    Instantiate(Corner, new Vector3(1.0f * (x - FieldX / 2), 0.051f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                }
                if (Circle[x, y] == 2)
                {
                    Instantiate(Straight, new Vector3(1.0f * (x - FieldX / 2), 0.051f, 1.0f * (y - FieldY / 2)), Quaternion.identity);
                }
            }
        }
    }

    void SaveCircle()
    {
        using StreamWriter sw = new("./Assets/stages.txt", false);
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                sw.Write(Circle[x, y]);
            }
            sw.WriteLine("");
        }
    }

    void LoadCircle(int n)
    {
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                Circle[x, y] = int.Parse(stage[n].Substring(x * FieldX + y, 1));
            }
        }
    }

    void DeleteAll()
    {
        GameObject[] obj;
        obj = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < obj.Length; i++)
        {
            Destroy(obj[i].gameObject);
        }
    }

    void SetStart()
    {
        for (int x = 0; x < FieldX; x++)
        {
            for (int y = 0; y < FieldY; y++)
            {
                Field[x, y] = 0;
                //if (Random.Range(0, 3) == 0) Circle[x, y] = 0;
            }
        }
        PosX = StartX;
        PosY = StartY;
        Field[PosX, PosY] = -5;
        Circle[PosX, PosY] = 0;
    }
}
