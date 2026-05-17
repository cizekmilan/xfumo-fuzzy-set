using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyRelationLogicTests
    {
        private readonly Universe leftUniverse = new Universe("a", "b");
        private readonly Universe middleUniverse = new Universe("x", "y");
        private readonly Universe rightUniverse = new Universe(1, 2, 3);

        // Testuje, že operace nad fuzzy relacemi odmítají povinné vstupy s hodnotou null.
        [Fact]
        public void RejectsNullInputs()
        {
            FuzzySet set = new FuzzySet(leftUniverse);
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, middleUniverse);

            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetCartesianProduct(null, set));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetCartesianProduct(set, null));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetLeftProjection(null));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetRightProjection(null));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetImage(null, relation));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetImage(set, null));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetComposition(null, relation));
            Assert.Throws<ArgumentNullException>(() => FuzzyRelationLogic.GetComposition(relation, null));
        }

        // Testuje, že obraz fuzzy množiny odmítne množinu nad nekompatibilním levým univerzem relace.
        [Fact]
        public void ImageRejectsDifferentInputUniverse()
        {
            FuzzySet set = new FuzzySet(new Universe("a", "c"));
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, middleUniverse);

            Assert.Throws<ArgumentException>(() => FuzzyRelationLogic.GetImage(set, relation));
        }

        // Testuje, že kompozice fuzzy relací odmítne nekompatibilní prostřední univerza.
        [Fact]
        public void CompositionRejectsDifferentMiddleUniverses()
        {
            FuzzyRelation first = new FuzzyRelation(leftUniverse, middleUniverse);
            FuzzyRelation second = new FuzzyRelation(new Universe("x", "z"), rightUniverse);

            Assert.Throws<ArgumentException>(() => FuzzyRelationLogic.GetComposition(first, second));
        }

        // Testuje, že kartézský součin fuzzy množin používá minimum stupňů příslušnosti.
        [Fact]
        public void GetCartesianProductUsesMinimumMembershipGrade()
        {
            FuzzySet left = new FuzzySet(leftUniverse);
            left.Add(new FuzzyElement("a", 0.4));
            left.Add(new FuzzyElement("b", 0.8));

            FuzzySet right = new FuzzySet(rightUniverse);
            right.Add(new FuzzyElement(1, 0.5));
            right.Add(new FuzzyElement(2, 1));

            FuzzyRelation expected = new FuzzyRelation(leftUniverse, rightUniverse);
            expected.Add(new FuzzyRelationElement("a", 1, 0.4));
            expected.Add(new FuzzyRelationElement("a", 2, 0.4));
            expected.Add(new FuzzyRelationElement("b", 1, 0.5));
            expected.Add(new FuzzyRelationElement("b", 2, 0.8));

            FuzzyRelation actual = FuzzyRelationLogic.GetCartesianProduct(left, right);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že projekce fuzzy relace počítají maximum přes druhé univerzum.
        [Fact]
        public void ProjectionReturnsMaximumMembershipGrades()
        {
            FuzzyRelation relation = CreateProjectionRelation();

            FuzzySet expectedLeftProjection = new FuzzySet(leftUniverse);
            expectedLeftProjection.Add(new FuzzyElement("a", 0.7));
            expectedLeftProjection.Add(new FuzzyElement("b", 0.9));

            FuzzySet expectedRightProjection = new FuzzySet(rightUniverse);
            expectedRightProjection.Add(new FuzzyElement(1, 0.4));
            expectedRightProjection.Add(new FuzzyElement(2, 0.7));
            expectedRightProjection.Add(new FuzzyElement(3, 0.9));

            FuzzySet actualLeftProjection = FuzzyRelationLogic.GetLeftProjection(relation);
            FuzzySet actualRightProjection = FuzzyRelationLogic.GetRightProjection(relation);

            SetAssert.Equal(expectedLeftProjection, actualLeftProjection);
            SetAssert.Equal(expectedRightProjection, actualRightProjection);
        }

        // Testuje, že obraz fuzzy množiny ve fuzzy relaci používá max-min kompozici.
        [Fact]
        public void GetImageUsesMaxMinComposition()
        {
            FuzzySet set = new FuzzySet(leftUniverse);
            set.Add(new FuzzyElement("a", 0.6));
            set.Add(new FuzzyElement("b", 0.8));

            FuzzyRelation relation = CreateProjectionRelation();

            FuzzySet expected = new FuzzySet(rightUniverse);
            expected.Add(new FuzzyElement(1, 0.4));
            expected.Add(new FuzzyElement(2, 0.6));
            expected.Add(new FuzzyElement(3, 0.8));

            FuzzySet actual = FuzzyRelationLogic.GetImage(set, relation);

            SetAssert.Equal(expected, actual);
        }

        // Testuje, že kompozice fuzzy relací používá max-min pravidlo přes prostřední univerzum.
        [Fact]
        public void GetCompositionUsesMaxMinComposition()
        {
            FuzzyRelation first = new FuzzyRelation(leftUniverse, middleUniverse);
            first.Add(new FuzzyRelationElement("a", "x", 0.4));
            first.Add(new FuzzyRelationElement("a", "y", 0.9));
            first.Add(new FuzzyRelationElement("b", "x", 0.7));

            FuzzyRelation second = new FuzzyRelation(middleUniverse, rightUniverse);
            second.Add(new FuzzyRelationElement("x", 1, 0.8));
            second.Add(new FuzzyRelationElement("x", 2, 0.3));
            second.Add(new FuzzyRelationElement("y", 1, 0.2));
            second.Add(new FuzzyRelationElement("y", 2, 0.6));

            FuzzyRelation expected = new FuzzyRelation(leftUniverse, rightUniverse);
            expected.Add(new FuzzyRelationElement("a", 1, 0.4));
            expected.Add(new FuzzyRelationElement("a", 2, 0.6));
            expected.Add(new FuzzyRelationElement("b", 1, 0.7));
            expected.Add(new FuzzyRelationElement("b", 2, 0.3));

            FuzzyRelation actual = FuzzyRelationLogic.GetComposition(first, second);

            SetAssert.Equal(expected, actual);
        }

        private FuzzyRelation CreateProjectionRelation()
        {
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, rightUniverse);
            relation.Add(new FuzzyRelationElement("a", 1, 0.2));
            relation.Add(new FuzzyRelationElement("a", 2, 0.7));
            relation.Add(new FuzzyRelationElement("b", 1, 0.4));
            relation.Add(new FuzzyRelationElement("b", 3, 0.9));

            return relation;
        }
    }
}
