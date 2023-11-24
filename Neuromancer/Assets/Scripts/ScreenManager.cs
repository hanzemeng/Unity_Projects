using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static int GAME_WINDOW_WIDTH = 1920;
    public static int GAME_WINDOW_HEIGHT = 1080;
    private static bool IS_FULLSCREEN = true;
    private void Awake()
    {
        Screen.SetResolution(GAME_WINDOW_WIDTH, GAME_WINDOW_HEIGHT, IS_FULLSCREEN);
    }

}
