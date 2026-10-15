using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.Helpers;
using TicTacToe.Models.Utils;
using TicTacToe.View.GridSelector;

namespace TicTacToe.View
{
    /// <summary>
    /// Generates menus to be rendered in the console.
    /// </summary>
    /// <typeparam name="TMenu">The type of menu to be handled.</typeparam>
    public class Menu<TMenu>
        where TMenu : struct, Enum
    {
        /// <summary>
        /// Gets or sets the menu's header.
        /// </summary>
        /// <value>Header text; text to be displayed at the top of the menu.</value>
        private string Header { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the menu's footer.
        /// </summary>
        /// <value>Footer text; text to be displayed at the bottom of the menu.</value>
        private string Footer { get; set; } = string.Empty;

        /// <summary>
        /// Renders the menu on-screen an returns the selected choice.
        /// </summary>
        /// <param name="menuHeader">The text to be displayed above the menu.</param>
        /// <param name="menuFooter">The text to be displayed below the menu.</param>
        /// <param name="isMenuCancellable">Whether or not the user can cancel the menu.</param>
        /// <returns>Returns the user's selection from the menu.</returns>
        /// <exception cref="MenuCancelledException">Thrown when the user cancels the menu (given the menu is cancellable).</exception>
        /// <exception cref="ApplicationException">Thrown due to unforeseen system crashes or corruption in the internal state.</exception>
        public TMenu Render(string menuHeader, string menuFooter, bool isMenuCancellable)
        {
            this.Header = menuHeader;
            this.Footer = menuFooter;

            string[] menuLines = Generate();
            using var selector = new Selector(
                    menuLines,
                    this.RenderMenu,
                    ConsoleUtils.ClearScreen,
                    isMenuCancellable);

            while (selector.ViewState == SelectorViewState.Active)
            {
                ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                selector.UpdateSelector(menuLines, keyPress);
            }

            return selector.ViewState switch
            {
                SelectorViewState.Selected => Enum.Parse<TMenu>(menuLines[selector.Selection.Y]),
                SelectorViewState.Cancelled => throw new MenuCancelledException(),
                _ => throw new ApplicationException("Unexpected state corruption"), // Can only be triggered during debug mode
            };
        }

        /// <summary>
        /// Utility for generating menus from their respective enums.
        /// </summary>
        /// <returns>Menu lines.</returns>
        internal static string[] Generate()
        {
            return Enum.GetNames<TMenu>()
                    .Select(CaseConverter.PascalToTitle)
                    .ToArray();
        }

        private void RenderMenu(string[,] menuGrid, CartesianStateTracker selectedOption)
        {
            Console.WriteLine(this.Header);
            Console.WriteLine();

            for (int i = 0; i < menuGrid.GetLength(0); ++i)
            {
                if (i == selectedOption.Y)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.WriteLine(menuGrid[i, 0]);

                if (i == selectedOption.Y)
                {
                    Console.ResetColor();
                }
            }

            Console.WriteLine(this.Footer);
        }
    }
}
