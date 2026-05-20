using System;
using System.Collections.Generic;
using Fuzzy;
using Xunit;

namespace Fuzzy.Tests
{
    public class FuzzyDefuzzificationTests
    {
        private static readonly Func<object, double> ToDouble = x => Convert.ToDouble(x);

        // Testuje, že COG vrací vážený průměr hodnot podle stupňů příslušnosti.
        [Fact]
        public void GetCenterOfGravityReturnsWeightedAverage()
        {
            FuzzySet set = new FuzzySet(new Universe(0, 10, 20));
            set.Add(new FuzzyElement(0, 0.2));
            set.Add(new FuzzyElement(10, 0.6));
            set.Add(new FuzzyElement(20, 1));

            double actual = FuzzyDefuzzification.GetCenterOfGravity(set, ToDouble);

            Assert.Equal((0 * 0.2 + 10 * 0.6 + 20 * 1) / (0.2 + 0.6 + 1), actual, 10);
        }

        // Testuje, že COA je pro konečnou fuzzy množinu aliasem metody COG.
        [Fact]
        public void GetCenterOfAreaReturnsCenterOfGravity()
        {
            FuzzySet set = new FuzzySet(new Universe(0, 10, 20));
            set.Add(new FuzzyElement(0, 0.2));
            set.Add(new FuzzyElement(10, 0.6));
            set.Add(new FuzzyElement(20, 1));

            double centerOfGravity = FuzzyDefuzzification.GetCenterOfGravity(set, ToDouble);
            double centerOfArea = FuzzyDefuzzification.GetCenterOfArea(set, ToDouble);

            Assert.Equal(centerOfGravity, centerOfArea);
        }

        // Testuje, že COS sčítá příspěvky dílčích fuzzy množin bez slévání překryvů.
        [Fact]
        public void GetCenterOfSumsReturnsWeightedAverageAcrossPartialSets()
        {
            FuzzySet first = new FuzzySet(new Universe(0, 10, 20));
            first.Add(new FuzzyElement(0, 0.5));
            first.Add(new FuzzyElement(10, 1));

            FuzzySet second = new FuzzySet(new Universe(0, 10, 20));
            second.Add(new FuzzyElement(10, 1));
            second.Add(new FuzzyElement(20, 1));

            double actual = FuzzyDefuzzification.GetCenterOfSums(new[] { first, second }, ToDouble);

            Assert.Equal((0 * 0.5 + 10 * 1 + 10 * 1 + 20 * 1) / (0.5 + 1 + 1 + 1), actual, 10);
        }

        // Testuje, že MOM vrací průměr všech hodnot s maximálním stupněm příslušnosti.
        [Fact]
        public void GetMeanOfMaximaReturnsAverageOfMaxima()
        {
            FuzzySet set = CreateSetWithTwoMaxima();

            double actual = FuzzyDefuzzification.GetMeanOfMaxima(set, ToDouble);

            Assert.Equal(15, actual);
        }

        // Testuje, že FOM vrací nejmenší numerickou hodnotu z hodnot s maximem.
        [Fact]
        public void GetFirstOfMaximaReturnsSmallestMaximum()
        {
            FuzzySet set = CreateSetWithTwoMaxima();

            double actual = FuzzyDefuzzification.GetFirstOfMaxima(set, ToDouble);

            Assert.Equal(10, actual);
        }

        // Testuje, že LOM vrací největší numerickou hodnotu z hodnot s maximem.
        [Fact]
        public void GetLastOfMaximaReturnsLargestMaximum()
        {
            FuzzySet set = CreateSetWithTwoMaxima();

            double actual = FuzzyDefuzzification.GetLastOfMaxima(set, ToDouble);

            Assert.Equal(20, actual);
        }

        // Testuje, že defuzzifikace odmítá neplatné vstupy.
        [Fact]
        public void RejectsInvalidInputs()
        {
            FuzzySet set = CreateSetWithTwoMaxima();

            Assert.Throws<ArgumentNullException>(() => FuzzyDefuzzification.GetCenterOfGravity(null, ToDouble));
            Assert.Throws<ArgumentNullException>(() => FuzzyDefuzzification.GetCenterOfGravity(set, null));
            Assert.Throws<ArgumentNullException>(() => FuzzyDefuzzification.GetCenterOfSums(null, ToDouble));
            Assert.Throws<ArgumentException>(() => FuzzyDefuzzification.GetCenterOfSums(new List<FuzzySet>(), ToDouble));
            Assert.Throws<InvalidOperationException>(() => FuzzyDefuzzification.GetCenterOfGravity(new FuzzySet(), ToDouble));
            Assert.Throws<ArgumentException>(() => FuzzyDefuzzification.GetCenterOfGravity(set, _ => double.NaN));
        }

        private static FuzzySet CreateSetWithTwoMaxima()
        {
            FuzzySet set = new FuzzySet(new Universe(0, 10, 20, 30));
            set.Add(new FuzzyElement(0, 0.2));
            set.Add(new FuzzyElement(10, 1));
            set.Add(new FuzzyElement(20, 1));
            set.Add(new FuzzyElement(30, 0.4));

            return set;
        }
    }
}
