using System;
using System.Drawing;
using Device.Abstractions;
using Device.Mock.Extensions;
using UnitsNet;
using UnitsNet.Units;

namespace Device.Mock
{
    public class MockSenseHat: ITemperatureSensor, IPressureSensor, IHumiditySensor, ILedMatrix
    {
        private const int MatrixLineLength = 8;
        
        public Temperature GetTemperature()
        {
            var rnd = new Random();

            return new Temperature(rnd.NextDouble() * 20.0, TemperatureUnit.DegreeCelsius);
        }
        
        public Pressure GetPressure()
        {
            var rnd = new Random();

            return new Pressure(rnd.NextDouble() * 1000, PressureUnit.Hectopascal);
        }

        public RelativeHumidity GetRelativeHumidity()
        {
            var rnd = new Random();

            return new RelativeHumidity(rnd.NextDouble() * 100, RelativeHumidityUnit.Percent);
        }

        public void SetLedMatrix(ReadOnlySpan<Color> colors)
        {
            var consoleOriginalColor = Console.BackgroundColor;
            Console.Clear();

            var chars = 0;
            
            foreach (var color in colors)
            {
                Console.BackgroundColor = color.ToConsoleColor(); 
                Console.Write(" ");

                if(++chars % MatrixLineLength == 0) Console.Write(Environment.NewLine);
            }

            Console.BackgroundColor = consoleOriginalColor;
        }

        public void FillMatrix(Color color)
        {
            var colors = new Color[64];
        
            for (var i = 0; i < 64; i++)
            {
                colors[i] = color;
            }

            SetLedMatrix(colors);
        }
    }
}