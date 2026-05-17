using System;
using System.Diagnostics;
using System.Globalization;

namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje jeden prvek binární fuzzy relace včetně jeho stupně příslušnosti.
    /// </summary>
    [DebuggerDisplay("\\{ LeftValue = {LeftValue}, RightValue = {RightValue}, Grade = {Grade} \\}")]
    public class FuzzyRelationElement : IEquatable<FuzzyRelationElement>
    {
        /// <summary>
        /// Hodnota z levého univerza relace.
        /// </summary>
        public object LeftValue { get; private set; }

        /// <summary>
        /// Hodnota z pravého univerza relace.
        /// </summary>
        public object RightValue { get; private set; }

        /// <summary>
        /// Stupeň příslušnosti dvojice hodnot k fuzzy relaci.
        /// </summary>
        public double Grade { get; private set; }

        /// <summary>
        /// Vytvoří prvek binární fuzzy relace se zadanými hodnotami a stupněm příslušnosti.
        /// </summary>
        /// <param name="leftValue">Hodnota z levého univerza.</param>
        /// <param name="rightValue">Hodnota z pravého univerza.</param>
        /// <param name="grade">Stupeň příslušnosti dvojice hodnot k relaci.</param>
        public FuzzyRelationElement(object leftValue, object rightValue, double grade)
        {
            if (leftValue == null)
            {
                throw new ArgumentNullException(nameof(leftValue));
            }

            if (rightValue == null)
            {
                throw new ArgumentNullException(nameof(rightValue));
            }

            if (double.IsNaN(grade) || grade < 0 || grade > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(grade),
                    grade,
                    "Stupeň příslušnosti musí být v intervalu [0, 1].");
            }

            LeftValue = leftValue;
            RightValue = rightValue;
            Grade = grade;
        }

        /// <summary>
        /// Určí, zda je zadaný objekt stejný prvek fuzzy relace.
        /// </summary>
        /// <param name="obj">Porovnávaný objekt.</param>
        /// <returns><c>true</c>, pokud má objekt stejnou dvojici hodnot i stupeň příslušnosti.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FuzzyRelationElement);
        }

        /// <summary>
        /// Určí, zda je zadaný prvek fuzzy relace stejný jako aktuální prvek.
        /// </summary>
        /// <param name="other">Porovnávaný prvek fuzzy relace.</param>
        /// <returns><c>true</c>, pokud mají prvky stejnou dvojici hodnot i stupeň příslušnosti.</returns>
        public bool Equals(FuzzyRelationElement other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return this == other;
        }

        /// <summary>
        /// Vrátí hash kód odvozený z dvojice hodnot.
        /// </summary>
        /// <returns>Hash kód prvku relace.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (LeftValue.GetHashCode() * 397) ^ RightValue.GetHashCode();
            }
        }

        /// <summary>
        /// Porovná dva prvky fuzzy relace podle dvojice hodnot a stupně příslušnosti.
        /// </summary>
        /// <param name="left">Levý prvek fuzzy relace.</param>
        /// <param name="right">Pravý prvek fuzzy relace.</param>
        /// <returns><c>true</c>, pokud mají prvky stejnou dvojici hodnot i stupeň příslušnosti.</returns>
        public static bool operator ==(FuzzyRelationElement left, FuzzyRelationElement right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.LeftValue.Equals(right.LeftValue)
                && left.RightValue.Equals(right.RightValue)
                && AreGradesEqual(left.Grade, right.Grade);
        }

        /// <summary>
        /// Porovná, zda se dva prvky fuzzy relace liší dvojicí hodnot nebo stupněm příslušnosti.
        /// </summary>
        /// <param name="left">Levý prvek fuzzy relace.</param>
        /// <param name="right">Pravý prvek fuzzy relace.</param>
        /// <returns><c>true</c>, pokud se prvky liší.</returns>
        public static bool operator !=(FuzzyRelationElement left, FuzzyRelationElement right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Vrátí textovou reprezentaci ve tvaru stupeň/(levá hodnota, pravá hodnota).
        /// </summary>
        /// <returns>Textová reprezentace prvku fuzzy relace.</returns>
        public override string ToString()
        {
            return Grade.ToString(CultureInfo.InvariantCulture) + "/(" + LeftValue + ", " + RightValue + ")";
        }

        private static bool AreGradesEqual(double left, double right)
        {
            return Math.Abs(left - right) <= FuzzyElement.GradeTolerance;
        }
    }
}
