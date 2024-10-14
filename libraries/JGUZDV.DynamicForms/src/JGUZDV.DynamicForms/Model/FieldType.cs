namespace JGUZDV.DynamicForms.Model
{
    public abstract class FieldType
    {
        public abstract Type Type { get; }

        public abstract string ToJson();
        public abstract FieldType FromJson();

    }

    public class DateOnlyField : FieldType
    {
        public override Type Type => throw new NotImplementedException();

        public override FieldType FromJson()
        {
            throw new NotImplementedException();
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
