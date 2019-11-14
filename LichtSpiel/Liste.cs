using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LichtSpiel
{
    public class Liste<T> : List<T>
    {
        public int Laenge 
        {
            get { return this.Count; }
        }

        internal void Hinzufuegen(T value)
        {
            base.Add(value);
        }
    }
}
