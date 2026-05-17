using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    internal static class SetAssert
    {
        // Ověří množinovou rovnost klasických množin a při chybě vypíše obě hodnoty.
        public static void Equal(CrispSet expected, CrispSet actual)
        {
            Assert.True(expected.Equals(actual), FormatSetComparison(expected, actual));
        }

        // Ověří množinovou rovnost fuzzy množin a při chybě vypíše obě hodnoty.
        public static void Equal(FuzzySet expected, FuzzySet actual)
        {
            Assert.True(expected.Equals(actual), FormatSetComparison(expected, actual));
        }

        // Ověří množinovou rovnost fuzzy relací a při chybě vypíše obě hodnoty.
        public static void Equal(FuzzyRelation expected, FuzzyRelation actual)
        {
            Assert.True(expected.Equals(actual), FormatSetComparison(expected, actual));
        }

        // Sestaví čitelnou chybovou zprávu pro porovnání očekávané a skutečné množiny.
        private static string FormatSetComparison(object expected, object actual)
        {
            return string.Format("Expected: {0}{1}Actual: {2}", expected, Environment.NewLine, actual);
        }
    }
}
