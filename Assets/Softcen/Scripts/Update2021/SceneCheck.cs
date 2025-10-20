using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCheck : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            SceneManager.LoadSceneAsync(GameConsts.Skenes.Loader);
            return;
        }
        Destroy(gameObject);
    }
}
