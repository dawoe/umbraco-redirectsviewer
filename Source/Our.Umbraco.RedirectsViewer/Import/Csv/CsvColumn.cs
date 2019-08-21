namespace Skybrud.Umbraco.Redirects.Import.Csv {

    /// <summary>
    /// Class representing a column in an instance of <see cref="CsvFile"/>.
    /// </summary>
    public class CsvColumn {

        #region Properties

        /// <summary>
        /// Gets a reference back to the parent <see cref="CsvFile"/>.
        /// </summary>
        public CsvFile File { get; set; }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new column to the parent <see cref="CsvFile"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The added column.</returns>
        public CsvColumn AddColumn(string name) {
            return File.AddColumn(name);
        }

        /// <summary>
        /// Adds a new row to the parent <see cref="CsvFile"/>.
        /// </summary>
        /// <returns>The added row.</returns>
        public CsvRow AddRow() {
            return File.AddRow();
        }

        #endregion

    }

}