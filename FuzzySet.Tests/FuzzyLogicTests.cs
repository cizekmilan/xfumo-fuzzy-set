using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyLogicTests
    {
        private readonly Universe universe = new Universe(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        private readonly FuzzySet firstSet = new FuzzySet();
        private readonly FuzzySet secondSet = new FuzzySet();

        public FuzzyLogicTests()
        {
            firstSet.Universe = universe;
            firstSet.Add(new FuzzyElement(1, 0.4));
            firstSet.Add(new FuzzyElement(2, 0.7));
            firstSet.Add(new FuzzyElement(4, 0.5));
            firstSet.Add(new FuzzyElement(6, 1));
            firstSet.Add(new FuzzyElement(7, 1));
            firstSet.Add(new FuzzyElement(9, 0.1));

            secondSet.Universe = universe;
            secondSet.Add(new FuzzyElement(1, 0.2));
            secondSet.Add(new FuzzyElement(2, 0.7));
            secondSet.Add(new FuzzyElement(3, 0.9));
            secondSet.Add(new FuzzyElement(4, 0.6));
            secondSet.Add(new FuzzyElement(6, 1));
            secondSet.Add(new FuzzyElement(7, 0.8));
            secondSet.Add(new FuzzyElement(9, 0.3));
        }

        // Testuje, že veřejné operace odmítají povinné vstupy s hodnotou null.
        [Fact]
        public void RejectsNullInputs()
        {
            Assert.Throws<ArgumentNullException>(() => FuzzyLogic.GetKernel(null));
            Assert.Throws<ArgumentNullException>(() => FuzzyLogic.GetStandardUnion(null, firstSet));
            Assert.Throws<ArgumentNullException>(() => FuzzyLogic.GetStandardUnion(firstSet, null));
            Assert.Throws<ArgumentNullException>(() => FuzzyLogic.GetComplement(null, firstSet));
            Assert.Throws<ArgumentNullException>(() => FuzzyLogic.GetComplement(universe, null));
        }

        // Testuje, že alfa řezy přijímají pouze hodnotu alfa z intervalu [0, 1].
        [Fact]
        public void AlphaCutOperationsRejectInvalidAlpha()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetAlphaCut(firstSet, -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetAlphaCut(firstSet, 1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetAlphaCut(firstSet, double.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetStrongAlphaCut(firstSet, -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetStrongAlphaCut(firstSet, 1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FuzzyLogic.GetStrongAlphaCut(firstSet, double.NaN));
        }

        // Testuje, že binární operace odmítnou fuzzy množiny nad různými univerzy.
        [Fact]
        public void BinaryOperationsRejectDifferentUniverses()
        {
            FuzzySet left = new FuzzySet(new Universe(1, 2, 3));
            FuzzySet right = new FuzzySet(new Universe(1, 2, 4));

            Assert.Throws<ArgumentException>(() => FuzzyLogic.IsSubsetOf(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetStandardUnion(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetStandardIntersection(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetLukasiewiczUnion(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetLukasiewiczIntersection(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetGodelResiduum(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetLukasiewiczResiduum(left, right));
            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetDifference(left, right));
        }

        // Testuje, že doplněk se počítá pouze vzhledem ke kompatibilnímu univerzu.
        [Fact]
        public void ComplementRejectsDifferentUniverse()
        {
            FuzzySet set = new FuzzySet(new Universe(1, 2, 3));

            Assert.Throws<ArgumentException>(() => FuzzyLogic.GetComplement(new Universe(1, 2, 4), set));
        }

        // Testuje, že jádro obsahuje právě prvky s plnou příslušností.
        [Fact]
        public void GetKernelReturnsElementsWithFullMembership()
        {
            CrispSet expected = new CrispSet(6, 7);

            CrispSet actual = FuzzyLogic.GetKernel(firstSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že nosič obsahuje právě prvky s kladným stupněm příslušnosti.
        [Fact]
        public void GetSupportReturnsElementsWithPositiveMembership()
        {
            CrispSet expected = new CrispSet(1, 2, 4, 6, 7, 9);

            CrispSet actual = FuzzyLogic.GetSupport(firstSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že alfa řez obsahuje prvky se stupněm příslušnosti větším nebo rovným alfa.
        [Fact]
        public void GetAlphaCutReturnsElementsAtOrAboveAlpha()
        {
            CrispSet expectedForHalf = new CrispSet(2, 4, 6, 7);
            CrispSet expectedForSevenTenths = new CrispSet(2, 6, 7);

            CrispSet actualForHalf = FuzzyLogic.GetAlphaCut(firstSet, 0.5);
            CrispSet actualForSevenTenths = FuzzyLogic.GetAlphaCut(firstSet, 0.7);

            SetAssert.Equal(expectedForHalf, actualForHalf);
            SetAssert.Equal(expectedForSevenTenths, actualForSevenTenths);
        }

        // Testuje, že silný alfa řez obsahuje pouze prvky se stupněm příslušnosti ostře větším než alfa.
        [Fact]
        public void GetStrongAlphaCutReturnsElementsAboveAlpha()
        {
            CrispSet expected = new CrispSet(2, 6, 7);

            CrispSet actual = FuzzyLogic.GetStrongAlphaCut(firstSet, 0.5);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že hranice obsahuje právě částečně příslušné prvky.
        [Fact]
        public void GetBoundaryReturnsPartiallyIncludedElements()
        {
            CrispSet expected = new CrispSet(1, 2, 4, 9);

            CrispSet actual = FuzzyLogic.GetBoundary(firstSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že výška fuzzy množiny odpovídá nejvyššímu stupni příslušnosti.
        [Fact]
        public void GetHeightReturnsMaximumMembershipGrade()
        {
            FuzzySet emptySet = new FuzzySet(universe);

            Assert.Equal(1, FuzzyLogic.GetHeight(firstSet));
            Assert.Equal(0, FuzzyLogic.GetHeight(emptySet));
        }

        // Testuje, že normální fuzzy množina má alespoň jeden prvek s plnou příslušností.
        [Fact]
        public void IsNormalReturnsTrueWhenAnyElementHasFullMembership()
        {
            FuzzySet nonNormalSet = new FuzzySet(universe);
            nonNormalSet.Add(new FuzzyElement(1, 0.9));
            nonNormalSet.Add(new FuzzyElement(2, 0.4));

            Assert.True(FuzzyLogic.IsNormal(firstSet));
            Assert.False(FuzzyLogic.IsNormal(nonNormalSet));
        }

        // Testuje, že kardinalita konečné fuzzy množiny je součtem stupňů příslušnosti.
        [Fact]
        public void GetCardinalityReturnsSumOfMembershipGrades()
        {
            double expected = 0.4 + 0.7 + 0.5 + 1 + 1 + 0.1;

            double actual = FuzzyLogic.GetCardinality(firstSet);

            Assert.Equal(expected, actual);
        }

        // Testuje, že relace podmnožiny porovnává stupně příslušnosti po jednotlivých hodnotách.
        [Fact]
        public void IsSubsetOfComparesMembershipGrades()
        {
            FuzzySet subset = new FuzzySet(universe);
            subset.Add(new FuzzyElement(1, 0.2));
            subset.Add(new FuzzyElement(2, 0.7));
            subset.Add(new FuzzyElement(7, 0.8));

            FuzzySet notSubset = new FuzzySet(universe);
            notSubset.Add(new FuzzyElement(1, 0.2));
            notSubset.Add(new FuzzyElement(5, 0.1));

            Assert.True(FuzzyLogic.IsSubsetOf(subset, firstSet));
            Assert.True(FuzzyLogic.IsSubsetOf(new FuzzySet(universe), firstSet));
            Assert.False(FuzzyLogic.IsSubsetOf(firstSet, secondSet));
            Assert.False(FuzzyLogic.IsSubsetOf(notSubset, firstSet));
        }

        // Testuje, že standardní sjednocení používá maximum stupňů příslušnosti.
        [Fact]
        public void GetStandardUnionUsesMaximumMembershipGrade()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(1, 0.4));
            expected.Add(new FuzzyElement(2, 0.7));
            expected.Add(new FuzzyElement(3, 0.9));
            expected.Add(new FuzzyElement(4, 0.6));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 1));
            expected.Add(new FuzzyElement(9, 0.3));

            FuzzySet actual = FuzzyLogic.GetStandardUnion(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že standardní průnik používá minimum stupňů příslušnosti.
        [Fact]
        public void GetStandardIntersectionUsesMinimumMembershipGrade()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(1, 0.2));
            expected.Add(new FuzzyElement(2, 0.7));
            expected.Add(new FuzzyElement(4, 0.5));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 0.8));
            expected.Add(new FuzzyElement(9, 0.1));

            FuzzySet actual = FuzzyLogic.GetStandardIntersection(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že starší názvy regular operací zůstávají kompatibilními aliasy.
        [Fact]
        public void RegularOperationNamesRemainAsCompatibilityAliases()
        {
#pragma warning disable CS0618
            FuzzySet union = FuzzyLogic.GetRegularUnion(firstSet, secondSet);
            FuzzySet intersection = FuzzyLogic.GetRegularIntersection(firstSet, secondSet);
#pragma warning restore CS0618

            SetAssert.Equal(FuzzyLogic.GetStandardUnion(firstSet, secondSet), union);
            SetAssert.Equal(FuzzyLogic.GetStandardIntersection(firstSet, secondSet), intersection);
        }

        // Testuje, že doplněk převrací příslušnost všech hodnot z univerza.
        [Fact]
        public void GetComplementReturnsUniverseRelativeComplement()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(0, 1));
            expected.Add(new FuzzyElement(1, 0.6));
            expected.Add(new FuzzyElement(2, 1 - 0.7));
            expected.Add(new FuzzyElement(3, 1));
            expected.Add(new FuzzyElement(4, 0.5));
            expected.Add(new FuzzyElement(5, 1));
            expected.Add(new FuzzyElement(8, 1));
            expected.Add(new FuzzyElement(9, 0.9));
            expected.Add(new FuzzyElement(10, 1));

            FuzzySet actual = FuzzyLogic.GetComplement(universe, firstSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že rozdíl odpovídá průniku levé množiny s doplňkem pravé množiny.
        [Fact]
        public void GetDifferenceReturnsIntersectionWithRightComplement()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(1, 0.4));
            expected.Add(new FuzzyElement(2, 1 - 0.7));
            expected.Add(new FuzzyElement(4, 1 - 0.6));
            expected.Add(new FuzzyElement(7, 1 - 0.8));
            expected.Add(new FuzzyElement(9, 0.1));

            FuzzySet actual = FuzzyLogic.GetDifference(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že Lukasiewiczovo sjednocení používá omezený součet.
        [Fact]
        public void GetLukasiewiczUnionUsesBoundedSum()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(1, 0.4 + 0.2));
            expected.Add(new FuzzyElement(2, 1));
            expected.Add(new FuzzyElement(3, 0.9));
            expected.Add(new FuzzyElement(4, 1));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 1));
            expected.Add(new FuzzyElement(9, 0.1 + 0.3));

            FuzzySet actual = FuzzyLogic.GetLukasiewiczUnion(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že Lukasiewiczův průnik používá omezený rozdíl.
        [Fact]
        public void GetLukasiewiczIntersectionUsesBoundedDifference()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(2, 0.7 + 0.7 - 1));
            expected.Add(new FuzzyElement(4, 0.5 + 0.6 - 1));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 1 + 0.8 - 1));

            FuzzySet actual = FuzzyLogic.GetLukasiewiczIntersection(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že Godelovo reziduum pracuje nad univerzem a chybějící hodnoty bere jako příslušnost 0.
        [Fact]
        public void GetGodelResiduumUsesUniverseAndMissingElementsAsZero()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(0, 1));
            expected.Add(new FuzzyElement(1, 0.2));
            expected.Add(new FuzzyElement(2, 1));
            expected.Add(new FuzzyElement(3, 1));
            expected.Add(new FuzzyElement(4, 1));
            expected.Add(new FuzzyElement(5, 1));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 0.8));
            expected.Add(new FuzzyElement(8, 1));
            expected.Add(new FuzzyElement(9, 1));
            expected.Add(new FuzzyElement(10, 1));

            FuzzySet actual = FuzzyLogic.GetGodelResiduum(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že Lukasiewiczovo reziduum pracuje nad univerzem a chybějící hodnoty bere jako příslušnost 0.
        [Fact]
        public void GetLukasiewiczResiduumUsesUniverseAndMissingElementsAsZero()
        {
            FuzzySet expected = new FuzzySet(universe);
            expected.Add(new FuzzyElement(0, 1));
            expected.Add(new FuzzyElement(1, 1 - 0.4 + 0.2));
            expected.Add(new FuzzyElement(2, 1));
            expected.Add(new FuzzyElement(3, 1));
            expected.Add(new FuzzyElement(4, 1));
            expected.Add(new FuzzyElement(5, 1));
            expected.Add(new FuzzyElement(6, 1));
            expected.Add(new FuzzyElement(7, 1 - 1 + 0.8));
            expected.Add(new FuzzyElement(8, 1));
            expected.Add(new FuzzyElement(9, 1));
            expected.Add(new FuzzyElement(10, 1));

            FuzzySet actual = FuzzyLogic.GetLukasiewiczResiduum(firstSet, secondSet);

            SetAssert.Equal(expected, actual);
        }
    }
}
