using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using python;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
#region fichier vers Frequence        
        [TestMethod]
        public void GetFreqWrongPath()
        {
            try
            {
                PyUtils.Getfreq(@"test");
                Assert.Fail();
            }
            catch (FileNotFoundException e)
            {
                Assert.AreEqual("Impossible de trouver le fichier spécifié.", e.Message);
            }
        }

        [TestMethod]
        public void GetFreqWrongTypeOfFile()
        {
            try
            {
                PyUtils.Getfreq(@"D:\programmation\python\TFE\note.py");
                Assert.Fail();
            }
            catch(IOException e)
            {
                Assert.AreEqual("Une erreur d'E/S s'est produite.", e.Message);
            }
        }
        [TestMethod]
        public void GetFreq1()
        {
            CollectionAssert.AreEqual(new List<string> { new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value, new Note("A").value }, PyUtils.Getfreq(@"D:\programmation\python\TFE\440Hz.wav").Select(note => note.value).ToList());
        }
#endregion fichier vers Frequence

#region Frequence vers note
        [TestMethod]
        public void FreqToNoteNegatif()
        {
            try
            {
                string test = PyUtils.FreqToNote(-5.2f);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("math domain error", e.Message);
            }
        }
        [TestMethod]
        public void FreqToNote1()
        {
            Assert.AreEqual("Eb", PyUtils.FreqToNote(19.45f));
        }
        [TestMethod]
        public void FreqToNote2()
        {
            Assert.AreEqual("A", PyUtils.FreqToNote(440f));
        }
        [TestMethod]
        public void FreqToNote3()
        {
            Assert.AreEqual("G#", PyUtils.FreqToNote(830.6f));
        }
        #endregion Frequence vers note

#region Lecture fichier Lilypond
        [TestMethod]
        public void ReadFileWrongExtension()
        {
            try
            {
                List<string> test = new Lily("test").ReadFile();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Une erreur d'E/S s'est produite.", e.Message);
            }
        }

        [TestMethod]
        public void ReadFileWrongPath()
        {
            try
            {
                List<string> test = new Lily(@"D:\programmation\c#\TFE\python\pdf.cs").ReadFile();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Une erreur d'E/S s'est produite.", e.Message);
            }
        }

        [TestMethod]
        public void ReadFile1()
        {
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\template.ly").ReadFile(), new List<string> { "\\version \"2.16.0\"  % necessary for upgrading to future LilyPond versions.", "", "\\header{", "\ttitle = \"pure\"", "\tsubtitle = \"pure\"", "}", "", "tagline = \"pure\"", "", "\\header{", "\tpiece = \"pure\"", "}", "", "\\relative c' {", "\t\\time 4/4", "\t\\key c \\major", "\tc1", "}" });
        }
#endregion Lecture fichier Lilypond

#region Tempo
        [TestMethod]
        public void TempoEmptyList()
        {
            List<Note> init = new List<Note> ();  // list to convert by the tempo function
            List<Note> test = new List<Note> ();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo1()
        {
            List<Note> init = new List<Note> { new Note("A"), new Note("A"), new Note("B"), new Note("B"), new Note("B"), new Note("B"), new Note("C"), new Note("C"), new Note("C"), new Note("C"), new Note("C"), new Note("C"), new Note("C"), new Note("C") };  // list to convert by the tempo function
            List<string> test = new List<string> { /*"A16",*/ "B8", "C4" };  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init),test);
        }
        [TestMethod]
        public void Tempo2()
        {
            List<Note> init = new List<Note> { new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A"), new Note("A") };  // list to convert by the tempo function
            List<string> test = new List<string> { "A1", "A8"};  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo4()
        {
            List<Note> init = new List<Note> { new Note("A"), new Note("B"), new Note("C") };  // list to convert by the tempo function
            List<string> test = new List<string>();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo5()
        {
            List<Note> init = new List<Note> { new Note("A")};  // list to convert by the tempo function
            List<string> test = new List<string>();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        #endregion Tempo

#region format
        [TestMethod]
        public void FormatEmpty()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<Note>());
            Assert.AreEqual(test, "");
        }
        [TestMethod]
        public void Format1()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<Note>() {new Note("A"),new Note("A"),new Note("A"),new Note("A"),new Note("B"),new Note("B"), new Note("B")});
            Assert.AreEqual(test, " a8 b8");
        }

        [TestMethod]
        public void Format2()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<Note>() { new Note("A#"), new Note("A#"), new Note("A#"), new Note("A#"), new Note("Bb"), new Note("Bb"), new Note("Bb") });
            Assert.AreEqual(test, " ais8 bes8");
        }
        #endregion

    }
}
