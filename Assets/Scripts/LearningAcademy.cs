using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class LearningAcademy : Academy
{
    public override void AcademyReset()
    {
        Monitor.SetActive(true);
    }
}