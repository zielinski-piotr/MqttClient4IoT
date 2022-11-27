using System;

namespace Shared.Messaging
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string message) : base(message)
        {
        }
    }
}