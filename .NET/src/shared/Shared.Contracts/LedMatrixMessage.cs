using System;
using System.Drawing;
using Shared.Messaging;

namespace Shared.Contracts
{
    public class LedMatrixMessage : IMessage
    {
        public Color[] Colors { get; set; } = Array.Empty<Color>();
    }
}