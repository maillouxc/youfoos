﻿using System;

namespace YouFoos.StatisticsService.Utils
{
    /// <summary>
    /// Contains several utility functions for calculating values related to gaussian distributions.
    /// </summary>
    public static class GaussianFunctions
    {
        public static double At(double x, double mean = 0, double standardDeviation = 1)
        {
            double multiplier = 1.0 / (standardDeviation * Math.Sqrt(2 * Math.PI));
            double expPart = Math.Exp((-1.0 * Math.Pow(x - mean, 2.0)) / (2 * (standardDeviation * standardDeviation)));
            return multiplier * expPart;
        }

        public static double CumulativeTo(double x)
        {
            const double invsqrt2 = -0.707106781186547524400844362104;
            return 0.5 * ErrorFunctionCumulativeTo(invsqrt2 * x);
        }

        public static double InverseCumulativeTo(double x, double mean, double standardDeviation)
        {
            return mean - Math.Sqrt(2) * standardDeviation * InverseErrorFunctionCumulativeTo(2 * x);
        }

        private static double ErrorFunctionCumulativeTo(double x)
        {
            double z = Math.Abs(x);
            double t = 2.0 / (2.0 + z);
            double ty = 4 * t - 2;

            double[] coefficients = {
                -1.3026537197817094,
                6.4196979235649026e-1,
                1.9476473204185836e-2,
                -9.561514786808631e-3,
                -9.46595344482036e-4,
                3.66839497852761e-4,
                4.2523324806907e-5,
                -2.0278578112534e-5,
                -1.624290004647e-6,
                1.303655835580e-6,
                1.5626441722e-8,
                -8.5238095915e-8,
                6.529054439e-9,
                5.059343495e-9,
                -9.91364156e-10,
                -2.27365122e-10,
                9.6467911e-11,
                2.394038e-12,
                -6.886027e-12,
                8.94487e-13,
                3.13092e-13,
                -1.12708e-13,
                3.81e-16,
                7.106e-15,
                -1.523e-15,
                -9.4e-17,
                1.21e-16,
                -2.8e-17
            };

            int ncof = coefficients.Length;
            double d = 0.0;
            double dd = 0.0;

            for (int j = ncof - 1; j > 0; j--)
            {
                double tmp = d;
                d = ty * d - dd + coefficients[j];
                dd = tmp;
            }

            double ans = t * Math.Exp(-z * z + 0.5 * (coefficients[0] + ty * d) - dd);
            return x >= 0.0 ? ans : (2.0 - ans);
        }

        private static double InverseErrorFunctionCumulativeTo(double p)
        {
            if (p >= 2.0) return -100;
            if (p <= 0.0) return 100;

            double pp = (p < 1.0) ? p : 2 - p;
            double t = Math.Sqrt(-2 * Math.Log(pp / 2.0));
            double x = -0.70711 * ((2.30753 + t * 0.27061) / (1.0 + t * (0.99229 + t * 0.04481)) - t);

            for (int j = 0; j < 2; j++)
            {
                double err = ErrorFunctionCumulativeTo(x) - pp;
                x += err / (1.12837916709551257 * Math.Exp(-(x * x)) - x * err);
            }

            return p < 1.0 ? x : -x;
        }
    }
}
