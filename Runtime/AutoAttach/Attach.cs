namespace RDTools.AutoAttach
{
    /// <summary>
    /// Specifies the attach modes for auto-attachment functionality.
    /// </summary>
    public enum Attach : byte
    {
        /// <summary>
        /// Default attachment mode.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Attach to the child object.
        /// </summary>
        Child = 1,

        /// <summary>
        /// Attach to the parent object.
        /// </summary>
        Parent = 2,

        /// <summary>
        /// Attach to the scene.
        /// </summary>
        Scene = 3,
    }
}