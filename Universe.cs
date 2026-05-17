namespace Fuzzy
{
    /// <summary>
    /// Reprezentuje univerzum hodnot, nad kterým jsou definované fuzzy množiny.
    /// </summary>
    public class Universe : CrispSet
    {
        /// <summary>
        /// Vytvoří univerzum z předaných hodnot.
        /// </summary>
        /// <param name="items">Hodnoty univerza.</param>
        public Universe(params object[] items)
            : base(items)
        {
        }
    }
}
