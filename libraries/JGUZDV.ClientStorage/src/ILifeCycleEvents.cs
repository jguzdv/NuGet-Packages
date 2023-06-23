namespace JGUZDV.ClientStorage;

public interface ILifeCycleEvents
{
    public event EventHandler? Stopped;
    public event EventHandler? Resumed;
}
