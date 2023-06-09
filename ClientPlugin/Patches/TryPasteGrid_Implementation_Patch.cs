using HarmonyLib;
using Sandbox.Engine.Multiplayer;
using Sandbox;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Entity;
using VRage.Network;
using VRage.Utils;
using VRage;
using VRageMath;
using static Sandbox.Game.Entities.MyCubeGrid;
using VRage.Game;
using ParallelTasks;
using System.Reflection.Emit;
using Sandbox.Game.Multiplayer;

namespace ClientPlugin.Patches
{
    //Works in local worlds only or multiplayer worlds hosted by the user.
    [HarmonyPatch(typeof(MyCubeGrid), nameof(MyCubeGrid.TryPasteGrid_Implementation))]
    internal static class TryPasteGrid_Implementation_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //No need for check if player is not admin on server as server will check.

            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                //For some reason, codes[i].operand does not return the actual object.
                if (codes[i].opcode == OpCodes.Ldloca_S && ((LocalBuilder)codes[i].operand).LocalType == typeof(Vector3D?))
                {
                    //nop the current instruction and the next 29.
                    for (var i2 = i; i2 < i + 30; i2++)
                    {
                        codes[i2].opcode = OpCodes.Nop;
                    }
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
