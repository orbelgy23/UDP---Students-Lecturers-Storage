using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Students
{
    public class StudentQuery
    {
        public int FirstDegreeFaculty { get; set; }
        public int Grade { get; set; }
        public int SecondDegree { get; set; }

        public override string ToString() 
        {
            return $"{FirstDegreeFaculty},{Grade},{SecondDegree}";
        }

    }
}
