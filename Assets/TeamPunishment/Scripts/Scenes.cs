using UnityEngine.SceneManagement;

namespace TeamPunishment
{
    public static class Scenes
    {
        public static void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }
        public static void LoadStandartGame()
        {
            SceneManager.LoadScene(1);
        }
        public static void LoadKickedOut()
        {
            SceneManager.LoadScene(2);
        }
        public static void LoadMobileInfo()
        {
            SceneManager.LoadScene(3);
        }
        public static void LoadMobileGame()
        {
            SceneManager.LoadScene(4);
        }
    }
}
