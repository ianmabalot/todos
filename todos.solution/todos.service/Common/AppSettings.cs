using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todos.service.Common
{
    public class AppSettings
    {
        public AzureCredentials AzureCredentials { get; set; }
        public AzureKeyVault AzureKeyVault { get; set; }
        public FileDirectory FileDirectory { get; set; }
    }
}
