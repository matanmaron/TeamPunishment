using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class MoveUp : MonoBehaviour
    {
        float speed = 44;

        public void Init(int number)
        {
            GetComponent<Text>().text = number.ToString() + "K";
        }

        private void Start()
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            Destroy(gameObject, 3f);
        }

        void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
        }
    }
}