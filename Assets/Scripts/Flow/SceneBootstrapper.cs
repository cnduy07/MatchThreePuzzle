using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrapper : MonoBehaviour
{
    void Start()
    {
        RuntimeSettingsState.EnsureInitialized();

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == SceneNames.Boot)
        {
            SceneFlow.LoadMenu();
            return;
        }

        if (sceneName == SceneNames.Menu)
        {
            MenuSceneUiBuilder.Build();
            return;
        }

        if (sceneName == SceneNames.LevelSelect)
        {
            LevelMapSceneUiBuilder.Build();
        }
    }
}
