using UnityEngine;

/// <summary>
/// SimulationClock：
/// </summary>
public class SimulationClock
{
    public long totalTicks; // 总时长
    public float timeScale; // 时间倍率
    private double remainder;

    public const long TicksPerSecond = 10_000_000;

    // 演化速度配置
    private static readonly float[] scaleSteps = { 0f, 0.25f, 0.5f, 1f, 2f, 4f, 10f, 15f };
    private int stepIndex = 3;  // 默认为1x
    private float lastNonZeroScale = 1f; // 暂停前速度

    public float GetStepValue()
    {
        if (stepIndex >= 0 && stepIndex < scaleSteps.Length)
        {
            return scaleSteps[stepIndex];
        }
        return 0f;
    }

    public void Init()
    {
        totalTicks = 0;
        timeScale = 1.0f;
        remainder = 0d;
    }

    public void Free()
    {
        totalTicks = 0;
        remainder = 0d;
    }

    // 计算一帧的增量Tick
    public long Advance(double deltaTime)
    {
        if (timeScale <= 0)
            return 0;

        double scaleDelta = deltaTime * timeScale;
        double totalToProcess = scaleDelta + remainder;

        long ticksToAdd = (long)(totalToProcess * TicksPerSecond);

        totalTicks += ticksToAdd;
        remainder = totalToProcess - (double)ticksToAdd / TicksPerSecond;

        return ticksToAdd;
    }

    public double ToSeconds(long ticks)
    {
        return (double)ticks / TicksPerSecond;
    }

    public long ToTicks(double seconds)
    {
        return (long)(seconds * TicksPerSecond);
    }

    public double TotalSeconds()
    {
        return (double)totalTicks / TicksPerSecond;
    }

    public void SpeedUp()
    {
        stepIndex = Mathf.Min(stepIndex + 1, scaleSteps.Length - 1);
        timeScale = scaleSteps[stepIndex];
        if (timeScale > 0)
            lastNonZeroScale = timeScale;
    }

    public void SpeedDown()
    {
        // 不能减速到暂停
        stepIndex = Mathf.Max(stepIndex - 1, 1);
        timeScale = scaleSteps[stepIndex];
        if (timeScale > 0)
            lastNonZeroScale = timeScale;
    }

    public void TogglePause()
    {
        if (timeScale > 0)
        {
            lastNonZeroScale = timeScale;
            timeScale = 0;
        }
        else
        {
            timeScale = lastNonZeroScale;
        }
    }
}
