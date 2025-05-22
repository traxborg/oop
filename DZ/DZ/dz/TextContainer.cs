using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ
{
    internal class TextContainer
    {
        private List<string> lines = new List<string>();

       
        public int LineCount
        {
            get { return lines.Count; }
        }

        
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < lines.Count)
                    return lines[index];
                else
                    throw new IndexOutOfRangeException("Недійсний індекс рядка.");
            }
            set
            {
                if (index >= 0 && index < lines.Count)
                    lines[index] = value;
                else
                    throw new IndexOutOfRangeException("Недійсний індекс рядка.");
            }
        }

        
        public void AddLine(string line)
        {
            lines.Add(line);
        }

        
        public void Clear()
        {
            lines.Clear();
        }
    }
}

