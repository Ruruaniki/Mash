using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    private float c, d, p;
    private int flag;
    private Vector3 scale;
    void Start() {
        flag = 0;
        p = 4.0f;
        c = Time.time;
        scale = transform.localScale;
        d = (Time.time - c) * p;
        transform.localScale = scale * d;
    }
    void Update() {
        if (d >= 0 && d <= 1.0f) {
            d = (Time.time - c) * p;
            if (d > 1.0f) d = 1.0f;
            if (flag == 0) transform.localScale = scale * (-2.0f * d * d + 3.0f * d);
            else transform.localScale = scale * (-2.0f * (1.0f - d) * (1.0f - d) + 3.0f * (1.0f - d));
        }
    }
    public float Wait() {
        return 1.0f / p;
    }
    public void DestInit() {
        flag = 1;
        c = Time.time;
        d = (Time.time - c) * p;
    }
}
