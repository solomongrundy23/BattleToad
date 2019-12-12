using System;
using System.Timers;

class Korrector: IDisposable
{
    private readonly Timer timer = new Timer();
    public int ClearingLevel = 0;
    public readonly double MaxGarbage = 0;
    private int maxClearingLevel;
    public int MaxClearingLevel
    {
        get => maxClearingLevel;
        set => maxClearingLevel = 
            (MaxClearingLevel > GC.MaxGeneration) ? GC.MaxGeneration : value;
    }
    public bool Enabled
    {
        get => timer.Enabled;
        set => timer.Enabled = value;
    }
    public Korrector(double interval_ms, int level = 0, double max_garbage = 0)
    {
        MaxClearingLevel = level;
        MaxGarbage = max_garbage;
        timer.Elapsed += TimerTick;
        timer.Interval = interval_ms;
        timer.Start();
    }

    public void Clearing(int type)
    {
        GC.Collect(type);
    }

    private void TimerTick(object sender, EventArgs args)
    {
        if (MaxGarbage == 0)
            GC.Collect(ClearingLevel);
        else
        {
            if (MaxGarbage < GC.GetTotalMemory(true))
            {
                GC.Collect(ClearingLevel);
            }
        }
    }

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                timer.Dispose();
            }
            disposedValue = true;
        }
    }

    ~Korrector()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
