﻿using PixiEditor.Helpers.Extensions;
using PixiEditor.Models.DataHolders;
using PixiEditor.Models.Layers;
using PixiEditor.Models.Position;
using PixiEditor.ViewModels;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixiEditor.Models.Tools.Tools
{
    public class MoveTool : BitmapOperationTool
    {
        public override ToolType ToolType => ToolType.Move;
        private Coordinates[] _startSelection;
        private Coordinates _lastStartMousePos;
        private Coordinates _lastMouseMove;
        private Color[] _startPixelColors;
        private bool _clearedPixels = false;

        public MoveTool()
        {
            Tooltip = "Moves selected pixels. (M)";
            Cursor = Cursors.Arrow;
            HideHighlight = true;
            RequiresPreviewLayer = true;
        }

        public override BitmapPixelChanges Use(Layer layer, Coordinates[] mouseMove, Color color)
        {
            if (ViewModelMain.Current.ActiveSelection.SelectedPoints == null) return BitmapPixelChanges.Empty;
            return MoveSelection(layer, mouseMove);
        }

        public BitmapPixelChanges MoveSelection(Layer layer, Coordinates[] mouseMove)
        {
            Coordinates start = mouseMove[^1];
            Coordinates end = mouseMove[0];

            if (_lastStartMousePos != start)
            {
                ResetSelectionValues(layer, start);
            }

            Coordinates[] previousSelection = TranslateSelection(end);
            ClearSelectedPixels(layer, previousSelection);


            _lastMouseMove = end;
            return BitmapPixelChanges.FromArrays(
                        ViewModelMain.Current.ActiveSelection.SelectedPoints, _startPixelColors);
        }

        private void ResetSelectionValues(Layer layer, Coordinates start)
        {
            _lastStartMousePos = start;
            _startSelection = ViewModelMain.Current.ActiveSelection.SelectedPoints;
            _startPixelColors = GetPixelsForSelection(layer, _startSelection);
            _lastMouseMove = start;
            _clearedPixels = false;
        }

        private Coordinates[] TranslateSelection(Coordinates end)
        {
            Coordinates translation = ImageManipulation.Transform.GetTranslation(_lastMouseMove, end);
            Coordinates[] previousSelection = ViewModelMain.Current.ActiveSelection.SelectedPoints.ToArray();
            ViewModelMain.Current.ActiveSelection =
                new Selection(ImageManipulation.Transform.Translate(previousSelection, translation));
            return previousSelection;
        }

        private void ClearSelectedPixels(Layer layer, Coordinates[] selection)
        {
            if (_clearedPixels == false)
            {
                layer.ApplyPixels(BitmapPixelChanges.FromSingleColoredArray(selection, System.Windows.Media.Colors.Transparent));

                _clearedPixels = true;
            }
        }

        private Color[] GetPixelsForSelection(Layer layer, Coordinates[] selection)
        {
            Color[] pixels = new Color[_startSelection.Length];
            layer.LayerBitmap.Lock();

            for (int i = 0; i < pixels.Length; i++)
            {
                Coordinates position = selection[i];
                if (position.X < 0 || position.X > layer.Width - 1 || position.Y < 0 || position.Y > layer.Height - 1)
                    continue;
                pixels[i] = layer.LayerBitmap.GetPixel(position.X, position.Y);
            }
            layer.LayerBitmap.Unlock();
            return pixels;
        }
    }
}
