using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyElementTests
    {
        // Testuje, že fuzzy prvky jsou stejné pouze při shodné hodnotě i stupni příslušnosti.
        [Fact]
        public void EqualsReturnsTrueOnlyWhenValueAndGradeMatch()
        {
            FuzzyElement firstElement = new FuzzyElement(1, 0.0);
            FuzzyElement secondElement = new FuzzyElement(2, 0.0);
            FuzzyElement lowGradeElement = new FuzzyElement(5, 0.1);
            FuzzyElement highGradeElement = new FuzzyElement(5, 0.9);
            FuzzyElement matchingElement = new FuzzyElement(5, 0.9);

            Assert.False(firstElement.Equals(secondElement));
            Assert.False(secondElement.Equals(firstElement));
            Assert.False(lowGradeElement.Equals(highGradeElement));
            Assert.False(highGradeElement.Equals(lowGradeElement));
            Assert.True(highGradeElement.Equals(matchingElement));
        }

        // Testuje, že porovnávání stupňů příslušnosti toleruje malé numerické odchylky.
        [Fact]
        public void EqualsComparesGradesWithTolerance()
        {
            FuzzyElement first = new FuzzyElement(1, 0.3);
            FuzzyElement second = new FuzzyElement(1, 0.1 + 0.2);

            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        // Testuje, že konstruktor odmítne neplatnou hodnotu nebo stupeň příslušnosti mimo interval [0, 1].
        [Fact]
        public void ConstructorRejectsInvalidValues()
        {
            Assert.Throws<ArgumentNullException>(() => new FuzzyElement(null, 0.5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyElement(1, -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyElement(1, 1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyElement(1, double.NaN));
        }
    }
}
