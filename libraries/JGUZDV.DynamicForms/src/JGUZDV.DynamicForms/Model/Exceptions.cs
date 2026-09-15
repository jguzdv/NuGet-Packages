namespace JGUZDV.DynamicForms.Model
{
    /// <summary>
    /// Represents an exception that occurs in the DynamicForms library.
    /// </summary>
    public class DynamicFormsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicFormsException"/> class with a specified error message.
        /// </summary>
        public DynamicFormsException(string message)
            :base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicFormsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public DynamicFormsException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// Represents an exception that is thrown when a field definition with a specified ID is not found.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FieldDefinitionNotFoundException"/> class with a specified field ID.
    /// </remarks>
    public class FieldDefinitionNotFoundException(FieldId fieldId) : DynamicFormsException($"Field definition with ID '{fieldId.Value}' not found.")
    { }
}
