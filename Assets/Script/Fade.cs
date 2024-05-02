using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    private float c, d, p;
    private int flag;
    private Vector3 scale, pos;
    void Start() {
        flag = flag == 2 ? 2 : 0;
        p = 4.0f;
        c = Time.time;
        scale = transform.localScale;
        pos = transform.position;
        d = (Time.time - c) * p;
        transform.localScale = scale * d;

    }
    void Update() {
        if (flag <= 2) {
            if (d >= 0 && d <= 1.0f) {
                d = (Time.time - c) * p;
                if (d > 1.0f) d = 1.0f;
                if (flag % 2 == 0) transform.localScale = scale * (-2.0f * d * d + 3.0f * d);
                if (flag % 2 == 1) transform.localScale = scale * (-2.0f * (1.0f - d) * (1.0f - d) + 3.0f * (1.0f - d));
            }
            if (flag / 2 == 1) {
                transform.Rotate(0, 0.6f, 0);
                transform.position = new(pos.x, pos.y + Mathf.Sin(Time.time) / 8.0f, pos.z);
            }
        }
    }
    public float Wait() {
        return 1.0f / p;
    }
    public void DestInit() {
        flag += 1;
        c = Time.time;
        d = (Time.time - c) * p;
    }
    public void RotInit() {
        flag = 2;
    }
}
