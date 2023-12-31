using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace GabrielFoodStand
{
    public static class BestUtilityEverCreated
    {
        public static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                TextureLoader.Init();
                SceneManager.sceneLoaded += OnSceneLoad;
            }
        }

        /// <summary>
        /// Returns initialized and stuff
        /// </summary>
        private static bool Initialized
        {
            get
            {
                return initalized;
            }

            set
            {
                initalized = value;
            }
        }

        private static bool initalized;

        //Serious part, this will be in UMM soon so Ive changed the enum name

        /// <summary>
        /// Enumerated version of the Ultrakill scene types
        /// </summary>
        public enum UltrakillLevelType { Intro, MainMenu, Level, Endless, Sandbox, Custom, Intermission, Unknown }

        /// <summary>
        /// Returns the current level type
        /// </summary>
        public static UltrakillLevelType CurrentLevelType = UltrakillLevelType.Intro;

        public delegate void OnLevelChangedHandler(UltrakillLevelType uKLevelType);

        /// <summary>
        /// Invoked whenever the current level type is changed.
        /// </summary>
        public static OnLevelChangedHandler OnLevelTypeChanged;

        /// <summary>
        /// Invoked whenever the scene is changed.
        /// </summary>
        public static OnLevelChangedHandler OnLevelChanged;

        private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            string sceneName = SceneHelper.CurrentScene;
            Debug.Log(string.Format("SCENE:[{0}]", sceneName));
            UltrakillLevelType newScene = GetUKLevelType(sceneName);

            if (scene != SceneManager.GetActiveScene())
                return;

            if (newScene != CurrentLevelType)
            {
                CurrentLevelType = newScene;
                OnLevelTypeChanged?.Invoke(newScene);
            }

            OnLevelChanged?.Invoke(CurrentLevelType);
        }

        //Perhaps there is a better way to do this. Also this will most definitely cause problems in the future if PITR or Hakita rename any scenes.

        /// <summary>
        /// Gets enumerated level type from the name of a scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene</param>
        /// <returns></returns>
        public static UltrakillLevelType GetUKLevelType(string sceneName)
        {
            sceneName = (sceneName.Contains("Level")) ? "Level" : (sceneName.Contains("Intermission")) ? "Intermission" : sceneName;

            switch (sceneName)
            {
                case "Main Menu":
                    return UltrakillLevelType.MainMenu;
                case "Custom Content":
                    return UltrakillLevelType.Custom;
                case "Intro":
                    return UltrakillLevelType.Intro;
                case "Endless":
                    return UltrakillLevelType.Endless;
                case "uk_construct":
                    return UltrakillLevelType.Sandbox;
                case "Intermission":
                    return UltrakillLevelType.Intermission;
                case "Level":
                    return UltrakillLevelType.Level;
                default:
                    return UltrakillLevelType.Unknown;
            }
        }

        /// <summary>
        /// Returns true if the current scene is playable
        /// </summary>
        /// <returns></returns>
        public static bool InLevel()
        {
            bool inNonPlayable = (CurrentLevelType == UltrakillLevelType.MainMenu || CurrentLevelType == UltrakillLevelType.Intro || CurrentLevelType == UltrakillLevelType.Intermission || CurrentLevelType == UltrakillLevelType.Unknown);
            return !inNonPlayable;
        }

        public delegate void OnLevelCompleteHandler();

        /// <summary>
        /// Fired when the player enters the final pit in any level
        /// </summary>
        public static OnLevelCompleteHandler OnLevelComplete;

        public delegate void PlayerEventHanler();

        public static PlayerEventHanler OnPlayerActivated;

        public static PlayerEventHanler OnPlayerDied;

        public static PlayerEventHanler OnPlayerParry;

        public static class TextureLoader
        {
            public static string GetTextureFolder()
            {
                return FoodData.GetDataPath("tex");
            }

            private static Texture2D[] cachedTextures = new Texture2D[0];

            private static bool initialized = false;

            public static void Init()
            {
                if (!initialized)
                {
                    BestUtilityEverCreated.OnLevelChanged += OnLevelChanged;
                    initialized = true;
                }
            }

            private static void OnLevelChanged(BestUtilityEverCreated.UltrakillLevelType ltype)
            {
                if (BestUtilityEverCreated.InLevel())
                {
                    RefreshTextures();
                }
            }

            public static void RefreshTextures()
            {
                CleanCachedTextures();
                cachedTextures = FindTextures();
            }

            public static bool TryLoadTexture(string path, out Texture2D tex, bool checkerIfNull = false)
            {
                tex = null;
                if (!File.Exists(path))
                {
                    Debug.Log("Invalid location: " + path);
                    return false;
                }

                byte[] byteArray = null;
                try
                {
                    byteArray = File.ReadAllBytes(path);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Invalid path: " + path);
                }

                tex = new Texture2D(16, 16);
                if (!tex.LoadImage(byteArray))
                {
                    Debug.Log("texture loading failed!");
                    if (checkerIfNull)
                    {
                        Checker(ref tex);
                    }
                    return false;
                }

                return true;
            }

            public static Texture2D PullRandomTexture()
            {
                if (cachedTextures.Length > 0)
                {
                    int rand = UnityEngine.Random.Range(0, cachedTextures.Length);
                    return cachedTextures[rand];
                }

                return null;
            }

            private static List<Texture2D> additionalTextures = new List<Texture2D>();

            public static void AddTextureToCache(Texture2D texture)
            {
                List<Texture2D> oldCache = new List<Texture2D>(cachedTextures);
                oldCache.Add(texture);
                additionalTextures.Add(texture);
                cachedTextures = oldCache.ToArray();
            }


            private static void CleanCachedTextures()
            {
                if (cachedTextures != null)
                {
                    int len = cachedTextures.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (cachedTextures[i] != null)
                        {
                            if (!additionalTextures.Contains(cachedTextures[i]))
                            {
                                UnityEngine.Object.Destroy(cachedTextures[i]);
                            }
                        }
                    }

                    cachedTextures = null;
                }
            }

            private static Texture2D[] FindTextures()
            {

                List<Texture2D> newTextures = new List<Texture2D>();

                string path = GetTextureFolder();
                string[] pngs = System.IO.Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                string[] jpgs = System.IO.Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

                for (int i = 0; i < pngs.Length; i++)
                {
                    if (TryLoadTexture(ImagePath(pngs[i]), out Texture2D newTex, false))
                    {
                        newTextures.Add(newTex);
                    }
                }

                for (int i = 0; i < jpgs.Length; i++)
                {
                    if (TryLoadTexture(ImagePath(jpgs[i]), out Texture2D newTex, false))
                    {
                        newTextures.Add(newTex);
                    }
                }

                string ImagePath(string filename)
                {
                    string imagePath = GetTextureFolder();
                    imagePath = Path.Combine(path, filename);
                    return imagePath;
                }

                for (int i = 0; i < additionalTextures.Count; i++)
                {
                    newTextures.Add(additionalTextures[i]);
                }

                return newTextures.ToArray();
            }

            private static void Checker(ref Texture2D tex)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    for (int x = 0; x < tex.width; x++)
                    {
                        bool Xeven = ((x % 2) == 0);
                        bool Yeven = ((y % 2) == 0);
                        if (Yeven != Xeven)
                        {
                            Xeven = !Xeven;
                        }
                        Color col = (Xeven) ? Color.white : Color.black;
                        tex.SetPixel(x, y, col);
                    }
                }

                tex.Apply();
            }
        }
    }
}