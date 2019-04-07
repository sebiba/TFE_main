using System;
using System.Collections.Generic;
using System.Globalization;
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
            Assert.AreEqual("Erreur: File doesn't exist", PyUtils.Getfreq("test"));
        }

        [TestMethod]
        public void GetFreqWrongTypeOfFile()
        {
            Assert.AreEqual("Erreur: File with wrong extension", PyUtils.Getfreq(@"D:\programmation\python\TFE\note.py"));
        }
        [TestMethod]
        public void GetFreq1()
        {
            Assert.AreEqual("0:440.2005778954972||1:440.2005778954972||2:440.2005778954972||3:440.2005778954972||4:440.2005778954972||5:440.2005778954972||6:440.2005778954972||7:440.2005778954972||8:440.2005778954972||9:440.2005778954972||10:440.2005778954972||11:440.2005778954972||12:440.2005778954972||13:440.2005778954972||14:440.2005778954972||15:440.2005778954972||16:440.3063583815029||17:440.3063583815029||", PyUtils.Getfreq(@"D:\programmation\python\TFE\440Hz.wav"));
        }
#endregion fichier vers Frequence

#region Frequence vers note
        [TestMethod]
        public void FreqToNoteNegatif()
        {
            Assert.AreEqual("math domain error", PyUtils.FreqToNote(-5.2f));
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
            CollectionAssert.AreEqual(new Lily("test").ReadFile(), new List<string> { "Erreur", "File with wrong extension" });
        }
        /*[TestMethod]
        public void ReadFileWrongPath()
        {
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\template.ly").ReadFile(), new List<string> { "Erreur", "File with wrong extension" });
        }*/
        [TestMethod]
        public void ReadFile1()
        {
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").ReadFile(), new List<string> { "\\version \"2.16.0\"  % necessary for upgrading to future LilyPond versions.", "", "\\header{", "\ttitle = \"test\"", "\tsubtitle = \"test\"", "}", "", "tagline = \"pure\"", "", "\\header{", "\tpiece = \"test\"", "}", "", "\\relative c' {", "\t\\time 4/4", "\t\\key c \\major", "\td8 b4 b8 a4 g8 b16 d8 e2", "}" });
        }
        #endregion Lecture fichier Lilypond

#region Tempo
        [TestMethod]
        public void TempoEmptyList()
        {
            List<string> init = new List<string> ();  // list to convert by the tempo function
            List<string> test = new List<string> ();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo1()
        {
            List<string> init = new List<string> { "A", "A", "B", "B", "B", "B", "C", "C", "C", "C", "C", "C", "C", "C" };  // list to convert by the tempo function
            List<string> test = new List<string> { /*"A16",*/ "B8", "C4" };  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init),test);
        }
        [TestMethod]
        public void Tempo2()
        {
            List<string> init = new List<string> { "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A" };  // list to convert by the tempo function
            List<string> test = new List<string> { "A1", "A8"};  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo4()
        {
            List<string> init = new List<string> { "A", "B", "C" };  // list to convert by the tempo function
            List<string> test = new List<string>();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        [TestMethod]
        public void Tempo5()
        {
            List<string> init = new List<string> { "A"};  // list to convert by the tempo function
            List<string> test = new List<string>();  // list that tempo should return
            CollectionAssert.AreEqual(new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Tempo(init), test);
        }
        #endregion Tempo

#region format
        [TestMethod]
        public void FormatEmpty()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<string>());
            Assert.AreEqual(test, "");
        }
        [TestMethod]
        public void Format1()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<string>() {"A","A","A","A","B","B", "B"});
            Assert.AreEqual(test, " a8 b8");
        }

        [TestMethod]
        public void Format2()
        {
            string test = new Lily(@"D:\programmation\c#\TFE\python\Lily\good.ly").Format(new List<string>() { "A#", "A#", "A#", "A#", "Bb", "Bb", "Bb" });
            Assert.AreEqual(test, " ais8 bes8");
        }
#endregion
    }
}
