using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todos.service.Common
{
    public class AzureKeyVault
    {
        public string KeyVaultName { get; set; }
        public string KeyVaultUrl { get; set; }
        public string TodoDbSecretName { get; set; }
    }
}
