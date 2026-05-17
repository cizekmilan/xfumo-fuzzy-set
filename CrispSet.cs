using System;
using System.Collections;
using System.Collections.Generic;

namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje klasickou množinu bez stupňů příslušnosti.
    /// </summary>
    public class CrispSet : IReadOnlyCollection<object>, IEquatable<CrispSet>
    {
        private readonly HashSet<object> items = new HashSet<object>();

        /// <summary>
        /// Počet prvků v množině.
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        /// Vytvoří klasickou množinu z předaných prvků.
        /// </summary>
        /// <param name="items">Prvky množiny.</param>
        public CrispSet(params object[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (object item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Přidá prvek do množiny.
        /// </summary>
        /// <param name="item">Přidávaný prvek.</param>
        /// <returns><c>true</c>, pokud byl prvek přidán; <c>false</c>, pokud už v množině byl.</returns>
        /// <exception cref="ArgumentNullException">Vyvolá se, pokud je prvek <c>null</c>.</exception>
        public bool Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.Add(item);
        }

        /// <summary>
        /// Určí, zda množina obsahuje zadaný prvek.
        /// </summary>
        /// <param name="item">Hledaný prvek.</param>
        /// <returns><c>true</c>, pokud množina prvek obsahuje.</returns>
        public bool Contains(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.Contains(item);
        }

        /// <summary>
        /// Určí, zda množina obsahuje stejné prvky jako jiná množina.
        /// </summary>
        /// <param name="other">Porovnávaná množina.</param>
        /// <returns><c>true</c>, pokud množiny obsahují stejné prvky.</returns>
        public bool SetEquals(IEnumerable<object> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return items.SetEquals(other);
        }

        /// <summary>
        /// Určí, zda je zadaný objekt stejná klasická množina.
        /// </summary>
        /// <param name="obj">Porovnávaný objekt.</param>
        /// <returns><c>true</c>, pokud objekt obsahuje stejné prvky.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as CrispSet);
        }

        /// <summary>
        /// Určí, zda klasická množina obsahuje stejné prvky jako jiná množina.
        /// </summary>
        /// <param name="other">Porovnávaná klasická množina.</param>
        /// <returns><c>true</c>, pokud jsou množiny stejné.</returns>
        public bool Equals(CrispSet other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return SetEquals(other);
        }

        /// <summary>
        /// Vrátí hash kód odvozený z prvků množiny.
        /// </summary>
        /// <returns>Hash kód množiny.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (object item in items)
                {
                    hash ^= item.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Porovná dvě klasické množiny podle množinové rovnosti.
        /// </summary>
        /// <param name="left">Levá množina.</param>
        /// <param name="right">Pravá množina.</param>
        /// <returns><c>true</c>, pokud obě množiny obsahují stejné prvky.</returns>
        public static bool operator ==(CrispSet left, CrispSet right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.SetEquals(right);
        }

        /// <summary>
        /// Porovná, zda se dvě klasické množiny liší.
        /// </summary>
        /// <param name="left">Levá množina.</param>
        /// <param name="right">Pravá množina.</param>
        /// <returns><c>true</c>, pokud množiny neobsahují stejné prvky.</returns>
        public static bool operator !=(CrispSet left, CrispSet right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Vrátí enumerátor prvků množiny.
        /// </summary>
        /// <returns>Enumerátor prvků.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Vrátí textovou reprezentaci klasické množiny.
        /// </summary>
        /// <returns>Textová reprezentace množiny.</returns>
        public override string ToString()
        {
            return string.Join(", ", items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
