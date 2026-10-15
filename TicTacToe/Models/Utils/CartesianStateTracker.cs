namespace TicTacToe.Models.Utils
{
    /// <summary>
    /// Tracks an internal cursor state with X and Y coordinates.
    /// </summary>
    public struct CartesianStateTracker
    {
        private readonly int _maxX;
        private readonly int _maxY;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianStateTracker"/> struct.
        /// </summary>
        /// <param name="maxX">The maximum horizontal threshold for the x-coordinate (exclusive).</param>
        /// <param name="maxY">The maximum vertical threshold for the y-coordinate (exclusive).</param>
        public CartesianStateTracker(int maxX, int maxY)
        {
            this._maxX = maxX;
            this._maxY = maxY;
        }

        /// <summary>
        /// Gets the value of the X-coordinate.
        /// </summary>
        /// <value>The X-coordinate.</value>
        public int X { get; private set; } = 0;

        /// <summary>
        /// Gets the value of the Y-coordinate.
        /// </summary>
        /// <value>The Y-coordinate.</value>
        public int Y { get; private set; } = 0;

        /// <summary>
        /// Moves the cursor left and updates the horizontal index.
        /// </summary>
        /// <returns>
        /// true if the index could be updated;
        /// false if the cursor was already at the leftmost point.
        /// </returns>
        public bool Left()
        {
            if (this.X == 0)
            {
                return false;
            }

            --this.X;
            return true;
        }

        /// <summary>
        /// Moves the cursor right and updates the horizontal index.
        /// </summary>
        /// <returns>
        /// true if the index could be updated;
        /// false if the cursor was already at the rightmost point.
        /// </returns>
        public bool Right()
        {
            if (this.X == this._maxX - 1)
            {
                return false;
            }

            ++this.X;
            return true;
        }

        /// <summary>
        /// Moves the cursor up and updates the vertical index.
        /// </summary>
        /// <returns>
        /// true if the index could be updated;
        /// false if the cursor was already at the highest point.
        /// </returns>
        public bool Up()
        {
            if (this.Y == 0)
            {
                return false;
            }

            --this.Y;
            return true;
        }

        /// <summary>
        /// Moves the cursor down and updates the vertical index.
        /// </summary>
        /// <returns>
        /// true if the index could be updated;
        /// false if the cursor was already at the lowest point.
        /// </returns>
        public bool Down()
        {
            if (this.Y == this._maxY - 1)
            {
                return false;
            }

            ++this.Y;
            return true;
        }
    }
}
