using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stebs.Model;
using Stebs.ViewModel;
using Rhino.Mocks;
using Stebs.View;
using System.IO;

namespace Stebs.Model
{
    [TestClass]
    class AssemblerParserTest
    {
        private AssemblerParser parser;
        private ProcessorViewModel model;
        private MockRepository mockRepo;
        private IOutputWindow outWindow;

        [TestInitialize]
        public void Setup() {
            mockRepo = new MockRepository();
            model = mockRepo.DynamicMock<ProcessorViewModel>(GetProcessorParser().Parse());
            outWindow = mockRepo.DynamicMock<IOutputWindow>();
            parser = new AssemblerParser(model, outWindow);
        }

        /// <summary>
        /// Successfully parse something that has an end at the end of the soruce code
        /// </summary>
        [TestMethod]
        public void TestEndExisits()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,BL\n");
            builder.Append("End");
            bool parseResut = parser.Assemble(builder.ToString());
            
            mockRepo.Verify(outWindow);
            Assert.IsTrue(parseResut);
        }
        
        /// <summary>
        /// Tests that an error happens if there's no end at all(even no label that is equal)
        /// </summary>
        [TestMethod]
        public void TestNoEnd() {
            model.Expect(x => x.Reset());
            StringBuilder builder = new StringBuilder();

            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();
            
            builder.Append("Start:\n");
            builder.Append("MOV AL,BL\n");
            builder.Append("Schnurpsel:");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            mockRepo.Verify(outWindow);
            Assert.IsFalse(parseResult);
            Assert.AreEqual("Line 4: Expected END at the end of the source code.", argsMade[0][0]);
        }
        
