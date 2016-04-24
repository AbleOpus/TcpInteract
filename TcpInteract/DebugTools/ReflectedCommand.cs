namespace TcpInteract.DebugTools
{
    /// <summary>
    /// Represents a command that has been identified with <see cref="System.Reflection"/>.
    /// Compared using the Value property.
    /// </summary>
    public class ReflectedCommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the command. This should be a base number type.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets whether this command is an enumeration.
        /// </summary>
        public bool IsEnum { get; }

        /// <summary>
        /// Initializes the <see cref="ReflectedCommand"/> class.
        /// </summary>
        public ReflectedCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectedCommand"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="value">The value of the command. This should be a base number type.</param>
        /// <param name="isEnum">Whether this command is an enumeration.</param>
        public ReflectedCommand(string name, object value, bool isEnum)
        {
            Name = name;
            Value = value;
            IsEnum = isEnum;
        }

        /// <summary>Determines whether the specified ReflectedCommand is equal to the current ReflectedCommand.</summary>
        /// <returns>true if the specified ReflectedCommand is equal to the current ReflectedCommand; otherwise, false.</returns>
        /// <param name="other">The ReflectedCommand to compare with the current ReflectedCommand.</param>
        /// <filterpriority>2</filterpriority>
        protected bool Equals(ReflectedCommand other)
        {
            return Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((ReflectedCommand) obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }


        /// <summary>
        /// Determines whether the two specified ReflectedCommands have the same value.
        /// </summary>
        /// <param name="a">The first command to compare, or null.</param>
        /// <param name="b">The first command to compare, or null.</param>
        /// <returns>True if the value of "a" is the same as the value of "b"; otherwise, false.</returns>
        public static bool operator ==(ReflectedCommand a, ReflectedCommand b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Determines whether the two specified ReflectedCommands do not have the same value.
        /// </summary>
        /// <param name="a">The first command to compare, or null.</param>
        /// <param name="b">The first command to compare, or null.</param>
        /// <returns>True if the value of "a" is different from the value of "b"; otherwise, false.</returns>
        public static bool operator !=(ReflectedCommand a, ReflectedCommand b)
        {
            return !Equals(a, b);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"Name: {Name}, Value: {Value}, IsEnum: {IsEnum}";
        }
    }
}
