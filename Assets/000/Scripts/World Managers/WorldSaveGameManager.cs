using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BBTUF
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        public int worldSceneInedx = 1;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneInedx);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneInedx;
        }
    }
}