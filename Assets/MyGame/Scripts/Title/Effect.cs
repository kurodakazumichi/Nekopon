using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitleScene
{
  public class Effect : MonoBehaviour
  {
    private float timer = 0;
    private float lifeTime = 0;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
      this.lifeTime = Random.Range(5, 10);
      this.SetRandomVelocity();
    }

    private void SetRandomVelocity()
    {
      this.velocity.x = Random.Range(-0.1f, 0.1f);
      this.velocity.y = Random.Range(0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
      this.timer += Time.deltaTime;

      if (Random.Range(0, 1f) < 0.001f) {
        SetRandomVelocity();
      }

      this.transform.position += this.velocity * Time.deltaTime;

      if (this.lifeTime <= this.timer) {
        Destroy(this.gameObject);
      }
    }
  }
}