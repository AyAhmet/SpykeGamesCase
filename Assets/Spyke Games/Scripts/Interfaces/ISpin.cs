using System;

public interface ISpin
{
    public event Action OnSpinStarted;
    public event Action OnSpinEnded;

    public bool IsSpinning();
}
