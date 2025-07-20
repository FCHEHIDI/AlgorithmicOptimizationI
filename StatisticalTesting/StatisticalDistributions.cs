using System;

namespace StatisticalTesting
{
    /// <summary>
    /// Statistical distribution functions for hypothesis testing
    /// Implements key distributions needed for A/B testing validation
    /// </summary>
    public static class StatisticalDistributions
    {
        /// <summary>
        /// Normal (Gaussian) distribution functions
        /// </summary>
        public static class NormalDistribution
        {
            /// <summary>
            /// Cumulative Distribution Function for standard normal distribution
            /// </summary>
            public static double CDF(double z)
            {
                // Using Abramowitz and Stegun approximation
                if (z < 0)
                    return 1 - CDF(-z);
                
                double t = 1.0 / (1.0 + 0.2316419 * z);
                double y = t * (0.319381530 + t * (-0.356563782 + t * (1.781477937 + t * (-1.821255978 + t * 1.330274429))));
                
                return 1.0 - 0.39894228 * Math.Exp(-0.5 * z * z) * y;
            }
            
            /// <summary>
            /// Inverse CDF (quantile function) for standard normal distribution
            /// </summary>
            public static double InverseCDF(double probability)
            {
                if (probability <= 0 || probability >= 1)
                    throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1");
                
                // Beasley-Springer-Moro algorithm
                double a0 = 2.50662823884;
                double a1 = -18.61500062529;
                double a2 = 41.39119773534;
                double a3 = -25.44106049637;
                
                double b1 = -8.47351093090;
                double b2 = 23.08336743743;
                double b3 = -21.06224101826;
                double b4 = 3.13082909833;
                
                double c0 = 0.3374754822726147;
                double c1 = 0.9761690190917186;
                double c2 = 0.1607979714918209;
                double c3 = 0.0276438810333863;
                double c4 = 0.0038405729373609;
                double c5 = 0.0003951896511919;
                double c6 = 0.0000321767881768;
                double c7 = 0.0000002888167364;
                double c8 = 0.0000003960315187;
                
                double y = probability - 0.5;
                
                if (Math.Abs(y) < 0.42)
                {
                    double r = y * y;
                    return y * (((a3 * r + a2) * r + a1) * r + a0) / ((((b4 * r + b3) * r + b2) * r + b1) * r + 1);
                }
                else
                {
                    double r = probability;
                    if (y > 0) r = 1 - probability;
                    r = Math.Log(-Math.Log(r));
                    
                    double x = c0 + r * (c1 + r * (c2 + r * (c3 + r * (c4 + r * (c5 + r * (c6 + r * (c7 + r * c8)))))));
                    if (y < 0) x = -x;
                    return x;
                }
            }
        }
        
        /// <summary>
        /// Student's t-distribution functions
        /// </summary>
        public static class TDistribution
        {
            /// <summary>
            /// Cumulative Distribution Function for t-distribution
            /// </summary>
            public static double CDF(double t, double degreesOfFreedom)
            {
                if (degreesOfFreedom <= 0)
                    throw new ArgumentException("Degrees of freedom must be positive");
                
                // For large df, approximate with normal distribution
                if (degreesOfFreedom >= 30)
                    return NormalDistribution.CDF(t);
                
                // Using incomplete beta function approximation
                double x = degreesOfFreedom / (degreesOfFreedom + t * t);
                double betaValue = IncompleteBeta(degreesOfFreedom / 2, 0.5, x);
                
                if (t >= 0)
                    return 0.5 + 0.5 * betaValue;
                else
                    return 0.5 - 0.5 * betaValue;
            }
            
            /// <summary>
            /// Inverse CDF for t-distribution (quantile function)
            /// </summary>
            public static double InverseCDF(double probability, double degreesOfFreedom)
            {
                if (probability <= 0 || probability >= 1)
                    throw new ArgumentOutOfRangeException(nameof(probability));
                
                // For large df, approximate with normal distribution
                if (degreesOfFreedom >= 30)
                    return NormalDistribution.InverseCDF(probability);
                
                // Newton-Raphson iteration for smaller df
                double t = NormalDistribution.InverseCDF(probability); // Initial guess
                
                for (int i = 0; i < 10; i++)
                {
                    double f = CDF(t, degreesOfFreedom) - probability;
                    double df = PDF(t, degreesOfFreedom);
                    
                    if (Math.Abs(f) < 1e-12) break;
                    
                    t = t - f / df;
                }
                
                return t;
            }
            
