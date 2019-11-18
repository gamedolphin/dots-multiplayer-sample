using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal struct FixedTimeLoop
{
    public float accumulatedTime;
    public const float fixedTimeStep = 1f / 60f;
    public const int maxTimeSteps = 4;
    public int timeSteps;

    public void BeginUpdate()
    {
        accumulatedTime += Time.deltaTime;
        timeSteps = 0;
    }
    public bool ShouldUpdate()
    {
        if (accumulatedTime < fixedTimeStep)
            return false;
        ++timeSteps;
        if (timeSteps > maxTimeSteps)
        {
            accumulatedTime = accumulatedTime % fixedTimeStep;
            return false;
        }
        accumulatedTime -= fixedTimeStep;
        return true;
    }
}