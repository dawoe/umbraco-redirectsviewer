using System;
using System.Collections;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Import.Csv
{

    /// <summary>
    /// Class representing a list of rows in a CSV file.
    /// </summary>
    public class CsvRowList : IEnumerable<CsvRow> {

        private readonly List<CsvRow> _rows = new List<CsvRow>();

        #region Properties

        /// <summary>
        /// Gets a reference back to the parent <see cref="CsvFile"/>.
        /// </summary>
        public CsvFile File { get; private set; }

        /// <summary>
        /// Gets the number of columns contained in the list.
        /// </summary>
        public int Count => _rows.Count;

        /// <summary>
        /// Alias of <see cref="Count"/>.
        /// </summary>
        public int Length => _rows.Count;

        /// <summary>
        /// Gets the row at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the row to get.</param>
        /// <returns>An instance of <see cref="CsvRow"/>.</returns>
        public CsvRow this[int index] => _rows[index];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new row list for the specified <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The parent file of the row list.</param>
        public CsvRowList(CsvFile file) {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new row to the list.
        /// </summary>
        /// <returns>The added row.</returns>
        public CsvRow AddRow() {
            CsvRow row = new CsvRow(_rows.Count, File);
            _rows.Add(row);
            return row;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CsvRowList"/>.
        /// </summary>
        /// <returns>A <see cref="List{CsvRow}.Enumerator"/> for the <see cref="CsvRowList"/>.</returns>
        public IEnumerator<CsvRow> GetEnumerator() {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

    }

}