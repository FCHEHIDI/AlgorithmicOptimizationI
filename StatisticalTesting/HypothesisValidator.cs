using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticalTesting
{
    /// <summary>
    /// Advanced statistical validation framework for algorithm comparison
    /// Implements rigorous hypothesis testing with multiple statistical methods
    /// </summary>
    public static class HypothesisValidator
    {
        /// <summary>
        /// Welch's t-test for comparing two algorithms with unequal variances
        /// Most robust method for algorithm performance comparison
        /// </summary>
        public static ABTestResult WelchTTest(
            List<double> samplesA, 
            List<double> samplesB, 
            string algorithmA = "Algorithm A",
            string algorithmB = "Algorithm B",
            double alphaLevel = 0.05)
        {
            // Input validation
            if (samplesA.Count < 2 || samplesB.Count < 2)
                throw new ArgumentException("Each sample group must have at least 2 observations");
            
            // Calculate descriptive statistics
            double meanA = samplesA.Average();
            double meanB = samplesB.Average();
            double varA = CalculateVariance(samplesA);
            double varB = CalculateVariance(samplesB);
            double stdA = Math.Sqrt(varA);
            double stdB = Math.Sqrt(varB);
            
            // Welch's t-test calculations
            double standardError = Math.Sqrt(varA / samplesA.Count + varB / samplesB.Count);
            double tStatistic = (meanA - meanB) / standardError;
            
            // Welch-Satterthwaite degrees of freedom approximation
            double numerator = Math.Pow(varA / samplesA.Count + varB / samplesB.Count, 2);
            double denominator = Math.Pow(varA / samplesA.Count, 2) / (samplesA.Count - 1) + 
                               Math.Pow(varB / samplesB.Count, 2) / (samplesB.Count - 1);
            double degreesOfFreedom = numerator / denominator;
            
            // Calculate p-value (two-tailed test)
            double pValue = 2 * (1 - StatisticalDistributions.TDistribution.CDF(Math.Abs(tStatistic), degreesOfFreedom));
            
            // Effect size (Cohen's d)
            double pooledStandardDeviation = Math.Sqrt((varA + varB) / 2);
            double cohensD = Math.Abs(meanA - meanB) / pooledStandardDeviation;
            
            // Confidence interval for difference in means
            double criticalValue = StatisticalDistributions.TDistribution.InverseCDF(1 - alphaLevel / 2, degreesOfFreedom);
            double marginOfError = criticalValue * standardError;
            double meanDifference = meanA - meanB;
            
            // Statistical power calculation (approximate)
            double power = CalculateStatisticalPower(cohensD, samplesA.Count, samplesB.Count, alphaLevel);
            
            // Performance improvement calculation
            double improvement = meanA > meanB ? 
                ((meanA - meanB) / meanB) * 100 : 
                ((meanB - meanA) / meanA) * 100;
            
            string direction = meanA < meanB ? "Algorithm A faster" : "Algorithm B faster";
            
            var result = new ABTestResult
            {
                AlgorithmA = algorithmA,
                AlgorithmB = algorithmB,
                PValue = pValue,
                TStatistic = tStatistic,
                DegreesOfFreedom = degreesOfFreedom,
                IsSignificant = pValue < alphaLevel,
                AlphaLevel = alphaLevel,
                EffectSize = cohensD,
                EffectSizeInterpretation = InterpretCohenD(cohensD),
                ConfidenceLevel = (1 - alphaLevel) * 100,
                ConfidenceIntervalLower = meanDifference - marginOfError,
                ConfidenceIntervalUpper = meanDifference + marginOfError,
                SampleSizeA = samplesA.Count,
                SampleSizeB = samplesB.Count,
                MeanA = meanA,
                MeanB = meanB,
                StandardDeviationA = stdA,
                StandardDeviationB = stdB,
                PerformanceImprovement = improvement,
                ImprovementDirection = direction,
                StatisticalPower = power,
                MinimumSampleSize = CalculateMinimumSampleSize(cohensD, alphaLevel, 0.80),
                TestTimestamp = DateTime.Now,
                TestType = "Welch's t-test (unequal variances)",
                AssumptionsValid = ValidateAssumptions(samplesA, samplesB)
            };
            
            result.Recommendation = GenerateRecommendation(result);
            result.BusinessImpact = GenerateBusinessImpact(result);
            result.Warnings = GenerateWarnings(result, samplesA, samplesB);
            
            return result;
        }
        
        /// <summary>
        /// Mann-Whitney U test for non-parametric comparison
        /// Used when data doesn't meet normality assumptions
        /// </summary>
        public static ABTestResult MannWhitneyUTest(
            List<double> samplesA, 
            List<double> samplesB,
            string algorithmA = "Algorithm A",
            string algorithmB = "Algorithm B",
            double alphaLevel = 0.05)
        {
            // Combine and rank all observations
            var combined = samplesA.Select(x => new { Value = x, Group = "A" })
                          .Concat(samplesB.Select(x => new { Value = x, Group = "B" }))
                          .OrderBy(x => x.Value)
                          .ToList();
            
            // Assign ranks (handle ties)
            var ranks = AssignRanks(combined.Select(x => x.Value).ToList());
            
            // Calculate U statistics
            double sumRanksA = 0;
            for (int i = 0; i < combined.Count; i++)
            {
                if (combined[i].Group == "A")
                    sumRanksA += ranks[i];
            }
            
            double UA = sumRanksA - (samplesA.Count * (samplesA.Count + 1)) / 2.0;
            double UB = samplesA.Count * samplesB.Count - UA;
            double U = Math.Min(UA, UB);
            
            // Calculate z-score for large samples
            double meanU = (samplesA.Count * samplesB.Count) / 2.0;
            double stdU = Math.Sqrt((samplesA.Count * samplesB.Count * (samplesA.Count + samplesB.Count + 1)) / 12.0);
            double zScore = (U - meanU) / stdU;
            
            // Calculate p-value
            double pValue = 2 * (1 - StatisticalDistributions.NormalDistribution.CDF(Math.Abs(zScore)));
            
            // Effect size (rank-biserial correlation)
            double effectSize = 1 - (2 * U) / (samplesA.Count * samplesB.Count);
            
            var result = new ABTestResult
            {
                AlgorithmA = algorithmA,
                AlgorithmB = algorithmB,
                PValue = pValue,
                TStatistic = zScore, // Using z-score in place of t-statistic
                IsSignificant = pValue < alphaLevel,
                AlphaLevel = alphaLevel,
                EffectSize = Math.Abs(effectSize),
                EffectSizeInterpretation = InterpretRankBiserialCorrelation(Math.Abs(effectSize)),
                SampleSizeA = samplesA.Count,
                SampleSizeB = samplesB.Count,
                MeanA = samplesA.Average(),
                MeanB = samplesB.Average(),
                TestTimestamp = DateTime.Now,
                TestType = "Mann-Whitney U test (non-parametric)",
                AssumptionsValid = true // Non-parametric test has fewer assumptions
            };
            
            result.Recommendation = GenerateRecommendation(result);
            result.BusinessImpact = GenerateBusinessImpact(result);
            
            return result;
        }
        
        /// <summary>
        /// ANOVA F-test for comparing multiple algorithms simultaneously
        /// Controls family-wise error rate when testing multiple comparisons
        /// </summary>
        public static MultiAlgorithmTestResult OneWayANOVA(
            Dictionary<string, List<double>> algorithmSamples,
            double alphaLevel = 0.05)
        {
            if (algorithmSamples.Count < 2)
                throw new ArgumentException("Need at least 2 algorithms for comparison");
            
            // Calculate overall statistics
            var allSamples = algorithmSamples.SelectMany(kvp => kvp.Value).ToList();
            double grandMean = allSamples.Average();
            int totalN = allSamples.Count;
            int k = algorithmSamples.Count; // number of groups
            
            // Calculate Sum of Squares Between Groups (SSB)
            double ssb = 0;
            foreach (var kvp in algorithmSamples)
            {
                double groupMean = kvp.Value.Average();
                int groupN = kvp.Value.Count;
                ssb += groupN * Math.Pow(groupMean - grandMean, 2);
            }
            
            // Calculate Sum of Squares Within Groups (SSW)
            double ssw = 0;
            foreach (var kvp in algorithmSamples)
            {
                double groupMean = kvp.Value.Average();
                foreach (double value in kvp.Value)
                {
                    ssw += Math.Pow(value - groupMean, 2);
                }
            }
            
            // Calculate F-statistic
            double msb = ssb / (k - 1); // Mean Square Between
            double msw = ssw / (totalN - k); // Mean Square Within
            double fStatistic = msb / msw;
            
            // Calculate p-value using F-distribution
            double pValue = 1 - StatisticalDistributions.FDistribution.CDF(fStatistic, k - 1, totalN - k);
            
            // Find best performing algorithm
            var bestAlgorithm = algorithmSamples
                .OrderBy(kvp => kvp.Value.Average())
                .First();
            
            var result = new MultiAlgorithmTestResult
            {
                AlgorithmNames = algorithmSamples.Keys.ToList(),
                FStatistic = fStatistic,
                PValue = pValue,
                IsSignificant = pValue < alphaLevel,
                BestAlgorithm = bestAlgorithm.Key,
                BestPerformance = bestAlgorithm.Value.Average(),
                TestTimestamp = DateTime.Now
            };
            
            // Perform pairwise comparisons if overall test is significant
            if (result.IsSignificant)
            {
                result.PairwiseComparisons = PerformPairwiseComparisons(algorithmSamples, alphaLevel);
            }
            
            return result;
        }
        
        #region Helper Methods
        
        private static double CalculateVariance(List<double> samples)
        {
            double mean = samples.Average();
            return samples.Sum(x => Math.Pow(x - mean, 2)) / (samples.Count - 1);
        }
        
        private static string InterpretCohenD(double cohensD)
        {
            if (cohensD < 0.2) return "Negligible effect";
            if (cohensD < 0.5) return "Small effect";
            if (cohensD < 0.8) return "Medium effect";
            return "Large effect";
        }
        
        private static string InterpretRankBiserialCorrelation(double r)
        {
            if (r < 0.1) return "Negligible effect";
            if (r < 0.3) return "Small effect";
            if (r < 0.5) return "Medium effect";
            return "Large effect";
        }
        
        private static double CalculateStatisticalPower(double effectSize, int n1, int n2, double alpha)
        {
            // Simplified power calculation (exact calculation requires non-central t-distribution)
            double delta = effectSize * Math.Sqrt((n1 * n2) / (double)(n1 + n2));
            double criticalValue = StatisticalDistributions.TDistribution.InverseCDF(1 - alpha / 2, n1 + n2 - 2);
            
            // Approximate power using normal approximation
            double power = 1 - StatisticalDistributions.NormalDistribution.CDF(criticalValue - delta) + 
                          StatisticalDistributions.NormalDistribution.CDF(-criticalValue - delta);
            
            return Math.Min(1.0, Math.Max(0.0, power));
        }
        
        private static int CalculateMinimumSampleSize(double effectSize, double alpha, double power)
        {
            // Cohen's sample size formula (approximate)
            double zAlpha = StatisticalDistributions.NormalDistribution.InverseCDF(1 - alpha / 2);
            double zBeta = StatisticalDistributions.NormalDistribution.InverseCDF(power);
            
            double n = 2 * Math.Pow(zAlpha + zBeta, 2) / Math.Pow(effectSize, 2);
            return (int)Math.Ceiling(n);
        }
        
        private static bool ValidateAssumptions(List<double> samplesA, List<double> samplesB)
        {
            // Check for normality (simplified Shapiro-Wilk approximation)
            bool normalityA = CheckNormality(samplesA);
            bool normalityB = CheckNormality(samplesB);
            
            // Check for outliers using IQR method
            bool outliersA = CheckForOutliers(samplesA);
            bool outliersB = CheckForOutliers(samplesB);
            
            return normalityA && normalityB && !outliersA && !outliersB;
        }
        
        private static bool CheckNormality(List<double> samples)
        {
            if (samples.Count < 8) return true; // Too small to test reliably
            
            // Simplified normality check using skewness and kurtosis
            double skewness = CalculateSkewness(samples);
            double kurtosis = CalculateKurtosis(samples);
            
            return Math.Abs(skewness) < 2 && Math.Abs(kurtosis - 3) < 4;
        }
        
        private static bool CheckForOutliers(List<double> samples)
        {
            samples.Sort();
            int n = samples.Count;
            
            double q1 = samples[n / 4];
            double q3 = samples[3 * n / 4];
            double iqr = q3 - q1;
            
            double lowerBound = q1 - 1.5 * iqr;
            double upperBound = q3 + 1.5 * iqr;
            
            return samples.Any(x => x < lowerBound || x > upperBound);
        }
        
        private static double CalculateSkewness(List<double> samples)
        {
            double mean = samples.Average();
            double std = Math.Sqrt(CalculateVariance(samples));
            double n = samples.Count;
            
            double skewness = samples.Sum(x => Math.Pow((x - mean) / std, 3)) / n;
            return skewness;
        }
        
        private static double CalculateKurtosis(List<double> samples)
        {
            double mean = samples.Average();
            double std = Math.Sqrt(CalculateVariance(samples));
            double n = samples.Count;
            
            double kurtosis = samples.Sum(x => Math.Pow((x - mean) / std, 4)) / n;
            return kurtosis;
        }
        
        private static List<double> AssignRanks(List<double> values)
        {
            var sorted = values.Select((value, index) => new { Value = value, Index = index })
                              .OrderBy(x => x.Value)
                              .ToList();
            
            var ranks = new double[values.Count];
            
            for (int i = 0; i < sorted.Count; i++)
            {
                // Handle ties by averaging ranks
                int start = i;
                while (i + 1 < sorted.Count && sorted[i + 1].Value == sorted[i].Value)
                    i++;
                
                double averageRank = (start + i + 2) / 2.0; // +1 for 1-based ranking, +1 for end inclusive
                
                for (int j = start; j <= i; j++)
                {
                    ranks[sorted[j].Index] = averageRank;
                }
            }
            
            return ranks.ToList();
        }
        
        private static List<ABTestResult> PerformPairwiseComparisons(
            Dictionary<string, List<double>> algorithmSamples,
            double alphaLevel)
        {
            var results = new List<ABTestResult>();
            var algorithms = algorithmSamples.Keys.ToList();
            
            // Bonferroni correction for multiple comparisons
            double adjustedAlpha = alphaLevel / (algorithms.Count * (algorithms.Count - 1) / 2);
            
            for (int i = 0; i < algorithms.Count; i++)
            {
                for (int j = i + 1; j < algorithms.Count; j++)
                {
                    var result = WelchTTest(
                        algorithmSamples[algorithms[i]],
                        algorithmSamples[algorithms[j]],
                        algorithms[i],
                        algorithms[j],
                        adjustedAlpha);
                    
                    results.Add(result);
                }
            }
            
            return results;
        }
        
        private static string GenerateRecommendation(ABTestResult result)
        {
            if (!result.IsSignificant)
            {
                return $"No statistically significant difference detected (p={result.PValue:F6} ≥ α={result.AlphaLevel:F3}). " +
                       "Consider collecting more data or the algorithms perform similarly.";
            }
            
            string betterAlgorithm = result.MeanA < result.MeanB ? result.AlgorithmA : result.AlgorithmB;
            
            return $"Statistically significant difference detected (p={result.PValue:F6} < α={result.AlphaLevel:F3}). " +
                   $"Recommend using {betterAlgorithm} with {result.EffectSizeInterpretation.ToLower()} practical significance " +
                   $"({result.PerformanceImprovement:F1}% improvement).";
        }
        
        private static string GenerateBusinessImpact(ABTestResult result)
        {
            if (!result.IsSignificant)
            {
                return "No significant business impact expected from algorithm change.";
            }
            
            double improvement = result.PerformanceImprovement;
            
            if (improvement > 50)
                return $"HIGH IMPACT: {improvement:F1}% performance improvement could significantly reduce operational costs and improve user experience.";
            else if (improvement > 20)
                return $"MEDIUM IMPACT: {improvement:F1}% performance improvement provides measurable business value.";
            else if (improvement > 5)
                return $"LOW IMPACT: {improvement:F1}% performance improvement provides marginal business value.";
            else
                return $"MINIMAL IMPACT: {improvement:F1}% performance improvement may not justify implementation costs.";
        }
        
        private static List<string> GenerateWarnings(ABTestResult result, List<double> samplesA, List<double> samplesB)
        {
            var warnings = new List<string>();
            
            if (result.StatisticalPower < 0.8)
                warnings.Add($"Low statistical power ({result.StatisticalPower:F3}). Consider increasing sample size to {result.MinimumSampleSize}+");
            
            if (!result.AssumptionsValid)
                warnings.Add("Normality assumptions may be violated. Consider Mann-Whitney U test for non-parametric analysis");
            
            if (result.SampleSizeA < 30 || result.SampleSizeB < 30)
                warnings.Add("Small sample sizes may affect reliability. Central Limit Theorem is more reliable with n≥30");
            
            // Check for high variance
            double cvA = result.StandardDeviationA / result.MeanA;
            double cvB = result.StandardDeviationB / result.MeanB;
            
            if (cvA > 0.5 || cvB > 0.5)
                warnings.Add("High coefficient of variation detected. Results may be influenced by outliers");
            
            return warnings;
        }
        
        #endregion
    }
}
