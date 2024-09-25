namespace todos.api.Common
{
    public class AzureKeyVault
    {
        public string KeyVaultName { get; set; }
        public string KeyVaultUrl { get; set; }
        public string TodoDbSecretName { get; set; }
    }
}
