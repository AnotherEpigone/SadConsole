﻿using SadRogue.Primitives;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace SadConsole
{
    /// <summary>
    /// A cell in structure format for temporary storage.
    /// </summary>
    [DataContract]
    public struct CellState : IEquatable<CellState>
    {
        /// <summary>
        /// Decorators of the state.
        /// </summary>
        [DataMember]
        public readonly CellDecorator[] Decorators;

        /// <summary>
        /// Foreground color of the state.
        /// </summary>
        [DataMember]
        public readonly Color Foreground;

        /// <summary>
        /// Background color of the state.
        /// </summary>
        [DataMember]
        public readonly Color Background;

        /// <summary>
        /// Glyph of the state.
        /// </summary>
        [DataMember]
        public readonly int Glyph;

        /// <summary>
        /// Mirror setting of the state.
        /// </summary>
        [DataMember]
        public readonly Mirror Mirror;

        /// <summary>
        /// Visible setting of the state.
        /// </summary>
        [DataMember]
        public readonly bool IsVisible;

        /// <summary>
        /// Creates a new state with the specified colors, glyph, visiblity, and mirror settings.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">Glyph value.</param>
        /// <param name="mirror">Mirror setting.</param>
        /// <param name="isVisible">Visbility setting.</param>
        /// <param name="decorators">Decorators setting.</param>
        public CellState(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, CellDecorator[] decorators)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
            IsVisible = isVisible;
            Decorators = decorators ?? Array.Empty<CellDecorator>();
        }

        /// <summary>
        /// Creates a cell state from a cell.
        /// </summary>
        /// <param name="source">The source cell to create a state from.</param>
        public CellState(Cell source) : this(source.Foreground, source.Background, source.Glyph, source.Mirror, source.IsVisible, source.Decorators) { }

        public static bool operator ==(CellState left, CellState right) => left.Background == right.Background &&
                   left.Foreground == right.Foreground &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror &&
                   left.IsVisible == right.IsVisible &&
                   left.Decorators.SequenceEqual(right.Decorators);

        public static bool operator !=(CellState left, CellState right) => left.Background != right.Background ||
                   left.Foreground != right.Foreground ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror ||
                   left.IsVisible != right.IsVisible &&
                   !left.Decorators.SequenceEqual(right.Decorators);

        public static bool operator ==(CellState left, Cell right) => left.Background == right.Background &&
                   left.Foreground == right.Foreground &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror &&
                   left.IsVisible == right.IsVisible &&
                   left.Decorators.SequenceEqual(right.Decorators);

        public static bool operator !=(CellState left, Cell right) => left.Background != right.Background ||
                   left.Foreground != right.Foreground ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror ||
                   left.IsVisible != right.IsVisible &&
                   !left.Decorators.SequenceEqual(right.Decorators);

        public static bool operator ==(Cell right, CellState left) => left.Background == right.Background &&
                   left.Foreground == right.Foreground &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror &&
                   left.IsVisible == right.IsVisible &&
                   left.Decorators.SequenceEqual(right.Decorators);

        public static bool operator !=(Cell left, CellState right) => left.Background != right.Background ||
                   left.Foreground != right.Foreground ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror ||
                   left.IsVisible != right.IsVisible &&
                   !left.Decorators.SequenceEqual(right.Decorators);

        public bool Equals(CellState other) => this == other;
    }
}
