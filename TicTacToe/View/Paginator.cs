namespace TicTacToe.View
{
    /// <summary>
    /// Utility used for pagination, avoiding technical overheads and UI clutter.
    /// </summary>
    /// <typeparam name="T">The type of object present in the source.</typeparam>
    public class Paginator<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly int _pageSize;
        private readonly int _maximumPages;
        private int _pageNumber = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Paginator{T}"/> class.
        /// </summary>
        /// <param name="source">The source to paginate.</param>
        /// <param name="pageSize">The size of each page.</param>
        public Paginator(IEnumerable<T> source, int pageSize)
        {
            this._source = source;
            this._pageSize = pageSize;
            this._maximumPages = ((source.Count() + pageSize - 1) / pageSize) + 1;  // Ceiling division

            this.CurrentPage = this.NextPage();
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The list of items present on the current page.</value>
        public IEnumerable<T> CurrentPage { get; private set; }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        /// <value>The current page number stored by the paginator.</value>
        public int CurrentPageNumber => this._pageNumber;

        /// <summary>
        /// Gets the number of elements to skip.
        /// </summary>
        /// <value>Number representing the number of elements to skip from the source.</value>
        private int SkipCount => this._pageSize * this._pageNumber;

        /// <summary>
        /// Gets the next page.
        /// </summary>
        /// <returns>The list of items present in the next page.</returns>
        /// <exception cref="InvalidOperationException">Thrown if there are no more pages to access.</exception>
        public IEnumerable<T> NextPage()
        {
            if (this._pageNumber == this._maximumPages)
            {
                throw new InvalidOperationException("No more pages available");
            }

            ++this._pageNumber;
            this.CurrentPage = this._source
                                .Skip(this.SkipCount)
                                .Take(this._pageSize);

            return this.CurrentPage;
        }

        /// <summary>
        /// Gets the previous page.
        /// </summary>
        /// <returns>The list of items from the previous page.</returns>
        /// <exception cref="InvalidOperationException">Thrown if already on first page.</exception>
        public IEnumerable<T> PreviousPage()
        {
            if (this._pageNumber == 1)
            {
                throw new InvalidOperationException("No previous pages found");
            }

            --this._pageNumber;
            this.CurrentPage = this._source
                                .Skip(this.SkipCount)
                                .Take(this._pageSize);

            return this.CurrentPage;
        }
    }
}
