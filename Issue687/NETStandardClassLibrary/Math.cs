namespace NETStandardClassLibrary
{
    using System;

    public static class Math
    {
        public static double Square(Func<double, double> func, double x) => func(x) * func(x);
    }
}
