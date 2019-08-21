using System;
using System.Collections;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Import.Csv {

    /// <summary>
    /// Class representing a list of cells in a CSV file.
    /// </summary>
    public class CsvCellList : IEnumerable<CsvCell> {

        private readonly List<CsvCell> _cells = new List<CsvCell>();

        #region Properties

        /// <summary>
        /// Gets a reference back to the parent <see cref="CsvRow"/>.
        /// </summary>
        public CsvRow Row { get; set; }

        /// <summary>
        /// Gets the number of cells contained in the row.
        /// </summary>
        public int Count => _cells.Count;

        /// <summary>
        /// Alias of <see cref="Count"/>.
        /// </summary>
        public int Length => _cells.Count;

        /// <summary>
        /// Gets the cell at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the cell to get.</param>
        /// <returns>An instance of <see cref="CsvColumn"/>.</returns>
        public CsvCell this[int index] {
            get {
                if (index >= Length && index < Row.File.Columns.Length) {
                    return new CsvCell {
                        Column = Row.File.Columns[index],
                        Row = Row,
                        Value = ""
                    };
                }
                return _cells[index];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new cell list for the specified <paramref name="row"/>.
        /// </summary>
        /// <param name="row">The parent row of the cells.</param>
        public CsvCellList(CsvRow row) {
            Row = row ?? throw new ArgumentNullException(nameof(row));
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new cell with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="column">The column header of the cell.</param>
        /// <param name="value">The value of the cell.</param>
        /// <returns>The added cell.</returns>
        public CsvCell AddCell(CsvColumn column, string value) {
            CsvCell cell = new CsvCell { Column = column, Row = Row, Value = value ?? "" };
            _cells.Add(cell);
            return cell;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CsvCellList"/>.
        /// </summary>
        /// <returns>A <see cref="List{CsvCell}.Enumerator"/> for the <see cref="CsvCellList"/>.</returns>
        public IEnumerator<CsvCell> GetEnumerator() {
            return _cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

    }

}