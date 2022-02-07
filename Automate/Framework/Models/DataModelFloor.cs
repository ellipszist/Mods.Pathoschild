namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>A floor to support by name in configuration.</summary>
    internal class DataModelFloor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The English name for the floor item.</summary>
        public string Name { get; }

        /// <summary>The item's unqualified item ID.</summary>
        public string ItemId { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The English name for the floor item.</param>
        /// <param name="itemId">The item's unqualified item ID.</param>
        public DataModelFloor(string name, string itemId)
        {
            this.Name = name;
            this.ItemId = itemId;
        }
    }
}
