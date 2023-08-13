using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasMinigameTiming
{
    public event EventHandler<OnMinigameTimingEventArgs> OnMinigameTimingStarted;
    public class OnMinigameTimingEventArgs : EventArgs
    {
        public float minigameTimingNormalized;
    }
}
