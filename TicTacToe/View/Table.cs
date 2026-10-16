using System.Text;

namespace TicTacToe.View
{
    /// <summary>
    /// Used to create and render console tables.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Generates a formatted table using column lengths and row data.
        /// </summary>
        /// <param name="columnHeader">The header row, text representing the data of each field.</param>
        /// <param name="rows">The data for each row in the table.</param>
        /// <param name="alignLeft">Alignment option for each field in the table.</param>
        /// <returns>The formatted table containing all the data.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when data string length exceeds column width,
        /// or when dependency array lengths don't match.
        /// </exception>
        public static string Generate(string[] columnHeader, string[][] rows, bool[] alignLeft)
        {
            if (columnHeader.Length != alignLeft.Length || alignLeft.Length != rows[0].Length)
            {
                throw new ArgumentException("Array lengths don't match");
            }

            int[] columnLengths = GetColumnLengths(columnHeader, rows);

            var table = new StringBuilder();
            var rowMarker = GetRowMarker(columnLengths);

            table.AppendLine(rowMarker);
            table.AppendLine(GetHeaderRow(columnLengths, columnHeader));
            table.AppendLine(rowMarker);

            for (int i = 0; i < rows.Length; ++i)
            {
                table.AppendLine(GetRow(columnLengths, rows[i], alignLeft));
                table.AppendLine(rowMarker);
            }

            return table.ToString();
        }

        /// <summary>
        /// Gets the length of each column by finding the maximum length of a data cell and adding 4 for padding.
        /// </summary>
        /// <param name="headerRow">The string array containing the header row elements.</param>
        /// <param name="dataRows">The data rows in the table.</param>
        /// <returns>The length of each column in the table.</returns>
        private static int[] GetColumnLengths(string[] headerRow, string[][] dataRows)
        {
            int[] columnLengths = headerRow.Select(heading => heading.Length + 4)
                                        .ToArray();

            foreach (string[] dataRow in dataRows)
            {
                for (int i = 0; i < dataRow.Length; ++i)
                {
                    columnLengths[i] = Math.Max(dataRow[i].Length + 4, columnLengths[i]);
                }
            }

            return columnLengths;
        }

        /// <summary>
        /// Gets the formatted header row of a table.
        /// </summary>
        /// <param name="columnLengths">The length of each column in the table.</param>
        /// <param name="header">The data to be present in the header row.</param>
        /// <returns>Formatted header row.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when array lengths do not match,
        /// or when the header cell width is lesser than the data string length.
        /// </exception>
        private static string GetHeaderRow(int[] columnLengths, string[] header)
        {
            int detailsCount = columnLengths.Length;
            if (header.Length != detailsCount)
            {
                throw new ArgumentException("Array lengths do not match");
            }

            var headerRow = new StringBuilder();
            headerRow.Append('|');

            for (int i = 0; i < detailsCount; i++)
            {
                headerRow.Append(CenterAlign(header[i], columnLengths[i]));
                headerRow.Append('|');
            }

            return headerRow.ToString();
        }

        /// <summary>
        /// Gets a formatted table row.
        /// </summary>
        /// <param name="columnLengths">The length of each column in the table.</param>
        /// <param name="details">The details to fill the row with.</param>
        /// <param name="alignLeft">Alignment option for each corresponding column.</param>
        /// <returns>A formatted table row.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when array lengths do not match,
        /// or when data length exceeds field width.
        /// </exception>
        private static string GetRow(int[] columnLengths, string[] details, bool[] alignLeft)
        {
            var row = new StringBuilder();
            row.Append('|');

            int detailsCount = columnLengths.Length;
            if (details.Length != detailsCount || alignLeft.Length != detailsCount)
            {
                throw new ArgumentException("Array lengths do not match");
            }

            for (int i = 0; i < detailsCount; ++i)
            {
                if (details[i].Length > columnLengths[i])
                {
                    throw new ArgumentException("Detail length exceeds column width");
                }

                row.Append(alignLeft[i]
                    ? " " + details[i].PadRight(columnLengths[i] - 1)
                    : details[i].PadLeft(columnLengths[i] - 1) + " ");
                row.Append('|');
            }

            return row.ToString();
        }

        /// <summary>
        /// Gets the row marker for a table, which is inserted as a horizontal border between various rows of the table.
        /// </summary>
        /// <param name="columnLengths">The length of each column in the table.</param>
        /// <returns>Formatted string representing the row marker for the given table.</returns>
        private static string GetRowMarker(int[] columnLengths)
        {
            var rowMarker = new StringBuilder();
            rowMarker.Append('+');

            foreach (int length in columnLengths)
            {
                rowMarker.Append(new string('-', length));
                rowMarker.Append('+');
            }

            return rowMarker.ToString();
        }

        /// <summary>
        /// Centers a string within a specified width by padding it with spaces to the left and right.
        /// </summary>
        /// <param name="inputString">The string to be padded.</param>
        /// <param name="width">The maximum allowed width.</param>
        /// <returns>A center-aligned version of the string with whitespaces padded to its left and right.</returns>
        /// <exception cref="ArgumentException">Thrown when the string's original length exceeds the maximum allowed width.</exception>
        private static string CenterAlign(string inputString, int width)
        {
            if (inputString.Length > width)
            {
                throw new ArgumentException("String length cannot be greater than max width");
            }

            int leftPadding = (width - inputString.Length) / 2;

            return inputString
                    .PadLeft(inputString.Length + leftPadding)
                    .PadRight(width);
        }
    }
}