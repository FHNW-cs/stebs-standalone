namespace Stebs.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rhino.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Stebs.ViewModel;

    /// <summary>
    /// Test class to test the raw parser
    /// </summary>
    /// 
    [TestClass]
    class RawParsertTest
    {
        private RawParser parser;
        private MockRepository mockRepo;
        private StreamReader rom1;
        private StreamReader rom2;
        private StreamReader instructions;

        [TestInitialize]
        public void setup()
        {
            mockRepo = new MockRepository();
            rom1 = mockRepo.DynamicMock<StreamReader>();
            rom2 = mockRepo.DynamicMock<StreamReader>();
            instructions = mockRepo.DynamicMock<StreamReader>();
            parser = new RawParser(rom1, rom2, instructions);
        }



        [TestMethod]
        public void TestRom1MPM_Adress_ExpectDigit()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("12300000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            Assert.IsTrue(processor.mpmEntries.ContainsKey("123".HexToInt()));
            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("123".HexToInt(), entry.Addr);
        }

        [TestMethod]
        public void TestRom1MPM_Ev_ExpectEvTrue()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00010000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.IsTrue(entry.Ev);

        }

        [TestMethod]
        public void TestRom1MPM_Cif_ExpectCifTrue()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00020000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.IsTrue(entry.Cif);

        }

        [TestMethod]
        public void TestRom1MPM_Na_ExpectDecoderEntryFetch()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00040000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FETCH", entry.GetName(entry.Na));
        }

        [TestMethod]
        public void TestRom1MPM_Na_ExpectDecoderEntryDecode()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00080000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("DECODE", entry.GetName(entry.Na));
        }

        [TestMethod]
        public void TestRom1MPM_Na_ExpectDecoderEntryNext()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("000C0000");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("NEXT", entry.GetName(entry.Na));
        }

        [TestMethod]
        public void TestRom1MPM_Val_ExpectIntValue()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("0000FFF0");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual(4095, entry.Val);
        }

        [TestMethod]
        public void TestRom1MPM_Crit_ExpectNz()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00000001");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("NZ", entry.GetName(entry.Crit));
        }
        [TestMethod]
        public void TestRom1MPM_Crit_ExpectNo()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00000002");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("NO", entry.GetName(entry.Crit));
        }
        [TestMethod]
        public void TestRom1MPM_Crit_ExpectNs()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00000003");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("NS", entry.GetName(entry.Crit));
        }


        [TestMethod]
        public void TestRom1MPM_Crit_ExpectO()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00000005");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("O", entry.GetName(entry.Crit));
        }

        [TestMethod]
        public void TestRom1MPM_Crit_ExpectS()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("00000006");
            PrepareMocks(builder.ToString(), String.Empty, String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("S", entry.GetName(entry.Crit));
        }

        [TestMethod]
        public void TestRom2MPM_Adress_ExpectDigit()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual(744, entry.Addr);
        }


        [TestMethod]
        public void TestRom2MPM_Alu_ExpectAdd()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E81000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("ADD", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectSub()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E82000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("SUB", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectMul()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E83000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("MUL", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectDiv()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E84000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("DIV", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectMod()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E85000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("MOD", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectDec()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E86000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("DEC", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectInc()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E87000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("INC", entry.GetName(entry.Alu));
        }


        [TestMethod]
        public void TestRom2MPM_Alu_ExpectOr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E88000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("OR", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectXor()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E89000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("XOR", entry.GetName(entry.Alu));
        }


        [TestMethod]
        public void TestRom2MPM_Alu_ExpectNot()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8A000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("NOT", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectAnd()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8B000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("AND", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectShr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8C000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("SHR", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectShl()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8D000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("SHL", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectRor()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8E000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("ROR", entry.GetName(entry.Alu));
        }

        [TestMethod]
        public void TestRom2MPM_Alu_ExpectRol()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E8F000");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("ROL", entry.GetName(entry.Alu));
        }


        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToY()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80100");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_Y", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToX()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80200");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_X", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToSr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80300");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_SR", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToSelRef()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80400");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_SEL_REF", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToSel()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80500");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_SEL", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToRes()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80600");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_RES", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToMdr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80700");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_MDR", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToMbr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80800");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_MBR", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToMar()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80900");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_MAR", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToIr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80A00");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_IR", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Dest_ExpectToIp()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80B00");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("TO_IP", entry.GetName(entry.Dst));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromSr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80030");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_SR", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromSelRes()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80040");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_SEL_REF", entry.GetName(entry.Src));
        }


        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromRes()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80060");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_RES", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromMdr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80070");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_MDR", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromMbr()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80080");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_MBR", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromMar()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80090");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_MAR", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromIp()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E800B0");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_IP", entry.GetName(entry.Src));
        }

        [TestMethod]
        public void TestRom2MPM_Src_ExpectFromData()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E800C0");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.AreEqual("FROM_DATA", entry.GetName(entry.Src));
        }


        [TestMethod]
        public void TestRom2MPM_Rw_ExpectTrue()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80008");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.IsTrue(entry.Rw);
        }

        [TestMethod]
        public void TestRom2MPM_IoMem_ExpectTrue()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendLine("v2.0 raw");
            builder.AppendLine("2E800000");
            builder2.AppendLine("v2.0 raw");
            builder2.AppendLine("02E80004");
            PrepareMocks(builder.ToString(), builder2.ToString(), String.Empty);

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyMPMEntries(processor, 1);

            MPMEntry entry = processor.mpmEntries.First().Value;

            Assert.IsTrue(entry.IoM);
        }

        [TestMethod]
        public void TestInstr_Opcode_ExpectDigit()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.AreEqual(10, entry.OpCode);
        }

        [TestMethod]
        public void TestInstr_Mnemonic_ExpectString()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual("Test", instrType.Mnemonic);
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndRegToIndAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |reg|,|addr|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]        
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndRegAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |reg|,addr");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndRegReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |reg|,reg");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndRegOff()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |reg|,offset");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndRegConst()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |reg|,const");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndAddrIndReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |addr|,|reg|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndAddrReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |addr|,reg");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndAddrAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |addr|,addr");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(1));
        }


        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndAddrOffset()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |addr|,offset");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectIndAddrConst()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test |addr|,const");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(1));
        }


        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectRegIndAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test reg,|addr|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectRegIndReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test reg,|reg|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectRegAdr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test reg,addr");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectRegOffset()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test reg,offset");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectRegConst()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test reg,const");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(1));
        }


        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectAddrIndAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test addr,|addr|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectAddrIndReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test addr,|reg|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectAddrReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test addr,reg");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectAddrOffset()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test addr,offset");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectAddrConst()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test addr,const");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectOffsetIndAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test offset,|addr|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectOffsetIndReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test offset,|reg|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectOffsetReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test offset,reg");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectOffsetAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test offset,addr");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectOffsetConst()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test offset,const");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(1));
        }


        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectConstIndAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test const,|addr|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectConstIndReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test const,|reg|");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectConstReg()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test const,reg");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.REGISTER, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectConstAddr()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test const,addr");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.ADDRESS, instrType.GetOperandType(1));
        }

        [TestMethod]
        public void TestInstr_IndirectRegIndirectAddr_ExpectConstOffset()
        {
            StringBuilder rom1 = new StringBuilder();
            rom1.AppendLine("v2.0 raw");
            rom1.AppendLine("00000000");

            StringBuilder instr = new StringBuilder();
            instr.Append("000;0A;Test const,offset");
            PrepareMocks(rom1.ToString(), String.Empty, instr.ToString());

            Processor processor = parser.Parse();
            mockRepo.VerifyAll();
            BaseVerifyDecoderEntreis(processor, 1);

            List<DecoderEntry> entries = processor.DecoderEntries;
            DecoderEntry entry = entries.First();
            Assert.IsNotNull(entry.InstructionType);
            InstructionType instrType = entry.InstructionType;

            Assert.AreEqual(OperandType.CONST, instrType.GetOperandType(0));
            Assert.AreEqual(OperandType.OFFSET, instrType.GetOperandType(1));
        }


        /// <summary>
        /// Helping method which verifies that the processor is not null,
        /// has a list of decoderentries with an expected number of expected entries
        /// </summary>
        /// <param name="processor">the processor which contains the decoder entries</param>
        /// <param name="expectedSize">the number of expected entries</param>
        private void BaseVerifyDecoderEntreis(Processor processor, int numberOfEntries)
        {
            Assert.IsNotNull(processor);
            List<DecoderEntry> entries = processor.DecoderEntries;
            Assert.IsNotNull(entries);
            Assert.AreEqual(numberOfEntries, entries.Count());
        }


        private void PrepareMocks(string r1, string r2, string i)
        {
            rom1.Expect(x => x.ReadToEnd()).Return(r1).Repeat.Any();
            rom2.Expect(x => x.ReadToEnd()).Return(r2).Repeat.Any();
            instructions.Expect(x => x.ReadToEnd()).Return(i).Repeat.Any();
            mockRepo.ReplayAll();
        }


        /// <summary>
        /// Helping method which verifies that the processor is not null,
        /// has a list of mpmentries with an expected number of entries
        /// </summary>
        /// <param name="processor">The processor which contains the mpmentries</param>
        /// <param name="expectedNumber">The number of expected entries</param>
        private void BaseVerifyMPMEntries(Processor processor, int expectedNumber)
        {
            Assert.IsNotNull(processor);
            Dictionary<int, MPMEntry> mpmEntries = processor.mpmEntries;
            Assert.IsNotNull(mpmEntries);
            Assert.AreEqual(expectedNumber, mpmEntries.Count());
        }

    }
}
