using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace RoslynEditor.Windows
{
    public sealed class TextMarker : TextSegment
    {
        #region Fields

        private readonly TextMarkerService _service;

        private Color? _backgroundColor;
        private Color? _foregroundColor;
        private FontWeight? _fontWeight;
        private FontStyle? _fontStyle;
        private Color _markerColor;
        #endregion

        #region Properties

        public bool IsDeleted => !IsConnectedToCollection;

        public Color? BackgroundColor
        {
            get => _backgroundColor; set
            {
                if (!EqualityComparer<Color?>.Default.Equals(_backgroundColor, value))
                {
                    _backgroundColor = value;
                    Redraw();
                }
            }
        }

        public Color? ForegroundColor
        {
            get => _foregroundColor; set
            {
                if (!EqualityComparer<Color?>.Default.Equals(_foregroundColor, value))
                {
                    _foregroundColor = value;
                    Redraw();
                }
            }
        }

        public FontWeight? FontWeight
        {
            get => _fontWeight; set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    Redraw();
                }
            }
        }

        public FontStyle? FontStyle
        {
            get => _fontStyle; set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    Redraw();
                }
            }
        }

        public object? Tag { get; set; }

        public Color MarkerColor
        {
            get => _markerColor; set
            {
                if (!EqualityComparer<Color>.Default.Equals(_markerColor, value))
                {
                    _markerColor = value;
                    Redraw();
                }
            }
        }

        public object? ToolTip { get; set; }
        #endregion

        #region Constructors

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            StartOffset = startOffset;
            Length = length;
        }
        #endregion

        #region Events

        public event EventHandler? Deleted;

        internal void OnDeleted()
        {
            Deleted?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods

        public void Delete() => _service.Remove(this);

        private void Redraw() => _service.Redraw(this);
        #endregion
    }
}