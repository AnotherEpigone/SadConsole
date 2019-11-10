﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using SadRogue.Primitives;
using SadConsole.StringParser;

namespace SadConsole
{
    public partial class CellSurface
    {
        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface down.
        /// </summary>
        public int TimesShiftedDown;

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface right.
        /// </summary>
        public int TimesShiftedRight;

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface left.
        /// </summary>
        public int TimesShiftedLeft;

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface up.
        /// </summary>
        public int TimesShiftedUp;

        /// <summary>
        /// When true, the <see cref="ColoredString.Parse(string, int, CellSurface, ParseCommandStacks)"/> command is used to print strings.
        /// </summary>
        public bool UsePrintProcessor = false;

        /// <summary>
        /// Processes the effects added to cells with <see cref="o:SetEffect"/>
        /// </summary>
        //[IgnoreDataMember]
        //public EffectsManager Effects { get; protected set; }

        /// <summary>
        /// The glyph used by the <see cref="Erase(int, int, int)"/> method. Defaults to 0.
        /// </summary>
        public int EraseGlyph { get; set; } = 0;

        /// <summary>
        /// Sets each background of a cell to the array of colors. Must be the same length as this cell surface.
        /// </summary>
        /// <param name="pixels">The colors to place.</param>
        public void SetPixels(Color[] pixels)
        {
            if (pixels.Length != Cells.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of this cell surface.");
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                Cells[i].Background = pixels[i];
            }

            IsDirty = true;
        }

        /// <summary>
        /// Sets each background of a cell to the array of colors.
        /// </summary>
        /// <param name="area">An area to fill with pixels.</param>
        /// <param name="pixels"></param>
        public void SetPixels(Rectangle area, Color[] pixels)
        {
            if (pixels.Length != area.Width * area.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of the area.");
            }

            for (int x = area.X; x < area.X + area.Width; x++)
            {
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    int index = y * BufferWidth + x;

                    if (IsValidCell(index))
                    {
                        Cells[y * BufferWidth + x].Background = pixels[(y - area.Y) * area.Width + (x - area.X)];
                    }
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidCell(int x, int y) =>
            x >= 0 && x < BufferWidth && y >= 0 && y < BufferHeight;

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <param name="index">If the cell is valid, the index of the cell when found.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidCell(int x, int y, out int index)
        {
            if (x >= 0 && x < BufferWidth && y >= 0 && y < BufferHeight)
            {
                index = y * BufferWidth + x;
                return true;
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Tests if a cell is valid based on its index.
        /// </summary>
        /// <param name="index">The index to test.</param>
        /// <returns>A true value indicating the cell index is in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidCell(int index) =>
            index >= 0 && index < Cells.Length;

        /// <summary>
        /// Changes the glyph of a specified cell to a new value.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph of the cell.</param>
        public void SetGlyph(int x, int y, int glyph)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Glyph = glyph;
            IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph and foreground of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Foreground = foreground;
            Cells[index].Glyph = glyph;
            IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, and background of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground, Color background)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Background = background;
            Cells[index].Foreground = foreground;
            Cells[index].Glyph = glyph;
            IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, background, and effect of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        /// <param name="mirror">Sets how the glyph will be mirrored.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground, Color background, Mirror mirror)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Background = background;
            Cells[index].Foreground = foreground;
            Cells[index].Glyph = glyph;
            Cells[index].Mirror = mirror;

            IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, background, and effect of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        /// <param name="mirror">Sets how the glyph will be mirrored.</param>
        /// <param name="decorators">Decorators to set on the cell. Will clear existing decorators first.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground, Color background, Mirror mirror, IEnumerable<CellDecorator> decorators)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Background = background;
            Cells[index].Foreground = foreground;
            Cells[index].Glyph = glyph;
            Cells[index].Mirror = mirror;
            Cells[index].Decorators = decorators?.ToArray() ?? Array.Empty<CellDecorator>();

            IsDirty = true;
        }

        /// <summary>
        /// Gets the glyph of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The glyph index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetGlyph(int x, int y) =>
            Cells[y * BufferWidth + x].Glyph;

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetForeground(int x, int y, Color color)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Foreground = color;
            IsDirty = true;
        }

        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color GetForeground(int x, int y) =>
            Cells[y * BufferWidth + x].Foreground;

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetBackground(int x, int y, Color color)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Background = color;
            IsDirty = true;
        }

        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color GetBackground(int x, int y) =>
            Cells[y * BufferWidth + x].Background;

        /*

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int x, int y, ICellEffect effect)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Effects.SetEffect(Cells[index], effect);
            IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="index">Index of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int index, ICellEffect effect)
        {
            if (!IsValidCell(index))
            {
                return;
            }

            Effects.SetEffect(Cells[index], effect);
            IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a list of cells to the specified effect.
        /// </summary>
        /// <param name="cells">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(IEnumerable<Cell> cells, ICellEffect effect)
        {
            Effects.SetEffect(cells, effect);
            IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="cell">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(Cell cell, ICellEffect effect)
        {
            Effects.SetEffect(cell, effect);
            IsDirty = true;
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ICellEffect GetEffect(int x, int y) =>
            Effects.GetEffect(Cells[GetIndexFromPoint(x, y)]);

        */

