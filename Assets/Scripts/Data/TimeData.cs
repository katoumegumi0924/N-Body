using UnityEngine;

/// <summary>
/// TimeData：
/// </summary>
public class TimeData
{
    public long tickCounter;
    public const int ticksPerSecond = 60;
    public const float tickDeltaTime = 1.0f / ticksPerSecond;

    private int _tickDelta;
    public int tickDelta { get { return _pausing ? 0 : _tickDelta; } }
    private readonly static int[] tickDeltaSteps = { 1, 2, 10, 15, 20, 40 };
    private bool _pausing;

    public void Init()
    {
        tickCounter = 0L;
        _tickDelta = 0;
    }

    public void Free()
    {
        tickCounter = 0L;
        _tickDelta = 0;
    }

    public void SetNew()
    {
        tickCounter = 0;
        _tickDelta = 10;
    }

    public void SetSpeed(int index)
    {
        if (index < 0 || index > tickDeltaSteps.Length - 1)
            return;
        _tickDelta = tickDeltaSteps[index];
    }

    public int GetSeconds(long currentTicks)
    {
        return (int)(currentTicks / ticksPerSecond);
    }

    public void TogglePause()
    {
        _pausing = !_pausing;
    }
}
