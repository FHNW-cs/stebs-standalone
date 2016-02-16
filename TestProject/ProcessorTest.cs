namespace TestProject
{
    using System;
    using MbUnit.Framework;
    using Stebs.Model;
    using Stebs.ViewModel;


    [TestFixture]
    public class ProcessorTest
    {
        private Processor processor;
        private Alu alu;

        [SetUp]
        public void ClassInit() {
            processor = new Processor();
            alu = new Alu(processor);
        }
        
        [Test]
        public void TestADD() {
            alu.X = "01000000".BinToByte();
            alu.Y =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.ADD, true);
            Assert.AreEqual("01000001".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X = "10000001".BinToByte();
            alu.Y = "01111111".BinToByte();
            alu.Execute(Alu.Cmd.ADD, true);

            Assert.AreEqual("00000000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(true, processor.Z);

            alu.X =  "01111111".BinToByte();
            alu.Y =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.ADD, true);
            Assert.AreEqual("10000000".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X =  "10000000".BinToByte();
            alu.Y =  "11111111".BinToByte();
            alu.Execute(Alu.Cmd.ADD, true);
            Assert.AreEqual("01111111".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestSUB() {
            alu.X =  "01111111".BinToByte();
            alu.Y =  "01111111".BinToByte();
            alu.Execute(Alu.Cmd.SUB, true);
            Assert.AreEqual("00000000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(true, processor.Z);

            alu.X =  "00000000".BinToByte();
            alu.Y =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.SUB, true);
            Assert.AreEqual("11111111".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X =  "10000000".BinToByte();
            alu.Y =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.SUB, true);
            Assert.AreEqual("01111111".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X =  "01111111".BinToByte();
            alu.Y =  "11111111".BinToByte();
            alu.Execute(Alu.Cmd.SUB, true);
            Assert.AreEqual("10000000".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestMUL() {
            alu.X =  "00000000".BinToByte();
            alu.Y =  "00000000".BinToByte();
            alu.Execute(Alu.Cmd.MUL, true);
            Assert.AreEqual("00000000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(true, processor.Z);

            alu.X =  "00000001".BinToByte();
            alu.Y =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.MUL, true);
            Assert.AreEqual("00000001".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X =  "01111111".BinToByte();
            alu.Y =  "00000010".BinToByte();
            alu.Execute(Alu.Cmd.MUL, true);
            Assert.AreEqual("11111110".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestDIV() {
            alu.X =  "00000010".BinToByte();
            alu.Y =  "00000010".BinToByte();
            alu.Execute(Alu.Cmd.DIV, true);
            Assert.AreEqual("00000001".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }
        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDIVException() {
            alu.X =  "00000001".BinToByte();
            alu.Y =  "00000000".BinToByte();
            alu.Execute(Alu.Cmd.DIV, true);
        }

        [Test]
        public void TestMOD() {
            alu.X =  "00000010".BinToByte();
            alu.Y =  "00000010".BinToByte();
            alu.Execute(Alu.Cmd.MOD, true);
            Assert.AreEqual("00000000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(true, processor.Z);
        }

        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestMODException() {
            alu.X =  "00000001".BinToByte();
            alu.Y =  "00000000".BinToByte();
            alu.Execute(Alu.Cmd.MOD, true);
        }

        [Test]
        public void TestDEC() {
            alu.X =  "00000001".BinToByte();
            alu.Execute(Alu.Cmd.DEC, true);
            Assert.AreEqual("00000000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(true, processor.Z);

            alu.X =  "10000000".BinToByte();
            alu.Execute(Alu.Cmd.DEC, true);
            Assert.AreEqual("01111111".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestINC() {
            alu.X =  "00000000".BinToByte();
            alu.Execute(Alu.Cmd.INC, true);
            Assert.AreEqual("00000001".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);

            alu.X =  "01111111".BinToByte();
            alu.Execute(Alu.Cmd.INC, true);
            Assert.AreEqual("10000000".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(true, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestOR() {
            alu.X =  "10101010".BinToByte();
            alu.Y =  "01010101".BinToByte();
            alu.Execute(Alu.Cmd.OR, true);
            Assert.AreEqual("11111111".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestXOR() {
            alu.X =  "10001001".BinToByte();
            alu.Y =  "11011101".BinToByte();
            alu.Execute(Alu.Cmd.XOR, true);
            Assert.AreEqual("01010100".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestNOT() {
            alu.X =  "10010011".BinToByte();
            alu.Execute(Alu.Cmd.NOT, true);
            Assert.AreEqual("01101100".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestAND() {
            alu.X =  "11110000".BinToByte();
            alu.Y =  "00110011".BinToByte();
            alu.Execute(Alu.Cmd.AND, true);
            Assert.AreEqual("00110000".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestSHR() {
            alu.X =  "10010011".BinToByte();
            alu.Execute(Alu.Cmd.SHR, true);
            Assert.AreEqual("01001001".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestSHL() {
            alu.X =  "10010011".BinToByte();
            alu.Execute(Alu.Cmd.SHL, true);
            Assert.AreEqual("00100110".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestROR() {
            alu.X =  "10010011".BinToByte();
            alu.Execute(Alu.Cmd.ROR, true);
            Assert.AreEqual("11001001".BinToByte(), alu.Res);
            Assert.AreEqual(true, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }

        [Test]
        public void TestROL() {
            alu.X =  "10010011".BinToByte();
            alu.Execute(Alu.Cmd.ROL, true);
            Assert.AreEqual("00100111".BinToByte(), alu.Res);
            Assert.AreEqual(false, processor.S);
            Assert.AreEqual(false, processor.O);
            Assert.AreEqual(false, processor.Z);
        }
    }
}