        /// <summary>
        /// Changes the appearance of the cell. The appearance represents the look of a cell and will first be cloned, then applied to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="appearance">The desired appearance of the cell. A null value cannot be passed.</param>
        public void SetCellAppearance(int x, int y, ColoredGlyph appearance)
        {
            if (appearance == null)
            {
                throw new NullReferenceException("Appearance may not be null.");
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            appearance.CopyAppearanceTo(Cells[index]);
            IsDirty = true;
        }

        /// <summary>
        /// Gets the appearance of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The appearance.</returns>
        public ColoredGlyph GetCellAppearance(int x, int y)
        {
            var appearance = new ColoredGlyph();
            Cells[y * BufferWidth + x].CopyAppearanceTo(appearance);
            return appearance;
        }

        /// <summary>
        /// Gets an enumerable of cells over a specific area.
        /// </summary>
        /// <param name="area">The area to get cells from.</param>
        /// <returns>A new array with references to each cell in the area.</returns>
        public IEnumerable<ColoredGlyph> GetCells(Rectangle area)
        {
            area = Rectangle.GetIntersection(area, new Rectangle(0, 0, BufferWidth, BufferHeight));

            if (area == Rectangle.Empty)
            {
                yield break;
            }

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    yield return Cells[(y + area.Y) * BufferWidth + (x + area.X)];
                }
            }
        }

        /// <summary>
        /// Gets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Mirror GetMirror(int x, int y) =>
            Cells[y * BufferWidth + x].Mirror;

