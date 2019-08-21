using System;
using System.Collections;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Import.Csv {

    /// <summary>
    /// Class representing a list of columns in a CSV file.
    /// </summary>
    public class CsvColumnList : IEnumerable<CsvColumn> {

        private readonly List<CsvColumn> _columns = new List<CsvColumn>();

        #region Properties

        /// <summary>
        /// Gets a reference back to the parent <see cref="CsvFile"/>.
        /// </summary>
        public CsvFile File { get; set; }

        /// <summary>
        /// Gets the number of columns contained in the list.
        /// </summary>
        public int Count => _columns.Count;

        /// <summary>
        /// Alias of <see cref="Count"/>.
        /// </summary>
        public int Length => _columns.Count;

        /// <summary>
        /// Gets the column at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the column to get.</param>
        /// <returns>An instance of <see cref="CsvColumn"/>.</returns>
        public CsvColumn this[int index] => _columns[index];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new column list for the specified <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The parent file of the column list.</param>
        public CsvColumnList(CsvFile file) {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        #endregion

        /// <summary>
        /// Adds a new column with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The added column.</returns>
        public CsvColumn AddColumn(string name) {
            CsvColumn column = new CsvColumn { Index = _columns.Count, File = File, Name = name ?? "" };
            _columns.Add(column);
            return column;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CsvColumnList"/>.
        /// </summary>
        /// <returns>A <see cref="List{CsvColumn}.Enumerator"/> for the <see cref="CsvColumnList"/>.</returns>
        public IEnumerator<CsvColumn> GetEnumerator() {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}