            /// <summary>
            /// Probability Density Function for t-distribution
            /// </summary>
            public static double PDF(double t, double degreesOfFreedom)
            {
                double gamma1 = LogGamma((degreesOfFreedom + 1) / 2);
                double gamma2 = LogGamma(degreesOfFreedom / 2);
                
                double coefficient = Math.Exp(gamma1 - gamma2) / Math.Sqrt(Math.PI * degreesOfFreedom);
                double power = -(degreesOfFreedom + 1) / 2;
                
                return coefficient * Math.Pow(1 + t * t / degreesOfFreedom, power);
            }
        }
        
        /// <summary>
        /// F-distribution functions for ANOVA
        /// </summary>
        public static class FDistribution
        {
            /// <summary>
            /// Cumulative Distribution Function for F-distribution
            /// </summary>
            public static double CDF(double f, double df1, double df2)
            {
                if (f <= 0) return 0;
                if (df1 <= 0 || df2 <= 0)
                    throw new ArgumentException("Degrees of freedom must be positive");
                
                // Using incomplete beta function
                double x = df1 * f / (df1 * f + df2);
                return IncompleteBeta(df1 / 2, df2 / 2, x);
            }
        }
        
        #region Helper Functions
        
        /// <summary>
        /// Incomplete Beta function approximation
        /// </summary>
        private static double IncompleteBeta(double a, double b, double x)
        {
            if (x <= 0) return 0;
            if (x >= 1) return 1;
            
            // Use continued fraction expansion
            double bt = Math.Exp(LogGamma(a + b) - LogGamma(a) - LogGamma(b) + 
                                a * Math.Log(x) + b * Math.Log(1 - x));
            
            if (x < (a + 1) / (a + b + 2))
                return bt * ContinuedFractionBeta(a, b, x) / a;
            else
                return 1 - bt * ContinuedFractionBeta(b, a, 1 - x) / b;
        }
        
        /// <summary>
        /// Continued fraction for incomplete beta function
        /// </summary>
        private static double ContinuedFractionBeta(double a, double b, double x)
        {
            const int maxIterations = 100;
            const double eps = 3e-7;
            
            double qab = a + b;
            double qap = a + 1;
            double qam = a - 1;
            double c = 1;
            double d = 1 - qab * x / qap;
            
            if (Math.Abs(d) < 1e-30) d = 1e-30;
            d = 1 / d;
            double h = d;
            
            for (int m = 1; m <= maxIterations; m++)
            {
                int m2 = 2 * m;
                double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1 + aa * d;
                
                if (Math.Abs(d) < 1e-30) d = 1e-30;
                c = 1 + aa / c;
                if (Math.Abs(c) < 1e-30) c = 1e-30;
                
                d = 1 / d;
                h *= d * c;
                
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1 + aa * d;
                
                if (Math.Abs(d) < 1e-30) d = 1e-30;
                c = 1 + aa / c;
                if (Math.Abs(c) < 1e-30) c = 1e-30;
                
                d = 1 / d;
                double del = d * c;
                h *= del;
                
                if (Math.Abs(del - 1) < eps) break;
            }
            
            return h;
        }
        
        /// <summary>
        /// Log Gamma function approximation (Stirling's approximation)
        /// </summary>
        private static double LogGamma(double x)
        {
            if (x <= 0)
                throw new ArgumentException("Gamma function is undefined for non-positive values");
            
            // Stirling's approximation for log(Gamma(x))
            return (x - 0.5) * Math.Log(x) - x + 0.5 * Math.Log(2 * Math.PI) + 
                   1.0 / (12 * x) - 1.0 / (360 * x * x * x);
        }
        
        #endregion
    }
}
