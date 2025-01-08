using JGUZDV.DynamicForms.Blazor.Constraints;
using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Blazor
{
    public static class ConstraintViewTypeFactory
    {
        private static readonly Dictionary<Type, Type> _viewTypes = new()
        {
            { typeof(RegexConstraint), typeof(RegexConstraintEdit) },
            { typeof(RangeConstraint), typeof(RangeConstraintEdit) },
            { typeof(SizeConstraint), typeof(SizeConstraintEdit) },
            { typeof(StringLengthConstraint), typeof(LengthConstraintEdit) },
        };

        public static Type GetViewType(Constraint constraint)
        {
            return _viewTypes.GetValueOrDefault(constraint.GetType()) ?? throw new InvalidOperationException("Unknown Constraint");
        }

        public static void SetViewType(Constraint contraint, Type ViewType)
        {
            _viewTypes[contraint.GetType()] = ViewType;
        }
    }
}
