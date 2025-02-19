﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using KamiLib.Game;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Mappy.Abstracts;
using Mappy.Controllers.Localization;
using Mappy.Models;
using Mappy.Models.Enums;
using Mappy.Models.ModuleConfiguration;

namespace Mappy.System.Modules;

public unsafe class Houses : ModuleBase {
    public override ModuleName ModuleName => ModuleName.HousingMarkers;
    public override IModuleConfig Configuration { get; protected set; } = new HousingConfig();
    
    private readonly ConcurrentBag<HousingMapMarkerInfo> housingMarkers = new();

    private bool isHousingDistrict;

    protected override bool ShouldDrawMarkers(Map map) {
        if (!isHousingDistrict) return false;
        
        return base.ShouldDrawMarkers(map);
    }

    public override void LoadForMap(MapData mapData) => Task.Run(() => {
        housingMarkers.Clear();
        isHousingDistrict = GetHousingDistrictID(mapData.Map) != uint.MaxValue;

        if (!isHousingDistrict) return;

        foreach (var marker in LuminaCache<HousingMapMarkerInfo>.Instance.Where(info => info.Map.Row == mapData.Map.RowId)) {
            housingMarkers.Add(marker);
        }
    });

    protected override void UpdateMarkers(Viewport viewport, Map map) {
        var config = GetConfig<HousingConfig>();

        if (Util.AssemblyVersion is "9.0.0.5") {
            UpdateText("DalamudVersion9.0.0.5Disable", () => new MappyMapText {
                TextId = "DalamudVersion9.0.0.5Disable",
                Text = "Incompatible Dalamud Version\nModule disabled until Dalamud v9.0.0.6",
                TexturePosition = new Vector2(1024.0f, 2048.0f),
            });
            return;
        }
        
        foreach (var marker in housingMarkers) {
            UpdateIcon((marker.RowId, marker.SubRowId), () => new MappyMapIcon {
                MarkerId = (marker.RowId, marker.SubRowId),
                IconId = GetIconId(marker, map),
                ObjectPosition = new Vector2(marker.X, marker.Z),
                Tooltip = GetTooltip(marker),
            }, icon => {
                icon.IconId = GetIconId(marker, map);
            });

            if (config.ShowHousingNumber && marker.SubRowId is not (61 or 60)) {
                UpdateText((marker.RowId, marker.SubRowId), () => new MappyMapText {
                    TextId = (marker.RowId, marker.SubRowId),
                    Text = $"{marker.SubRowId + 1}",
                    ObjectPosition = new Vector2(marker.X, marker.Z) + new Vector2(4.0f, 4.0f),
                });
            }
        }
    }
    
    private uint GetIconId(ExcelRow marker, ExcelRow map) {
        if (GetHousingLandSet(map) is not {} housingSizeInfo) return 0;

        return marker.SubRowId switch {
            60 when IsHousingManagerValid() => (uint) HousingManager.Instance()->OutdoorTerritory->GetPlotIcon(128),
            61 when IsHousingManagerValid() => (uint) HousingManager.Instance()->OutdoorTerritory->GetPlotIcon(129),
            _  when IsHousingManagerValid() => (uint) HousingManager.Instance()->OutdoorTerritory->GetPlotIcon((byte) marker.SubRowId),
                
            60 when !IsHousingManagerValid() => 60789,
            61 when !IsHousingManagerValid() => 60789,
            _ when !IsHousingManagerValid() => housingSizeInfo.PlotSize[marker.SubRowId] switch {
                0 => 60754, // Small House
                1 => 60755, // Medium House
                2 => 60756, // Large House
                _ => 60750  // Housing Placeholder Marker
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string GetTooltip(ExcelRow marker) => marker.SubRowId switch {
        60 or 61 => Strings.Apartment,
        _ => $"{marker.SubRowId + 1} {Strings.Plot}"
    };

    private bool IsHousingManagerValid() {
        if (HousingManager.Instance() is null) return false;
        if (HousingManager.Instance()->OutdoorTerritory is null) return false;

        return true;
    }

    private static HousingLandSet? GetHousingLandSet(ExcelRow map) 
        => LuminaCache<HousingLandSet>.Instance.GetRow(GetHousingDistrictID(map));
    
    private static uint GetHousingDistrictID(ExcelRow map) => map.RowId switch {
        72 or 192 => 0,
        82 or 193 => 1,
        83 or 194 => 2,
        364 or 365 => 3,
        679 or 680 => 4,
        _ => uint.MaxValue
    };
}