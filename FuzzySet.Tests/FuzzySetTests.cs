using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzySetTests
    {
        private readonly Universe universe = new Universe(0, 1, 2, 3, 4, 5);

        // Testuje, že fuzzy množiny se porovnávají množinově podle uložených fuzzy prvků.
        [Fact]
        public void EqualsUsesSetEquality()
        {
            FuzzySet first = new FuzzySet(universe);
            FuzzySet second = new FuzzySet(universe);

            Assert.True(first.Equals(second));
            Assert.True(second.Equals(first));

            first.Add(new FuzzyElement(1, 0.5));
            second.Add(new FuzzyElement(1, 0.5));

            Assert.True(first.Equals(second));
            Assert.True(second.Equals(first));

            first.Add(new FuzzyElement(2, 0.7));
            second.Add(new FuzzyElement(2, 0.3));

            Assert.False(first.Equals(second));
            Assert.False(second.Equals(first));
        }

        // Testuje, že fuzzy množina explicitně neukládá prvky s nulovým stupněm příslušnosti.
        [Fact]
        public void StoresOnlyPositiveMembershipGrades()
        {
            FuzzySet set = new FuzzySet(universe);

            bool added = set.Add(new FuzzyElement(1, 0));

            Assert.False(added);
            Assert.Empty(set);
            Assert.Equal(0, set.GetMembershipGrade(1));
        }

        // Testuje, že opakované vložení stejného fuzzy prvku nezmění obsah množiny.
        [Fact]
        public void AddIsIdempotentForMatchingElements()
        {
            FuzzySet set = new FuzzySet(universe);

            Assert.True(set.Add(new FuzzyElement(1, 0.5)));
            Assert.False(set.Add(new FuzzyElement(1, 0.5 + FuzzyElement.GradeTolerance / 2)));
            Assert.Single(set);
        }

        // Testuje, že jedna hodnota nemůže být v množině uložena se dvěma různými stupni příslušnosti.
        [Fact]
        public void AddRejectsSameValueWithDifferentGrade()
        {
            FuzzySet set = new FuzzySet(universe);
            set.Add(new FuzzyElement(1, 0.5));

            Assert.Throws<ArgumentException>(() => set.Add(new FuzzyElement(1, 0.7)));
        }

        // Testuje, že FuzzySet neposkytuje mutační API zděděné z List a zachovává vlastní řízené chování.
        [Fact]
        public void DoesNotExposeListMutationApi()
        {
            Assert.False(typeof(System.Collections.Generic.List<FuzzyElement>).IsAssignableFrom(typeof(FuzzySet)));
        }

        // Testuje, že chybějící hodnota má implicitní stupeň příslušnosti 0.
        [Fact]
        public void GetMembershipGradeReturnsZeroForMissingValues()
        {
            FuzzySet set = new FuzzySet(universe);
            set.Add(new FuzzyElement(2, 0.7));

            Assert.Equal(0.7, set.GetMembershipGrade(2));
            Assert.Equal(0, set.GetMembershipGrade(3));
        }
    }
}
