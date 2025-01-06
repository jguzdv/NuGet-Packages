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
            { typeof(LengthConstraint), typeof(LengthConstraintEdit) },
        };

        public static Type GetViewType(Type ConstraintType)
        {
            return _viewTypes.GetValueOrDefault(ConstraintType) ?? throw new InvalidOperationException("Unknown Constraint");
        }

        public static void SetViewType(Type ConstraintType, Type ViewType)
        {
            _viewTypes[ConstraintType] = ViewType;
        }
    }
}
