﻿using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPlugin.Patches
{
    [HarmonyPatch(typeof(MyCubeGrid), nameof(MyCubeGrid.TestVoxelPlacement))]
    internal static class TestVoxelPlacement_Patch
    {
        private static bool Prefix(ref bool __result)
        {
            if (Sync.MultiplayerActive && !MySession.Static.IsUserAdmin(Sync.MyId))
            {
                return true;
            }

            if (!MySession.Static.CreativeMode)
            {
                if (!MySession.Static.CreativeToolsEnabled(Sync.MyId))
                {
                    return true;
                }
            }

            __result = true;
            return false;
        }
    }
}
