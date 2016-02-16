namespace Stebs.ViewModel
{
    using MbUnit.Framework;

    /// <summary>
    /// Tests class to check the functionality of the ByteHelper
    /// </summary>
    [TestFixture]
    class ByteHelperTest
    {
        
        /// <summary>
        /// Tests to ensure that the evaluation of a string to a possible
        /// hex value works correctly.
        /// </summary>
        [Test]
        public void TestIsHex()
        {
            Assert.IsTrue(ByteHelper.IsHexValue("FF"));
            Assert.IsTrue(ByteHelper.IsHexValue("F"));
            Assert.IsTrue(ByteHelper.IsHexValue("a0"));
            Assert.IsTrue(ByteHelper.IsHexValue("0F"));
            Assert.IsTrue(ByteHelper.IsHexValue("0"));
            Assert.IsTrue(ByteHelper.IsHexValue("00"));
            Assert.IsTrue(ByteHelper.IsHexValue("      01"));
            Assert.IsTrue(ByteHelper.IsHexValue("\t01"));
            Assert.IsTrue(ByteHelper.IsHexValue("02   "));
            Assert.IsTrue(ByteHelper.IsHexValue("02\t"));

            Assert.IsFalse(ByteHelper.IsHexValue("100"));
            Assert.IsFalse(ByteHelper.IsHexValue("G"));
            Assert.IsFalse(ByteHelper.IsHexValue("es"));
        }
    }
}
