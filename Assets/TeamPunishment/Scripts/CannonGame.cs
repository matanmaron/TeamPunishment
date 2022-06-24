using UnityEngine;

namespace TeamPunishment
{
    public class CannonGame : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Scenes.LoadMenu();
            }
        }
    }
}