using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPA
{
    class Linter
    {
        public static string[] Lint(string[] file)
        {
            return Lint(new List<string>(file)).ToArray();
        }


        public static List<string> Lint(List<string> file)
        {
            List<string> data = file;
            for(int i = 0; i < file.Count; i++)
            {
                //Change tabs to spaces
                data[i] = data[i].Replace('\t', ' ');

                //Remove extra spaces
                data[i] = data[i].Trim();

                //Remove empty lines
                if(data[i] == "")
                {
                    data.RemoveAt(i);
                    i--;
                }else if (data[i].StartsWith("//")) //Remove Comments
                {
                    data.RemoveAt(i);
                    i--;
                }


            }
            return data;
        }

    }
}
