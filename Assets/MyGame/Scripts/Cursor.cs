using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
  // カーソル加速度
  public float Acceleration = 0;

  // 可動域
  public Vector2 MovableRange = new Vector2(1, 0.75f);

  // カーソル速度
  private Vector3 velocity = Vector3.zero;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    Vector3 v = Vector3.zero;

    if (Input.GetKey(KeyCode.LeftArrow)) {
      v.x = -Acceleration;
    }

    if (Input.GetKey(KeyCode.RightArrow)) {
      v.x = Acceleration;
    }

    if (Input.GetKey(KeyCode.UpArrow)) {
      v.y = Acceleration;
    }

    if (Input.GetKey(KeyCode.DownArrow)) {
      v.y = -Acceleration;
    }

    this.velocity += v * Time.deltaTime;

    if (v.sqrMagnitude == 0) {
      this.velocity *= 0.99f;  
    }

    transform.position += this.velocity;
    Vector3 pos = transform.position;

    if (Mathf.Abs(MovableRange.x) <= Mathf.Abs(pos.x)) {
      this.velocity.x *= -1f;

    }
    if (Mathf.Abs(MovableRange.y) <= Mathf.Abs(pos.y)) {
      this.velocity.y *= -1f;
    }

    this.velocity *= 0.999f;
  }
}
