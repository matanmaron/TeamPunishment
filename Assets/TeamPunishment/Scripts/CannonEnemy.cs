using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class CannonEnemy : MonoBehaviour
    {
        float speed = 1;

        private void Start()
        {
            speed = Random.Range(0.2f, 1.5f);
            transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        }

        void Update()
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);
            if (collision.gameObject.tag == "BULLET")
            {
                Destroy(collision.gameObject);
                Hit();
            }
        }

        public void Hit()
        {
            Destroy(gameObject);
        }
    }
}