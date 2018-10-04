using System;

public sealed class CustomFieltypeEditorAttribute :Attribute
{
    public string[] extensions { get; private set; }

    /// <summary>
    /// Turns the class into an editor class for the specified filetype
    /// </summary>
    /// <param name="aExtension">Extension (Without the dot)</param>
    public CustomFieltypeEditorAttribute(params string[] aExtension)
    {
        extensions = aExtension;
    }

    public bool HasExtension(string aExtension)
    {
        for (int i = 0; i < extensions.Length; i++)
        {
            if (extensions[i] == aExtension)
                return true;
        }
        return false;
    }
}
