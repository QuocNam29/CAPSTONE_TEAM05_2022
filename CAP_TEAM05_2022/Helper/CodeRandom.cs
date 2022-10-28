using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAP_TEAM05_2022.Helper
{
    public class CodeRandom
    {
        public static string RandomCode()
        {
            string code = "";
            Random random = new Random();
            for (int i = 0; i < 9; i++)
            {
                int Numrd = random.Next(1,4);
                if (Numrd == 1)
                {
                    code += random.Next(1, 10).ToString();
                }
                else if (Numrd == 2)
                {
                    code += Convert.ToString((char)random.Next(65, 90));
                }
                else
                {
                    code += Convert.ToString((char)random.Next(97, 122));
                }
            }
            return code;
        }
    }
}