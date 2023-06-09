using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientPlugin.Patches
{
    //Works in local worlds only or multiplayer worlds hosted by the user.
    //Bypasses the check in MyCubeGrid.PasteGridData.TryPasteGrid
    [HarmonyPatch]
    internal static class TestPastedGridPlacement_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method("Sandbox.Game.Entities.MyCubeGrid+PasteGridData:TestPastedGridPlacement");
        }

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
