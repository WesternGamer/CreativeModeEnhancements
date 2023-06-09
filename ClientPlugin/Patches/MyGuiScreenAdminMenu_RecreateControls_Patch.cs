using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;
using VRage;
using VRageMath;
using Sandbox.Game.Localization;
using EmptyKeys.UserInterface.Controls;
using Sandbox.Game.World;
using VRage.Game;
using Sandbox.Game.Multiplayer;
using static System.Net.Mime.MediaTypeNames;
using Sandbox.Game.SessionComponents;

namespace ClientPlugin.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenAdminMenu), nameof(MyGuiScreenAdminMenu.RecreateControls))]
    internal static class MyGuiScreenAdminMenu_RecreateControls_Patch
    {
        private static void Postfix(MyGuiScreenAdminMenu __instance)
        {
            if (Sync.MultiplayerActive && !MySession.Static.IsUserAdmin(Sync.MyId))
            {
                return;
            }
            Plugin.Instance.Log.Info("Adding custom controls to admin menu.");
            MyGuiScreenAdminMenu.MyPageEnum m_currentPage = (MyGuiScreenAdminMenu.MyPageEnum)AccessTools.Field(typeof(MyGuiScreenAdminMenu), "m_currentPage").GetValue(__instance);

            switch (m_currentPage)
            {
                case MyGuiScreenAdminMenu.MyPageEnum.AdminTools:
                    {
                        Plugin.Instance.Log.Info("Adding Admin Menu custom controls");
                        Vector2 m_currentPosition = (Vector2)AccessTools.Field(typeof(MyGuiScreenAdminMenu), "m_currentPosition").GetValue(__instance);
                        MyGuiControlLabel myGuiControlLabel = new MyGuiControlLabel
                        {
                            Position = m_currentPosition + new Vector2(0.001f, 0f),
                            OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                            Text = MyTexts.GetString(MySpaceTexts.WorldSettings_EnableSunRotation),
                            IsAutoScaleEnabled = true,
                            IsAutoEllipsisEnabled = true
                        };
                        myGuiControlLabel.SetMaxSize(new Vector2(0.25f, float.PositiveInfinity));
                        MyGuiControlCheckbox enableSunRotationCheckBox = new MyGuiControlCheckbox(new Vector2(m_currentPosition.X + 0.293f, m_currentPosition.Y - 0.01f), null, null, isChecked: false, MyGuiControlCheckboxStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
                        enableSunRotationCheckBox.IsCheckedChanged = delegate (MyGuiControlCheckbox checkbox) {  MySession.Static.Settings.EnableSunRotation = checkbox.IsChecked; };
                        enableSunRotationCheckBox.SetToolTip(MySpaceTexts.ToolTipWorldSettings_EnableSunRotation);
                        enableSunRotationCheckBox.IsChecked = MySession.Static.Settings.EnableSunRotation;
                        enableSunRotationCheckBox.Enabled = MySession.Static.IsUserAdmin(Sync.MyId);
                        __instance.Controls.Add(enableSunRotationCheckBox);
                        __instance.Controls.Add(myGuiControlLabel);
                        
                        break;
                    }
                case MyGuiScreenAdminMenu.MyPageEnum.Weather:
                    {
                        Plugin.Instance.Log.Info("Adding Weather Menu custom controls");
                        Vector2 buttonPosition = Vector2.Zero;
                        MyGuiControlMultilineText weatherInfo = null;
                        foreach (MyGuiControlBase control in __instance.Controls)
                        {
                            if (control.GetType() == typeof(MyGuiControlButton))
                            {
                                MyGuiControlButton button = (MyGuiControlButton)control;

                                if (button.Text == MyTexts.Get(MySpaceTexts.ScreenDebugAdminMenu_Weather_Remove).ToString())
                                {
                                    buttonPosition = button.Position;
                                    buttonPosition.Y += 0.0425f;
                                }
                            }

                            if (control.GetType() == typeof(MyGuiControlMultilineText))
                            {
                                MyGuiControlMultilineText myGuiControlMultilineText = (MyGuiControlMultilineText)control;

                                weatherInfo = myGuiControlMultilineText;
                            }
                        }

                        MyGuiControlButton freezeWeatherButton = __instance.AddButton("Weather On/Off", delegate { OnWeatherToggle(); });
                        freezeWeatherButton.Position = buttonPosition;
                        freezeWeatherButton.VisualStyle = MyGuiControlButtonStyleEnum.Rectangular;
                        freezeWeatherButton.TextScale = 0.8f;
                        freezeWeatherButton.Size = new Vector2(0.14f, freezeWeatherButton.Size.Y);
                        freezeWeatherButton.Enabled = true;
                        freezeWeatherButton.IsAutoScaleEnabled = false;
                        freezeWeatherButton.IsAutoEllipsisEnabled = false;

                        MyGuiControlLabel controlLabel = new MyGuiControlLabel();
                        controlLabel.Position = freezeWeatherButton.Position + new Vector2(0f, 0.0425f);
                        controlLabel.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP;
                        controlLabel.Text = MyTexts.GetString(MySpaceTexts.BlockPropertyTitle_LightIntensity);
                        __instance.Controls.Add(controlLabel);

                        MyGuiControlSlider intensitySlider = new MyGuiControlSlider();
                        intensitySlider.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP;
                        intensitySlider.Position = controlLabel.Position + new Vector2(0f, 0.0325f);
                        intensitySlider.ValueChanged = OnWeatherSliderChanged;
                        __instance.Controls.Add(intensitySlider);

                        weatherInfo.Position += new Vector2(0f, 0.0725f);

                        break;
                    }
            }
            Plugin.Instance.Log.Info("Done adding custom controls to admin menu.");
        }

        private static void OnWeatherSliderChanged(MyGuiControlSlider sender)
        {
            MySectorWeatherComponent component = MySession.Static.GetComponent<MySectorWeatherComponent>();
            if (component == null)
            {
                return;
            }

            List<MyObjectBuilder_WeatherPlanetData> m_weatherPlanetData = (List<MyObjectBuilder_WeatherPlanetData>)AccessTools.Field(typeof(MySectorWeatherComponent), "m_weatherPlanetData").GetValue(component);

            foreach (MyObjectBuilder_WeatherPlanetData weatherPlanetDatum in m_weatherPlanetData)

            {
                foreach (MyObjectBuilder_WeatherEffect weather in weatherPlanetDatum.Weathers)
                {
                    if (InsideWeather(MySector.MainCamera.Position, weather))
                    {
                        weather.Intensity = sender.Value;
                    }
                }
            }
        }

        public static bool InsideWeather(Vector3D position, MyObjectBuilder_WeatherEffect weather)
        {
            if (Vector3D.Distance(position, weather.Position) < (double)weather.Radius)
            {
                return true;
            }
            return false;
        }



        private static void OnWeatherToggle()
        {
            if (MySession.Static == null)
            {
                Plugin.Instance.Log.Error("Unable to disable weather from admin menu as there is no session active.");
                return;
            }

            if (MySession.Static.Settings == null)
            {
                Plugin.Instance.Log.Error("Unable to disable weather from admin menu as current session has no world settings.");
                return;
            }

            MySession.Static.GetComponent<MySectorWeatherComponent>().SetWeather("Clear", 0f, null, verbose: false, Vector3.Zero);

            if (MySession.Static.Settings.WeatherSystem)
            {
                MySession.Static.Settings.WeatherSystem = false;

            }
            else 
            {
                MySession.Static.Settings.WeatherSystem = true;

            }

            Plugin.Instance.Log.Info($"Weather enabled = {MySession.Static.Settings.WeatherSystem}. Toggled from custom button in admin menu.");
        }
    }
}
