using UnityEngine;
using System.Collections.Generic;

namespace RDTools
{
    public enum EColor
    {
        Clear,
        White,
        Black,
        Gray,
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }

    public static class EColorExtensions
    {
        private static readonly Dictionary<EColor, Color32> colorMap = new Dictionary<EColor, Color32>
        {
            { EColor.Clear, new Color32(0, 0, 0, 0) },
            { EColor.White, new Color32(255, 255, 255, 255) },
            { EColor.Black, new Color32(0, 0, 0, 255) },
            { EColor.Gray, new Color32(128, 128, 128, 255) },
            { EColor.Red, new Color32(255, 0, 63, 255) },
            { EColor.Pink, new Color32(255, 152, 203, 255) },
            { EColor.Orange, new Color32(255, 128, 0, 255) },
            { EColor.Yellow, new Color32(255, 211, 0, 255) },
            { EColor.Green, new Color32(98, 200, 79, 255) },
            { EColor.Blue, new Color32(0, 135, 189, 255) },
            { EColor.Indigo, new Color32(75, 0, 130, 255) },
            { EColor.Violet, new Color32(128, 0, 255, 255) }
        };

        public static Color32 GetColor(this EColor color)
        {
            // Return the mapped color from the dictionary, or default to black if not found.
            return colorMap.TryGetValue(color, out Color32 result) ? result : colorMap[EColor.Black];
        }
    }
}