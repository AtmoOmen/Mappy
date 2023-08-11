﻿using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using ImGuiScene;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using Mappy.Utility;
using Action = System.Action;

namespace Mappy.Models;

public class MappyMapIcon
{
    public uint IconId { get; set; }
    public List<IconLayer> Layers { get; set; } = new();
    public float IconScale { get; set; } = 1.0f;
    public bool ShowIcon { get; set; }
    
    public float Radius { get; set; }
    public Vector4 RadiusColor { get; set; } = KnownColor.Aqua.AsVector4();

    public string Tooltip { get; set; } = string.Empty;
    public uint TooltipExtraIcon { get; set; }
    public string TooltipDescription { get; set; } = string.Empty;
    public Vector4 TooltipColor { get; set; } = KnownColor.White.AsVector4();
    public bool ShowTooltip { get; set; }
    
    public float VerticalPosition { get; set; }
    public float VerticalThreshold { get; set; }
    public bool ShowDirectionalIndicator { get; set; }
    
    public Vector2? TexturePosition { get; set; }
    public Vector2? ObjectPosition { get; set; }

    public Vector2 GetDrawPosition(Map map)
    {
        if (TexturePosition is not null) return TexturePosition.Value;
        if (ObjectPosition is not null) return Position.GetTexturePosition(ObjectPosition.Value, map);
        
        return Vector2.Zero;
    }
    
    public Action? OnClickAction { get; set; }
    
    public TextureWrap? IconTexture => IconCache.Instance.GetIcon(IconId);
    public Vector2 IconSize => IconTexture is null ? Vector2.Zero : new Vector2(IconTexture.Width, IconTexture.Height);
}