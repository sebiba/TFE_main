using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using python;

namespace UnitTestProject1
{
    [TestClass]
    public class NoteTest
    {
        #region constructeur
        [TestMethod]
        public void string1()
        {
            Assert.AreEqual(new Note("A").value, "A");
        }

        [TestMethod]
        public void string2()
        {
            Assert.AreEqual(new Note("E").value, "E");
        }

        [TestMethod]
        public void stringis()
        {
            Assert.AreEqual(new Note("A#").value, "A#");
        }

        [TestMethod]
        public void stringes()
        {
            Assert.AreEqual(new Note("Ab").value, "Ab");
        }

        [TestMethod]
        public void float1()
        {
            Assert.AreEqual(new Note(440.0f).value, "A");
        }

        [TestMethod]
        public void float2()
        {
            Assert.AreEqual(new Note(1210f).value, "Eb");
        }

        [TestMethod]
        public void floatNegatif()
        {
            try { 
                Note test = new Note(-440f);
                Assert.Fail();
            }
            catch(Exception e)
            {
                Assert.AreEqual("math domain error", e.Message);
            }
        }
        #endregion constructeur

        #region GetGesture
        [TestMethod]
        public void GetGesture1()
        {
            CollectionAssert.AreEqual(new Note("A").GetGesture(), new Dictionary<string, string> { { "Do", "5p"}, { "Sol", "7t"} });
        }

        [TestMethod]
        public void GetGesture2()
        {
            CollectionAssert.AreEqual(new Note("D").GetGesture(), new Dictionary<string, string> { { "Do", "3t" }, { "Sol", "5p" } });
        }

        [TestMethod]
        public void GetGestureis()
        {
            var test = new Note("D#").GetGesture();
            CollectionAssert.AreEqual(new Note("D#").GetGesture(), new Dictionary<string, string> { { "Do", "0" }, { "Sol", "1p" } });
        }
        [TestMethod]
        public void GetGestureis2()
        {
            var test = new Note("Dis").GetGesture();
            CollectionAssert.AreEqual(new Note("Dis").GetGesture(), new Dictionary<string, string> { { "Do", "0" }, { "Sol", "1p" } });
        }

        [TestMethod]
        public void GetGesturees()
        {
            CollectionAssert.AreEqual(new Note("Db").GetGesture(), new Dictionary<string, string> { { "Do", "0" }, { "Sol", "1t" } });
        }

        [TestMethod]
        public void GetGesturees2()
        {
            CollectionAssert.AreEqual(new Note("Des").GetGesture(), new Dictionary<string, string> { { "Do", "0" }, { "Sol", "1t" } });
        }

        [TestMethod]
        public void GetWrongGesturees()
        {
            try
            {
                Dictionary<string, string> testGest = new Note("z").GetGesture();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("La clé donnée était absente du dictionnaire.", e.Message);
            }
        }
        #endregion GetGesture

        #region equals
        [TestMethod]
        public void Equals1()
        {
            Assert.IsTrue(new Note("A").Equals(new Note("A")));
        }

        [TestMethod]
        public void Equalsis()
        {
            Assert.IsTrue(new Note("B#").Equals(new Note("B#")));
        }

        [TestMethod]
        public void Equalses()
        {
            Assert.IsTrue(new Note("Bb").Equals(new Note("Bb")));
        }

        [TestMethod]
        public void EqualWrong()
        {
            Assert.IsFalse(new Note("Bb").Equals(new Note("B#")));
        }

        [TestMethod]
        public void EqualWrong2()
        {
            Assert.IsFalse(new Note("B").Equals(new List<string>()));
        }
        #endregion equals
    }
}
