namespace Stebs.Model
{
    using MbUnit.Framework;
    using System;

    [TestFixture]
    class OperandTypeFactoryTest
    {
        [Test]
        public void TestTypeConstValue() {
           OperandType result = OperandTypeFactory.GetTypeForName("const");
           Assert.AreEqual(OperandType.CONST, result);
        }

        [Test]
        public void TestTypeOffsetValue()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("offset");
            Assert.AreEqual(OperandType.OFFSET, result);
        }

        [Test]
        public void TestTypeAddr()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("addr");
            Assert.AreEqual(OperandType.ADDRESS, result);
        }

        [Test]
        public void TestTypeReg()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("reg");
            Assert.AreEqual(OperandType.REGISTER, result);
        }

        [Test]
        public void TestTypeIndirectAddr()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|addr|");
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, result);
        }

        [Test]
        public void TestTypeIndirectReg()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|reg|");
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidType()
        {
            OperandTypeFactory.GetTypeForName("|Invalid|");
        }

        [Test]
        public void TestIsTrimmed()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("         |reg|          ");
            Assert.AreEqual(OperandType.INDIRECT_REGISTER, result);
        }

        [Test]
        public void TestIgnoreCase()
        {
            OperandType result = OperandTypeFactory.GetTypeForName("|aDDr|");
            Assert.AreEqual(OperandType.INDIRECT_ADDRESS, result);
        }
    }
}
