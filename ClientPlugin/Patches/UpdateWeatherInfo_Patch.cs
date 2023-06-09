using EmptyKeys.UserInterface.Controls;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.SessionComponents;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace ClientPlugin.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenAdminMenu), "UpdateWeatherInfo")]
    internal static class UpdateWeatherInfo_Patch
    {
        private static bool Prefix(MyGuiScreenAdminMenu __instance)
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

            if (MySession.Static == null)
            {
                Plugin.Instance.Log.Error("Unable to disable weather enable status from admin menu as there is no session active.");
                return true;
            }

            if (MySession.Static.Settings == null)
            {
                Plugin.Instance.Log.Error("Unable to disable weather enable status from admin menu as current session has no world settings.");
                return true;
            }

            if (!MySession.Static.Settings.WeatherSystem)
            {
                if (__instance.Controls.GetControlByName("weatherinfo") != null)
                {
                    if (Sync.MultiplayerActive && !MySession.Static.IsUserAdmin(Sync.MyId))
                    {
                        (__instance.Controls.GetControlByName("weatherinfo") as MyGuiControlMultilineText).Text = new StringBuilder("Weather is disabled.");
                    }
                    else {
                        (__instance.Controls.GetControlByName("weatherinfo") as MyGuiControlMultilineText).Text = new StringBuilder("Weather is disabled. If the server has\nweather on, the server will still have\nweather on, but you won't see it.");
                    }
                }
                return false;
            }

            return true;
        }
    }
}
