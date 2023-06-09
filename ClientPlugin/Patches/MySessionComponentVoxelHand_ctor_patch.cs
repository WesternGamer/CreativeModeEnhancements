using HarmonyLib;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.SessionComponents;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPlugin.Patches
{
    [HarmonyPatch(typeof(MySessionComponentVoxelHand), MethodType.Constructor)]
    internal static class MySessionComponentVoxelHand_ctor_patch
    {
        private static void Postfix(MySessionComponentVoxelHand __instance)
        {
            __instance.SnapToVoxel = false;
            __instance.ShowGizmos = false;
        }
    }
}
