using UnityEngine;

/// <summary>
/// SimulationClock：
/// </summary>
public class SimulationTimeData
{
    public long totalTicks; // 总时长

    public const long TicksPerSecond = 10_000_000;

    // 演化速度配置
    public static readonly float[] scaleSteps = { 0f, 0.25f, 0.5f, 1f, 2f, 4f, 10f, 15f, 50f };
    public float _timeScale; // 时间倍率
    public float timeScale { get { return _timeScale; }  }

    public void Init()
    {
        totalTicks = 0;
        _timeScale = 1.0f;
    }

    public void Free()
    {
        totalTicks = 0;
    }

    public double ToSeconds(long ticks)
    {
        return (double)ticks / TicksPerSecond;
    }

    public long ToTicks(double seconds)
    {
        return (long)(seconds * TicksPerSecond);
    }
}