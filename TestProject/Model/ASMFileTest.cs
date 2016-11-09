namespace Stebs.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using Stebs.View;
    using Stebs.ViewModel;
    using System;
    using System.IO;

    [TestClass]
    class ASMFileTest
    {
        private AssemblerParser parser;
        private ProcessorViewModel model;
        private MockRepository mockRepo;
        private IOutputWindow outWindow;

        [TestInitialize]
        public void Setup()
        {
            Processor processor = GetProcessorParser().Parse();

            mockRepo = new MockRepository();
            model = mockRepo.DynamicMock<ProcessorViewModel>(processor);
            outWindow = mockRepo.DynamicMock<IOutputWindow>();
            parser = new AssemblerParser(model, outWindow);
        }

        /// <summary>
        /// Test TestMultipy.asm
        /// </summary>
        public void TestBase(String asmFile, String binFile)
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            System.Console.WriteLine("Test: ");
            bool parseResult = parser.Assemble(asmFile);
            Assert.IsTrue(parseResult);
            mockRepo.Verify(outWindow);

            byte[] expected = ParseMachineCode(binFile); 
            Assert.AreEqual(expected, parser.MachineCode);
        }

        [TestMethod]
        public void TestMultipy()
        {
            TestBase(TestProject.Properties.Resources.multiply,
                     TestProject.Properties.Resources.multiply_bin);
        }

        [TestMethod]
        public void TrafficLightASMTest()
        {
            TestBase(TestProject.Properties.Resources.tLight,
                     TestProject.Properties.Resources.tLight_bin);
        }

        [TestMethod]
        public void FirstAsmTest()
        {
            TestBase(TestProject.Properties.Resources.first,
                     TestProject.Properties.Resources.first_bin);
        }

        [TestMethod]
        public void TestsASMTest()
        {
            TestBase(TestProject.Properties.Resources.tests,
                     TestProject.Properties.Resources.tests_bin);
        }

        [TestMethod]
        public void IncJmpASMTest()
        {
            TestBase(TestProject.Properties.Resources.incJmp,
                     TestProject.Properties.Resources.incJmp_bin);
        }

        [TestMethod]
        public void CliStiASMTest()
        {
            TestBase(TestProject.Properties.Resources.cliSti,
                     TestProject.Properties.Resources.cliSti_bin);
        }

        [TestMethod]
        public void BubbleSort01ASMTst()
        {
            TestBase(
                TestProject.Properties.Resources.BubbleSortTst01,
                TestProject.Properties.Resources.BubbleSortTst01_bin);
        }

        [TestMethod]
        public void SwIntASMTest()
        {
            TestBase(TestProject.Properties.Resources.swInt,
                     TestProject.Properties.Resources.swInt_bin);
        }

        [TestMethod]
        public void MovASMTest()
        {
            TestBase(TestProject.Properties.Resources.move,
                     TestProject.Properties.Resources.move_bin);
        }

        [TestMethod]
        public void ProcASMTest()
        {
            TestBase(TestProject.Properties.Resources.proc,
                     TestProject.Properties.Resources.proc_bin);
        }

        [TestMethod]
        public void ArithmeticsASMTest()
        {
            TestBase(TestProject.Properties.Resources.arithmetics,
                     TestProject.Properties.Resources.arithmetics_bin);
        }

        [TestMethod]
        
        public void HwIntASMTest()
        {
            TestBase(TestProject.Properties.Resources.hw_int,
                     TestProject.Properties.Resources.hw_int_bin1);
        }


        [TestMethod]
        public void CallTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.call_test,
                     TestProject.Properties.Resources.call_test_bin);
        }

        [TestMethod]
        public void CallDemoASMTest()
        {
            TestBase(TestProject.Properties.Resources.callDemo,
                     TestProject.Properties.Resources.callDemo_bin);
        }

        [TestMethod]
        public void ParamASMTest()
        {
            TestBase(TestProject.Properties.Resources.param,
                     TestProject.Properties.Resources.param_bin);
        }

        [TestMethod]
        public void SevSegASMTest()
        {
            TestBase(TestProject.Properties.Resources.sevSeg,
                     TestProject.Properties.Resources.sevSeg_bin);
        }

        [TestMethod]
        public void TLight99ASMTest()
        {
            TestBase(TestProject.Properties.Resources.tLight99,
                     TestProject.Properties.Resources.tLight99_bin);
        }

        [TestMethod]
        public void HeatCoolASMTest()
        {
            TestBase(TestProject.Properties.Resources.heatCool,
                     TestProject.Properties.Resources.heatCool_bin);
        }

        [TestMethod]
        public void CompilerExampleASMTest()
        {
            TestBase(TestProject.Properties.Resources.compilerExample,
                     TestProject.Properties.Resources.compilerExample_bin);
        }

        [TestMethod]
        public void IntTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.int_test,
                     TestProject.Properties.Resources.int_test_bin);
        }

        [TestMethod]
        
        public void DemoASMTest()
        {
            TestBase(TestProject.Properties.Resources.demo,
                TestProject.Properties.Resources.demo_bin);
        }

        [TestMethod]
        public void CompareASMTest()
        {
            TestBase(TestProject.Properties.Resources.compare,
                     TestProject.Properties.Resources.compare_bin);
        }

        [TestMethod]
        public void IntsASMTest()
        {
            TestBase(TestProject.Properties.Resources.ints,
                     TestProject.Properties.Resources.ints_bin);
        }

        [TestMethod]
        
        public void InstructionsASMTest()
        {
            TestBase(TestProject.Properties.Resources.instructions,
                     TestProject.Properties.Resources.instructions_bin);
        }

        [TestMethod]
        public void JsJnsASMTest()
        {
            TestBase(TestProject.Properties.Resources.jsJns,
                     TestProject.Properties.Resources.jsJns_bin);
        }

        [TestMethod]
        
        public void MovesASMTest()
        {
            TestBase(TestProject.Properties.Resources.moves,
                     TestProject.Properties.Resources.moves_bin);
        }

        [TestMethod]
        public void PushPopASMTest()
        {
            TestBase(TestProject.Properties.Resources.PUSH_POP,
                     TestProject.Properties.Resources.PUSH_POP_bin);
        }

        [TestMethod]
        public void LogicASMTest()
        {
            TestBase(TestProject.Properties.Resources.logic,
                     TestProject.Properties.Resources.logic_bin);
        }

        [TestMethod]
        public void StiEtstASMTest()
        {
            TestBase(TestProject.Properties.Resources.sti_etst,
                     TestProject.Properties.Resources.sti_etst_bin);
        }

        [TestMethod]
        public void MovTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.mov_test,
                     TestProject.Properties.Resources.mov_test_bin);
        }

        [TestMethod]
        public void MultitaskerASMTest()
        {
            TestBase(TestProject.Properties.Resources.multitasker,
                     TestProject.Properties.Resources.multitasker_bin);
        }

        [TestMethod]
        public void JzJnzASMTest()
        {
            TestBase(TestProject.Properties.Resources.jzJnz,
                     TestProject.Properties.Resources.jzJnz_bin);
        }

        [TestMethod]
        public void JumpTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.JumpTest,
                     TestProject.Properties.Resources.JumpTest_bin);
        }

        [TestMethod]
        public void MeineTestsASMTest()
        {
            TestBase(TestProject.Properties.Resources.MeineTests,
                     TestProject.Properties.Resources.MeineTests_bin);
        }

        [TestMethod]
        public void StepperASMTest()
        {
            TestBase(TestProject.Properties.Resources.stepper,
                     TestProject.Properties.Resources.stepper_bin);
        }

        [TestMethod]
        public void StackCrashASMTest()
        {
            TestBase(TestProject.Properties.Resources.stackCrash,
                     TestProject.Properties.Resources.stackCrash_bin);
        }

        [TestMethod]
        public void  MeinSyntaxHighlighterASMTest()
        {
            TestBase(TestProject.Properties.Resources.MeinSyntaxHighlightingText,
                     TestProject.Properties.Resources.MeinSyntaxHighlightingText_bin);
        }

        [TestMethod]
        
        public void MazeBinASMTest()
        {
            TestBase(TestProject.Properties.Resources.maze,
                     TestProject.Properties.Resources.maze_bin);
        }

        [TestMethod]
        public void Multitasker2ASMTest()
        {
            TestBase(TestProject.Properties.Resources.multitasker2,
                     TestProject.Properties.Resources.multitasker2_bin);
        }

        [TestMethod]
        public void SwapNibblesASMTest()
        {
            TestBase(TestProject.Properties.Resources.swapNnibbles,
                     TestProject.Properties.Resources.swapNnibbles_bin);
        }

        [TestMethod]
        public void PopPushTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.pop_push_test,
                     TestProject.Properties.Resources.pop_push_test_bin);
        }

        [TestMethod]
        public void LiftASMTest()
        {
            TestBase(TestProject.Properties.Resources.lift,
                     TestProject.Properties.Resources.lift_bin);
        }

        [TestMethod]
        public void SSegHwASMTest()
        {
            TestBase(TestProject.Properties.Resources.ssegHw,
                     TestProject.Properties.Resources.ssegHw_bin);
        }

        [TestMethod]
        public void IvoTestASMTest()
        {
            TestBase(TestProject.Properties.Resources.ivo_tests,
                     TestProject.Properties.Resources.ivo_tests_bin);
        }

        [TestMethod]
        public void JoJnoASMTest()
        {
            TestBase(TestProject.Properties.Resources.joJno,
                     TestProject.Properties.Resources.joJno_bin);
        }

        private static Stream GetFileStream(String filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static IProcessorParser GetProcessorParser()
        {
            StreamReader rom1Reader = null;
            StreamReader rom2Reader = null;
            StreamReader instrReader = null;
            rom1Reader = new StreamReader(GetFileStream(@"res/rom1.data"));
            rom2Reader = new StreamReader(GetFileStream(@"res/rom2.data"));
            instrReader = new StreamReader(GetFileStream(@"res/instruction.data"));

            return new RawParser(rom1Reader, rom2Reader, instrReader);
        }

        private byte[] ParseMachineCode(String machineCode)
        {
            machineCode = machineCode.Replace("\n", "");
            machineCode = machineCode.Replace("\r", "");
            machineCode = machineCode.Replace("\t", " ");
            machineCode = machineCode.Replace("  ", " ");
            machineCode = machineCode.Trim();

            String[] tokens = machineCode.Split(new char[] { ' ' });

            byte[] result = new byte[tokens.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(tokens[i], 16);
            }
            return result;
        }

    }

    
}
