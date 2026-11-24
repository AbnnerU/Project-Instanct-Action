

using System;

public interface IHasActiveState
{
    void Disable();

    void Enable();
}


public interface ICanEvolve
{
    public event Action OnReadyToEnvolve;
}