        /// <summary>
        /// Test that a label that contains the expression "End" does not influence the
        /// error message.
        /// </summary>
        [TestMethod]
        public void TestLabelLikeEnd()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,BL\n");
            builder.Append("MyEnd:\n");
            builder.Append("Org 10\n");
            builder.Append("Halt"); // But no end

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);
            Assert.AreEqual("Line 6: Expected END at the end of the source code.", argsMade[0][0]);
        }
        

        /// <summary>
        /// Test that an End between the code is possible
        /// </summary>
        public void TestEndBetween()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,BL\n");
            builder.Append("End");
            builder.Append("Org 10");
            builder.Append("Halt"); // But no end

            bool parseResult = parser.Assemble(builder.ToString());
            Assert.IsTrue(parseResult);
            mockRepo.Verify(outWindow);
        }

        /// <summary>
        /// Tests that an unknown instruction is correctly detected
        /// </summary>
        [TestMethod]
        public void TestUnknownInstruction()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("XYZ\n");
            builder.Append("END\n");

            bool parseResult = parser.Assemble(builder.ToString());
            
            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);
            Assert.AreEqual("Line 1: Error, expected a comment, label or machine instruction, got \"XYZ\"", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #3 Missing comma between parameters
        /// </summary>
        [TestMethod]
        public void TestCommaBetweenParameters()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL 09\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
            
            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected ',' between AL and 09.\nOn Line: MOV AL 09", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #4 Missing comma between parameters with indirect addressing(type read)
        /// </summary>
        [TestMethod]
        public void TestCommaBetweenParameterWithIndirectAddressingRead()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL [BL]\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected ',' between AL and [BL].\nOn Line: MOV AL [BL]", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #4 Missing comma between parameters with indirect addressing(type write)
        /// </summary>
        [TestMethod]
        public void TestCommaBetweenParameterWithIndirectAddressingWrite()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV [AL] BL\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected ',' between [AL] and BL.\nOn Line: MOV [AL] BL", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #5 Missing comma between parameters with an illegal parameter
        /// </summary>
        [TestMethod]
        public void TestCommaBetweenIllegalParameters()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL nonsense\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Error: Expected a hexadecimal number or AL, BL, CL, DL - found \"NONSENSE\"\nOn Line: MOV AL NONSENSE", argsMade[0][0]);
        }

        /// <summary>
        /// Test illegal parameter
        /// </summary>
        [TestMethod]
        public void TestIllegalParameters()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL, nonsense\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Error: Expected a hexadecimal number or AL, BL, CL, DL - found \"NONSENSE\"\nOn Line: MOV AL, NONSENSE", argsMade[0][0]);
        }

        /// <summary>
        /// Tests Register Indirect Addressing
        /// </summary>
        [TestMethod]
        public void TestRegisterIndirectAddressing()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,15\n"); 
            builder.Append("MOV CL,50\n");
            builder.Append("MOV [CL],AL\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }

        /// <summary>
        /// Test Nothing to assemble. Empty code
        /// </summary>
        [TestMethod]
        public void TestNothingToAssemble()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("                ");
            builder.Append("\n");
            builder.Append("\n");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Error, nothing to assemble", argsMade[0][0]);
        }


        /// <summary>
        /// Ticket #23 Expected Register or Number
        /// </summary>
        [TestMethod]
        public void TestExpectedRegisterOrNumber()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,[st]\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Error: Expected a hexadecimal number or AL, BL, CL, DL - found \"ST\"\nOn Line: MOV AL,[ST]", argsMade[0][0]);
        }


        /// <summary>
        /// Ticket #23 Expected Register or Number
        /// </summary>
        [TestMethod]
        public void TestExpectedRegisterOrNumberWithAddressRegister()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,[BL]\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }

        /// <summary>
        /// Ticket #23 Expected Register or Number
        /// </summary>
        [TestMethod]
        public void TestExpectedRegisterOrNumberWithAddressNumber()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,[FF]\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        /// <summary>
        /// Ticket #23 Expected Register or Number
        /// </summary>
        [TestMethod]
        public void TestExpectedRegisterOrNumberWithAdress()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,BL\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        /// <summary>
        /// Ticket #23 Expected Register or Number
        /// </summary>
        [TestMethod]
        public void TestExpectedRegisterOrNumberWithNumber()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,FF\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        
        /// <summary>
        /// Ticket #17 Jump cannot exceed -127..+128
        /// </summary>
        [TestMethod]
        public void TestTooFarPositiveJump()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("ORG D0\n");
            builder.Append("JMP Go\n");
            builder.Append("DIV AL,00\n");
            builder.Append("\n");
            builder.Append("ORG 00\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 3: Jump cannot exceed -127..+128(Hex 80..7F): \"GO\" too far.", argsMade[0][0]);
        }

        public void TestMaxPositiveJump() 
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("JMP Go\n");
            builder.Append("DIV AL,00\n");
            builder.Append("\n");
            builder.Append("ORG 80\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        [TestMethod]
        public void TestPositiveJump()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,1\n");
            builder.Append("JMP Go\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }



        [TestMethod]
        public void TestNegativeJump()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,1\n");
            builder.Append("JMP Go\n");
            builder.Append("Go:\n");
            builder.Append("JMP Start\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }

        /// <summary>
        /// Ticket #17 Jump cannot exceed -127..+128
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestMaxNegativeJump()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,0");
            builder.Append("Stop:\n");
            builder.Append("JMP Start");
            builder.Append("END");
            

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("All your base are belong to us", argsMade[0][0]);
        }

        /// <summary>
        /// Checklabel(no ":" at the end)
        /// </summary>
        [TestMethod]
        public void TestJumpToNotValidLabel()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,12\n");
            builder.Append("JMP XLY:\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 3: The label should begin with a character and end without \":\".\nOn Line: JMP XLY:", argsMade[0][0]);
        }


        /// <summary>
        /// Jump to non-existent Label
        /// </summary>
        [TestMethod]
        public void TestJumpToNotExistingLabel()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL,12\n");
            builder.Append("JMP XLY\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 3: Jump to non-existent label, searched for \"XLY:\"\nOn Line: JMP XLY", argsMade[0][0]);
        }


        /// <summary>
        /// Test Used RAM
        /// </summary>
        [TestMethod]
        public void TestUsedRAM()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("ORG 00\n");
            builder.Append("MOV AL,12\n");
            builder.Append("ORG 00\n");
            builder.Append("JMP Go\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 4: Cannot override code already generated. Conflict at address \"00\"\nOn Line: ORG 00", argsMade[0][0]);
        }

        /// <summary>
        /// Test End of RAM
        /// </summary>
        [TestMethod]
        public void TestEORAM()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("ORG FE\n");
            builder.Append("JMP Go\n");
            builder.Append("Go:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Cannot generate code beyond the end of RAM.", argsMade[0][0]);
        }


        [TestMethod]
        public void TestIncNotRegister()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC 99\n");
            builder.Append("Stop:\n");
            builder.Append("END");
            

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Error: Expected AL, BL, CL, DL - found \"99\"\nOn Line: INC 99", argsMade[0][0]);
        }


        [TestMethod]
        public void TestIncRegister()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC BL\n");
            builder.Append("Stop:\n");
            builder.Append("END");


            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        /// <summary>
        /// Ticket #15 The label should begin with a character
        /// </summary>
        [TestMethod]
        public void TestLabelStartsWithNumber()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("9Start:\n");
            builder.Append("MOV AL 09\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 1: The label should begin with a character, got 9START", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #12 Dublicate labels are not allowed
        /// </summary>
        [TestMethod]
        public void TestLabelDublicateLabels()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL 09\n");
            builder.Append("LABEL:\n");
            builder.Append("LABEL:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 4: Dublicate labels are not allowed, found multiple LABEL:\nLabel on Line:3", argsMade[0][0]);
        }

        /// <summary>
        /// Asserts that if a number is bigger that FF then this is indicated as error
        /// </summary>
        [TestMethod]
        public void TestNumberTooBig()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("CALL 999\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected a hexadecimal number 00..FF, got \"999\".", argsMade[0][0]);
        }

        /// <summary>
        /// Asserts that if there's an even number of quotes, there's no error
        /// </summary>
        [TestMethod]
        public void TestNoMissingQuote()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB \"jj\"\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
           
            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
            mockRepo.Verify(outWindow);

        }

        /// <summary>
        /// Tests that if a quote sign is missing, this leads to an error
        /// </summary>
        [TestMethod]
        public void TestMissingQuote()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB \"jj\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 1: Missing closing quote, got \"JJ.", argsMade[0][0]);
        }


        /// <summary>
        ///  Ticket #4 Unexpected Token
        /// </summary>
        [TestMethod]
        public void TestExpectedToken()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC AL\n");
            builder.Append("Stop:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsTrue(parseResult);
        }


        /// <summary>
        /// Ticket #4 Unexpected Token
        /// </summary>
        [TestMethod]
        public void TestUnexpectedToken()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC AL 09\n");
            builder.Append("Stop:\n");
            builder.Append("END");

            bool parseResult = this.parser.Assemble(builder.ToString());

            var argsMade = this.outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            this.mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected a comment, label or machine instruction, got \"09\"\nOn Line: INC AL 09", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #4 Unexpected Token(with Comma)
        /// </summary>
        [TestMethod]
        public void TestUnexpectedTokenWithComma()
        {
            this.model.Expect(x => x.Reset());
            this.outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            this.outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            this.mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC AL,09\n");
            builder.Append("Stop:\n");
            builder.Append("END");

            bool parseResult = this.parser.Assemble(builder.ToString());

            var argsMade = this.outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected a comment, label or machine instruction, got \",\"\nOn Line: INC AL,09", argsMade[0][0]);
        }


        /// <summary>
        /// Ticket #4 Unexpected Token(with Comma)
        /// </summary>
        [TestMethod]
        public void TestUnexpectedTokenWithThreePlusParameters()
        {
            this.model.Expect(x => x.Reset());
            this.outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            this.outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            this.mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("INC AL,09 + 22\n");
            builder.Append("Stop:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 2: Expected a comment, label or machine instruction, got \"+\"\nOn Line: INC AL,09 + 22", argsMade[0][0]);
        }

        /// <summary>
        /// Ensures that DB accepts String parameters
        /// </summary>
        [TestMethod]
        public void TestDBStringParamAccepted()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB \"jj\"\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
            Assert.IsTrue(parseResult);
            
        }

        /// <summary>
        /// Ensures that DB accepts character as parameter
        /// </summary>
        [TestMethod]
        public void TestDBCharParamAccepted()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB 'j'\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
            Assert.IsTrue(parseResult);
        }

        /// <summary>
        /// Ensures that DB accepts numbers as parameters
        /// </summary>
        [TestMethod]
        public void TestDBNumberParamAccepted()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB FF\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
            Assert.IsTrue(parseResult);
        }

        /// <summary>
        /// Ensures that DB invalid parameter
        /// </summary>
        [TestMethod]
        public void TestDBNumberParamInvalid()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("DB jj\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());
            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);

            Assert.AreEqual("Line 1: Expected a hexadecimal number or a text string, got \"JJ\".", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #14 Dublicate labels are not allowed, Case Sensitive
        /// </summary>
        [TestMethod]
        public void TestLabelDublicateLabelsNotCaseSensitive()
        {
            model.Expect(x => x.Reset());
            outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("START:\n");
            builder.Append("MOV AL 09\n");
            builder.Append("LABEL:\n");
            builder.Append("LaBeL:\n");
            builder.Append("END");

            bool parseResult = parser.Assemble(builder.ToString());

            var argsMade = outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            mockRepo.Verify(outWindow);

            Assert.AreEqual("Line 4: Dublicate labels are not allowed, found multiple LABEL:\nLabel on Line:3", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #24 Negative values not allowed
        /// </summary>
        [TestMethod]
        public void TestLabelNegativeValues()
        {
            this.model.Expect(x => x.Reset());
            this.outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            this.outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Once();
            this.mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("START:\n");
            builder.Append("MOV AL, -12\n");
            builder.Append("END");

            bool parseResult = this.parser.Assemble(builder.ToString());

            var argsMade = this.outWindow.GetArgumentsForCallsMadeOn<IOutputWindow>(s => s.WriteError(Arg<string>.Is.Anything), s => s.IgnoreArguments());
            Assert.IsFalse(parseResult);
            this.mockRepo.Verify(this.outWindow);

            Assert.AreEqual("Line 2: Error: Expected a hexadecimal number or AL, BL, CL, DL - found \"-12\"\nOn Line: MOV AL, -12", argsMade[0][0]);
        }

        /// <summary>
        /// Ticket #29 Test label "End:".
        /// </summary>
        [TestMethod]
        public void TestLabelEnd()
        {
            this.model.Expect(x => x.Reset());
            this.outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            this.outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            this.mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL, 09\n");
            builder.Append("END:\n");
            builder.Append("END");

            bool parseResult = this.parser.Assemble(builder.ToString());

            Assert.IsTrue(parseResult);
            this.mockRepo.Verify(this.outWindow);
        }

        /// <summary>
        /// Ticket #29 Test label "Halt:".
        /// </summary>
        [TestMethod]
        public void TestLabelHalt()
        {
            this.model.Expect(x => x.Reset());
            this.outWindow.Expect(x => x.WriteOutput(Arg<string>.Is.Anything)).Repeat.Any();
            this.outWindow.Expect(x => x.WriteError(Arg<string>.Is.Anything)).Repeat.Never();
            this.mockRepo.ReplayAll();

            StringBuilder builder = new StringBuilder();
            builder.Append("Start:\n");
            builder.Append("MOV AL, BL\n");
            builder.Append("END:\n");
            builder.Append("Org 10\n");
            builder.Append("Halt:\n");
            builder.Append("Halt\n");
            builder.Append("End");

            bool parseResult = this.parser.Assemble(builder.ToString());

            Assert.IsTrue(parseResult);
            this.mockRepo.Verify(this.outWindow);
         
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

        private static Stream GetFileStream(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    } 
}