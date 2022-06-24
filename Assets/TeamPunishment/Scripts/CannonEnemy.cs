using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class CannonEnemy : MonoBehaviour
    {
        [SerializeField] List<Sprite> planetStates = new List<Sprite>();
        [SerializeField] SpriteRenderer planetImage;
        [SerializeField] CannonPlayer player;
        int counter = 0;

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
            counter++;
            planetImage.sprite = planetStates[counter];
            if (counter>4)
            {
                player.ShowEnd();
                Destroy(gameObject);
            }
        }
    }
}