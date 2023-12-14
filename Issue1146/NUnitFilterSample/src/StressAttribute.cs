using System;
using NUnit.Framework;

namespace NUnitFilterSample;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
public class StressAttribute : CategoryAttribute
{
    public StressAttribute() : base("Stress")
    { }
}