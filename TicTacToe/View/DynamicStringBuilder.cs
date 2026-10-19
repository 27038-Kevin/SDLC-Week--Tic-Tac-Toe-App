using System.Text;

namespace TicTacToe.View
{
    /// <summary>
    /// Builds strings dynamically by taking user input character by character.
    /// </summary>
    public class DynamicStringBuilder
    {
        /// <summary>
        /// Key for toggling visibility, if secret mode is enabled.
        /// </summary>
        public const ConsoleKey ToggleKey = ConsoleKey.Tab;

        /// <summary>
        /// Key for submitting the query, invoking the Submitted event.
        /// </summary>
        public const ConsoleKey SubmitKey = ConsoleKey.Enter;

        /// <summary>
        /// Whether or not the secret mode toggle key is enabled.
        /// </summary>
        private readonly bool _secretToggleEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicStringBuilder"/> class.
        /// </summary>
        /// <param name="isSecret">Whether or not the query string is secret.</param>
        /// <param name="enableVisibilityToggle">Whether or not the visibility toggle is enabled.</param>
        /// <param name="onSubmit">Actions to be taken when the Submit key is pressed by the user.</param>
        public DynamicStringBuilder(bool isSecret, bool enableVisibilityToggle, params EventHandler<StringBuilder>[] onSubmit)
        {
            this.IsSecret = isSecret;
            this._secretToggleEnabled = enableVisibilityToggle;

            foreach (var action in onSubmit)
            {
                this.Submitted += action;
            }
        }

        /// <summary>
        /// Raises an event when the Submit key is pressed, submitting the typed query.
        /// </summary>
        private event EventHandler<StringBuilder>? Submitted;

        /// <summary>
        /// Gets a value indicating whether the string was submitted.
        /// </summary>
        /// <value>true if the string was submitted; false otherwise.</value>
        public bool IsSubmitted { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the query is meant to be secret.
        /// </summary>
        /// <value>true if the query is secret; false otherwise.</value>
        public bool IsSecret { get; private set; }

        /// <summary>
        /// Gets the display string representing the query.
        /// </summary>
        /// <value>Query string if not secret; else obfuscated string.</value>
        public string DisplayString => this.IsSecret
            ? new string('*', this.Query.Length)
            : this.Query.ToString();

        /// <summary>
        /// Gets the query.
        /// </summary>
        private StringBuilder Query { get; } = new StringBuilder();

        /// <summary>
        /// Updates the state of the dynamic string builder.
        /// </summary>
        /// <param name="keyPress">The user's last keypress.</param>
        /// <remarks>
        /// Toggles visibility if Tab is pressed for a secret query.
        /// Raises an event if Enter is pressed.
        /// Removes the last character if Backspace is pressed.
        /// Appends a character to the query if it is a letter, number, symbol or space.
        /// </remarks>
        public void UpdateState(ConsoleKeyInfo? keyPress)
        {
            if (keyPress is null)
            {
                return; // Do not update state for null keypresses
            }

            switch (keyPress.Value.Key)
            {
                case ToggleKey:
                    if (this._secretToggleEnabled)
                    {
                        this.IsSecret = !this.IsSecret;
                    }
                    break;

                case SubmitKey:
                    this.IsSubmitted = true;
                    this.Submitted?.Invoke(this, this.Query);
                    break;

                case ConsoleKey.Backspace:
                    if (this.Query.Length > 0)
                    {
                        this.Query.Remove(this.Query.Length - 1, 1);
                    }
                    break;

                default:
                    if (!char.IsControl(keyPress.Value.KeyChar))
                    {
                        this.Query.Append(keyPress.Value.KeyChar);
                    }
                    break;
            }
        }
    }
}