namespace XAMLDebuggingTechniques.Services
{
    /// <summary>
    /// Supplies the SQLCipher encryption key—keep this one tight like your leash.
    /// </summary>
    public interface IEncryptionKeyProvider
    {
        /// <summary>
        /// Retrieve the SQLCipher passphrase.
        /// </summary>
        /// <remarks>
        /// Production: derive per device/session or fetch from secure storage / keystore.
        /// </remarks>
        string GetKey();
    }
}