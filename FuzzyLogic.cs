using System;
using System.Linq;

namespace Fuzzy
{
    /// <summary>
    /// Poskytuje základní operace nad fuzzy množinami.
    /// </summary>
    public static class FuzzyLogic
    {
        /// <summary>
        /// Vrátí jádro fuzzy množiny, tedy prvky se stupněm příslušnosti rovným 1.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Klasická množina prvků, které do fuzzy množiny patří plně.</returns>
        public static CrispSet GetKernel(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            CrispSet result = new CrispSet();

            foreach (FuzzyElement element in set)
            {
                if (element.Grade == 1)
                {
                    result.Add(element.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí nosič fuzzy množiny, tedy prvky s kladným stupněm příslušnosti.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Klasická množina prvků, které mají nenulovou příslušnost.</returns>
        public static CrispSet GetSupport(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            CrispSet result = new CrispSet();

            foreach (FuzzyElement element in set)
            {
                if (element.Grade > 0)
                {
                    result.Add(element.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí alfa řez fuzzy množiny pro zadanou hodnotu alfa.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <param name="alpha">Hraniční stupeň příslušnosti.</param>
        /// <returns>Klasická množina prvků, jejichž stupeň příslušnosti je větší nebo roven hodnotě alfa.</returns>
        public static CrispSet GetAlphaCut(FuzzySet set, double alpha)
        {
            ValidateSet(set, nameof(set));
            ValidateAlpha(alpha);

            CrispSet result = new CrispSet();

            foreach (FuzzyElement element in set)
            {
                if (element.Grade >= alpha)
                {
                    result.Add(element.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí silný alfa řez fuzzy množiny pro zadanou hodnotu alfa.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <param name="alpha">Hraniční stupeň příslušnosti.</param>
        /// <returns>Klasická množina prvků, jejichž stupeň příslušnosti je větší než hodnota alfa.</returns>
        public static CrispSet GetStrongAlphaCut(FuzzySet set, double alpha)
        {
            ValidateSet(set, nameof(set));
            ValidateAlpha(alpha);

            CrispSet result = new CrispSet();

            foreach (FuzzyElement element in set)
            {
                if (element.Grade > alpha)
                {
                    result.Add(element.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí hranici fuzzy množiny, tedy prvky s příslušností mezi 0 a 1.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Klasická množina částečně příslušných prvků.</returns>
        public static CrispSet GetBoundary(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            CrispSet result = new CrispSet();

            foreach (FuzzyElement element in set)
            {
                if (element.Grade > 0 && element.Grade < 1)
                {
                    result.Add(element.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí výšku fuzzy množiny, tedy nejvyšší stupeň příslušnosti jejích prvků.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Maximální stupeň příslušnosti; pro prázdnou množinu vrací 0.</returns>
        public static double GetHeight(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            if (set.Count == 0)
            {
                return 0;
            }

            return set.Max(x => x.Grade);
        }

        /// <summary>
        /// Určí, zda je fuzzy množina normální.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns><c>true</c>, pokud má množina alespoň jeden prvek se stupněm příslušnosti 1.</returns>
        public static bool IsNormal(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            return GetHeight(set) == 1;
        }

        /// <summary>
        /// Vrátí kardinalitu konečné fuzzy množiny jako součet stupňů příslušnosti všech prvků.
        /// </summary>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Součet stupňů příslušnosti prvků.</returns>
        public static double GetCardinality(FuzzySet set)
        {
            ValidateSet(set, nameof(set));

            return set.Sum(x => x.Grade);
        }

        /// <summary>
        /// Určí, zda je jedna fuzzy množina podmnožinou druhé fuzzy množiny.
        /// </summary>
        /// <param name="subset">Kandidát na podmnožinu.</param>
        /// <param name="superset">Kandidát na nadmnožinu.</param>
        /// <returns><c>true</c>, pokud má každý prvek první množiny nejvýše takový stupeň příslušnosti jako v druhé množině.</returns>
        public static bool IsSubsetOf(FuzzySet subset, FuzzySet superset)
        {
            ValidateBinaryOperation(subset, superset);

            foreach (FuzzyElement element in subset)
            {
                if (element.Grade > superset.GetMembershipGrade(element.Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Vrátí standardní průnik fuzzy množin počítaný pomocí minima stupňů příslušnosti.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina obsahující průnik vstupních množin.</returns>
        public static FuzzySet GetStandardIntersection(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetIntersection(left, right, Math.Min);
        }

        /// <summary>
        /// Vrátí standardní sjednocení fuzzy množin počítané pomocí maxima stupňů příslušnosti.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina obsahující sjednocení vstupních množin.</returns>
        public static FuzzySet GetStandardUnion(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetUnion(left, right, Math.Max);
        }

        /// <summary>
        /// Vrátí standardní průnik fuzzy množin počítaný pomocí minima stupňů příslušnosti.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina obsahující průnik vstupních množin.</returns>
        [Obsolete("Použijte GetStandardIntersection.")]
        public static FuzzySet GetRegularIntersection(FuzzySet left, FuzzySet right)
        {
            return GetStandardIntersection(left, right);
        }

        /// <summary>
        /// Vrátí standardní sjednocení fuzzy množin počítané pomocí maxima stupňů příslušnosti.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina obsahující sjednocení vstupních množin.</returns>
        [Obsolete("Použijte GetStandardUnion.")]
        public static FuzzySet GetRegularUnion(FuzzySet left, FuzzySet right)
        {
            return GetStandardUnion(left, right);
        }

        /// <summary>
        /// Vrátí Lukasiewiczův průnik fuzzy množin.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina vzniklá aplikací Lukasiewiczovy konjunkce na společné prvky.</returns>
        public static FuzzySet GetLukasiewiczIntersection(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetIntersection(left, right, LukasiewiczConjunction);
        }

        /// <summary>
        /// Vrátí Lukasiewiczovo sjednocení fuzzy množin.
        /// </summary>
        /// <param name="left">První fuzzy množina.</param>
        /// <param name="right">Druhá fuzzy množina.</param>
        /// <returns>Fuzzy množina vzniklá aplikací Lukasiewiczovy disjunkce.</returns>
        public static FuzzySet GetLukasiewiczUnion(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetUnion(left, right, LukasiewiczDisjunction);
        }

        /// <summary>
        /// Vrátí Gödelovo reziduum pro dvě fuzzy množiny.
        /// </summary>
        /// <param name="left">Fuzzy množina na levé straně implikace.</param>
        /// <param name="right">Fuzzy množina na pravé straně implikace.</param>
        /// <returns>Fuzzy množina se stupni příslušnosti vypočtenými pomocí Gödelova rezidua.</returns>
        public static FuzzySet GetGodelResiduum(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetResiduum(left, right, GodelResiduum);
        }

        /// <summary>
        /// Vrátí Lukasiewiczovo reziduum pro dvě fuzzy množiny.
        /// </summary>
        /// <param name="left">Fuzzy množina na levé straně implikace.</param>
        /// <param name="right">Fuzzy množina na pravé straně implikace.</param>
        /// <returns>Fuzzy množina se stupni příslušnosti vypočtenými pomocí Lukasiewiczova rezidua.</returns>
        public static FuzzySet GetLukasiewiczResiduum(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            return GetResiduum(left, right, LukasiewiczResiduum);
        }

        /// <summary>
        /// Vrátí doplněk fuzzy množiny vzhledem k zadanému univerzu.
        /// </summary>
        /// <param name="universe">Univerzum, vůči kterému se doplněk počítá.</param>
        /// <param name="set">Fuzzy množina.</param>
        /// <returns>Fuzzy množina, kde je stupeň příslušnosti každého prvku roven 1 minus původní stupeň.</returns>
        public static FuzzySet GetComplement(Universe universe, FuzzySet set)
        {
            ValidateUniverse(universe, nameof(universe));
            ValidateSet(set, nameof(set));
            ValidateUniverseCompatibility(universe, set.Universe, nameof(universe), nameof(set));

            FuzzySet result = new FuzzySet(universe);

            foreach (object value in universe)
            {
                FuzzyElement element = set.FirstOrDefault(x => x.Value.Equals(value));
                double grade = element == null ? 1 : 1 - element.Grade;
                result.Add(new FuzzyElement(value, grade));
            }

            return result;
        }

        /// <summary>
        /// Vrátí rozdíl dvou fuzzy množin jako průnik první množiny s doplňkem druhé.
        /// </summary>
        /// <param name="left">Fuzzy množina, ze které se odečítá.</param>
        /// <param name="right">Fuzzy množina, jejíž doplněk se použije pro výpočet rozdílu.</param>
        /// <returns>Fuzzy množina reprezentující rozdíl vstupních množin.</returns>
        public static FuzzySet GetDifference(FuzzySet left, FuzzySet right)
        {
            ValidateBinaryOperation(left, right);

            FuzzySet result = new FuzzySet(GetResultUniverse(left, right));

            foreach (FuzzyElement leftElement in left)
            {
                double rightComplementGrade = 1 - right.GetMembershipGrade(leftElement.Value);
                double grade = Math.Min(leftElement.Grade, rightComplementGrade);

                result.Add(new FuzzyElement(leftElement.Value, grade));
            }

            return result;
        }

        private static double LukasiewiczConjunction(double leftGrade, double rightGrade)
        {
            return Math.Max(0, leftGrade + rightGrade - 1);
        }

        private static double LukasiewiczDisjunction(double leftGrade, double rightGrade)
        {
            return Math.Min(1, leftGrade + rightGrade);
        }

        private static double LukasiewiczResiduum(double leftGrade, double rightGrade)
        {
            return Math.Min(1, 1 - leftGrade + rightGrade);
        }

        private static double GodelResiduum(double leftGrade, double rightGrade)
        {
            return leftGrade <= rightGrade ? 1 : rightGrade;
        }

        private static FuzzySet GetIntersection(
            FuzzySet left,
            FuzzySet right,
            Func<double, double, double> calculateGrade)
        {
            FuzzySet result = new FuzzySet(GetResultUniverse(left, right));

            foreach (FuzzyElement leftElement in left)
            {
                FuzzyElement rightElement = right.FirstOrDefault(x => x.Value.Equals(leftElement.Value));

                if (rightElement != null)
                {
                    result.Add(new FuzzyElement(
                        leftElement.Value,
                        calculateGrade(leftElement.Grade, rightElement.Grade)));
                }
            }

            return result;
        }

        private static FuzzySet GetUnion(
            FuzzySet left,
            FuzzySet right,
            Func<double, double, double> calculateGrade)
        {
            FuzzySet result = new FuzzySet(GetResultUniverse(left, right));

            foreach (FuzzyElement leftElement in left)
            {
                FuzzyElement rightElement = right.FirstOrDefault(x => x.Value.Equals(leftElement.Value));
                double grade = rightElement == null
                    ? leftElement.Grade
                    : calculateGrade(leftElement.Grade, rightElement.Grade);

                result.Add(new FuzzyElement(leftElement.Value, grade));
            }

            foreach (FuzzyElement rightElement in right)
            {
                bool alreadyAdded = left.Any(x => x.Value.Equals(rightElement.Value));

                if (!alreadyAdded)
                {
                    result.Add(rightElement);
                }
            }

            return result;
        }

        private static FuzzySet GetResiduum(
            FuzzySet left,
            FuzzySet right,
            Func<double, double, double> calculateResiduum)
        {
            CrispSet universe = GetOperationUniverse(left, right);
            FuzzySet result = new FuzzySet(GetResultUniverse(left, right));

            foreach (object value in universe)
            {
                double leftGrade = left.GetMembershipGrade(value);
                double rightGrade = right.GetMembershipGrade(value);
                double grade = calculateResiduum(leftGrade, rightGrade);

                result.Add(new FuzzyElement(value, grade));
            }

            return result;
        }

        private static void ValidateSet(FuzzySet set, string paramName)
        {
            if (set == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateUniverse(Universe universe, string paramName)
        {
            if (universe == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateAlpha(double alpha)
        {
            if (double.IsNaN(alpha) || alpha < 0 || alpha > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(alpha),
                    alpha,
                    "Hodnota alfa musí být v intervalu [0, 1].");
            }
        }

        private static void ValidateBinaryOperation(FuzzySet left, FuzzySet right)
        {
            ValidateSet(left, nameof(left));
            ValidateSet(right, nameof(right));
            ValidateUniverseCompatibility(left.Universe, right.Universe, nameof(left), nameof(right));
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
                    "Fuzzy množiny musí být definované nad stejným univerzem.",
                    leftParamName + ", " + rightParamName);
            }
        }

        private static Universe GetResultUniverse(FuzzySet left, FuzzySet right)
        {
            return left.Universe ?? right.Universe;
        }

        private static CrispSet GetOperationUniverse(FuzzySet left, FuzzySet right)
        {
            if (left.Universe != null)
            {
                return left.Universe;
            }

            if (right.Universe != null)
            {
                return right.Universe;
            }

            return new CrispSet(left.Select(x => x.Value).Concat(right.Select(x => x.Value)).ToArray());
        }
    }
}
