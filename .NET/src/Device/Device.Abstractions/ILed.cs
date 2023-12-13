namespace Device.Abstractions;

public interface ILed
{
    public void On();
    public void Off();
    public void On(TimeSpan period);
    public void Off(TimeSpan period);
}