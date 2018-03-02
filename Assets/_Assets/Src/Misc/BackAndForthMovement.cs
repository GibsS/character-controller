using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthMovement : MonoBehaviour {

    public Vector2 relativeTarget;
    public float duration = 10;

    float halfDuration;

    Vector2 point1;
    Vector2 point2;

    void Start() {
        halfDuration = duration / 2f;

        point1 = transform.position;
        point2 = (Vector2)transform.position + relativeTarget;
    }

    void Update() {
        float currentTime = Time.time % duration;

        if(currentTime < halfDuration) {
            transform.position = Vector2.Lerp(point1, point2, currentTime / halfDuration);
        } else {
            transform.position = Vector2.Lerp(point2, point1, (currentTime - halfDuration) / halfDuration);
        }
    }
}
