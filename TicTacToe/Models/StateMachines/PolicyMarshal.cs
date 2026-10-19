using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// Notifies other entities of compliance/non-compliance to a policy.
    /// </summary>
    /// <typeparam name="T">The type of the object that needs to undergo a compliance check.</typeparam>
    internal class PolicyMarshal<T>
    {
        private bool _checked = false;
        private PolicyCompliance _status = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyMarshal{T}"/> class.
        /// </summary>
        /// <param name="policy">The policy to be checked.</param>
        public PolicyMarshal(Predicate<T> policy)
        {
            this.Policy = policy;
        }

        /// <summary>
        /// Gets the policy that needs to be checked.
        /// </summary>
        /// <value>The compliance policy that needs to be applied.</value>
        public Predicate<T> Policy { get; init; }

        /// <summary>
        /// Gets or sets the compliance status with the policy.
        /// </summary>
        /// <value>
        /// Unknown if compliance still hasn't been checked.
        /// Compliant if the policy is complied to.
        /// Incompliant if the policy is not complied to.
        /// </value>
        /// <remarks>Setting is only possible if compliance has not already been checked.</remarks>
        public PolicyCompliance ComplianceStatus
        {
            get => this._checked ? this._status : PolicyCompliance.Unknown;
            set
            {
                // Can only set status if compliance is not yet checked
                if (!this._checked && value != PolicyCompliance.Unknown) // Safeguard against permanent unknown status
                {
                    this._checked = true;
                    this._status = value;
                }
            }
        }
    }
}
