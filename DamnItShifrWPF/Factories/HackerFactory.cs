using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Models;
using DamnItShifrWPF.Services.CipherHackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Factories
{
    public  class HackerFactory
    {
        public static IHacker CreateHacker(CipherTypeEnum cipherType,IEncrypter encrypter) 
        {
            switch (cipherType) 
            {
                case CipherTypeEnum.CaesarCipher:
                    return new CaesarHacker(encrypter);

                case CipherTypeEnum.HillCipher:
                    return new HillHacker(encrypter);
                default:
                    throw new ArgumentException("Ошибка! Не существует взломщиика для этого метода шифрования");
            }
        }
    }
}
