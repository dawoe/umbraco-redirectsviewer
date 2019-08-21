using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Skybrud.Umbraco.Redirects.Import.Csv {

    /// <summary>
    /// Class representing a CSV file parsed either from a file or a string.
    /// </summary>
    public class CsvFile {

        #region Properties
        
        /// <summary>
        /// Gets the path from where the CSV file was loaded.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets whether this instance has a reference to the path the CSV file was loaded from.
        /// </summary>
        public bool HasPath => !String.IsNullOrWhiteSpace(Path);

        /// <summary>
        /// Gets a list of the columns of the file.
        /// </summary>
        public CsvColumnList Columns { get; private set; }

        /// <summary>
        /// Gets a list of the rows of the file.
        /// </summary>
        public CsvRowList Rows { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty CSV file.
        /// </summary>
        public CsvFile() {
            Columns = new CsvColumnList(this);
            Rows = new CsvRowList(this);
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Adds a new column to the file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The added column.</returns>
        public CsvColumn AddColumn(string name) {
            return Columns.AddColumn(name);
        }

        /// <summary>
        /// Adds a new row to the file.
        /// </summary>
        /// <returns>The added row.</returns>
        public CsvRow AddRow() {
            return Rows.AddRow();
        }

        /// <summary>
        /// Returns a string representation of the CSV file, using <see cref="CsvSeparator.SemiColon"/> as a separator.
        /// </summary>
        /// <returns>A string representation of the CSV file.</returns>
        public override string ToString() {
            return ToString(CsvSeparator.SemiColon);
        }

        /// <summary>
        /// Returns a string representation of the CSV file, using the specified <paramref name="separator"/>.
        /// </summary>
        /// <param name="separator">The separator to be used.</param>
        /// <returns>A string representation of the CSV file.</returns>
        public string ToString(CsvSeparator separator) {

            StringBuilder sb = new StringBuilder();
            
            // Get the separator as a "char"
            char sep;
            switch (separator) {
                case CsvSeparator.Comma: sep = ','; break;
                case CsvSeparator.Colon: sep = ':'; break;
                case CsvSeparator.SemiColon: sep = ';'; break;
                case CsvSeparator.Space: sep = ' '; break;
                case CsvSeparator.Tab: sep = '\t'; break;
                default: sep = ';'; break;
            }

            // Append the first line with the column headers
            for (int i = 0; i < Columns.Length; i++) {
                if (i > 0) sb.Append(sep);
                sb.Append(Escape(Columns[i].Name, sep));
            }

            foreach (CsvRow row in Rows) {
                sb.AppendLine();
                for (int i = 0; i < Columns.Length; i++) {
                    if (i > 0) sb.Append(sep);
                    CsvCell cell = i < row.Cells.Length ? row.Cells[i] : null;
                    sb.Append(Escape(cell == null ? "" : cell.Value, sep));
                }
            }
            
            return sb.ToString();

        }

        /// <summary>
        /// Helper method for escaping special characters (eg. double quotes and line breaks). If the value contains
        /// any characters that should be escaped, the value will be enclosed with double quotes. The value will not be
        /// modified if it doesn't contain invalid characters.
        /// </summary>
        /// <param name="value">The value to be escaped.</param>
        /// <param name="separator">The column separator.</param>
        /// <returns>The escaped string.</returns>
        private string Escape(string value, char separator) {

            if (value.Contains('"') || value.Contains('\n') || value.Contains(separator)) {
                
                // Double quotes are escaped by adding a new double quote for each existing double quote
                return "\"" + value.Replace("\"", "\"\"") + "\"";

            }

            return value;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public CsvFile Save(string path) {
            return Save(path, CsvSeparator.Colon, Encoding.UTF8);
        }

        /// <summary>
        /// Saves the CSV file at the specified <paramref name="path"/>, using the specified
        /// <paramref name="separator"/> and <see cref="Encoding.UTF8"/>..
        /// </summary>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <returns>The original instance of <see cref="CsvFile"/>.</returns>
        public CsvFile Save(string path, CsvSeparator separator) {
            return Save(path, separator, Encoding.UTF8);
        }

        /// <summary>
        /// Saves the CSV file at the specified <paramref name="path"/>, using <see cref="CsvSeparator.Colon"/> and the
        /// specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="encoding">The encoding to be used.</param>
        /// <returns>The original instance of <see cref="CsvFile"/>.</returns>
        public CsvFile Save(string path, Encoding encoding) {
            return Save(path, CsvSeparator.Colon, encoding);
        }

        /// <summary>
        /// Saves the CSV file at the specified <paramref name="path"/>, using the specified
        /// <paramref name="separator"/> and <paramref name="encoding"/>.
        /// </summary>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <param name="encoding">The encoding to be used.</param>
        /// <returns>The original instance of <see cref="CsvFile"/>.</returns>
        public CsvFile Save(string path, CsvSeparator separator, Encoding encoding) {
            File.WriteAllText(path, ToString(separator), encoding);
            return this;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="text"/> into an instance of <see cref="CsvFile"/>, using
        /// <see cref="CsvSeparator.SemiColon"/> as a separator.
        /// </summary>
        /// <param name="text">The text representing the contents of the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Parse(string text) {
            return Parse(text, CsvSeparator.SemiColon);
        }

        /// <summary>
        /// Parses the specified <paramref name="text"/> into an instance of <see cref="CsvFile"/>, using the specified
        /// <paramref name="separator"/>.
        /// </summary>
        /// <param name="text">The text representing the contents of the CSV file.</param>
        /// <param name="separator">The separator used in the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Parse(string text, CsvSeparator separator) {

            // Initialize a new CSV file
            CsvFile file = new CsvFile();

            // Parse the contents
            return ParseInternal(file, text, separator);

        }

        /// <summary>
        /// Loads the CSV file at the specified <paramref name="path"/>. <see cref="Encoding.UTF8"/> is assumed as
        /// encoding. <see cref="CsvSeparator.SemiColon"/> is used as separator.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Load(string path) {
            return Load(path, CsvSeparator.SemiColon, Encoding.UTF8);
        }

        /// <summary>
        /// Loads the CSV file at the specified <paramref name="path"/>. <see cref="Encoding.UTF8"/> is assumed as
        /// encoding. <paramref name="separator"/> is used as separator.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <param name="separator">The separator used in the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Load(string path, CsvSeparator separator) {
            return Load(path, separator, Encoding.UTF8);
        }

        /// <summary>
        /// Loads the CSV file at the specified <paramref name="path"/>. <paramref name="encoding"/> is assumed as
        /// encoding. <see cref="CsvSeparator.SemiColon"/> is used as separator.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <param name="encoding">The encoding of the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Load(string path, Encoding encoding) {
            return Load(path, CsvSeparator.SemiColon, encoding);
        }

        /// <summary>
        /// Loads the CSV file at the specified <paramref name="path"/>. <paramref name="separator"/> is used as
        /// separator. <paramref name="encoding"/> is assumed as encoding. 
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <param name="separator">The separator used in the CSV file.</param>
        /// <param name="encoding">The encoding of the CSV file.</param>
        /// <returns>An instance of <see cref="CsvFile"/>.</returns>
        public static CsvFile Load(string path, CsvSeparator separator, Encoding encoding) {

            // Load the contents of the CSV file
            string contents = File.ReadAllText(path, encoding ?? Encoding.UTF8).Trim();

            // Initialize a new CSV file
            CsvFile file = new CsvFile { Path = path };

            // Parse the contents
            ParseInternal(file, contents, separator);

            return file;

        }

        /// <summary>
        /// Internal helper method for parsing the contents of a CSV file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="contents"></param>
        /// <param name="separator"></param>
        /// <returns><paramref name="file"/>.</returns>
        private static CsvFile ParseInternal(CsvFile file, string contents, CsvSeparator separator = CsvSeparator.SemiColon) {

            // Normalize line endings
            contents = contents.Replace("\r\n", "\n");
            contents = contents.Replace("\r", "\n");

            // Get the separator as a "char"
            char sep;
            switch (separator) {
                case CsvSeparator.Comma: sep = ','; break;
                case CsvSeparator.Colon: sep = ':'; break;
                case CsvSeparator.SemiColon: sep = ';'; break;
                case CsvSeparator.Space: sep = ' '; break;
                case CsvSeparator.Tab: sep = '\t'; break;
                default: sep = ';'; break;
            }

            // Parse each line into a list of cell values
            List<List<string>> lines = ParseLines(contents, sep);

            if (lines.Count == 0) throw new Exception("WTF?");

            // If malformed, each line/row may not have the same amount of cells
            int maxColumns = lines.Max(x => x.Count);

            // Parse the columns (column headers)
            for (int c = 0; c < maxColumns; c++) {
                string name = lines[0].Skip(c).FirstOrDefault() ?? "";
                file.AddColumn(name);
            }

            // Parse the rows
            for (int r = 1; r < lines.Count; r++) {
                CsvRow row = file.AddRow();
                for (int c = 0; c < maxColumns; c++) {
                    CsvColumn column = file.Columns[c];
                    string value = lines[r].Skip(c).FirstOrDefault() ?? "";
                    row.AddCell(column, value);
                }
            }

            return file;

        }

        /// <summary>
        /// Internal helper method for parsing each line of the 
        /// </summary>
        /// <param name="contents">The contents of the CSV file.</param>
        /// <param name="separator">The separator used in the CSV file.</param>
        /// <returns>A list of <see cref="List{String}"/>.</returns>
        private static List<List<string>> ParseLines(string contents, char separator) {

            List<List<string>> lines = new List<List<string>>();

            string buffer = String.Empty;
            bool enclosed = false;
            bool escaped = false;

            List<string> line = new List<string>();

            // Parse each character in the input string
            for (int i = 0; i < contents.Length; i++) {

                char chr = contents[i];

                if (chr == '"') {

                    // If the value is already enclosed, we handle further scenarios
                    if (enclosed) {

                        // Get the next character
                        char next = i < contents.Length - 1 ? contents[i + 1] : ' ';

                        // A double quote may be used to escape another double quote if already in an enclosed value
                        if (next == '"') {
                            if (escaped) {
                                buffer += chr;
                                escaped = false;
                                i++;
                            } else {
                                buffer += chr;
                                escaped = true;
                                i++;
                            }
                        } else {
                            enclosed = false;
                        }

                    } else {
                        enclosed = true;
                    }

                } else if (enclosed) {
                    buffer += chr;
                } else if (chr == separator) {
                    line.Add(buffer);
                    buffer = String.Empty;
                } else if (chr == '\n') {
                    line.Add(buffer);
                    lines.Add(line);
                    buffer = String.Empty;
                    line = new List<string>();
                } else {
                    buffer += chr;
                }


            }

            // Append the last line
            if (line.Count > 0) {
                line.Add(buffer);
                lines.Add(line);
            }

            return lines;

        }
        
        #endregion

    }

}