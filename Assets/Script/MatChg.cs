using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatChg : MonoBehaviour
{
    private int flag;
    private float cnt;
    private Color c;
    void Start() {
        flag = 0;
        cnt = 0;
        c = GetComponent<Renderer>().material.color;
    }
    void Update() {
        if (flag >= 1) {
            float f = -1.0f * Mathf.Cos(cnt * 6) / 4 + 0.25f;
            if (flag == 2 && f <= 0.01f) {
                f = 0;
                cnt = 0;
                flag = 0;
            } else {
                cnt += Time.deltaTime;
            }
            GetComponent<Renderer>().material.color = new Color(c.r - f, c.g - f, c.b - f);
        }
    }
    public void ChgInit() {
        flag = 1;
    }
    public void ChgStop() {
        flag = 2;
    }
}
