using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje konečnou binární fuzzy relaci jako řízenou kolekci dvojic hodnot se stupni příslušnosti.
    /// </summary>
    public class FuzzyRelation : IReadOnlyCollection<FuzzyRelationElement>, IEquatable<FuzzyRelation>
    {
        private readonly List<FuzzyRelationElement> elements = new List<FuzzyRelationElement>();

        /// <summary>
        /// Levé univerzum hodnot, vůči kterému je fuzzy relace interpretována.
        /// </summary>
        public Universe LeftUniverse { get; set; }

        /// <summary>
        /// Pravé univerzum hodnot, vůči kterému je fuzzy relace interpretována.
        /// </summary>
        public Universe RightUniverse { get; set; }

        /// <summary>
        /// Počet explicitně uložených prvků fuzzy relace.
        /// </summary>
        public int Count
        {
            get
            {
                return elements.Count;
            }
        }

        /// <summary>
        /// Vytvoří prázdnou fuzzy relaci bez přiřazených univerz.
        /// </summary>
        public FuzzyRelation()
        {
        }

        /// <summary>
        /// Vytvoří prázdnou fuzzy relaci nad zadanými univerzy.
        /// </summary>
        /// <param name="leftUniverse">Levé univerzum hodnot relace.</param>
        /// <param name="rightUniverse">Pravé univerzum hodnot relace.</param>
        public FuzzyRelation(Universe leftUniverse, Universe rightUniverse)
        {
            LeftUniverse = leftUniverse;
            RightUniverse = rightUniverse;
        }

        /// <summary>
        /// Přidá prvek do fuzzy relace, pokud má kladný stupeň příslušnosti.
        /// </summary>
        /// <param name="item">Přidávaný prvek fuzzy relace.</param>
        /// <returns><c>true</c>, pokud byl prvek přidán; <c>false</c>, pokud má nulový stupeň nebo už v relaci existuje stejný prvek.</returns>
        /// <exception cref="ArgumentNullException">Vyvolá se, pokud je prvek <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Vyvolá se, pokud už relace obsahuje stejnou dvojici hodnot s jiným stupněm příslušnosti.</exception>
        public bool Add(FuzzyRelationElement item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Grade == 0)
            {
                return false;
            }

            FuzzyRelationElement existingElement = elements.FirstOrDefault(
                x => x.LeftValue.Equals(item.LeftValue) && x.RightValue.Equals(item.RightValue));

            if (existingElement != null)
            {
                if (existingElement.Equals(item))
                {
                    return false;
                }

                throw new ArgumentException(
                    "Fuzzy relace už obsahuje stejnou dvojici hodnot s jiným stupněm příslušnosti.",
                    nameof(item));
            }

            elements.Add(item);
            return true;
        }

        /// <summary>
        /// Určí, zda fuzzy relace obsahuje zadaný prvek.
        /// </summary>
        /// <param name="item">Hledaný prvek fuzzy relace.</param>
        /// <returns><c>true</c>, pokud relace obsahuje prvek se stejnou dvojicí hodnot i stupněm příslušnosti.</returns>
        public bool Contains(FuzzyRelationElement item)
        {
            return elements.Contains(item);
        }

        /// <summary>
        /// Vrátí stupeň příslušnosti zadané dvojice hodnot k fuzzy relaci.
        /// </summary>
        /// <param name="leftValue">Hodnota z levého univerza.</param>
        /// <param name="rightValue">Hodnota z pravého univerza.</param>
        /// <returns>Stupeň příslušnosti, nebo 0, pokud dvojice hodnot v relaci není uvedena.</returns>
        public double GetMembershipGrade(object leftValue, object rightValue)
        {
            if (leftValue == null)
            {
                throw new ArgumentNullException(nameof(leftValue));
            }

            if (rightValue == null)
            {
                throw new ArgumentNullException(nameof(rightValue));
            }

            FuzzyRelationElement element = elements.FirstOrDefault(
                x => x.LeftValue.Equals(leftValue) && x.RightValue.Equals(rightValue));

            return element == null ? 0 : element.Grade;
        }

        /// <summary>
        /// Určí, zda je zadaný objekt stejná fuzzy relace.
        /// </summary>
        /// <param name="obj">Porovnávaný objekt.</param>
        /// <returns><c>true</c>, pokud objekt obsahuje stejné prvky fuzzy relace.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FuzzyRelation);
        }

        /// <summary>
        /// Určí, zda fuzzy relace obsahuje stejné prvky jako jiná relace bez ohledu na pořadí.
        /// </summary>
        /// <param name="other">Porovnávaná fuzzy relace.</param>
        /// <returns><c>true</c>, pokud jsou relace stejné.</returns>
        public bool Equals(FuzzyRelation other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return Count == other.Count && elements.All(other.Contains);
        }

        /// <summary>
        /// Vrátí hash kód odvozený z prvků fuzzy relace.
        /// </summary>
        /// <returns>Hash kód relace.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (FuzzyRelationElement element in elements)
                {
                    hash ^= element.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Vrátí enumerátor prvků fuzzy relace.
        /// </summary>
        /// <returns>Enumerátor prvků fuzzy relace.</returns>
        public IEnumerator<FuzzyRelationElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        /// <summary>
        /// Vrátí textovou reprezentaci fuzzy relace.
        /// </summary>
        /// <returns>Textová reprezentace relace.</returns>
        public override string ToString()
        {
            return string.Join(", ", elements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
