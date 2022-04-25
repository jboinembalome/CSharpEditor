using System;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using System.Collections.Generic;

namespace RoslynEditor.Windows
{
    internal class SearchReplaceResultBackgroundRenderer : IBackgroundRenderer
    {
        #region Fields

        private Brush _markerBrush;
        private Pen _markerPen;
        #endregion

        #region Properties

        public List<ISearchResult> CurrentResults { get; } = new List<ISearchResult>();

        public KnownLayer Layer => KnownLayer.Selection;

        public Brush MarkerBrush
        {
            get => _markerBrush;
            set
            {
                _markerBrush = value;
                _markerPen = new Pen(_markerBrush, 1);
            }
        }
        #endregion

        #region Constructors

        public SearchReplaceResultBackgroundRenderer()
        {
            _markerBrush = Brushes.LightGreen;
            _markerPen = new Pen(_markerBrush, 1);
        }
        #endregion

        #region Methods

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));

            if (drawingContext == null)
                throw new ArgumentNullException(nameof(drawingContext));

            if (CurrentResults == null || !textView.VisualLinesValid)
                return;

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
                return;

            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            foreach (var result in CurrentResults
                .Where(r => viewStart <= r.Offset && r.Offset <= viewEnd || viewStart <= r.EndOffset && r.EndOffset <= viewEnd))
            {
                var geoBuilder = new BackgroundGeometryBuilder
                {
                    //BorderThickness = markerPen != null ? markerPen.Thickness : 0,
                    AlignToWholePixels = true,
                    CornerRadius = 3
                };
                geoBuilder.AddSegment(textView, result);

                var geometry = geoBuilder.CreateGeometry();
                if (geometry != null)
                    drawingContext.DrawGeometry(_markerBrush, _markerPen, geometry);
            }
        }
        #endregion
    }
}