        /// <summary>
        /// Sets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="mirror">The mirror of the cell.</param>
        public void SetMirror(int x, int y, Mirror mirror)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Mirror = mirror;
            IsDirty = true;
        }

        /// <summary>
        /// Sets the decorator of one or more cells.
        /// </summary>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDecorator(int x, int y, int count, params CellDecorator[] decorators) =>
            SetDecorator(GetIndexFromPoint(x, y), count, decorators);

        /// <summary>
        /// Sets the decorator of one or more cells.
        /// </summary>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        public void SetDecorator(int index, int count, params CellDecorator[] decorators)
        {
            if (count <= 0)
            {
                return;
            }

            if (!IsValidCell(index))
            {
                return;
            }

            if (index + count > Cells.Length)
            {
                count = Cells.Length - index;
            }

            if (decorators != null && decorators.Length == 0)
            {
                decorators = null;
            }

            for (int i = index; i < index + count; i++)
            {
                if (decorators == null)
                {
                    Cells[i].Decorators = Array.Empty<CellDecorator>();
                }
                else
                {
                    Cells[i].Decorators = (CellDecorator[])decorators.Clone();
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Appends the decorators to one or more cells
        /// </summary>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDecorator(int x, int y, int count, params CellDecorator[] decorators) =>
            AddDecorator(GetIndexFromPoint(x, y), count, decorators);

        /// <summary>
        /// Appends the decorators to one or more cells
        /// </summary>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        /// <param name="decorators">The decorators. If <code>null</code>, does nothing.</param>
        public void AddDecorator(int index, int count, params CellDecorator[] decorators)
        {
            if (count <= 0)
            {
                return;
            }

            if (!IsValidCell(index))
            {
                return;
            }

            if (index + count > Cells.Length)
            {
                count = Cells.Length - index;
            }

            if (decorators == null || decorators.Length == 0)
            {
                return;
            }

            for (int i = index; i < index + count; i++)
            {
                Cells[i].Decorators = Cells[i].Decorators.Union(decorators).Distinct().ToArray();
            }

            IsDirty = true;
        }

        /// <summary>
        /// Clears the decorators of the specified cells.
        /// </summary>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDecorators(int x, int y, int count) =>
            SetDecorator(x, y, count, null);

        /// <summary>
        /// Clears the decorators of the specified cells.
        /// </summary>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDecorators(int index, int count) =>
            SetDecorator(index, count, null);

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public void Print(int x, int y, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            if (!UsePrintProcessor)
            {
                int end = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    Cells[index].Glyph = text[charIndex];
                    charIndex++;
                }
            }
            else
            {
                PrintNoCheck(index, ColoredString.Parse(text, index, this));
            }

            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location and color, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        public void Print(int x, int y, string text, Color foreground)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            if (!UsePrintProcessor)
            {
                int end = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    Cells[index].Glyph = text[charIndex];
                    Cells[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behavior = new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behavior);
                PrintNoCheck(index, ColoredString.Parse(text, index, this, stacks));
            }
            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified foreground and background color, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        public void Print(int x, int y, string text, Color foreground, Color background)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            if (!UsePrintProcessor)
            {
                int end = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    Cells[index].Glyph = text[charIndex];
                    Cells[index].Background = background;
                    Cells[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behaviorFore = new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var behaviorBack = new ParseCommandRecolor { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behaviorFore);
                stacks.AddSafe(behaviorBack);
                PrintNoCheck(index, ColoredString.Parse(text, index, this, stacks));
            }
            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        /// <param name="mirror">The mirror to set on all characters in the text.</param>
        public void Print(int x, int y, string text, Color foreground, Color background, Mirror mirror)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            if (!UsePrintProcessor)
            {
                int end = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    Cells[index].Glyph = text[charIndex];

                    Cells[index].Background = background;
                    Cells[index].Foreground = foreground;
                    Cells[index].Mirror = mirror;

                    charIndex++;
                }
            }
            else
            {
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground });
                stacks.AddSafe(new ParseCommandRecolor { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background });
                stacks.AddSafe(new ParseCommandMirror { Mirror = mirror, CommandType = CommandTypes.Mirror });

                PrintNoCheck(index, ColoredString.Parse(text, index, this, stacks));
            }
            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="mirror">The mirror to set on all characters in the text.</param>
        public void Print(int x, int y, string text, Mirror mirror)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            if (!UsePrintProcessor)
            {
                int total = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    Cells[index].Glyph = text[charIndex];
                    Cells[index].Mirror = mirror;

                    charIndex++;
                }
            }
            else
            {
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(new ParseCommandMirror { Mirror = mirror, CommandType = CommandTypes.Mirror });

                PrintNoCheck(index, ColoredString.Parse(text, index, this, stacks));
            }
            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="appearance">The appearance of the cell</param>
        /// <param name="effect">An optional effect to apply to the printed cells.</param>
        public void Print(int x, int y, string text, ColoredGlyph appearance)//, ICellEffect effect = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            int end = index + text.Length > Cells.Length ? Cells.Length : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                ColoredGlyph cell = Cells[index];
                appearance.CopyAppearanceTo(cell);
                cell.Glyph = text[charIndex];
                //Effects.SetEffect(cell, effect);
                charIndex++;
            }
            IsDirty = true;
        }

        /// <summary>
        /// Draws a single glyph on the console at the specified location.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="glyph">The glyph to display.</param>
        public void Print(int x, int y, ColoredGlyph glyph)
        {
            if (glyph == null)
            {
                return;
            }

            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            ColoredGlyph cell = Cells[index];
            cell.CopyAppearanceFrom(glyph);
            cell.Glyph = glyph.Glyph;
            IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public void Print(int x, int y, ColoredString text)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            PrintNoCheck(index, text);
            IsDirty = true;
        }


        private void PrintNoCheck(int index, ColoredString text)
        {
            int end = index + text.Count > Cells.Length ? Cells.Length : index + text.Count;
            int charIndex = 0;

            for (; index < end; index++)
            {
                if (!text.IgnoreGlyph)
                {
                    Cells[index].Glyph = text[charIndex].GlyphCharacter;
                }

                if (!text.IgnoreBackground)
                {
                    Cells[index].Background = text[charIndex].Background;
                }

                if (!text.IgnoreForeground)
                {
                    Cells[index].Foreground = text[charIndex].Foreground;
                }

                if (!text.IgnoreMirror)
                {
                    Cells[index].Mirror = text[charIndex].Mirror;
                }

                if (!text.IgnoreEffect)
                {
                    //SetEffect(index, text[charIndex].Effect);
                }

                charIndex++;
            }
            IsDirty = true;
        }

        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(int x, int y, int length) =>
            GetString(y * BufferWidth + x, length);

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public string GetString(int index, int length)
        {
            if (index >= 0 && index < Cells.Length)
            {
                var sb = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    int tempIndex = i + index;

                    if (tempIndex < Cells.Length)
                    {
                        sb.Append((char)Cells[tempIndex].Glyph);
                    }
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ColoredString GetStringColored(int x, int y, int length) =>
            GetStringColored(y * BufferWidth + x, length);

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public ColoredString GetStringColored(int index, int length)
        {
            if (index < 0 || index >= Cells.Length)
            {
                return new ColoredString(string.Empty);
            }

            var sb = new ColoredString(length);

            for (int i = 0; i < length; i++)
            {
                int tempIndex = i + index;
                var cell = (ColoredGlyph)sb[i];
                if (tempIndex < Cells.Length)
                {
                    Cells[tempIndex].CopyAppearanceTo(cell);
                }
            }

            return sb;

        }

        /// <summary>
        /// Resets the shifted amounts to 0, as if the surface has never shifted.
        /// </summary>
        public void ClearShiftValues()
        {
            TimesShiftedDown = 0;
            TimesShiftedUp = 0;
            TimesShiftedLeft = 0;
            TimesShiftedRight = 0;
        }

        /// <summary>
        /// Scrolls all the console data up by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftUp() =>
            ShiftUp(1);

        /// <summary>
        /// Scrolls all the console data up by the specified amount of rows.
        /// </summary>
        /// <param name="amount">How many rows to shift.</param>
        /// <param name="wrap">When false, a blank line appears at the bottom. When true, the top line appears at the bottom.</param>
        public void ShiftUp(int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftDown(Math.Abs(amount), wrap);
                return;
            }

            TimesShiftedUp += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(BufferHeight * amount);

                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < BufferWidth; x++)
                    {
                        var tempCell = new ColoredGlyph();
                        Cells[y * BufferWidth + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, (BufferHeight - amount + y) * BufferWidth + x));
                    }
                }
            }

            for (int y = amount; y < BufferHeight; y++)
            {
                for (int x = 0; x < BufferWidth; x++)
                {
                    ColoredGlyph destination = Cells[(y - amount) * BufferWidth + x];
                    ColoredGlyph source = Cells[y * BufferWidth + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }


            if (!wrap)
            {
                for (int y = BufferHeight - amount; y < BufferHeight; y++)
                {
                    for (int x = 0; x < BufferWidth; x++)
                    {
                        Clear(x, y);
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = Cells[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data down by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftDown() =>
            ShiftDown(1);

        /// <summary>
        /// Scrolls all the console data down by the specified amount of rows.
        /// </summary>
        /// <param name="amount">How many rows to shift.</param>
        /// <param name="wrap">When false, a blank line appears at the top. When true, the bottom line appears at the top.</param>
        public void ShiftDown(int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftUp(Math.Abs(amount), wrap);
                return;
            }

            TimesShiftedDown += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(BufferHeight * amount);

                for (int y = BufferHeight - amount; y < BufferHeight; y++)
                {
                    for (int x = 0; x < BufferWidth; x++)
                    {
                        var tempCell = new ColoredGlyph();
                        Cells[y * BufferWidth + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, (amount - (BufferHeight - y)) * BufferWidth + x));
                    }
                }
            }

            for (int y = (BufferHeight - 1) - amount; y >= 0; y--)
            {
                for (int x = 0; x < BufferWidth; x++)
                {
                    ColoredGlyph destination = Cells[(y + amount) * BufferWidth + x];
                    ColoredGlyph source = Cells[y * BufferWidth + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
            {
                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < BufferWidth; x++)
                    {
                        ColoredGlyph source = Cells[y * BufferWidth + x];
                        source.Clear();
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = Cells[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data right by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftRight() =>
            ShiftRight(1);

        /// <summary>
        /// Scrolls all the console data right by the specified amount.
        /// </summary>
        /// <param name="amount">How much to scroll.</param>
        /// <param name="wrap">When false, a blank line appears at the left. When true, the right line appears at the left.</param>
        public void ShiftRight(int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftLeft(Math.Abs(amount), wrap);
                return;
            }

            TimesShiftedRight += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(BufferHeight * amount);

                for (int x = BufferWidth - amount; x < BufferWidth; x++)
                {
                    for (int y = 0; y < BufferHeight; y++)
                    {
                        var tempCell = new ColoredGlyph();
                        Cells[y * BufferWidth + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, y * BufferWidth + amount - (BufferWidth - x)));
                    }
                }
            }


            for (int x = BufferWidth - 1 - amount; x >= 0; x--)
            {
                for (int y = 0; y < BufferHeight; y++)
                {
                    ColoredGlyph destination = Cells[y * BufferWidth + (x + amount)];
                    ColoredGlyph source = Cells[y * BufferWidth + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
            {
                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < BufferHeight; y++)
                    {
                        Clear(x, y);

                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = Cells[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data left by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftLeft() =>
            ShiftLeft(1);

        /// <summary>
        /// Scrolls all the console data left by the specified amount.
        /// </summary>
        /// <param name="amount">How much to scroll.</param>
        /// <param name="wrap">When false, a blank line appears at the right. When true, the left line appears at the right.</param>
        public void ShiftLeft(int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftRight(Math.Abs(amount), wrap);
                return;
            }

            TimesShiftedLeft += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(BufferHeight * amount);

                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < BufferHeight; y++)
                    {
                        var tempCell = new ColoredGlyph();
                        Cells[y * BufferWidth + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, y * BufferWidth + (BufferWidth - amount + x)));
                    }
                }
            }

            for (int x = amount; x < BufferWidth; x++)
            {
                for (int y = 0; y < BufferHeight; y++)
                {
                    ColoredGlyph destination = Cells[y * BufferWidth + (x - amount)];
                    ColoredGlyph source = Cells[y * BufferWidth + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
            {
                for (int x = BufferWidth - amount; x < BufferWidth; x++)
                {
                    for (int y = 0; y < BufferHeight; y++)
                    {
                        Clear(x, y);
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = Cells[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Starting at the specified coordinate, clears the glyph, mirror, and decorators, for the specified count of cells.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="count">The count of glyphs to erase.</param>
        /// <returns>The cells processed by this method.</returns>
        /// <remarks>
        /// Cells altered by this method has the <see cref="ColoredGlyph.Glyph"/> set to <see cref="EraseGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public ColoredGlyph[] Erase(int x, int y, int count)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return Array.Empty<ColoredGlyph>();
            }

            int end = index + count > Cells.Length ? Cells.Length - index : index + count;
            int total = end - index;
            ColoredGlyph[] result = new ColoredGlyph[total];
            int resultIndex = 0;
            for (; index < end; index++)
            {
                ColoredGlyph c = Cells[index];

                c.Glyph = EraseGlyph;
                c.Mirror = Mirror.None;
                c.Decorators = Array.Empty<CellDecorator>();

                result[resultIndex] = c;
                resultIndex++;
            }

            IsDirty = true;
            return result;
        }

        /// <summary>
        /// Clears the glyph, mirror, and decorators, for the specified cell.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <remarks>
        /// The cell altered by this method has the <see cref="ColoredGlyph.Glyph"/> set to <see cref="EraseGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public void Erase(int x, int y)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            Cells[index].Glyph = EraseGlyph;
            Cells[index].Mirror = Mirror.None;
            Cells[index].Decorators = Array.Empty<CellDecorator>();

            IsDirty = true;
        }

        /// <summary>
        /// Erases all cells which clears the glyph, mirror, and decorators.
        /// </summary>
        /// <remarks>
        /// All cells have <see cref="ColoredGlyph.Glyph"/> set to <see cref="EraseGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public void Erase()
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Glyph = EraseGlyph;
                Cells[i].Mirror = Mirror.None;
                Cells[i].Decorators = Array.Empty<CellDecorator>();
            }

            IsDirty = true;
        }


        /// <summary>
        /// Clears the console data. Characters are reset to 0, the foreground and background are set to default, and effect set to none. Clears cell decorators.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() =>
            Fill(DefaultForeground, DefaultBackground, 0, Mirror.None);

        /// <summary>
        /// Clears a cell. Character is reset to 0, the foreground and background is set to default, and effect is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        public void Clear(int x, int y)
        {
            if (!IsValidCell(x, y, out int index))
            {
                return;
            }

            ColoredGlyph cell = Cells[index];
            cell.Clear();
            cell.Foreground = DefaultForeground;
            cell.Background = DefaultBackground;
            IsDirty = true;
        }

        /// <summary>
        /// Clears a segment of cells, starting from the left, extending to the right, and wrapping if needed. Character is reset to 0, the foreground and background is set to default, and effect is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="x">The x position of the left end of the segment.</param>
        /// <param name="y">The y position of the segment.</param>
        /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
        /// <remarks>This works similarly to printing a string of whitespace</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(int x, int y, int length) =>
            Fill(x, y, length, DefaultForeground, DefaultBackground, 0, Mirror.None);

        /// <summary>
        /// Clears an area of cells. Character is reset to 0, the foreground and background is set to default, and effect is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="area">The area to clear.</param>
        public void Clear(Rectangle area) =>
            Fill(area, DefaultForeground, DefaultBackground, 0, Mirror.None);

        /// <summary>
        /// Fills the console. Clears cell decorators.
        /// </summary>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Foreground to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Sprite effect to apply. If null, skips.</param>
        /// <returns>The array of all cells in this console, starting from the top left corner.</returns>
        public ColoredGlyph[] Fill(Color? foreground, Color? background, int? glyph, Mirror? mirror = null)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                if (background.HasValue)
                {
                    Cells[i].Background = background.Value;
                }

                if (foreground.HasValue)
                {
                    Cells[i].Foreground = foreground.Value;
                }

                if (glyph.HasValue)
                {
                    Cells[i].Glyph = glyph.Value;
                }

                if (mirror.HasValue)
                {
                    Cells[i].Mirror = mirror.Value;
                }

                Cells[i].Decorators = Array.Empty<CellDecorator>();
            }

            IsDirty = true;
            return Cells;
        }

        /// <summary>
        /// Fills a segment of cells, starting from the left, extending to the right, and wrapping if needed. Clears cell decorators.
        /// </summary>
        /// <param name="x">The x position of the left end of the segment. </param>
        /// <param name="y">The y position of the segment.</param>
        /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Background to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Sprite effect to apply. If null, skips.</param>
        /// <returns>An array containing the affected cells, starting from the top left corner. If x or y are out of bounds, nothing happens and an empty array is returned</returns>
        public ColoredGlyph[] Fill(int x, int y, int length, Color? foreground, Color? background, int? glyph, Mirror? mirror = null)
        {


            if (!IsValidCell(x, y, out int index))
            {
                return Array.Empty<ColoredGlyph>();
            }

            int end = index + length > Cells.Length ? Cells.Length - index : index + length;
            int total = end - index;
            ColoredGlyph[] result = new ColoredGlyph[total];
            int resultIndex = 0;
            for (; index < end; index++)
            {
                ColoredGlyph c = Cells[index];
                if (background.HasValue)
                {
                    c.Background = background.Value;
                }

                if (foreground.HasValue)
                {
                    c.Foreground = foreground.Value;
                }

                if (glyph.HasValue)
                {
                    c.Glyph = glyph.Value;
                }

                if (mirror.HasValue)
                {
                    c.Mirror = mirror.Value;
                }

                c.Decorators = Array.Empty<CellDecorator>();

                result[resultIndex] = c;
                resultIndex++;
            }


            IsDirty = true;
            return result;
        }

        /// <summary>
        /// Fills the specified area. Clears cell decorators.
        /// </summary>
        /// <param name="area">The area to fill.</param>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Background to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Sprite effect to apply. If null, skips.</param>
        /// <returns>An array containing the affected cells, starting from the top left corner. If the area is out of bounds, nothing happens and an empty array is returned.</returns>
        public ColoredGlyph[] Fill(Rectangle area, Color? foreground, Color? background, int? glyph, Mirror? mirror = null)
        {
            area = Rectangle.GetIntersection(area, new Rectangle(0, 0, BufferWidth, BufferHeight));

            if (area == Rectangle.Empty)
            {
                return new ColoredGlyph[0];
            }

            var result = new ColoredGlyph[area.Width * area.Height];
            int resultIndex = 0;

            for (int x = area.X; x < area.X + area.Width; x++)
            {
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    ColoredGlyph cell = Cells[y * BufferWidth + x];

                    if (background.HasValue)
                    {
                        cell.Background = background.Value;
                    }

                    if (foreground.HasValue)
                    {
                        cell.Foreground = foreground.Value;
                    }

                    if (glyph.HasValue)
                    {
                        cell.Glyph = glyph.Value;
                    }

                    if (mirror.HasValue)
                    {
                        cell.Mirror = mirror.Value;
                    }

                    cell.Decorators = Array.Empty<CellDecorator>();

                    result[resultIndex] = cell;
                    resultIndex++;
                }
            }

            IsDirty = true;
            return result;
        }

        /// <summary>
        /// Draws a line from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="start">Starting point of the line.</param>
        /// <param name="end">Ending point of the line.</param>
        /// <param name="foreground">Foreground to set. If null, skipped.</param>
        /// <param name="background">Background to set. If null, skipped.</param>
        /// <param name="glyph">Glyph to set. If null, skipped.</param>
        /// <returns>A list of cells the line touched; ordered from first to last.</returns>
        /// <remarks>If no foreground, background, or glyph are specified, then the list of affected cells are returned but nothing is drawn.</remarks>
        public IEnumerable<ColoredGlyph> DrawLine(Point start, Point end, Color? foreground = null, Color? background = null, int? glyph = null)
        {
            var result = new List<ColoredGlyph>();
            Func<int, int, bool> processor;

            if (foreground.HasValue || background.HasValue || glyph.HasValue)
            {
                processor = (x, y) =>
                {
                    if (IsValidCell(x, y, out int index))
                    {
                        ColoredGlyph cell = Cells[index];
                        result.Add(cell);

                        if (foreground.HasValue)
                        {
                            cell.Foreground = foreground.Value;
                            IsDirty = true;
                        }
                        if (background.HasValue)
                        {
                            cell.Background = background.Value;
                            IsDirty = true;
                        }
                        if (glyph.HasValue)
                        {
                            cell.Glyph = glyph.Value;
                            IsDirty = true;
                        }

                        return true;
                    }

                    return false;
                };
            }
            else
            {
                processor = (x, y) =>
                {
                    if (IsValidCell(x, y, out int index))
                    {
                        result.Add(Cells[index]);
                        return true;
                    }

                    return false;
                };
            }

            Algorithms.Line(start.X, start.Y, end.X, end.Y, processor);

            return result;
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="area">The area of the box.</param>
        /// <param name="border">The border style.</param>
        /// <param name="fill">The fill style. If null, the box is not filled.</param>
        /// <param name="connectedLineStyle">The lien style of the border. If null, <paramref name="border"/> glyph is used.</param>
        public void DrawBox(Rectangle area, ColoredGlyph border, ColoredGlyph fill = null, int[] connectedLineStyle = null)
        {
            if (connectedLineStyle == null)
            {
                connectedLineStyle = Enumerable.Range(0, Enum.GetValues(typeof(ConnectedLineIndex)).Length)
                    .Select(_ => border.Glyph).ToArray();
            }

            if (!ValidateLineStyle(connectedLineStyle))
            {
                throw new ArgumentException("Array is either null or does not have the required line style elements", nameof(connectedLineStyle));
            }

            // Draw the major sides
            DrawLine(area.Position, area.Position + new Point(area.Width - 1, 0), border.Foreground, border.Background, connectedLineStyle[(int)ConnectedLineIndex.Top]);
            DrawLine(area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1), border.Foreground, border.Background, connectedLineStyle[(int)ConnectedLineIndex.Bottom]);
            DrawLine(area.Position, area.Position + new Point(0, area.Height - 1), border.Foreground, border.Background, connectedLineStyle[(int)ConnectedLineIndex.Left]);
            DrawLine(area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1), border.Foreground, border.Background, connectedLineStyle[(int)ConnectedLineIndex.Right]);

            // TODO: Check that MaxExtentX/Y need the "- 1"

            // Tweak the corners
            SetGlyph(area.X, area.Y, connectedLineStyle[(int)ConnectedLineIndex.TopLeft]);
            SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ConnectedLineIndex.TopRight]);
            SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ConnectedLineIndex.BottomLeft]);
            SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ConnectedLineIndex.BottomRight]);

            // Fill
            if (fill == null)
            {
                return;
            }

            area = area.Expand(-1, -1);
            Fill(area, fill.Foreground, fill.Background, fill.Glyph);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="area">The area the ellipse </param>
        /// <param name="outer">The appearance of the outer line of the ellipse.</param>
        /// <param name="inner">The appearance of the inside of hte ellipse. If null, it will not be filled.</param>
        public void DrawCircle(Rectangle area, ColoredGlyph outer, ColoredGlyph inner = null)
        {
            var cells = new List<ColoredGlyph>(area.Width * area.Height);
            var masterCells = new List<ColoredGlyph>(Cells);

            Algorithms.Ellipse(area.X, area.Y, area.MaxExtentX, area.MaxExtentY, (x, y) =>
            {
                if (IsValidCell(x, y))
                {
                    SetCellAppearance(x, y, outer);
                    cells.Add(this[x, y]);
                }
            });

            if (inner != null)
            {
                Func<ColoredGlyph, bool> isTargetCell = c => !cells.Contains(c);
                Action<ColoredGlyph> fillCell = c =>
                {
                    inner.CopyAppearanceTo(c);
                    cells.Add(c);
                };
                Func<ColoredGlyph, Algorithms.NodeConnections<ColoredGlyph>> getConnectedCells = c =>
                {
                    var connections = new Algorithms.NodeConnections<ColoredGlyph>();

                    (int x, int y) = GetPointFromIndex(masterCells.IndexOf(c));

                    connections.West = IsValidCell(x - 1, y) ? this[x - 1, y] : null;
                    connections.East = IsValidCell(x + 1, y) ? this[x + 1, y] : null;
                    connections.North = IsValidCell(x, y - 1) ? this[x, y - 1] : null;
                    connections.South = IsValidCell(x, y + 1) ? this[x, y + 1] : null;

                    return connections;
                };

                Algorithms.FloodFill(this[area.Center.X, area.Center.Y], isTargetCell, fillCell, getConnectedCells);
            }
        }

        /// <summary>
        /// Connects all lines in a surface for both <see cref="ConnectedLineThin"/> and <see cref="ConnectedLineThick"/> styles.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConnectLines()
        {
            //ConnectLines(ConnectedLineThin);
            //ConnectLines(ConnectedLineThick);
        }

        /*

        /// <summary>
        /// Connects all lines in this based on the <paramref name="lineStyle"/> style provided.
        /// </summary>
        /// <param name="lineStyle">The array of line styles indexed by <see cref="ConnectedLineIndex"/>.</param>
        public void ConnectLines(int[] lineStyle)
        {
            var area = new Rectangle(0, 0, BufferWidth, BufferHeight);

            for (int x = 0; x < BufferWidth; x++)
            {
                for (int y = 0; y < BufferHeight; y++)
                {
                    var pos = new Point(x, y);
                    int index = GetIndexFromPoint(pos);

                    // Check if this pos is a road
                    if (!lineStyle.Contains(Cells[index].Glyph))
                    {
                        continue;
                    }

                    // Get all valid positions and indexes around this point
                    bool[] valids = pos.GetValidDirections(area);
                    int[] posIndexes = pos.GetDirectionIndexes(area);
                    bool[] roads = new[] { false, false, false, false, false, false, false, false, false };

                    for (int i = 0; i < 8; i++)
                    {
                        if (!valids[i])
                        {
                            continue;
                        }

                        if (lineStyle.Contains(Cells[posIndexes[i]].Glyph))
                        {
                            roads[i] = true;
                        }
                    }

                    if (roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.Middle];
                        IsDirty = true;
                    }
                    else if (!roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.TopMiddleToDown];
                        IsDirty = true;
                    }
                    else if (roads[(int)Directions.DirectionEnum.North] &&
                    !roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.BottomMiddleToTop];
                        IsDirty = true;
                    }
                    else if (roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    !roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.RightMiddleToLeft];
                        IsDirty = true;
                    }
                    else if (roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    !roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.LeftMiddleToRight];
                        IsDirty = true;
                    }
                    else if (!roads[(int)Directions.DirectionEnum.North] &&
                    !roads[(int)Directions.DirectionEnum.South] &&
                    (roads[(int)Directions.DirectionEnum.East] ||
                    roads[(int)Directions.DirectionEnum.West]))
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.Top];
                        IsDirty = true;
                    }
                    else if ((roads[(int)Directions.DirectionEnum.North] ||
                    roads[(int)Directions.DirectionEnum.South]) &&
                    !roads[(int)Directions.DirectionEnum.East] &&
                    !roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.Left];
                        IsDirty = true;
                    }
                    else if (roads[(int)Directions.DirectionEnum.North] &&
                    !roads[(int)Directions.DirectionEnum.South] &&
                    !roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.BottomRight];
                        IsDirty = true;
                    }
                    else if (roads[(int)Directions.DirectionEnum.North] &&
                    !roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    !roads[(int)Directions.DirectionEnum.West])
                    {

                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.BottomLeft];
                        IsDirty = true;
                    }
                    else if (!roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    !roads[(int)Directions.DirectionEnum.East] &&
                    roads[(int)Directions.DirectionEnum.West])
                    {
                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.TopRight];
                        IsDirty = true;
                    }
                    else if (!roads[(int)Directions.DirectionEnum.North] &&
                    roads[(int)Directions.DirectionEnum.South] &&
                    roads[(int)Directions.DirectionEnum.East] &&
                    !roads[(int)Directions.DirectionEnum.West])
                    {
                        Cells[index].Glyph = lineStyle[(int)ConnectedLineIndex.TopLeft];
                        IsDirty = true;
                    }
                }
            }
        }

        */

        /// <summary>
        /// Copies the contents of the cell surface to the destination.
        /// </summary>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="destination">The destination surface.</param>
        public void Copy(CellSurface destination)
        {
            int maxX = BufferWidth >= destination.BufferWidth ? destination.BufferWidth : BufferWidth;
            int maxY = BufferHeight >= destination.BufferHeight ? destination.BufferHeight : BufferHeight;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (IsValidCell(x, y, out int sourceIndex) && destination.IsValidCell(x, y, out int destIndex))
                        Cells[sourceIndex].CopyAppearanceTo(destination.Cells[destIndex]);
                }
            }

            destination.IsDirty = true;
        }

        /// <summary>
        /// Copies the contents of the cell surface to the destination at the specified x,y.
        /// </summary>
        /// <param name="x">The x coordinate of the destination.</param>
        /// <param name="y">The y coordinate of the destination.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(CellSurface destination, int x, int y)
        {
            for (int curX = 0; curX < BufferWidth; curX++)
            {
                for (int curY = 0; curY < BufferHeight; curY++)
                {
                    if (IsValidCell(curX, curY, out int sourceIndex) && destination.IsValidCell(x + curX, y + curY, out int destIndex))
                        Cells[sourceIndex].CopyAppearanceTo(destination.Cells[destIndex]);
                }
            }

            destination.IsDirty = true;
        }

        /// <summary>
        /// Copies an area of this cell surface to the destination surface.
        /// </summary>
        /// <param name="area">The area to copy.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(Rectangle area, CellSurface destination, int destinationX, int destinationY) =>
            Copy(area.X, area.Y, area.Width, area.Height, destination, destinationX, destinationY);

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified BufferWidth and BufferHeight, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The BufferWidth to copy from.</param>
        /// <param name="height">The BufferHeight to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        public void Copy(int x, int y, int width, int height, CellSurface destination, int destinationX, int destinationY)
        {
            int destX = destinationX;
            int destY = destinationY;

            for (int curX = 0; curX < width; curX++)
            {
                for (int curY = 0; curY < height; curY++)
                {
                    if (IsValidCell(curX + x, curY + y, out int sourceIndex) && destination.IsValidCell(destX, destY, out int destIndex))
                        Cells[sourceIndex].CopyAppearanceTo(destination.Cells[destIndex]);

                    destY++;
                }
                destY = destinationY;
                destX++;
            }

            destination.IsDirty = true;
        }

        /// <summary>
        /// Resizes the surface to the specified width and height.
        /// </summary>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="bufferWidth">The maximum width of the surface.</param>
        /// <param name="bufferHeight">The maximum height of the surface.</param>
        /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
        public void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear)
        {
            var newCells = new ColoredGlyph[bufferWidth * bufferHeight];

            for (int y = 0; y < bufferHeight; y++)
            {
                for (int x = 0; x < bufferWidth; x++)
                {
                    int index = new Point(x, y).ToIndex(bufferWidth);

                    if (IsValidCell(x, y))
                    {
                        newCells[index] = this[x, y];

                        if (clear)
                        {
                            newCells[index].Foreground = DefaultForeground;
                            newCells[index].Background = DefaultBackground;
                            newCells[index].Glyph = 0;
                            newCells[index].Mirror = Mirror.None;
                        }
                    }
                    else
                    {
                        newCells[index] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
                    }
                }
            }

            Cells = newCells;
            _viewPosition = new Point(0, 0);
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;
            ViewWidth = width;
            ViewHeight = height;
            //Effects = new EffectsManager(this);
            IsDirty = true;
            OnCellsReset();
        }

        /// <summary>
        /// Returns a new surface instance from the current instance based on the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">An area of the surface to create a view of.</param>
        /// <returns>A new surface</returns>
        public CellSurface GetSubSurface(Rectangle view)
        {
            if (!new Rectangle(0, 0, BufferWidth, BufferHeight).Contains(view))
            {
                throw new Exception("View is outside of surface bounds.");
            }

            var cells = new ColoredGlyph[view.Width * view.Height];

            int index = 0;

            for (int y = 0; y < view.Height; y++)
            {
                for (int x = 0; x < view.Width; x++)
                {
                    cells[index] = this[x + view.X, y + view.Y];
                    index++;
                }
            }

            return new CellSurface(view.Width, view.Height, cells);
        }

        /// <summary>
        /// Remaps the cells of this surface to a view of the <paramref name="surface"/>.
        /// </summary>
        /// <typeparam name="T">The surface type.</typeparam>
        /// <param name="view">A view rectangle of the target surface.</param>
        /// <param name="surface">The target surface to map cells from.</param>
        public void SetSurface<T>(in T surface, Rectangle view = default) where T : CellSurface
        {
            Rectangle rect = view == Rectangle.Empty ? new Rectangle(0, 0, surface.BufferWidth, surface.BufferHeight) : view;

            if (!new Rectangle(0, 0, surface.BufferWidth, surface.BufferHeight).Contains(rect))
            {
                throw new ArgumentOutOfRangeException(nameof(view), "The view is outside the bounds of the surface.");
            }

            _viewPosition = new Point(0, 0);
            BufferWidth = rect.Width;
            BufferHeight = rect.Height;
            ViewWidth = rect.Width;
            ViewHeight = rect.Height;
            Cells = new ColoredGlyph[rect.Width * rect.Height];

            int index = 0;

            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    Cells[index] = surface.Cells[(y + rect.Y) * surface.BufferWidth + (x + rect.X)];
                    index++;
                }
            }

            IsDirty = true;
            OnCellsReset();
        }

        /// <summary>
        /// Changes the cells of the surface to the provided array.
        /// </summary>
        /// <param name="cells">The cells to replace in this surface.</param>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="bufferWidth">The maximum width of the surface.</param>
        /// <param name="bufferHeight">The maximum height of the surface.</param>
        public void SetSurface(in ColoredGlyph[] cells, int width, int height, int bufferWidth, int bufferHeight)
        {
            if (cells.Length != bufferWidth * bufferHeight) throw new Exception("buffer width * buffer height must match the amount of cells.");

            _viewPosition = new Point(0, 0);
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;
            ViewWidth = width;
            ViewHeight = height;
            Cells = cells;

            IsDirty = true;
            OnCellsReset();
        }

        /// <summary>
        /// Fills a console with random colors and glyphs.
        /// </summary>
        public void FillWithRandomGarbage(bool useEffect = false)
        {
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < BufferHeight; y++)
            {
                for (int x = 0; x < BufferWidth; x++)
                {
                    SetGlyph(x, y, charCounter);
                    SetForeground(x, y, new Color((byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)255));
                    SetBackground(x, y, DefaultBackground);
                    SetBackground(x, y, new Color((byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)255));
                    SetMirror(x, y, (Mirror)Global.Random.Next(0, 4));
                    charCounter++;
                    if (charCounter > 255)
                    {
                        charCounter = 0;
                    }
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Gets the index of a location on the surface by point.
        /// </summary>
        /// <param name="location">The location of the index to get.</param>
        /// <returns>The cell index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndexFromPoint(Point location) =>
            location.ToIndex(BufferWidth);

        /// <summary>
        /// Gets the index of a location on the surface by coordinate.
        /// </summary>
        /// <param name="x">The x of the location.</param>
        /// <param name="y">The y of the location.</param>
        /// <returns>The cell index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndexFromPoint(int x, int y) =>
            new Point(x, y).ToIndex(BufferWidth);

        /// <summary>
        /// Gets the x,y of an index on the surface.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The x,y as a <see cref="Point"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetPointFromIndex(int index) =>
            Point.FromIndex(index, BufferWidth);
    }
}
