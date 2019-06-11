using System;
using System.Collections.Generic;
using System.Text;

namespace Requete
{
    public class IdentificationException : Exception
    {
        public IdentificationException() : base("Erreur de connection au serveur")
        {
        }

        public IdentificationException(string message)
            : base(message)
        {
        }
    }
}
