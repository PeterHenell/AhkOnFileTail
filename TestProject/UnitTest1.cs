using StdinToOut;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        class MockManager : IAHKManager
        {
            public string LatestCommand { get; private set; } = "";

            public void SendCommand(string command)
            {
                Console.WriteLine(command);
                this.LatestCommand = command;
            }
        }

        MockManager mockAHK = new MockManager();

        static string nonRegexpPattern = @"
[ 
	{
		""pattern"": ""You feel yourself starting to appear"",
		""commands"": [""Send, {k}""]
	}
] ";
        static string regexpPattern = @"
[ 
	{
		""pattern"": ""(.*) feel yourself starting to appear"",
		""commands"": [""Send, {@1}""]
	}
] ";

        [TestMethod]
        public void ShouldParseRegularCommand()
        {
         
            //var am = TriggerManager.FromText(nonRegexpPattern, mockAHK);
            //am.CheckAndAct("You feel yourself starting to appear");

            //Assert.AreEqual("Send, {k}", mockAHK.LatestCommand);
        }

        [TestMethod]
        public void ShouldParseRegularExpressionCommand()
        {

            //var am = TriggerManager.FromText(regexpPattern, mockAHK);
            //am.CheckAndAct("Peter feel yourself starting to appear");

            //Assert.AreEqual("Send, {Peter}", mockAHK.LatestCommand);
        }
    }
}