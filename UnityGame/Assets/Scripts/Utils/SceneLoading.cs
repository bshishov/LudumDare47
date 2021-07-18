using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneLoading
    {
        public static void TryLoadSceneAdditive(string sceneName)
        {
            var isUiSceneLoaded = false;
            for (var sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.name.Equals(sceneName))
                    isUiSceneLoaded = true;
            }

            if (!isUiSceneLoaded)
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}