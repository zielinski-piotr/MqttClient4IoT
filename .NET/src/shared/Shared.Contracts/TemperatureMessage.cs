using Shared.Messaging;
using UnitsNet;

namespace Shared.Contracts
{
    public class TemperatureMessage : IMessage
    {
        public Temperature Temperature { get; set; }
    }
}