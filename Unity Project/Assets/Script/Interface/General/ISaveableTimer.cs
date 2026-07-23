
public interface ISaveableTimer
{
    public float ElapsedTime {  get; set; }

    public bool IsRunning { get; set; }

    public void RestoreTimer();
}
