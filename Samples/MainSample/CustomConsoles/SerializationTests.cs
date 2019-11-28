﻿using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    internal class SerializationTests : Console
    {
        private readonly ControlsConsole controlsConsole;
        private readonly Console masterView;
        private readonly SadConsole.Console loadedView;

        // Controls
        private readonly SadConsole.Controls.RadioButton optionButtonSurface;
        private readonly SadConsole.Controls.RadioButton optionButtonView;
        private readonly SadConsole.Controls.RadioButton optionButtonLayered;
        private readonly SadConsole.Controls.RadioButton optionButtonAnimated;
        private readonly SadConsole.Console basicSurface;
        private readonly SadConsole.AnimatedConsole animatedSurface;
        private readonly SadConsole.Console viewSurface;
        private SadConsole.LayeredConsole layeredSurface;
        private readonly SadConsole.Console emptySurface;

        public SerializationTests() : base(80, 23)
        {
            //Settings.SerializationIsCompressed = true;
            controlsConsole = new ControlsConsole(80, 4);

            masterView = new Console(34, 15);
            loadedView = new Console(34, 15);

            masterView.Fill(Color.White, Color.Red, 0);
            loadedView.Fill(Color.White, Color.Blue, 0);

            UseMouse = true;

            // Add the consoles to the list.
            Children.Add(controlsConsole);
            Children.Add(masterView);
            Children.Add(loadedView);

            // Setup main view
            masterView.Position = new Point(3, 6);

            // Setup sub view
            loadedView.Position = new Point(80 - 37, 6);


            // Setup controls
            controlsConsole.Position = new Point(0, 0);

            optionButtonSurface = new SadConsole.Controls.RadioButton(18, 1)
            {
                Text = "Surface",
                Position = new Point(1, 1),
            };
            optionButtonSurface.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonSurface);

            optionButtonView = new SadConsole.Controls.RadioButton(18, 1)
            {
                Text = "Surface View",
                Position = new Point(1, 2)
            };
            optionButtonView.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonView);

            optionButtonLayered = new SadConsole.Controls.RadioButton(21, 1)
            {
                Text = "Layered Surface",
                Position = new Point(optionButtonSurface.Bounds.Right + 1, 1)
            };
            optionButtonLayered.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonLayered);

            optionButtonAnimated = new SadConsole.Controls.RadioButton(21, 1)
            {
                Text = "Animated Surface",
                Position = new Point(optionButtonSurface.Bounds.Right + 1, 2)
            };
            optionButtonAnimated.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonAnimated);

            var buttonSave = new SadConsole.Controls.Button(17, 1)
            {
                Text = "Save and Load",
                Position = new Point(controlsConsole.Width - 19, 1)
            };
            buttonSave.Click += ButtonSave_Click;
            controlsConsole.Add(buttonSave);

            basicSurface = new SadConsole.Console(34, 15);

            animatedSurface = SadConsole.AnimatedConsole.CreateStatic(34, 15, 15, 0.3d);
            viewSurface = new Console(1, 1);
            viewSurface.SetSurface(basicSurface, new Rectangle(5, 2, 34 - 10, 15 - 4));
            //emptySurface = (SadConsole.Surfaces.BasicSurface)loadedView.TextSurface;

            MakeBasicSurface();
            MakeLayeredSurface();

            optionButtonSurface.IsSelected = true;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            loadedView.Children.Clear();

            if (optionButtonSurface.IsSelected)
            {
                basicSurface.Save("basicsurface.surface");
                loadedView.Children.Add(SadConsole.Console.Load("basicsurface.surface"));
            }
            else if (optionButtonView.IsSelected)
            {
                viewSurface.Save("viewsurface.view");
                var loaded = Console.Load("viewsurface.view");
                loaded.SetSurface(basicSurface, new Rectangle(5, 2, 34 - 10, 15 - 4));
                basicSurface.IsDirty = true;
                viewSurface.IsDirty = true;
                loadedView.Children.Add(loaded);
                loaded.Fill(new Rectangle(1, 1, loaded.Width - 2, 3), Color.White, Color.DarkBlue, 0, SpriteEffects.None);
                loaded.Print(2, 2, "Loaded view");
                //loadedView.Children.Add(SadConsole.Surfaces.Basic.Load("viewsurface.view", basicSurface);
            }
            else if (optionButtonLayered.IsSelected)
            {
                layeredSurface.Save("layeredObject.layered");
                var layers = LayeredConsole.Load("layeredObject.layered");
                loadedView.Children.Add(layers);
            }
            else if (optionButtonAnimated.IsSelected)
            {
                animatedSurface.Save("animatedsurface.animation");
                var animation = AnimatedConsole.Load("animatedsurface.animation");
                animation.Start();
                loadedView.Children.Add(animation);
            }
        }

        private void OptionButton_IsSelectedChanged(object sender, EventArgs e)
        {
            loadedView.Children.Clear();
            masterView.Children.Clear();

            if (optionButtonSurface.IsSelected)
            {
                masterView.Children.Add(basicSurface);
            }
            else if (optionButtonView.IsSelected)
            {
                masterView.Children.Add(viewSurface);
            }
            else if (optionButtonLayered.IsSelected)
            {
                masterView.Children.Add(layeredSurface);
            }
            else if (optionButtonAnimated.IsSelected)
            {
                masterView.Children.Add(animatedSurface);
            }
        }

        public override void Update(TimeSpan delta)
        {
            if (optionButtonAnimated.IsSelected)
            {
                //animatedSurface.Update(delta);
                //(loadedView as SadConsole.Surfaces.Animated)?.Update(delta);

            }

            base.Update(delta);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Process mouse for each console
            var childState = new MouseConsoleState(controlsConsole, state.Mouse);

            if (childState.IsOnConsole)
            {
                return controlsConsole.ProcessMouse(childState);
            }

            return false;
        }

        public override bool ProcessKeyboard(Keyboard info) => controlsConsole.ProcessKeyboard(info);


        private void MakeBasicSurface()
        {
            basicSurface.Print(0, 0, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".Repeat(9));
            basicSurface.SetGlyph(0, 0, 7);
            basicSurface.SetGlyph(1, 0, 8);
            basicSurface.SetGlyph(2, 0, 9);
            basicSurface.SetGlyph(3, 0, 10);
            ColorGradient gradient = new ColorGradient(FeatureDemo.Theme.Blue, FeatureDemo.Theme.Yellow);
            for (int i = 0; i < 510; i += 10)
            {
                Point point = basicSurface.GetPointFromIndex(i);
                basicSurface.Print(point.X, point.Y, gradient.ToColoredString(basicSurface.GetString(i, 10)));
            }

            // Mirror 1
            int startSet1 = new Point(0, 3).ToIndex(34);
            int startSet2 = new Point(0, 6).ToIndex(34);
            int startSet3 = new Point(0, 9).ToIndex(34);

            for (int i = 0; i < 34 * 3; i++)
            {
                basicSurface.Cells[startSet1 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                basicSurface.Cells[startSet1 + i].Background = FeatureDemo.Theme.PurpleDark;

                basicSurface.Cells[startSet2 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                basicSurface.Cells[startSet2 + i].Background = FeatureDemo.Theme.OrangeDark;

                basicSurface.Cells[startSet3 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally | Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                basicSurface.Cells[startSet3 + i].Background = FeatureDemo.Theme.GreenDark;
            }
        }

        private void MakeLayeredSurface()
        {
            layeredSurface = new LayeredConsole(35, 15, 3);

            CellSurfaceLayer layer = layeredSurface.GetLayer(0);
            layer.Fill(FeatureDemo.Theme.Gray, FeatureDemo.Theme.GrayDark, 0);
            layer.Print(14, 4, "Layer 0");

            layer = layeredSurface.GetLayer(1);
            layer.Fill(FeatureDemo.Theme.Green, FeatureDemo.Theme.GreenDark, 0);
            layer.Fill(new Rectangle(10, 4, 34 - 20, 15 - 8), Color.White, Color.Transparent, 0);
            layer.Print(14, 2, "Layer 1");

            layer = layeredSurface.GetLayer(2);
            layer.Fill(FeatureDemo.Theme.Brown, FeatureDemo.Theme.BrownDark, 0);
            layer.Fill(new Rectangle(5, 2, 34 - 10, 15 - 4), Color.White, Color.Transparent, 0);
            layer.Print(14, 0, "Layer 2");
        }
    }

    internal static class StringHelpers
    {
        public static string Repeat(this string value, int times)
        {
            StringBuilder builder = new StringBuilder(value.Length * times);
            for (int i = 0; i != times; i++)
            {
                builder.Append(value);
            }
            return builder.ToString();
        }
    }


}

