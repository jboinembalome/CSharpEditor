using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RoslynEditor.Core.QuickInfo
{
    internal partial class DeferredQuickInfoContentProvider
    {
        private class QuickInfoDisplayPanel : StackPanel
        {
            #region Properties

            private TextBlock MainDescription { get; }
            private TextBlock Documentation { get; }
            private TextBlock TypeParameterMap { get; }
            private TextBlock AnonymousTypes { get; }
            private TextBlock UsageText { get; }
            private TextBlock ExceptionText { get; }
            #endregion

            #region Constructors

            public QuickInfoDisplayPanel(FrameworkElement? symbolGlyph, FrameworkElement? warningGlyph, 
                FrameworkElement mainDescription, FrameworkElement documentation, FrameworkElement typeParameterMap,
                FrameworkElement anonymousTypes, FrameworkElement usageText, FrameworkElement exceptionText)
            {
                MainDescription = (TextBlock)mainDescription;
                Documentation = (TextBlock)documentation;
                TypeParameterMap = (TextBlock)typeParameterMap;
                AnonymousTypes = (TextBlock)anonymousTypes;
                UsageText = (TextBlock)usageText;
                ExceptionText = (TextBlock)exceptionText;

                Orientation = Orientation.Vertical;

                Border? symbolGlyphBorder = null;
                if (symbolGlyph != null)
                {
                    symbolGlyph.Margin = new Thickness(1, 1, 3, 1);
                    symbolGlyphBorder = new Border()
                    {
                        BorderThickness = new Thickness(0),
                        BorderBrush = Brushes.Transparent,
                        VerticalAlignment = VerticalAlignment.Top,
                        Child = symbolGlyph
                    };
                }

                mainDescription.Margin = new Thickness(1);
                var mainDescriptionBorder = new Border()
                {
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.Transparent,
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = mainDescription
                };

                var symbolGlyphAndMainDescriptionDock = new DockPanel()
                {
                    LastChildFill = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Transparent
                };

                if (symbolGlyphBorder != null)
                    symbolGlyphAndMainDescriptionDock.Children.Add(symbolGlyphBorder);

                symbolGlyphAndMainDescriptionDock.Children.Add(mainDescriptionBorder);

                if (warningGlyph != null)
                {
                    warningGlyph.Margin = new Thickness(1, 1, 3, 1);
                    var warningGlyphBorder = new Border()
                    {
                        BorderThickness = new Thickness(0),
                        BorderBrush = Brushes.Transparent,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Child = warningGlyph
                    };

                    symbolGlyphAndMainDescriptionDock.Children.Add(warningGlyphBorder);
                }

                Children.Add(symbolGlyphAndMainDescriptionDock);
                Children.Add(documentation);
                Children.Add(usageText);
                Children.Add(typeParameterMap);
                Children.Add(anonymousTypes);
                Children.Add(exceptionText);
            }
            #endregion

            #region Methods

            public override string ToString()
            {
                var sb = new StringBuilder();

                BuildStringFromInlineCollection(MainDescription.Inlines, sb);

                if (Documentation.Inlines.Count > 0)
                {
                    sb.AppendLine();
                    BuildStringFromInlineCollection(Documentation.Inlines, sb);
                }

                if (TypeParameterMap.Inlines.Count > 0)
                {
                    sb.AppendLine();
                    BuildStringFromInlineCollection(TypeParameterMap.Inlines, sb);
                }

                if (AnonymousTypes.Inlines.Count > 0)
                {
                    sb.AppendLine();
                    BuildStringFromInlineCollection(AnonymousTypes.Inlines, sb);
                }

                if (UsageText.Inlines.Count > 0)
                {
                    sb.AppendLine();
                    BuildStringFromInlineCollection(UsageText.Inlines, sb);
                }

                if (ExceptionText.Inlines.Count > 0)
                {
                    sb.AppendLine();
                    BuildStringFromInlineCollection(ExceptionText.Inlines, sb);
                }

                return sb.ToString();
            }

            private static void BuildStringFromInlineCollection(InlineCollection inlines, StringBuilder sb)
            {
                foreach (var inline in inlines)
                    if (inline != null)
                    {
                        var inlineText = GetStringFromInline(inline);
                        if (!string.IsNullOrEmpty(inlineText))
                            sb.Append(inlineText);
                    }
            }

            private static string? GetStringFromInline(Inline currentInline)
            {
                if (currentInline is LineBreak /*lineBreak*/)
                    return Environment.NewLine;

                var run = currentInline as Run;
                return run?.Text;
            }
            #endregion
        }
    }
}