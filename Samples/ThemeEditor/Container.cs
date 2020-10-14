﻿using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI.Controls;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace ThemeEditor
{
    class Container: ScreenObject
    {
        public ControlsConsole OptionsPanel;
        public SettingsConsole SettingsPanel;
        public ControlsTest TestingPanel;

        public static SadConsole.UI.Colors EditingColors;

        public Container()
        {
            //EditingColors = SadConsole.UI.Themes.Library.Default.Colors.Clone();
            EditingColors = SadConsole.UI.Colors.CreateSadConsoleBlue();

            OptionsPanel = new ControlsConsole(6, 2);
            Border.AddToSurface(OptionsPanel, "");
            OptionsPanel.Position = (2, 1);
            OptionsPanel.Controls.Add(new Button(6) { Text = "Load", Position = (0, 0) });
            OptionsPanel.Controls.Add(new Button(6) { Text = "Save", Position = (0, 1), IsEnabled = false });

            SettingsPanel = new SettingsConsole(30, 40);
            Border.AddToSurface(SettingsPanel, "Settings");
            SettingsPanel.Position = (4, 3);
            
            TestingPanel = new ControlsTest();
            Border.AddToSurface(TestingPanel, "Preview");
            TestingPanel.Position = SettingsPanel.Position + (SettingsPanel.Surface.Buffer.Width, 0) + (4, 0);
            TestingPanel.Controls.ThemeColors = EditingColors;

            Children.Add(SettingsPanel);
            Children.Add(OptionsPanel);
            Children.Add(TestingPanel);
        }

        public void RefreshTestingPanel()
        {
            TestingPanel.RedrawColors();
        }

        public static void PrintHeader(ICellSurface target, SadConsole.UI.Colors themeColors, Point position, int totalWidth, string text)
        {
            totalWidth = totalWidth - text.Length - 3;

            if (totalWidth < 0)
                throw new Exception("Header text too big");

            var lineText = ColoredString.FromGradient(
                new Gradient(
                    new[] { themeColors.White, themeColors.White, themeColors.Gray, themeColors.Gray, themeColors.GrayDark, themeColors.GrayDark },
                    new[] { 0f, 0.3f, 0.31f, 0.65f, 0.66f, 1f }),
                new string((char)196, totalWidth));
            
            target.Print(position.X, position.Y, new ColoredString(new ColoredGlyph(themeColors.White, Color.Transparent, 196))
                                                + new ColoredString(new ColoredGlyph(themeColors.Cyan, Color.Transparent, 254))
                                                + new ColoredString(text, themeColors.Title, Color.Transparent)
                                                + new ColoredString(new ColoredGlyph(themeColors.Cyan, Color.Transparent, 254))
                                                );

            var targetPosition = Point.FromIndex(position.ToIndex(target.BufferWidth) + text.Length + 3, target.BufferWidth);
            target.Print(targetPosition.X, targetPosition.Y, lineText);
        }
    }
}
