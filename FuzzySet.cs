using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje konečnou fuzzy množinu jako řízenou kolekci prvků se stupni příslušnosti.
    /// </summary>
    public class FuzzySet : IReadOnlyCollection<FuzzyElement>, IEquatable<FuzzySet>
    {
        private readonly List<FuzzyElement> elements = new List<FuzzyElement>();

        /// <summary>
        /// Univerzum hodnot, vůči kterému je fuzzy množina interpretována.
        /// </summary>
        public Universe Universe { get; set; }

        /// <summary>
        /// Počet explicitně uložených prvků fuzzy množiny.
        /// </summary>
        public int Count
        {
            get
            {
                return elements.Count;
            }
        }

        /// <summary>
        /// Vytvoří prázdnou fuzzy množinu bez přiřazeného univerza.
        /// </summary>
        public FuzzySet()
        {
        }

        /// <summary>
        /// Vytvoří prázdnou fuzzy množinu nad zadaným univerzem.
        /// </summary>
        /// <param name="universe">Univerzum hodnot fuzzy množiny.</param>
        public FuzzySet(Universe universe)
        {
            Universe = universe;
        }

        /// <summary>
        /// Přidá prvek do fuzzy množiny, pokud má kladný stupeň příslušnosti.
        /// </summary>
        /// <param name="item">Přidávaný fuzzy prvek.</param>
        /// <returns><c>true</c>, pokud byl prvek přidán; <c>false</c>, pokud má nulový stupeň nebo už v množině existuje stejný prvek.</returns>
        /// <exception cref="ArgumentNullException">Vyvolá se, pokud je prvek <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Vyvolá se, pokud už množina obsahuje stejnou hodnotu s jiným stupněm příslušnosti.</exception>
        public bool Add(FuzzyElement item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Grade == 0)
            {
                return false;
            }

            FuzzyElement existingElement = elements.FirstOrDefault(x => x.Value.Equals(item.Value));

            if (existingElement != null)
            {
                if (existingElement.Equals(item))
                {
                    return false;
                }

                throw new ArgumentException(
                    "Fuzzy množina už obsahuje stejnou hodnotu s jiným stupněm příslušnosti.",
                    nameof(item));
            }

            elements.Add(item);
            return true;
        }

        /// <summary>
        /// Určí, zda fuzzy množina obsahuje zadaný fuzzy prvek.
        /// </summary>
        /// <param name="item">Hledaný fuzzy prvek.</param>
        /// <returns><c>true</c>, pokud množina obsahuje prvek se stejnou hodnotou i stupněm příslušnosti.</returns>
        public bool Contains(FuzzyElement item)
        {
            return elements.Contains(item);
        }

        /// <summary>
        /// Vrátí stupeň příslušnosti zadané hodnoty k fuzzy množině.
        /// </summary>
        /// <param name="value">Hodnota prvku.</param>
        /// <returns>Stupeň příslušnosti, nebo 0, pokud hodnota v množině není uvedena.</returns>
        public double GetMembershipGrade(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            FuzzyElement element = elements.FirstOrDefault(x => x.Value.Equals(value));
            return element == null ? 0 : element.Grade;
        }

        /// <summary>
        /// Určí, zda je zadaný objekt stejná fuzzy množina.
        /// </summary>
        /// <param name="obj">Porovnávaný objekt.</param>
        /// <returns><c>true</c>, pokud objekt obsahuje stejné fuzzy prvky.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FuzzySet);
        }

        /// <summary>
        /// Určí, zda fuzzy množina obsahuje stejné fuzzy prvky jako jiná množina bez ohledu na pořadí.
        /// </summary>
        /// <param name="other">Porovnávaná fuzzy množina.</param>
        /// <returns><c>true</c>, pokud jsou množiny stejné.</returns>
        public bool Equals(FuzzySet other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return Count == other.Count && elements.All(other.Contains);
        }

        /// <summary>
        /// Vrátí hash kód odvozený z fuzzy prvků v množině.
        /// </summary>
        /// <returns>Hash kód množiny.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (FuzzyElement element in elements)
                {
                    hash ^= element.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Vrátí enumerátor prvků fuzzy množiny.
        /// </summary>
        /// <returns>Enumerátor fuzzy prvků.</returns>
        public IEnumerator<FuzzyElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        /// <summary>
        /// Vrátí textovou reprezentaci fuzzy množiny.
        /// </summary>
        /// <returns>Textová reprezentace množiny.</returns>
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
