﻿using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utility;
using Mappy.Abstracts;

namespace Mappy.Views.Components;

public class ModuleSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    public string ID => module.ModuleName.ToString();
    private readonly ModuleBase module;

    public ModuleSelectable(ModuleBase module) => this.module = module;

    public void DrawLabel() => ImGui.TextUnformatted(module.ModuleName.Label());

    public void Draw() => module.DrawConfig();
}