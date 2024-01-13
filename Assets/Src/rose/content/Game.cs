using UnityEngine;

namespace com.rose.content
{

    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        [Header("Settings")]
        public FramerateSettings framerateSettings;

        private bool isWindowFocused;

        public bool IsWindowFocused
        {
            get { return isWindowFocused; }
        }

        private void Awake()
        {
            SetSingleton();
            Initialize();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            ManageFullscreenState();
        }

        private void OnApplicationFocus(bool focus)
        {
            isWindowFocused = focus;
            UpdateTargetFramerate();
        }

        private void SetSingleton()
        {
            Instance = this;
        }

        private void Initialize()
        {
            UpdateTargetFramerate();
        }

        private void ManageFullscreenState()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Screen.fullScreen)
                {
                    Screen.SetResolution(1920 / 4, 1080 / 4, FullScreenMode.Windowed);
                    return;
                }

                Screen.SetResolution(1920 / 4, 1080 / 4, FullScreenMode.ExclusiveFullScreen);
            }
        }

        private void UpdateTargetFramerate()
        {
            Application.targetFrameRate = isWindowFocused ? framerateSettings.ingameTargetFramerate :
                                                            framerateSettings.unfocusedTargetFramerate;
        }
    }
}