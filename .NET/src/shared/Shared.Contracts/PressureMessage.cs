using Shared.Messaging;
using UnitsNet;

namespace Shared.Contracts
{
    public class PressureMessage : IMessage
    {
        public Pressure Pressure { get; set; }
    }
}