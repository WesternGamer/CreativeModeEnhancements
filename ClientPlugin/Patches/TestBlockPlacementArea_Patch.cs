using HarmonyLib;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Entity;
using VRage.Game.ObjectBuilders.Definitions.SessionComponents;
using VRageMath;

namespace ClientPlugin.Patches
{
    [HarmonyPatch]
    internal static class TestBlockPlacementArea_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(MyCubeGrid), nameof(MyCubeGrid.TestBlockPlacementArea), new Type[] { typeof(MyCubeBlockDefinition), 
                typeof(MyBlockOrientation?), 
                typeof(MatrixD), 
                typeof(MyGridPlacementSettings).MakeByRefType(), 
                typeof(BoundingBoxD), 
                typeof(bool), 
                typeof(MyEntity), 
                typeof(bool) });
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
