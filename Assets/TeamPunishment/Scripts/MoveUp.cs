using UnityEngine;
using UnityEngine.UI;

public class MoveUp : MonoBehaviour
{
    float speed = 32;

    public void Init(int number)
    {
        GetComponent<Text>().text = number.ToString();
    }

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
    }
}
