using System;
using System.Diagnostics;
using System.Globalization;

namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje jeden prvek fuzzy množiny včetně jeho stupně příslušnosti.
    /// </summary>
    [DebuggerDisplay("\\{ Value = {Value}, Grade = {Grade} \\}")]
    public class FuzzyElement : IEquatable<FuzzyElement>
    {
        /// <summary>
        /// Výchozí tolerance používaná při porovnávání stupňů příslušnosti.
        /// </summary>
        public const double GradeTolerance = 1e-10;

        /// <summary>
        /// Hodnota prvku v univerzu.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Stupeň příslušnosti prvku k fuzzy množině.
        /// </summary>
        public double Grade { get; private set; }

        /// <summary>
        /// Vytvoří prvek fuzzy množiny se zadanou hodnotou a stupněm příslušnosti.
        /// </summary>
        /// <param name="value">Hodnota prvku.</param>
        /// <param name="grade">Stupeň příslušnosti prvku k fuzzy množině.</param>
        public FuzzyElement(object value, double grade)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (double.IsNaN(grade) || grade < 0 || grade > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(grade),
                    grade,
                    "Stupeň příslušnosti musí být v intervalu [0, 1].");
            }

            Value = value;
            Grade = grade;
        }

        /// <summary>
        /// Určí, zda je zadaný objekt stejný fuzzy prvek.
        /// </summary>
        /// <param name="obj">Porovnávaný objekt.</param>
        /// <returns><c>true</c>, pokud má objekt stejnou hodnotu i stupeň příslušnosti.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FuzzyElement);
        }

        /// <summary>
        /// Určí, zda je zadaný fuzzy prvek stejný jako aktuální prvek.
        /// </summary>
        /// <param name="other">Porovnávaný fuzzy prvek.</param>
        /// <returns><c>true</c>, pokud mají prvky stejnou hodnotu i stupeň příslušnosti.</returns>
        public bool Equals(FuzzyElement other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return this == other;
        }

        /// <summary>
        /// Vrátí hash kód odvozený z hodnoty a stupně příslušnosti.
        /// </summary>
        /// <returns>Hash kód prvku.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Porovná dva fuzzy prvky podle hodnoty a stupně příslušnosti.
        /// </summary>
        /// <param name="left">Levý fuzzy prvek.</param>
        /// <param name="right">Pravý fuzzy prvek.</param>
        /// <returns><c>true</c>, pokud mají prvky stejnou hodnotu i stupeň příslušnosti.</returns>
        public static bool operator ==(FuzzyElement left, FuzzyElement right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Value.Equals(right.Value) && AreGradesEqual(left.Grade, right.Grade);
        }

        /// <summary>
        /// Porovná, zda se dva fuzzy prvky liší hodnotou nebo stupněm příslušnosti.
        /// </summary>
        /// <param name="left">Levý fuzzy prvek.</param>
        /// <param name="right">Pravý fuzzy prvek.</param>
        /// <returns><c>true</c>, pokud se prvky liší.</returns>
        public static bool operator !=(FuzzyElement left, FuzzyElement right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Vrátí textovou reprezentaci ve tvaru stupeň/hodnota.
        /// </summary>
        /// <returns>Textová reprezentace fuzzy prvku.</returns>
        public override string ToString()
        {
            return Grade.ToString(CultureInfo.InvariantCulture) + "/" + Value;
        }

        private static bool AreGradesEqual(double left, double right)
        {
            return Math.Abs(left - right) <= GradeTolerance;
        }
    }
}
