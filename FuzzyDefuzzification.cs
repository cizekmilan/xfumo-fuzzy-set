using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuzzy
{
    /// <summary>
    /// Poskytuje defuzzifikační metody pro konečné fuzzy množiny nad numericky interpretovatelným univerzem.
    /// </summary>
    public static class FuzzyDefuzzification
    {
        /// <summary>
        /// Vrátí metodu COG, tedy těžiště konečné fuzzy množiny.
        /// </summary>
        /// <param name="set">Defuzzifikovaná fuzzy množina.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Ostrá hodnota vypočtená jako vážený průměr hodnot podle stupňů příslušnosti.</returns>
        public static double GetCenterOfGravity(FuzzySet set, Func<object, double> valueSelector)
        {
            ValidateSetForDefuzzification(set, nameof(set));
            ValidateValueSelector(valueSelector);

            double numerator = 0;
            double denominator = 0;

            foreach (FuzzyElement element in set)
            {
                double value = GetNumericValue(element.Value, valueSelector);
                numerator += value * element.Grade;
                denominator += element.Grade;
            }

            return Divide(numerator, denominator);
        }

        /// <summary>
        /// Vrátí metodu COA, tedy těžiště plochy. Pro konečné fuzzy množiny odpovídá metodě COG.
        /// </summary>
        /// <param name="set">Defuzzifikovaná fuzzy množina.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Ostrá hodnota vypočtená jako vážený průměr hodnot podle stupňů příslušnosti.</returns>
        public static double GetCenterOfArea(FuzzySet set, Func<object, double> valueSelector)
        {
            return GetCenterOfGravity(set, valueSelector);
        }

        /// <summary>
        /// Vrátí metodu COS, tedy střed součtů dílčích výstupních fuzzy množin.
        /// </summary>
        /// <param name="sets">Dílčí fuzzy množiny, jejichž příspěvky se sčítají bez slévání překryvů.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Ostrá hodnota vypočtená jako vážený průměr přes všechny dílčí fuzzy množiny.</returns>
        public static double GetCenterOfSums(IEnumerable<FuzzySet> sets, Func<object, double> valueSelector)
        {
            if (sets == null)
            {
                throw new ArgumentNullException(nameof(sets));
            }

            ValidateValueSelector(valueSelector);

            double numerator = 0;
            double denominator = 0;
            bool hasAnySet = false;

            foreach (FuzzySet set in sets)
            {
                hasAnySet = true;
                ValidateSetForDefuzzification(set, nameof(sets));

                foreach (FuzzyElement element in set)
                {
                    double value = GetNumericValue(element.Value, valueSelector);
                    numerator += value * element.Grade;
                    denominator += element.Grade;
                }
            }

            if (!hasAnySet)
            {
                throw new ArgumentException("Defuzzifikace COS vyžaduje alespoň jednu fuzzy množinu.", nameof(sets));
            }

            return Divide(numerator, denominator);
        }

        /// <summary>
        /// Vrátí metodu MOM, tedy průměr hodnot s maximálním stupněm příslušnosti.
        /// </summary>
        /// <param name="set">Defuzzifikovaná fuzzy množina.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Průměr numerických hodnot všech maxim fuzzy množiny.</returns>
        public static double GetMeanOfMaxima(FuzzySet set, Func<object, double> valueSelector)
        {
            FuzzyElement[] maxima = GetMaxima(set);
            ValidateValueSelector(valueSelector);

            return maxima.Average(x => GetNumericValue(x.Value, valueSelector));
        }

        /// <summary>
        /// Vrátí metodu FOM, tedy první z maxim fuzzy množiny.
        /// </summary>
        /// <param name="set">Defuzzifikovaná fuzzy množina.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Nejmenší numerická hodnota z hodnot s maximálním stupněm příslušnosti.</returns>
        public static double GetFirstOfMaxima(FuzzySet set, Func<object, double> valueSelector)
        {
            FuzzyElement[] maxima = GetMaxima(set);
            ValidateValueSelector(valueSelector);

            return maxima.Min(x => GetNumericValue(x.Value, valueSelector));
        }

        /// <summary>
        /// Vrátí metodu LOM, tedy poslední z maxim fuzzy množiny.
        /// </summary>
        /// <param name="set">Defuzzifikovaná fuzzy množina.</param>
        /// <param name="valueSelector">Funkce převádějící hodnotu prvku na numerickou osu.</param>
        /// <returns>Největší numerická hodnota z hodnot s maximálním stupněm příslušnosti.</returns>
        public static double GetLastOfMaxima(FuzzySet set, Func<object, double> valueSelector)
        {
            FuzzyElement[] maxima = GetMaxima(set);
            ValidateValueSelector(valueSelector);

            return maxima.Max(x => GetNumericValue(x.Value, valueSelector));
        }

        private static FuzzyElement[] GetMaxima(FuzzySet set)
        {
            ValidateSetForDefuzzification(set, nameof(set));

            double height = set.Max(x => x.Grade);

            return set.Where(x => Math.Abs(x.Grade - height) <= FuzzyElement.GradeTolerance).ToArray();
        }

        private static void ValidateSetForDefuzzification(FuzzySet set, string paramName)
        {
            if (set == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (set.Count == 0)
            {
                throw new InvalidOperationException("Nelze defuzzifikovat prázdnou fuzzy množinu.");
            }
        }

        private static void ValidateValueSelector(Func<object, double> valueSelector)
        {
            if (valueSelector == null)
            {
                throw new ArgumentNullException(nameof(valueSelector));
            }
        }

        private static double GetNumericValue(object value, Func<object, double> valueSelector)
        {
            double numericValue = valueSelector(value);

            if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
            {
                throw new ArgumentException("Numerická hodnota prvku musí být konečné číslo.", nameof(valueSelector));
            }

            return numericValue;
        }

        private static double Divide(double numerator, double denominator)
        {
            if (denominator == 0)
            {
                throw new InvalidOperationException("Součet stupňů příslušnosti musí být větší než 0.");
            }

            return numerator / denominator;
        }
    }
}
