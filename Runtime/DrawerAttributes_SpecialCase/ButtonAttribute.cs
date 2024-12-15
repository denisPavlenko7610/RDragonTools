using System;

namespace RDTools
{
    public enum ButtonEnableMode
    {
        /// <summary>
        /// Button is always active.
        /// </summary>
        Always,

        /// <summary>
        /// Button is active only in the editor.
        /// </summary>
        Editor,

        /// <summary>
        /// Button is active only in play mode.
        /// </summary>
        Playmode
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : SpecialCaseDrawerAttribute
    {
        /// <summary>
        /// Gets the button text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the button enable mode.
        /// </summary>
        public ButtonEnableMode EnableMode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="text">The display text for the button. Defaults to null.</param>
        /// <param name="enableMode">The enable mode for the button. Defaults to <see cref="ButtonEnableMode.Always"/>.</param>
        public ButtonAttribute(string text = null, ButtonEnableMode enableMode = ButtonEnableMode.Always)
        {
            Text = text;
            EnableMode = enableMode;
        }
    }
}
