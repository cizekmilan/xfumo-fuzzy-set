using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyRelationTests
    {
        private readonly Universe leftUniverse = new Universe("a", "b");
        private readonly Universe rightUniverse = new Universe(1, 2, 3);

        // Testuje, že fuzzy relace se porovnávají množinově podle uložených prvků relace.
        [Fact]
        public void EqualsUsesSetEquality()
        {
            FuzzyRelation first = new FuzzyRelation(leftUniverse, rightUniverse);
            FuzzyRelation second = new FuzzyRelation(leftUniverse, rightUniverse);

            Assert.True(first.Equals(second));
            Assert.True(second.Equals(first));

            first.Add(new FuzzyRelationElement("a", 1, 0.5));
            second.Add(new FuzzyRelationElement("a", 1, 0.5));

            Assert.True(first.Equals(second));
            Assert.True(second.Equals(first));

            first.Add(new FuzzyRelationElement("b", 2, 0.7));
            second.Add(new FuzzyRelationElement("b", 2, 0.3));

            Assert.False(first.Equals(second));
            Assert.False(second.Equals(first));
        }

        // Testuje, že fuzzy relace explicitně neukládá dvojice s nulovým stupněm příslušnosti.
        [Fact]
        public void StoresOnlyPositiveMembershipGrades()
        {
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, rightUniverse);

            bool added = relation.Add(new FuzzyRelationElement("a", 1, 0));

            Assert.False(added);
            Assert.Empty(relation);
            Assert.Equal(0, relation.GetMembershipGrade("a", 1));
        }

        // Testuje, že opakované vložení stejného prvku relace nezmění obsah relace.
        [Fact]
        public void AddIsIdempotentForMatchingElements()
        {
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, rightUniverse);

            Assert.True(relation.Add(new FuzzyRelationElement("a", 1, 0.5)));
            Assert.False(relation.Add(new FuzzyRelationElement("a", 1, 0.5 + FuzzyElement.GradeTolerance / 2)));
            Assert.Single(relation);
        }

        // Testuje, že jedna dvojice hodnot nemůže být v relaci uložena se dvěma různými stupni příslušnosti.
        [Fact]
        public void AddRejectsSamePairWithDifferentGrade()
        {
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, rightUniverse);
            relation.Add(new FuzzyRelationElement("a", 1, 0.5));

            Assert.Throws<ArgumentException>(() => relation.Add(new FuzzyRelationElement("a", 1, 0.7)));
        }

        // Testuje, že FuzzyRelation neposkytuje mutační API zděděné z List a zachovává vlastní řízené chování.
        [Fact]
        public void DoesNotExposeListMutationApi()
        {
            Assert.False(typeof(System.Collections.Generic.List<FuzzyRelationElement>).IsAssignableFrom(typeof(FuzzyRelation)));
        }

        // Testuje, že chybějící dvojice hodnot má implicitní stupeň příslušnosti 0.
        [Fact]
        public void GetMembershipGradeReturnsZeroForMissingPairs()
        {
            FuzzyRelation relation = new FuzzyRelation(leftUniverse, rightUniverse);
            relation.Add(new FuzzyRelationElement("a", 2, 0.7));

            Assert.Equal(0.7, relation.GetMembershipGrade("a", 2));
            Assert.Equal(0, relation.GetMembershipGrade("b", 2));
        }
    }
}
