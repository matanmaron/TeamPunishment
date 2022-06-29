using UnityEngine.SceneManagement;

namespace TeamPunishment
{
    public static class Scenes
    {
        public static void LoadMenu()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                return;
            }
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

        public static void LoadDemo()
        {
            SceneManager.LoadScene(5);
        }

        public static void LoadNegotiate()
        {
            SceneManager.LoadScene(6);
        }

        public static void LoadAttack()
        {
            SceneManager.LoadScene(7);
        }
    }
}
