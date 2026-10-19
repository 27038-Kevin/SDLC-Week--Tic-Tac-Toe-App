namespace TicTacToe.Enums
{
    /// <summary>
    /// Describes the compliance status of a policy; used in <see cref="TicTacToe.Models.StateMachines.ComplianceNotifier"/>.
    /// </summary>
    internal enum PolicyCompliance
    {
        /// <summary>
        /// The compliance status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The policy is complied.
        /// </summary>
        Compliant,

        /// <summary>
        /// The policy is not complied.
        /// </summary>
        Incompliant,
    }
}
