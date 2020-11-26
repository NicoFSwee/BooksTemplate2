using Books.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Books.CoreTests
{
    [TestClass()]
    public class BookTests
    {

        [TestMethod()]
        public void CheckIsbn_CorrectLength_ShouldReturnTrue()
        {
            Assert.IsTrue(Book.CheckIsbn("3760778739"));
        }

        [TestMethod()]
        public void CheckIsbn_TooShort_ShouldReturnFalse()
        {
            Assert.IsFalse(Book.CheckIsbn("376077879"));
        }

        [TestMethod()]
        public void CheckIsbn_TooLong_ShouldReturnFalse()
        {
            Assert.IsFalse(Book.CheckIsbn("37607787390"));
        }


        [TestMethod()]
        public void CheckIsbn_IllegalChar_ShouldReturnFalse()
        {
            Assert.IsFalse(Book.CheckIsbn("3760A78739"));
        }

        [TestMethod()]
        public void CheckIsbn_XAtEnd_ShouldReturnTrue()
        {
            Assert.IsTrue(Book.CheckIsbn("355151710X"));
        }

        [TestMethod()]
        public void CheckIsbn_XInTheMiddle_ShouldReturnFalse()
        {
            Assert.IsFalse(Book.CheckIsbn("355X151710"));
        }


        [TestMethod()]
        public void CheckIsbn_WrongCheckSum_ShouldReturnFalse()
        {
            Assert.IsFalse(Book.CheckIsbn("355150710X"));
        }


    }
}