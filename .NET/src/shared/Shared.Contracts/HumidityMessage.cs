using Shared.Messaging;
using UnitsNet;

namespace Shared.Contracts
{
    public class HumidityMessage : IMessage
    {
        public RelativeHumidity Humidity { get; set; }
    }
}