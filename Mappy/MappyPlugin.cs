﻿using Dalamud.Plugin;
using KamiLib;
using KamiLib.System;
using Mappy.Controllers.Localization;
using Mappy.System;
using Mappy.Views.Windows;

namespace Mappy;

public sealed class MappyPlugin : IDalamudPlugin {
    public static MappySystem System = null!;
    
    public MappyPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();
        
        KamiCommon.Initialize(pluginInterface, "Mappy");
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));
                
        System = new MappySystem();
        System.Load();

        CommandController.RegisterMainCommand("/mappy");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new MapWindow());
        KamiCommon.WindowManager.AddWindow(new IpcDemoWindow());
    }

    public void Dispose() {
        KamiCommon.Dispose();
        
        System.Unload();
    }
}