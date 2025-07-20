using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticalTesting
{
    /// <summary>
    /// Statistical A/B Test Result with comprehensive hypothesis validation
    /// Provides scientific validation for algorithm performance comparisons
    /// </summary>
    public class ABTestResult
    {
        public string AlgorithmA { get; set; } = string.Empty;
        public string AlgorithmB { get; set; } = string.Empty;
        
        // Core Statistical Metrics
        public double PValue { get; set; }
        public double TStatistic { get; set; }
        public double DegreesOfFreedom { get; set; }
        public bool IsSignificant { get; set; }
        public double AlphaLevel { get; set; }
        
        // Effect Size Metrics
        public double EffectSize { get; set; }  // Cohen's d
        public string EffectSizeInterpretation { get; set; } = string.Empty;
        
        // Confidence Intervals
        public double ConfidenceLevel { get; set; }
        public double ConfidenceIntervalLower { get; set; }
        public double ConfidenceIntervalUpper { get; set; }
        
        // Sample Statistics
        public int SampleSizeA { get; set; }
        public int SampleSizeB { get; set; }
        public double MeanA { get; set; }
        public double MeanB { get; set; }
        public double StandardDeviationA { get; set; }
        public double StandardDeviationB { get; set; }
        
        // Performance Improvement
        public double PerformanceImprovement { get; set; }
        public string ImprovementDirection { get; set; } = string.Empty;
        
        // Statistical Power
        public double StatisticalPower { get; set; }
        public int MinimumSampleSize { get; set; }
        
        // Recommendations
        public string Recommendation { get; set; } = string.Empty;
        public string BusinessImpact { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new List<string>();
        
        // Test Metadata
        public DateTime TestTimestamp { get; set; }
        public string TestType { get; set; } = string.Empty; // "Welch t-test", "Mann-Whitney U", etc.
        public bool AssumptionsValid { get; set; }
        
        /// <summary>
        /// Generate comprehensive statistical report
        /// </summary>
        public string GenerateReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("=== STATISTICAL A/B TEST REPORT ===");
            report.AppendLine($"Test Date: {TestTimestamp:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Test Type: {TestType}");
            report.AppendLine();
            
            report.AppendLine("ALGORITHMS COMPARED:");
            report.AppendLine($"  Algorithm A: {AlgorithmA}");
            report.AppendLine($"  Algorithm B: {AlgorithmB}");
            report.AppendLine();
            
            report.AppendLine("SAMPLE STATISTICS:");
            report.AppendLine($"  Algorithm A: n={SampleSizeA}, mean={MeanA:F3}ms, sd={StandardDeviationA:F3}ms");
            report.AppendLine($"  Algorithm B: n={SampleSizeB}, mean={MeanB:F3}ms, sd={StandardDeviationB:F3}ms");
            report.AppendLine();
            
            report.AppendLine("HYPOTHESIS TEST RESULTS:");
            report.AppendLine($"  H₀: μA = μB (no difference between algorithms)");
            report.AppendLine($"  H₁: μA ≠ μB (significant difference exists)");
            report.AppendLine($"  α = {AlphaLevel:F3} ({ConfidenceLevel:F1}% confidence level)");
            report.AppendLine($"  t-statistic = {TStatistic:F4}");
            report.AppendLine($"  degrees of freedom = {DegreesOfFreedom:F1}");
            report.AppendLine($"  p-value = {PValue:F6}");
            report.AppendLine($"  Result: {(IsSignificant ? "REJECT H₀" : "FAIL TO REJECT H₀")}");
            report.AppendLine();
            
            report.AppendLine("EFFECT SIZE ANALYSIS:");
            report.AppendLine($"  Cohen's d = {EffectSize:F3} ({EffectSizeInterpretation})");
            report.AppendLine($"  Performance Change: {PerformanceImprovement:F1}% ({ImprovementDirection})");
            report.AppendLine($"  {ConfidenceLevel:F1}% CI: [{ConfidenceIntervalLower:F3}, {ConfidenceIntervalUpper:F3}]");
            report.AppendLine();
            
            report.AppendLine("STATISTICAL POWER:");
            report.AppendLine($"  Power = {StatisticalPower:F3}");
            report.AppendLine($"  Minimum Required Sample Size: {MinimumSampleSize}");
            report.AppendLine();
            
            if (Warnings.Count > 0)
            {
                report.AppendLine("WARNINGS:");
                foreach (var warning in Warnings)
                {
                    report.AppendLine($"  ⚠️  {warning}");
                }
                report.AppendLine();
            }
            
            report.AppendLine("RECOMMENDATION:");
            report.AppendLine($"  {Recommendation}");
            report.AppendLine();
            
            report.AppendLine("BUSINESS IMPACT:");
            report.AppendLine($"  {BusinessImpact}");
            
            return report.ToString();
        }
        
        /// <summary>
        /// Save the detailed statistical test result to a file
        /// </summary>
        public string SaveDetailedReport(string baseDirectory = ".")
        {
            var timestamp = TestTimestamp.ToString("yyyyMMdd_HHmmss");
            var sanitizedNameA = SanitizeFileName(AlgorithmA);
            var sanitizedNameB = SanitizeFileName(AlgorithmB);
            var fileName = $"ABTest_{sanitizedNameA}_vs_{sanitizedNameB}_{timestamp}.txt";
            var filePath = System.IO.Path.Combine(baseDirectory, fileName);
            
            var detailedReport = GenerateDetailedReport();
            System.IO.File.WriteAllText(filePath, detailedReport);
            
            Console.WriteLine($"📄 Detailed A/B test result saved to: {fileName}");
            return filePath;
        }
        
        /// <summary>
        /// Generate an extended detailed report with additional analysis
        /// </summary>
        private string GenerateDetailedReport()
        {
            var report = new System.Text.StringBuilder();
            
            // Header with metadata
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine("📊 SWIFTCOLLAB DETAILED STATISTICAL A/B TEST ANALYSIS");
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Test Execution: {TestTimestamp:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Framework: SwiftCollab Advanced Statistical Testing Suite");
            report.AppendLine();
            
            // Main statistical report
            report.AppendLine(GenerateReport());
            report.AppendLine();
            
            // Additional detailed analysis
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine("📈 EXTENDED STATISTICAL ANALYSIS");
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            
            // Raw data summary
            report.AppendLine("RAW DATA SUMMARY:");
            report.AppendLine($"  Total Observations: {SampleSizeA + SampleSizeB}");
            report.AppendLine($"  Pooled Standard Deviation: {CalculatePooledStandardDeviation():F3}ms");
            report.AppendLine($"  Standard Error of Difference: {CalculateStandardError():F3}ms");
            report.AppendLine($"  Mean Difference: {Math.Abs(MeanA - MeanB):F3}ms");
            report.AppendLine();
            
            // Effect size interpretation
            report.AppendLine("EFFECT SIZE DETAILED INTERPRETATION:");
            report.AppendLine($"  Cohen's d = {EffectSize:F3}");
            report.AppendLine($"  Interpretation: {GetDetailedEffectSizeInterpretation()}");
            report.AppendLine($"  Practical Significance: {GetPracticalSignificanceAssessment()}");
            report.AppendLine();
            
            // Statistical assumptions
            report.AppendLine("STATISTICAL ASSUMPTIONS:");
            report.AppendLine($"  Test Type Selected: {TestType}");
            report.AppendLine($"  Assumptions Valid: {(AssumptionsValid ? "✅ YES" : "❌ NO - Non-parametric test used")}");
            report.AppendLine($"  Sample Size Adequacy: {GetSampleSizeAssessment()}");
            report.AppendLine();
            
            // Business context
            report.AppendLine("BUSINESS DECISION FRAMEWORK:");
            report.AppendLine($"  Statistical Significance: {(IsSignificant ? "✅ ACHIEVED" : "❌ NOT ACHIEVED")}");
            report.AppendLine($"  Practical Significance: {GetPracticalSignificanceLevel()}");
            report.AppendLine($"  Implementation Risk: {GetImplementationRisk()}");
            report.AppendLine($"  Confidence in Decision: {GetDecisionConfidence()}");
            report.AppendLine();
            
            return report.ToString();
        }
        
        /// <summary>
        /// Helper methods for detailed analysis
        /// </summary>
        private double CalculatePooledStandardDeviation()
        {
            var pooledVariance = ((SampleSizeA - 1) * Math.Pow(StandardDeviationA, 2) + 
                                 (SampleSizeB - 1) * Math.Pow(StandardDeviationB, 2)) / 
                                (SampleSizeA + SampleSizeB - 2);
            return Math.Sqrt(pooledVariance);
        }
        
        private double CalculateStandardError()
        {
            return Math.Sqrt((Math.Pow(StandardDeviationA, 2) / SampleSizeA) + 
                           (Math.Pow(StandardDeviationB, 2) / SampleSizeB));
        }
        
        private string GetDetailedEffectSizeInterpretation()
        {
            var absEffect = Math.Abs(EffectSize);
            if (absEffect < 0.2) return "Negligible effect - differences are minimal";
            if (absEffect < 0.5) return "Small effect - noticeable but limited practical impact";
            if (absEffect < 0.8) return "Medium effect - meaningful practical difference";
            if (absEffect < 1.2) return "Large effect - substantial practical impact";
            return "Very large effect - exceptional practical significance";
        }
        
        private string GetPracticalSignificanceAssessment()
        {
            var improvement = Math.Abs(PerformanceImprovement);
            if (improvement < 5) return "Minimal practical impact";
            if (improvement < 15) return "Moderate practical benefit";
            if (improvement < 50) return "Significant practical improvement";
            return "Exceptional practical impact";
        }
        
        private string GetSampleSizeAssessment()
        {
            var totalSample = SampleSizeA + SampleSizeB;
            if (totalSample < 30) return "⚠️ Small sample - consider increasing";
            if (totalSample < 100) return "✅ Adequate sample size";
            return "✅ Large sample - high reliability";
        }
        
        private string GetPracticalSignificanceLevel()
        {
            var improvement = Math.Abs(PerformanceImprovement);
            if (improvement < 5) return "🔸 LOW";
            if (improvement < 25) return "🔶 MODERATE";
            return "🚀 HIGH";
        }
        
        private string GetImplementationRisk()
        {
            if (!IsSignificant) return "🟡 MEDIUM (No statistical significance)";
            if (StatisticalPower < 0.8) return "🟡 MEDIUM (Low statistical power)";
            if (Math.Abs(EffectSize) < 0.5) return "🟡 MEDIUM (Small effect size)";
            return "🟢 LOW (Strong statistical evidence)";
        }
        
        private string GetDecisionConfidence()
        {
            if (!IsSignificant) return "🔸 LOW";
            if (StatisticalPower >= 0.9 && Math.Abs(EffectSize) >= 0.8) return "🚀 VERY HIGH";
            if (StatisticalPower >= 0.8 && Math.Abs(EffectSize) >= 0.5) return "✅ HIGH";
            return "🔶 MODERATE";
        }
        
        private string SanitizeFileName(string fileName)
        {
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.Replace(" ", "_").Replace("(", "").Replace(")", "");
        }
    }
    
    /// <summary>
    /// Multi-algorithm comparison result for ANOVA-style testing
    /// </summary>
    public class MultiAlgorithmTestResult
    {
        public List<string> AlgorithmNames { get; set; } = new List<string>();
        public double FStatistic { get; set; }
        public double PValue { get; set; }
        public bool IsSignificant { get; set; }
        public List<ABTestResult> PairwiseComparisons { get; set; } = new List<ABTestResult>();
        public string BestAlgorithm { get; set; } = string.Empty;
        public double BestPerformance { get; set; }
        public DateTime TestTimestamp { get; set; }
        
        public string GenerateSummaryReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("=== MULTI-ALGORITHM COMPARISON (ANOVA) ===");
            report.AppendLine($"Test Date: {TestTimestamp:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Algorithms Tested: {string.Join(", ", AlgorithmNames)}");
            report.AppendLine();
            
            report.AppendLine("OVERALL F-TEST:");
            report.AppendLine($"  F-statistic = {FStatistic:F4}");
            report.AppendLine($"  p-value = {PValue:F6}");
            report.AppendLine($"  Result: {(IsSignificant ? "Significant differences detected" : "No significant differences")}");
            report.AppendLine();
            
            report.AppendLine("BEST PERFORMING ALGORITHM:");
            report.AppendLine($"  Winner: {BestAlgorithm}");
            report.AppendLine($"  Performance: {BestPerformance:F3}ms average");
            report.AppendLine();
            
            if (PairwiseComparisons.Count > 0)
            {
                report.AppendLine("PAIRWISE COMPARISONS:");
                foreach (var comparison in PairwiseComparisons.Where(c => c.IsSignificant))
                {
                    report.AppendLine($"  {comparison.AlgorithmA} vs {comparison.AlgorithmB}: p={comparison.PValue:F6} (significant)");
                }
            }
            
            return report.ToString();
        }
        
        /// <summary>
        /// Save the multi-algorithm comparison result to a file
        /// </summary>
        public string SaveDetailedReport(string baseDirectory = ".")
        {
            var timestamp = TestTimestamp.ToString("yyyyMMdd_HHmmss");
            var fileName = $"MultiAlgorithm_ANOVA_{string.Join("_", AlgorithmNames.Take(3))}_{timestamp}.txt";
            var filePath = System.IO.Path.Combine(baseDirectory, fileName);
            
            var detailedReport = GenerateDetailedMultiReport();
            System.IO.File.WriteAllText(filePath, detailedReport);
            
            Console.WriteLine($"📄 Multi-algorithm ANOVA result saved to: {fileName}");
            return filePath;
        }
        
        private string GenerateDetailedMultiReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine("📊 SWIFTCOLLAB MULTI-ALGORITHM STATISTICAL COMPARISON (ANOVA)");
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Test Execution: {TestTimestamp:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();
            
            report.AppendLine(GenerateSummaryReport());
            report.AppendLine();
            
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            report.AppendLine("📈 DETAILED PAIRWISE ANALYSIS");
            report.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            
            if (PairwiseComparisons.Count > 0)
            {
                foreach (var comparison in PairwiseComparisons)
                {
                    report.AppendLine($"🔍 {comparison.AlgorithmA} vs {comparison.AlgorithmB}:");
                    report.AppendLine($"   p-value: {comparison.PValue:F6} {(comparison.IsSignificant ? "(significant)" : "(not significant)")}");
                    report.AppendLine($"   Effect size: {comparison.EffectSize:F3} ({comparison.EffectSizeInterpretation})");
                    report.AppendLine($"   Performance difference: {comparison.PerformanceImprovement:F1}%");
                    report.AppendLine();
                }
            }
            
            return report.ToString();
        }
    }
}
