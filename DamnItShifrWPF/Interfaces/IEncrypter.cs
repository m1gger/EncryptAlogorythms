﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Interfaces
{
    public interface  IEncrypter
    {
        string Encrypt();

        string Decrypt();

        (int, string) Hack();
    }

}
