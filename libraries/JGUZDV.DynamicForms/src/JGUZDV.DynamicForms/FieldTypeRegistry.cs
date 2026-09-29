using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms;

public class FieldTypeRegistry
{
    private readonly Dictionary<FieldTypeId, FieldType> _registeredFieldTypes;
    private readonly ConstraintTypeRegistry _constraintTypeRegistry;

    public FieldTypeRegistry()
        : this(new ConstraintTypeRegistry())
    {
    }

    public FieldTypeRegistry(ConstraintTypeRegistry constraintTypeRegistry)
    {
        _constraintTypeRegistry = constraintTypeRegistry;
        _registeredFieldTypes = new FieldType[]
        {
            BoolFieldType.Instance,
            DateOnlyFieldType.Instance,
            FileFieldType.Instance,
            FloatFieldType.Instance,
            IntFieldType.Instance,
            StringFieldType.Instance,
            TimeOnlyFieldType.Instance
        }.ToDictionary(x => x.TypeId);
    }

    public IReadOnlyDictionary<FieldTypeId, FieldType> RegisteredFieldTypes => _registeredFieldTypes;

    public bool TryAdd(FieldType type)
    {
        foreach (var constraintId in type.AllowedConstraints)
        {
            if (!_constraintTypeRegistry.RegisteredConstraintTypes.ContainsKey(constraintId))
            {
                throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
            }
        }

        return _registeredFieldTypes.TryAdd(type.TypeId, type);
    }

    public bool Remove(FieldTypeId fieldTypeId)
    {
        return _registeredFieldTypes.Remove(fieldTypeId);
    }

    public void SetConstraintTypes(FieldType fieldType, IEnumerable<ConstraintId> constraintTypes)
    {
        var constraintIds = constraintTypes.ToList();
        foreach (var constraintId in constraintIds)
        {
            if (!_constraintTypeRegistry.RegisteredConstraintTypes.ContainsKey(constraintId))
            {
                throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
            }
        }

        fieldType.AllowedConstraints.UnionWith(constraintIds);
    }
}

public class ConstraintTypeRegistry
{
    private readonly Dictionary<ConstraintId, Type> _registeredConstraintTypes = new()
    {
        { RegexConstraint.ConstraintId, typeof(RegexConstraint) },
        { StringLengthConstraint.ConstraintId, typeof(StringLengthConstraint) },
        { RangeConstraint.ConstraintId, typeof(RangeConstraint) },
        { SizeConstraint.ConstraintId, typeof(SizeConstraint) },
        { FileSizeConstraint.ConstraintId, typeof(FileSizeConstraint) }
    };

    public IReadOnlyDictionary<ConstraintId, Type> RegisteredConstraintTypes => _registeredConstraintTypes;

    public bool TryAdd<TConstraint>()
        where TConstraint : IConstraint
    {
        return _registeredConstraintTypes.TryAdd(TConstraint.ConstraintId, typeof(TConstraint));
    }

    public bool Remove<TConstraint>()
        where TConstraint : IConstraint
    {
        return _registeredConstraintTypes.Remove(TConstraint.ConstraintId);
    }

    public L10nString GetName(ConstraintId constraintId)
    {
        return _registeredConstraintTypes.TryGetValue(constraintId, out var constraintType)
            ? (L10nString)constraintType.GetProperty("DisplayName")!.GetValue(null)!
            : throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
    }

    public void AddToFieldDefinition(FieldDefinition fieldDefinition, ConstraintId constraintId)
    {
        if (!_registeredConstraintTypes.TryGetValue(constraintId, out var constraintType))
        {
            throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
        }

        fieldDefinition.Constraints.Add((IConstraint)Activator.CreateInstance(constraintType)!);
    }
}
