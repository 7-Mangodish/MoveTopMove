#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScenes : Editor {
    [MenuItem("Open Scene/_01_SplashScene #1")]
    public static void OpenSpashScene() {
        OpenScene("_Scenes/SplashScene");
    }
    //
    [MenuItem("Open Scene/_02_Map1 #2")]
    public static void OpenMap1() {
        OpenScene("_Scenes/Map1");
    }
    //
    [MenuItem("Open Scene/_03_Map2 #3")]
    public static void OpenMap2() {
        OpenScene("_Scenes/Map2");
    }

    private static void OpenScene(string path) {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            EditorSceneManager.OpenScene("Assets/" + path + ".unity");
        }
    }
}
#endif
