using System;
using AMO.Framework.Managers.InputManagerDomain;
using AMO.Framework.Managers.UIManagerDomain;
using AMO.Framework.Managers.UIManagerDomain.UI.MainPage;
using AMO.Framework.Managers.UIManagerDomain.UI.Page;
using AMO.Framework.Managers.UIManagerDomain.UI.Pop;
using AMO.Framework.Modules.DebugModuleDomain;
using AMO.Framework.Modules.LocatorModuleDomain;
using AMO.Framework.Modules.SingletonModuleDomain;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Runtime.Managers
{
    public class MasterManager : PersistentMonoSingleton<MasterManager>
    {
        public event Func<UniTask> OnAllServiceAndModuleInitialized;
        
        private ServiceLocator serviceLocator;

        protected override async UniTask OnSingletonInitializeAsync()
        {
            await base.OnSingletonInitializeAsync();
            await InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            await Addressables.InitializeAsync();
            await UniTask.DelayFrame(1);

            InitializeLocators();

            RegisterServices();

            AddUIPrefabPaths();
            await InitializeServicesAsync();
            
            
            Application.targetFrameRate = 120;
            D.Log("Master :: Application ready");

            OnAllServiceAndModuleInitialized?.Invoke();
        }
        

        public void RegisterService<T>(T service) where T : IService
        {
            if (serviceLocator.IsServiceRegistered<T>())
            {
                D.LogWarning($"Service of type {typeof(T).Name} is already registered.");
                return;
            }

            serviceLocator.Register(service);
        }

        public void UnregisterService<T>() where T : IService
        {
            if (!serviceLocator.IsServiceRegistered<T>())
            {
                D.LogWarning($"Service of type {typeof(T).Name} is not registered.");
                return;
            }

            serviceLocator.Unregister<T>();
        }

        public T GetService<T>() where T : class
        {
            return serviceLocator.GetService<T>();
        }

        private void AddUIPrefabPaths()
        {
            var uiManager = GetService<UIManager>();
            uiManager.AddMainPagePrefabPath<MainPageDEMO>(nameof(MainPageDEMO));
            
            // uiManager.AddMainPagePrefabPath<CatalogMainPage>(nameof(CatalogMainPage));
            // uiManager.AddMainPagePrefabPath<BionVersionMainPage>(nameof(BionVersionMainPage));
            //
            //
            uiManager.AddUIPrefabPath<PageDEMO>(nameof(PageDEMO), 1);
            // uiManager.AddUIPrefabPath<BionManualSaveAndLoadingTexturePage>(nameof(BionManualSaveAndLoadingTexturePage), 1);
            // uiManager.AddUIPrefabPath<BionRuntimePage>(nameof(BionRuntimePage), 1);
            //
            //
            uiManager.AddUIPrefabPath<PopDEMO>(nameof(PopDEMO), 101);
            // uiManager.AddUIPrefabPath<CommonPop>(nameof(CommonPop), 101);
            // uiManager.AddUIPrefabPath<FtpSettingPop>(nameof(FtpSettingPop), 102);
            // uiManager.AddUIPrefabPath<RunTimeDebugConsolePop>(nameof(RunTimeDebugConsolePop), 300);
            // uiManager.AddUIPrefabPath<VisualCoordinatePanelPop>(nameof(VisualCoordinatePanelPop), 102);
            // uiManager.AddUIPrefabPath<ConnectedIpPop>(nameof(ConnectedIpPop), 102);
        }

        protected override void OnDestroy()
        {
            foreach (var service in serviceLocator.GetAllServices())
            {
                service.Shutdown();
            }
        }

        internal void CloseApplication()
        {
            CloseApplicationAsync().GetAwaiter().GetResult();
        }

        private async UniTask CloseApplicationAsync()
        {
            try
            {
                await UniTask.CompletedTask;
                D.Log("Starting application shutdown...");
            }
            catch (Exception ex)
            {
                D.LogError($"Error during shutdown: {ex.Message}");
            }
            finally
            {
                Application.Quit();
            }
        }

        private void RegisterServices()
        {
            RegisterService(UIManager.Instance);
            RegisterService(InputManager.Instance);
            
        }


        private async UniTask InitializeServicesAsync()
        {
            foreach (var service in serviceLocator.GetAllServices())
            {
                await service.InitializeAsync();
            }
        }
        
        private void InitializeLocators()
        {
            serviceLocator = new ServiceLocator();
        }
    }
}