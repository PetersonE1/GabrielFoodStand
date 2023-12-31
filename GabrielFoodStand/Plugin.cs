using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GabrielFoodStand.Agent;
using HarmonyLib;
using UnityEngine;
using GabrielFoodStand.Hydra;

namespace GabrielFoodStand
{
    [BepInPlugin("agent.gabriel_food_stand", "Gabriel Food Stand", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            BestUtilityEverCreated.Initialize();
            Harmony harmony = new Harmony("gabriel_food_stand");
            harmony.PatchAll();
            StartCoroutine(AgentRegistry.GetAudio());
            FoodStandInitializer.Init();

            RegisterAssets();
            CoinCollectorManager.Initialize();
        }

        private void RegisterAssets()
        {
            new HydraLoader.CustomAssetPrefab("CollectableCoin", new Component[] { new CollectableCoin() });
            new HydraLoader.CustomAssetPrefab("CollectableCoinUI", new Component[] { new CollectableCoinUI() });
            new HydraLoader.CustomAssetPrefab("CollectableCoinFX", new Component[] { new CollectableCoinUI(), new DestroyAfterTime() { timeLeft = 4.0f } });

            HydraLoader.RegisterAll(Properties.Resources.hydrabundle);
        }
    }
}
