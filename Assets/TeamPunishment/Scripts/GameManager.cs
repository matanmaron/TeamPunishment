using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameObject debugPrefab;

        private void Start()
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Instantiate debug console");
                Instantiate(debugPrefab);
            }
        }
    }
}