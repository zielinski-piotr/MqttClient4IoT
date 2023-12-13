using System.Drawing;

namespace Clients.Supervisor.Emoji;

public static class Emoji
{
    private static readonly Color[] Smiling =
    {
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black,
        Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Black, Color.Gold, Color.Black, Color.Black, Color.Black, Color.Black, Color.Gold, Color.Black,
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black
    };

    private static readonly Color[] Cry =
    {
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black,
        Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black,
        Color.Gold, Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Black, Color.Black, Color.Gold,
        Color.Gold, Color.DeepSkyBlue, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.DeepSkyBlue, Color.Gold,
        Color.Gold, Color.DeepSkyBlue, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.DeepSkyBlue, Color.Gold,
        Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold,
        Color.Black, Color.Gold, Color.Gold, Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Black,
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black
    };

    private static readonly Color[] Happy =
    {
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black,
        Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold,
        Color.Gold, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Gold,
        Color.Black, Color.Gold, Color.Black, Color.Black, Color.Black, Color.Black, Color.Gold, Color.Black,
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black
    };

    private static readonly Color[] Sad =
    {
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black,
        Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Gold,
        Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Gold,
        Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold,
        Color.Black, Color.Gold, Color.Black, Color.Gold, Color.Gold, Color.Black, Color.Gold, Color.Black,
        Color.Black, Color.Black, Color.Gold, Color.Gold, Color.Gold, Color.Gold, Color.Black, Color.Black
    };

    public static Color[] PickFace(bool reverseArray = false)
    {
        var rnd = new Random();
        var pick = rnd.Next(1, 4);
        var pixels = pick switch
        {
            1 => Cry,
            2 => Happy,
            3 => Sad,
            4 => Smiling,
            _ => throw new Exception("Unknown emoji")
        };

        return reverseArray ? ReverseArray(pixels) : pixels;
    }
    
    private static Color[] ReverseArray(IReadOnlyList<Color> colors)
    {
        var reversedColors = new Color[colors.Count];
        
        for (var i = 0; i < colors.Count; i++)
        {
            reversedColors[i] = colors[colors.Count - i - 1];
        }

        return reversedColors;
    }
}