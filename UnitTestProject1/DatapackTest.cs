using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server_GUI2;

namespace UnitTestProject1
{
    [TestClass]
    public class DatapackTest
    {
        [TestMethod]
        public void ImportDp()
        {
            Version version = Version.TryGetInstance("1.16.5");
            World world = World.TryGetInstance("TestWorld", version);
            ImportDatapack datapack = ImportDatapack.TryGenInstance(world, "BoostMinecart", @"D:\Games\Minecraft\Datapacks\boost_minecart", false);
            if (datapack == null)
            {
                System.Console.WriteLine("Invalid Datapack");
                return;
            }
            datapack.Ready();
        }
    }
}
