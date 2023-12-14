using System;
using NUnit.Framework;

namespace NUnitFilterSample;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
public class SampleAttribute : CategoryAttribute
{
    public SampleAttribute() : base("Sample")
    { }
}