using System;

namespace Skybrud.Umbraco.Redirects.Import.Csv
{
    /// <summary>
    /// Class representing a cell in an instance of <see cref="CsvFile"/>.
    /// </summary>
    public class CsvCell
    { 
        #region Properties

        /// <summary>
        /// Gets a reference to the parent <see cref="CsvFile"/>.
        /// </summary>
        public CsvFile File => Row.File;

        /// <summary>
        /// Gets a reference to the parent <see cref="CsvRow"/>.
        /// </summary>
        public CsvRow Row { get; internal set; }

        /// <summary>
        /// Gets a reference to the <see cref="CsvColumn"/> of the cell.
        /// </summary>
        public CsvColumn Column { get; internal set; }

        /// <summary>
        /// Gets the string value of the cell.
        /// </summary>
        public string Value { get; internal set; }

        #endregion

        #region Constructors

        internal CsvCell() { }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new row to the parent <see cref="CsvFile"/>.
        /// </summary>
        /// <returns>The added row.</returns>
        public CsvRow AddRow() {
            return Row.File.AddRow();
        }

        /// <summary>
        /// Adds a new cell to the parent <see cref="CsvRow"/>.
        /// </summary>
        /// <param name="value">The value of the cell.</param>
        /// <returns>The cell row.</returns>
        public CsvCell AddCell(string value) {
            return Row.AddCell(value);
        }

        /// <summary>
        /// Gets the cell value as an instance of <see cref="int"/>.
        /// </summary>
        /// <returns>An instance of <see cref="int"/>.</returns>
        public int AsInt32() {
            return Int32.Parse(Value.Trim());
        }

        /// <summary>
        /// Gets the cell value as an instance of <see cref="long"/>.
        /// </summary>
        /// <returns>An instance of <see cref="long"/>.</returns>
        public long AsInt64() {
            return Int64.Parse(Value.Trim());
        }

        /// <summary>
        /// Gets the cell value as an instance of <see cref="double"/>.
        /// </summary>
        /// <returns>An instance of <see cref="double"/>.</returns>
        public double AsDouble() {
            return Double.Parse(Value.Trim());
        }

        #endregion

    }

}