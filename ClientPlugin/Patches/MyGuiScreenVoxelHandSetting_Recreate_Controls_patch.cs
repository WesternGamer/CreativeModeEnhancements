using HarmonyLib;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using VRageMath;
using static Sandbox.Graphics.GUI.MyGuiControlSeparatorList;

namespace ClientPlugin.Patches
{
    //Hides the show bounding boxes button
    [HarmonyPatch]
    internal static class MyGuiScreenVoxelHandSetting_Recreate_Controls_patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "RecreateControls", new Type[] {
                typeof(bool)
            });
        }

        private static void Postfix(MyGuiScreenBase __instance)
        {
            if (Sync.MultiplayerActive && !MySession.Static.IsUserAdmin(Sync.MyId))
            {
                return;
            }
            //Hides the show bounding boxes button and it's label
            __instance.Controls.Remove((MyGuiControlBase)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_showGizmos").GetValue(__instance));
            AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_showGizmos").SetValue(__instance, null);

            __instance.Controls.Remove((MyGuiControlBase)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_labelShowGizmos").GetValue(__instance));
            AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_labelShowGizmos").SetValue(__instance, null);

            //Move the rest of the controls up
            MyGuiControlLabel m_labelTransparency = (MyGuiControlLabel)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_labelTransparency").GetValue(__instance);
            m_labelTransparency.PositionY = m_labelTransparency.PositionY - 0.045f;

            MyGuiControlSlider m_sliderTransparency = (MyGuiControlSlider)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_sliderTransparency").GetValue(__instance);
            m_sliderTransparency.PositionY = m_sliderTransparency.PositionY - 0.045f;

            MyGuiControlLabel m_labelZoom = (MyGuiControlLabel)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_labelZoom").GetValue(__instance);
            m_labelZoom.PositionY = m_labelZoom.PositionY - 0.045f;

            MyGuiControlSlider m_sliderZoom = (MyGuiControlSlider)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_sliderZoom").GetValue(__instance);
            m_sliderZoom.PositionY = m_sliderZoom.PositionY - 0.045f;

            MyGuiControlVoxelHandSettings m_voxelControl = (MyGuiControlVoxelHandSettings)AccessTools.Field(AccessTools.TypeByName("Sandbox.Game.Gui.MyGuiScreenVoxelHandSetting"), "m_voxelControl").GetValue(__instance);
            m_voxelControl.PositionY = m_voxelControl.PositionY - 0.045f;

            foreach (MyGuiControlBase control in __instance.Controls)
            {
                if (control.GetType() == typeof(MyGuiControlSeparatorList))
                {
                    MyGuiControlSeparatorList separatorList = (MyGuiControlSeparatorList)control;
                    List<Separator> m_separators = (List<Separator>)AccessTools.Field(typeof(MyGuiControlSeparatorList), "m_separators").GetValue(separatorList);

                    Separator separator1 = m_separators[1];
                    separator1.Start = separator1.Start - new Vector2(0, 0.045f);
                    m_separators[1] = separator1;

                    Separator separator2 = m_separators[2];
                    separator2.Start = separator2.Start - new Vector2(0, 0.045f);
                    m_separators[2] = separator2;

                    continue;
                }

                if (control.GetType() == typeof(MyGuiControlMultilineText))
                {
                    MyGuiControlMultilineText text = (MyGuiControlMultilineText)control;

                    text.PositionY = text.PositionY - 0.045f;

                    text.Size = new Vector2(text.Size.X, text.Size.Y + 0.045f);
                }
            }
        }
    }
}
