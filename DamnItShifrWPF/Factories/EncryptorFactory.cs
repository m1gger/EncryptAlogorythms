using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Factories
{
   public class EncryptorFactory
    {
        public IEncrypter CreateCaesarCipher(string text,int key,string alphabet) 
        {
            return new CaesarCipherService(text,key,alphabet);
        }

        public IEncrypter CreateDictionaryCipher(string text, string key, string alphabet) 
        {
            return new DictionaryCipherService(text, alphabet, key);
        }

        public IEncrypter CreateTrithemiusCipher(string text, string key, string alphabet) 
        {
            return new TrithemiusCipherService(text,key,alphabet);
        }
    }
}
