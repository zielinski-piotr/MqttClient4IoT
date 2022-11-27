using System.Drawing;

namespace Device.Abstractions;

public interface ILedMatrix
{
    public void SetLedMatrix(ReadOnlySpan<Color> colors);
    void FillMatrix(Color color);
}