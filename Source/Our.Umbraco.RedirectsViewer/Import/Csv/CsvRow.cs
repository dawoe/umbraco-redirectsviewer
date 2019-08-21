using System;
using System.Linq;

namespace Skybrud.Umbraco.Redirects.Import.Csv {

    /// <summary>
    /// Class representing a row in an instance of <see cref="CsvFile"/>.
    /// </summary>
    public class CsvRow {

        #region Properties

        /// <summary>
        /// Gets a reference back to the parent <see cref="CsvFile"/>.
        /// </summary>
        public CsvFile File { get; private set; }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the value of the first cell matching the specified <paramref name="columnName"/>, or <code>null</code>
        /// if not found.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>An instance of <see cref="CsvCell"/>, or <code>null</code> if not found.</returns>
        public CsvCell this[string columnName] {
            get { return Cells.FirstOrDefault(x => x.Column.Name == columnName); }
        }

        /// <summary>
        /// Gets a reference to the cells of the row.
        /// </summary>
        public CsvCellList Cells { get; private set; }

        #endregion

        #region Constructors

        internal CsvRow(int index, CsvFile file) {
            Index = index;
            File = file;
            Cells = new CsvCellList(this);
        }

        #endregion

        #region Member methods

        internal CsvCell AddCell(CsvColumn column, string value) {
            return Cells.AddCell(column, value);
        }

        /// <summary>
        /// Adds a new cell with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of the cell.</param>
        /// <returns>The added cell.</returns>
        public CsvCell AddCell(string value) {
            CsvColumn column = File.Columns[Cells.Length];
            return Cells.AddCell(column, value);
        }
    
        /// <summary>
        /// Gets the string value of the cell at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The string value of the cell.</returns>
        public string GetCellValue(int index) {
            CsvCell cell = Cells[index];
            return cell == null ? null : cell.Value;
        }

        /// <summary>
        /// Gets the string value of the cell at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The string value of the cell.</returns>
        public T GetCellValue<T>(int index) {
            CsvCell cell = Cells[index];
            return cell == null ? default(T) : (T) Convert.ChangeType(cell.Value, typeof(T));
        }

        /// <summary>
        /// Gets the string value of the cell at the specified <paramref name="index"/>, and converts it using
        /// <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="index">The index of the cell.</param>
        /// <param name="callback">The callback function to be used for converting the value.</param>
        /// <returns>An instance of <typeparamref name="T"/> representing the value of the cell.</returns>
        public T GetCellValue<T>(int index, Func<string, T> callback) {
            CsvCell cell = Cells[index];
            return cell == null ? default(T) : callback(cell.Value);
        }

        /// <summary>
        /// Gets the string value of the cell with the specified <paramref name="columnName"/>. If multiple columns
        /// match <paramref name="columnName"/>, only the value of the first cell will be returned.
        /// </summary>
        /// <param name="columnName">The name of the cell.</param>
        /// <returns>The string value of the cell.</returns>
        public string GetCellValue(string columnName) {
            CsvCell cell = this[columnName];
            return cell == null ? null : cell.Value;
        }

        /// <summary>
        /// Gets the string value of the cell with the specified <paramref name="columnName"/>, and converts it to the
        /// type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="columnName">The name of the cell.</param>
        /// <returns>An instance of <typeparamref name="T"/> representing the value of the cell.</returns>
        public T GetCellValue<T>(string columnName) {
            CsvCell cell = this[columnName];
            return cell == null ? default(T) : (T) Convert.ChangeType(cell.Value, typeof(T));
        }

        /// <summary>
        /// Gets the string value of the cell with the specified <paramref name="columnName"/>, and converts it using
        /// <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="columnName">The name of the cell.</param>
        /// <param name="callback">The callback function to be used for converting the value.</param>
        /// <returns>An instance of <typeparamref name="T"/> representing the value of the cell.</returns>
        public T GetCellValue<T>(string columnName, Func<string, T> callback) {
            CsvCell cell = this[columnName];
            return cell == null ? default(T) : callback(cell.Value);
        }

        #endregion

    }

}