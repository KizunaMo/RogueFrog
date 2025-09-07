using AMO.Framework.Infrastructure.Utilities.AssetSeries.AssetLoader.AMO.Framework.Infrastructure.Utilities.AssetSeries;
using AMO.Framework.Managers;
using AMO.Framework.Modules.DebugModuleDomain;
using AMO.Framework.Services.AddressableServiceDomain;
using AMO.Framework.Services.SceneManagerServiceDomain;
using AMO.Framework.Services.SceneManagerServiceDomain.Configs;
using AMO.Framework.Services.SceneManagerServiceDomain.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static AMO.Consts.Values;

namespace _Project.Scripts.Runtime.Managers
{
    public class MasterManager : MasterManagerBase
    {
        private AddressableAssetService addressableAssetService;
        
        protected override async UniTask OnSingletonInitializeAsync()
        {
            await base.OnSingletonInitializeAsync();

            addressableAssetService = ServiceLocator.GetService<AddressableAssetService>();
            var sceneOrechestrator = ServiceLocator.GetService<SceneOrchestrator>();
            var sceneConfigAsset = await addressableAssetService.LoadAssetAsync<ScriptableObject>("SceneOrchestrationConfig");
            if (sceneConfigAsset.Success)
            {
                sceneOrechestrator.UpdateConfig(sceneConfigAsset.Result as SceneOrchestrationConfig);
                await sceneOrechestrator.LoadSceneGroupAsync("Labs");
                _ = sceneOrechestrator.UnloadSceneAsync(SceneName.PreLoadScene);
            }
            else
            {
                D.LogError($"Cannot load scene orchestration config: {sceneConfigAsset.Result}");
            }
          
        }
    }
}