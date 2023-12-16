using System;
using Device.Abstractions;

namespace Device.Mock;

public class LedMock : IFailureLed, ISuccessLed
{
    public void On()
    {
        
    }

    public void Off()
    {
        
    }

    public void On(TimeSpan period)
    {
        
    }

    public void Off(TimeSpan period)
    {
        
    }
}