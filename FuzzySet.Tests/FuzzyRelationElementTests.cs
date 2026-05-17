using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyRelationElementTests
    {
        // Testuje, že prvky fuzzy relace jsou stejné pouze při shodné dvojici hodnot i stupni příslušnosti.
        [Fact]
        public void EqualsReturnsTrueOnlyWhenValuesAndGradeMatch()
        {
            FuzzyRelationElement firstElement = new FuzzyRelationElement(1, "a", 0.5);
            FuzzyRelationElement secondElement = new FuzzyRelationElement(1, "b", 0.5);
            FuzzyRelationElement lowGradeElement = new FuzzyRelationElement(2, "c", 0.2);
            FuzzyRelationElement highGradeElement = new FuzzyRelationElement(2, "c", 0.8);
            FuzzyRelationElement matchingElement = new FuzzyRelationElement(2, "c", 0.8);

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
            FuzzyRelationElement first = new FuzzyRelationElement(1, "a", 0.3);
            FuzzyRelationElement second = new FuzzyRelationElement(1, "a", 0.1 + 0.2);

            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        // Testuje, že konstruktor odmítne neplatné hodnoty nebo stupeň příslušnosti mimo interval [0, 1].
        [Fact]
        public void ConstructorRejectsInvalidValues()
        {
            Assert.Throws<ArgumentNullException>(() => new FuzzyRelationElement(null, "a", 0.5));
            Assert.Throws<ArgumentNullException>(() => new FuzzyRelationElement(1, null, 0.5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyRelationElement(1, "a", -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyRelationElement(1, "a", 1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FuzzyRelationElement(1, "a", double.NaN));
        }
    }
}
