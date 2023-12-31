using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabrielFoodStand
{
    public static class FoodData
    {
        private const string folderName = "Foodstand_Data";
        private static string dataFilePath = GetDataPath("data", "save.food");

        public static string GetDataPath(params string[] subpath)
        {
            string modDir = Assembly.GetExecutingAssembly().Location;
            modDir = Path.GetDirectoryName(modDir);
            string localPath = Path.Combine(modDir, folderName);

            if (subpath.Length > 0)
            {
                string subLocalPath = Path.Combine(subpath);
                localPath = Path.Combine(localPath, subLocalPath);
            }

            ValidatePath(localPath);

            return localPath;
        }

        private static void ValidatePath(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);

            if (!info.Exists)
            {
                if (!info.Name.Contains("."))
                {
                    info.Create();
                }
                else
                {
                    if (!info.Parent.Exists)
                    {
                        info.Parent.Create();
                    }
                }
            }
        }

        private static string[] dataToCheck = new string[] { "tex", "data", "audio" };

        private static FoodPersistentData data;
        public static bool AutoSave = true;

        public static FoodPersistentData Data
        {
            get
            {
                if (data == null)
                {
                    LoadData();
                }

                return data;
            }

            set
            {
                if (value != null)
                {
                    if (data != value)
                    {
                        data = value;
                        OnDataChanged?.Invoke(data);
                        if (AutoSave)
                        {
                            SaveData();
                        }
                        return;
                    }
                }
            }
        }

        public static bool IsFirstTime()
        {
            if (!File.Exists(dataFilePath))
            {
                return true;
            }
            return false;
        }

        public static void SaveData()
        {
            string serializedData = JsonConvert.SerializeObject(Data);
            if (!Directory.Exists(GetDataPath("data")))
                Directory.CreateDirectory(GetDataPath("data"));
            File.WriteAllText(dataFilePath, serializedData);
            Debug.Log($"Foodstand_Save: Game data saved to {dataFilePath}");
        }

        public static void LoadData()
        {
            Debug.Log("Foodstand_Save: Searching for datafile.");
            if (File.Exists(dataFilePath))
            {
                string jsonData;
                using (StreamReader reader = new StreamReader(dataFilePath))
                {
                    jsonData = reader.ReadToEnd();
                }

                data = JsonConvert.DeserializeObject<FoodPersistentData>(jsonData);
            }

            if (data == null)
            {
                NewData();
            }
        }

        public static void NewData()
        {
            Debug.Log("GabrielFoodStand: Creating new save-file.");
            Data = FoodPersistentData.Default;
            SaveData();
        }

        public delegate void OnDataChangedHandler(FoodPersistentData newData);
        public static OnDataChangedHandler OnDataChanged;
    }

    [System.Serializable]
    public class FoodPersistentData
    {
        public List<string> standsIncomplete;

        public static FoodPersistentData Default
        {
            get
            {
                FoodPersistentData newData = new FoodPersistentData
                {
                    standsIncomplete = new List<string>()
                {
                    "Level 0-1",
                    "Level 0-2",
                    "Level 0-3",
                    "Level 0-4",
                    "Level 0-5",
                    "Level 1-1",
                    "Level 1-2",
                    "Level 1-3",
                    "Level 1-4",
                    "Level 2-1",
                    "Level 2-2",
                    "Level 2-3",
                    "Level 2-4",
                    "Level 3-1",
                    "Level 3-2",
                    "Level 4-1",
                    "Level 4-2",
                    "Level 4-3",
                    "Level 4-4",
                    "Level 5-1",
                    "Level 5-2",
                    "Level 5-3",
                    "Level 5-4",
                    "Level 6-1",
                    "Level 6-2",
                }
                };
                return newData;
            }
        }
    }
}
