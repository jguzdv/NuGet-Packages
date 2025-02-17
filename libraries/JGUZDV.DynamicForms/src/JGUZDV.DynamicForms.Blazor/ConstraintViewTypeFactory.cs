using JGUZDV.DynamicForms.Blazor.Constraints;
using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Blazor
{
    /// <summary>
    /// Provides a factory for mapping constraints to their corresponding view types.
    /// </summary>
    public static class ConstraintViewTypeFactory
    {
        private static readonly Dictionary<Type, Type> _viewTypes = new()
            {
                { typeof(RegexConstraint), typeof(RegexConstraintEdit) },
                { typeof(RangeConstraint), typeof(RangeConstraintEdit) },
                { typeof(SizeConstraint), typeof(SizeConstraintEdit) },
                { typeof(StringLengthConstraint), typeof(LengthConstraintEdit) },
            };

        /// <summary>
        /// Gets the view type associated with the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint to get the view type for.</param>
        /// <returns>The view type associated with the specified constraint.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the constraint type is unknown.</exception>
        public static Type GetViewType(Constraint constraint)
        {
            return _viewTypes.GetValueOrDefault(constraint.GetType()) ?? throw new InvalidOperationException("Unknown Constraint");
        }

        /// <summary>
        /// Sets the view type for the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint to set the view type for.</param>
        /// <param name="viewType">The view type to associate with the constraint.</param>
        public static void SetViewType(Constraint constraint, Type viewType)
        {
            _viewTypes[constraint.GetType()] = viewType;
        }
    }
}
