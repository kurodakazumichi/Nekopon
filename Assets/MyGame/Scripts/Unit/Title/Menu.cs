﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Title
{
  public class Menu : Unit<Menu.State>
  {
    public enum State { }
    public Vector4 Move = Vector4.zero;
    public Vector4 Scale = Vector4.zero;
    public Color SelectedColor = Color.white;
    private Vector3 startPos;
    private Vector3 startScale;

    public SceneManager.SceneType SceneType { private get; set; } = SceneManager.SceneType.None;


    // Start is called before the first frame update
    protected override void MyStart()
    {
      this.startPos = transform.position;
      this.startScale = transform.localScale;
    }

    // Update is called once per frame
    protected override void MyUpdate()
    {
      transform.position = this.startPos + new Vector3(
        Move.x * Mathf.Cos(Time.time * Move.z),
        Move.y * Mathf.Sin(Time.time * Move.w),
        0
      );

      transform.localScale = this.startScale + new Vector3(
        Scale.x * Mathf.Abs(Mathf.Cos(Time.time * Scale.z)),
        Scale.y * Mathf.Abs(Mathf.Sin(Time.time * Scale.w)),
        0
      );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      GetComponent<SpriteRenderer>().material.color = SelectedColor;
      SceneManager.Instance.ReservedScene = SceneType;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      GetComponent<SpriteRenderer>().material.color = Color.white;
      SceneManager.Instance.ReservedScene = SceneManager.SceneType.None;
    }
  }
}