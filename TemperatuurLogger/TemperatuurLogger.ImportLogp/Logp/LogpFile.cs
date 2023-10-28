namespace TemperatuurLogger.ImportLogp
{

    /// <summary>
    /// Decoded logp file.
    /// </summary>
    public class LogpFile
    {
        #pragma warning disable 1591
        public LogpFileHeader Header { get; set; }

        public LogpFileItem[] Items { get; set; }
    }
}