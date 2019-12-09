using System;
using System.Timers;

class Korrector: IDisposable
{
    private readonly Timer timer = new Timer();
    public int TypeOfAutoClearing = 0;
    public double MaxGarbage = 0;
    public static class Type
    {
        public static int Lite = 0;
        public static int Normal = 1;
        public static int Strong = 2;
    }
    public bool Enabled
    {
        get => timer.Enabled;
        set => timer.Enabled = value;
    }

    public Korrector(double interval_ms, double max_garbage = 0)
    {
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
            GC.Collect(TypeOfAutoClearing);
        else
        {
            if (MaxGarbage < GC.GetTotalMemory(true))
            {
                GC.Collect(TypeOfAutoClearing);
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
