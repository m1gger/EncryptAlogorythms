using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Services;
using DamnItShifrWPF.Utils;
using MathNet.Numerics.LinearAlgebra;
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

        public IEncrypter CreateHillCipher(string text, int matrixsize, string alphabet= "абвгдеёжзийклмнопрстуфхцчшщъыьэюя") 
        {
            Matrix<double> mat=MatrixHelper.GetRandomMatrix(matrixsize);
            do
            {
                mat = MatrixHelper.GetRandomMatrix(matrixsize);

            } 
            while (!MatrixHelper.CheckConstraints(mat, alphabet));
            return new HillCipherService(text, mat, alphabet); 
        }
    }
}
