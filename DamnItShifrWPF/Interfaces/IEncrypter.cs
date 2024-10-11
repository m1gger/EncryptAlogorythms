    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace DamnItShifrWPF.Interfaces
    {
        public interface  IEncrypter
        {
            string EncryptedText { get; set; }
          
            string Alphabet { get; set; }

            string Encrypt();

            string Decrypt();
        }

    }
