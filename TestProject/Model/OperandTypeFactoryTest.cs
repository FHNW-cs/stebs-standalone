namespace Stebs.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    class OperandTypeFactoryTest
    {
        [TestMethod]
        public void TestTypeConstValue() {
           OperandType result = OperandTypeFactory.GetTypeForName("const");
           Assert.AreEqual(OperandType.CONST, result);
        }

        [TestMethod]
        public void TestTypeOffsetValue()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("offset");
            Assert.AreEqual(OperandType.OFFSET, result);
        }

        [TestMethod]
        public void TestTypeAddr()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("addr");
            Assert.AreEqual(OperandType.ADDRESS, result);
        }

        [TestMethod]
        public void TestTypeReg()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("reg");
            Assert.AreEqual(OperandType.REGISTER, result);
        }

        [TestMethod]
        public void TestTypeIndirectAddr()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|addr|");
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, result);
        }

        [TestMethod]
        public void TestTypeIndirectReg()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|reg|");
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidType()
        {
            OperandTypeFactory.GetTypeForName("|Invalid|");
        }

        [TestMethod]
        public void TestIsTrimmed()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("         |reg|          ");
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, result);
        }

        [TestMethod]
        public void TestIgnoreCase()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|aDDr|");
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, result);
        }
    }
}
