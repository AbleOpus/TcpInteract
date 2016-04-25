using System;

namespace TcpInteract
{
    /// <summary>
    /// Represents a command and the data that corresponds to the command.
    /// </summary>
    [Serializable]
    public class Package
    {
        /// <summary>
        /// Gets the command that denotes what the package contains and what should be done
        /// in response to the package being received.
        /// </summary>
        public int Command { get; }

        /// <summary>
        /// Gets the content of the package (the type is determined by the command).
        /// </summary>
        public byte[] Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="command">The command/interaction to specify the meaning 
        /// of the package and what is in the contents.</param>
        /// <param name="content">The contents of the package.</param>
        public Package(int command, byte[] content) : this(command)
        {
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="command">The command/interaction to specify the meaning 
        /// of the package and what is in the contents.</param>
        /// <param name="serializable">The contents of the package.</param>
        public Package(int command, ISerializable serializable) : this(command)
        {
            Content = serializable.Serialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="command">The command to specify the meaning of the package and what is in the contents.</param>
        public Package(int command)
        {
            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="command">The command to specify the meaning of the package and what is in the contents.</param>
        internal Package(BaseCommands command)
                 : this((int)command) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class with the specified arguments.
        /// </summary>
        /// <param name="command">The command/interaction to specify the meaning of the package and what is in the contents.</param>
        /// <param name="content">The contents of the package (the type is determined by the Interaction).</param>
        internal Package(BaseCommands command, byte[] content)
            : this(command)
        {
            Content = content;
        }

        /// <summary>
        /// Serializes the instance into bytes.
        /// </summary>
        public byte[] Serialize()
        {
            byte[] tempData = BitConverter.GetBytes(Command);

            if (Content != null)
                tempData = tempData.Append(Content);

            return BitConverter.GetBytes(tempData.Length).Append(tempData);

        }
    }
}
