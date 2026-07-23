
public interface IMovementHandler : IHasActiveState
{
    public bool LastActiveState { get; set; }

    public void BlockMovement();

    public void ResetLastState();
}
