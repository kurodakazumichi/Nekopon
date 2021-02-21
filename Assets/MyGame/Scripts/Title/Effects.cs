using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitleScene
{ 
  public class Effects : MonoBehaviour
  {
    public Sprite[] Sprites = { };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (Random.Range(0, 1f) < 0.003f) {
        GameObject go = new GameObject();
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprites[Random.Range(0, Sprites.Length)];
        sr.sortingOrder = Random.Range(0 , 1);
        go.AddComponent<Effect>();
        go.transform.position = new Vector3(Random.Range(-1f, 1f), -1.2f, 0);
        go.transform.localScale = Vector3.one * Random.Range(0.4f, 0.7f);
        go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        
      }
    }
  }

}
