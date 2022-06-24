using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class CannonPlayer : MonoBehaviour
    {
        [SerializeField] Animator cannonAnim;
        [SerializeField] Transform bulletStart;
        public GameObject bulletPrefab;
        public float bulletSpeed;
        bool canClick = true;
        Rigidbody2D rb;
        Vector2 mousePos = Vector2.up;
        float bulletForce = 20f;
        bool gameEnded;

        private void Start()
        {
            gameEnded = false;
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (gameEnded)
                return;
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Debug.Log("GetTouch");
                    mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    Shoot();
                }
            }
            if (Input.GetMouseButtonDown(0) && canClick)
            {
                Debug.Log("GetMouseButtonDown");
                canClick = false;
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartCoroutine(Shoot());
            }
        }

        IEnumerator Shoot()
        {
            Debug.Log("[Shoot]");
            yield return new WaitForSeconds(0.03f);
            if (gameEnded)
            {
                yield break;
            }
            cannonAnim.Play("CannonFire");
            yield return new WaitForSeconds(0.35f);
            GameObject bullet = Instantiate(bulletPrefab, bulletStart.position, bulletStart.rotation);
            Rigidbody2D go_rb = bullet.GetComponent<Rigidbody2D>();
            go_rb.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);
            canClick = true;
            Destroy(bullet, 3f);
        }

        private void FixedUpdate()
        {
            SetLook();
        }

        private void SetLook()
        {
            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }

        public void ShowEnd()
        {
            gameEnded = true;
            StartCoroutine(End());
        }

        IEnumerator End()
        {
            yield return new WaitForSeconds(3);
            GetComponent<SpriteRenderer>().enabled = false;
            VideoManager.instance.PlayEnd(() => Scenes.LoadMenu());
        }
    }
}