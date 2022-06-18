using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class CannonGame : MonoBehaviour
    {
        [SerializeField] Text score;
        [SerializeField] GameObject enemyPrefab;

        int y = 5;
        float prevX = -99;
        float minX = -8;
        float maxX = 8;
        float minTime = 0.5f;
        float maxTime = 4;
        private void Start()
        {
            StartCoroutine(MakeEnemy());
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Scenes.LoadMenu();
            }
        }
        
        IEnumerator MakeEnemy()
        {
            if (maxTime > 1)
            {
                maxTime -= 0.02f;
                Debug.Log("[MAXTIME] - "+maxTime);
            }
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            float x = Random.Range(minX, maxX);
            while (Mathf.Abs(x-prevX) < 2)
            {
                x = Random.Range(minX, maxX);
            }
            prevX = x;
            var enemy = Instantiate(enemyPrefab, new Vector3(x, 5, 0), Quaternion.identity);
            StartCoroutine(MakeEnemy());
        }
    }
}