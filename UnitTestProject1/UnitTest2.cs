using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    class UnitTest2
    {
        public virtual string Path { get; set; }

        public void Parent()
        {
            Console.WriteLine(Path);
        }
    }

    class UnitTest2_1 : UnitTest2
    {
        public override string Path => base.Path;

        public UnitTest2_1()
        {
            Path = "Kusa";
            Parent();
        }
    }
}
