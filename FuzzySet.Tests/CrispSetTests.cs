using System;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class CrispSetTests
    {
        // Testuje, že klasická množina porovnává prvky množinově bez ohledu na pořadí vložení.
        [Fact]
        public void SetEqualsUsesSetEquality()
        {
            CrispSet first = new CrispSet();
            CrispSet second = new CrispSet();

            Assert.True(first.SetEquals(second));
            Assert.True(second.SetEquals(first));

            first.Add(1);
            first.Add(2);
            second.Add(2);
            second.Add(1);

            Assert.True(first.SetEquals(second));
            Assert.True(second.SetEquals(first));

            first.Add(8);
            second.Add(9);

            Assert.False(first.SetEquals(second));
            Assert.False(second.SetEquals(first));

            first.Add(9);
            second.Add(8);

            Assert.True(first.SetEquals(second));
            Assert.True(second.SetEquals(first));
        }

        // Testuje, že klasická množina nepřijímá hodnoty null.
        [Fact]
        public void RejectsNullValues()
        {
            CrispSet set = new CrispSet();

            Assert.Throws<ArgumentNullException>(() => new CrispSet(null));
            Assert.Throws<ArgumentNullException>(() => set.Add(null));
            Assert.Throws<ArgumentNullException>(() => set.Contains(null));
        }

        // Testuje, že CrispSet neposkytuje mutační API zděděné z HashSet a zachovává vlastní řízené chování.
        [Fact]
        public void DoesNotExposeHashSetMutationApi()
        {
            CrispSet set = new CrispSet(1);

            Assert.False(set.Add(1));
            Assert.False(typeof(System.Collections.Generic.HashSet<object>).IsAssignableFrom(typeof(CrispSet)));
        }
    }
}
