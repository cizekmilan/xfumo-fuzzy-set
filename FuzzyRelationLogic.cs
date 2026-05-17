using System;
using System.Linq;

namespace Fuzzy
{
    /// <summary>
    /// Poskytuje základní operace nad binárními fuzzy relacemi.
    /// </summary>
    public static class FuzzyRelationLogic
    {
        /// <summary>
        /// Vrátí kartézský součin dvou fuzzy množin počítaný pomocí minima stupňů příslušnosti.
        /// </summary>
        /// <param name="left">Fuzzy množina tvořící levé univerzum relace.</param>
        /// <param name="right">Fuzzy množina tvořící pravé univerzum relace.</param>
        /// <returns>Binární fuzzy relace reprezentující kartézský součin vstupních fuzzy množin.</returns>
        public static FuzzyRelation GetCartesianProduct(FuzzySet left, FuzzySet right)
        {
            ValidateSet(left, nameof(left));
            ValidateSet(right, nameof(right));

            Universe leftUniverse = GetSetUniverse(left);
            Universe rightUniverse = GetSetUniverse(right);
            FuzzyRelation result = new FuzzyRelation(leftUniverse, rightUniverse);

            foreach (object leftValue in leftUniverse)
            {
                foreach (object rightValue in rightUniverse)
                {
                    double grade = Math.Min(
                        left.GetMembershipGrade(leftValue),
                        right.GetMembershipGrade(rightValue));

                    result.Add(new FuzzyRelationElement(leftValue, rightValue, grade));
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí levou projekci fuzzy relace.
        /// </summary>
        /// <param name="relation">Fuzzy relace.</param>
        /// <returns>Fuzzy množina nad levým univerzem, kde každý stupeň odpovídá maximu přes pravé hodnoty.</returns>
        public static FuzzySet GetLeftProjection(FuzzyRelation relation)
        {
            ValidateRelation(relation, nameof(relation));

            Universe leftUniverse = GetLeftUniverse(relation);
            CrispSet rightUniverse = GetRightOperationUniverse(relation);
            FuzzySet result = new FuzzySet(leftUniverse);

            foreach (object leftValue in leftUniverse)
            {
                double grade = GetMaximumGrade(rightUniverse, rightValue => relation.GetMembershipGrade(leftValue, rightValue));
                result.Add(new FuzzyElement(leftValue, grade));
            }

            return result;
        }

        /// <summary>
        /// Vrátí pravou projekci fuzzy relace.
        /// </summary>
        /// <param name="relation">Fuzzy relace.</param>
        /// <returns>Fuzzy množina nad pravým univerzem, kde každý stupeň odpovídá maximu přes levé hodnoty.</returns>
        public static FuzzySet GetRightProjection(FuzzyRelation relation)
        {
            ValidateRelation(relation, nameof(relation));

            CrispSet leftUniverse = GetLeftOperationUniverse(relation);
            Universe rightUniverse = GetRightUniverse(relation);
            FuzzySet result = new FuzzySet(rightUniverse);

            foreach (object rightValue in rightUniverse)
            {
                double grade = GetMaximumGrade(leftUniverse, leftValue => relation.GetMembershipGrade(leftValue, rightValue));
                result.Add(new FuzzyElement(rightValue, grade));
            }

            return result;
        }

        /// <summary>
        /// Vrátí obraz fuzzy množiny ve fuzzy relaci pomocí max-min kompozice.
        /// </summary>
        /// <param name="set">Fuzzy množina nad levým univerzem relace.</param>
        /// <param name="relation">Fuzzy relace, přes kterou se obraz počítá.</param>
        /// <returns>Fuzzy množina nad pravým univerzem relace.</returns>
        public static FuzzySet GetImage(FuzzySet set, FuzzyRelation relation)
        {
            ValidateSet(set, nameof(set));
            ValidateRelation(relation, nameof(relation));
            ValidateUniverseCompatibility(set.Universe, relation.LeftUniverse, nameof(set), nameof(relation));

            CrispSet leftUniverse = GetImageLeftUniverse(set, relation);
            Universe rightUniverse = GetRightUniverse(relation);
            FuzzySet result = new FuzzySet(rightUniverse);

            foreach (object rightValue in rightUniverse)
            {
                double grade = GetMaximumGrade(
                    leftUniverse,
                    leftValue => Math.Min(set.GetMembershipGrade(leftValue), relation.GetMembershipGrade(leftValue, rightValue)));

                result.Add(new FuzzyElement(rightValue, grade));
            }

            return result;
        }

        /// <summary>
        /// Vrátí max-min kompozici dvou fuzzy relací.
        /// </summary>
        /// <param name="left">První fuzzy relace z levého do prostředního univerza.</param>
        /// <param name="right">Druhá fuzzy relace z prostředního do pravého univerza.</param>
        /// <returns>Fuzzy relace z levého do pravého univerza.</returns>
        public static FuzzyRelation GetComposition(FuzzyRelation left, FuzzyRelation right)
        {
            ValidateRelation(left, nameof(left));
            ValidateRelation(right, nameof(right));
            ValidateUniverseCompatibility(left.RightUniverse, right.LeftUniverse, nameof(left), nameof(right));

            Universe leftUniverse = GetLeftUniverse(left);
            CrispSet middleUniverse = GetCompositionMiddleUniverse(left, right);
            Universe rightUniverse = GetRightUniverse(right);
            FuzzyRelation result = new FuzzyRelation(leftUniverse, rightUniverse);

            foreach (object leftValue in leftUniverse)
            {
                foreach (object rightValue in rightUniverse)
                {
                    double grade = GetMaximumGrade(
                        middleUniverse,
                        middleValue => Math.Min(
                            left.GetMembershipGrade(leftValue, middleValue),
                            right.GetMembershipGrade(middleValue, rightValue)));

                    result.Add(new FuzzyRelationElement(leftValue, rightValue, grade));
                }
            }

            return result;
        }

        private static double GetMaximumGrade(CrispSet values, Func<object, double> getGrade)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            return values.Max(getGrade);
        }

        private static void ValidateSet(FuzzySet set, string paramName)
        {
            if (set == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateRelation(FuzzyRelation relation, string paramName)
        {
            if (relation == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateUniverseCompatibility(
            Universe left,
            Universe right,
            string leftParamName,
            string rightParamName)
        {
            if (left != null && right != null && !left.Equals(right))
            {
                throw new ArgumentException(
                    "Fuzzy množiny nebo relace musí být definované nad kompatibilními univerzy.",
                    leftParamName + ", " + rightParamName);
            }
        }

        private static Universe GetSetUniverse(FuzzySet set)
        {
            return set.Universe ?? new Universe(set.Select(x => x.Value).ToArray());
        }

        private static Universe GetLeftUniverse(FuzzyRelation relation)
        {
            return relation.LeftUniverse ?? new Universe(relation.Select(x => x.LeftValue).ToArray());
        }

        private static Universe GetRightUniverse(FuzzyRelation relation)
        {
            return relation.RightUniverse ?? new Universe(relation.Select(x => x.RightValue).ToArray());
        }

        private static CrispSet GetLeftOperationUniverse(FuzzyRelation relation)
        {
            return relation.LeftUniverse ?? new CrispSet(relation.Select(x => x.LeftValue).ToArray());
        }

        private static CrispSet GetRightOperationUniverse(FuzzyRelation relation)
        {
            return relation.RightUniverse ?? new CrispSet(relation.Select(x => x.RightValue).ToArray());
        }

        private static CrispSet GetImageLeftUniverse(FuzzySet set, FuzzyRelation relation)
        {
            if (set.Universe != null)
            {
                return set.Universe;
            }

            if (relation.LeftUniverse != null)
            {
                return relation.LeftUniverse;
            }

            return new CrispSet(set.Select(x => x.Value).Concat(relation.Select(x => x.LeftValue)).ToArray());
        }

        private static CrispSet GetCompositionMiddleUniverse(FuzzyRelation left, FuzzyRelation right)
        {
            if (left.RightUniverse != null)
            {
                return left.RightUniverse;
            }

            if (right.LeftUniverse != null)
            {
                return right.LeftUniverse;
            }

            return new CrispSet(left.Select(x => x.RightValue).Concat(right.Select(x => x.LeftValue)).ToArray());
        }
    }
}
