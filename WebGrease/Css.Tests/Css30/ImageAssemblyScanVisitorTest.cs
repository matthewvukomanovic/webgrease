﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyScanVisitorTest.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   This is a test class for ImageAssemblyScanVisitorTest and is intended
//   to contain all ImageAssemblyScanVisitorTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Css.Tests.Css30
{
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WebGrease.Tests;

    using TestSuite;
    using WebGrease.Css;
    using WebGrease.Css.Ast;
    using WebGrease.Css.ImageAssemblyAnalysis;
    using WebGrease.Css.Visitor;

    /// <summary>This is a test class for ImageAssemblyScanVisitorTest and is intended
    /// to contain all ImageAssemblyScanVisitorTest Unit Tests</summary>
    [TestClass]
    public class ImageAssemblyScanVisitorTest
    {
        /// <summary>The base directory.</summary>
        private static readonly string BaseDirectory;

        /// <summary>The expect directory.</summary>
        private static readonly string ActualDirectory;

        /// <summary>Initializes static members of the <see cref="ImageAssemblyScanVisitorTest"/> class.</summary>
        static ImageAssemblyScanVisitorTest()
        {
            BaseDirectory = Path.Combine(TestDeploymentPaths.TestDirectory, @"css.tests\css30\imageassemblyscanvisitor");
            ActualDirectory = Path.Combine(BaseDirectory, @"actual");
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>A test for background selectors which should be sprited.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void SpritingCandidatesTest()
        {
            const string FileName = @"spritingcandidates.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));
            
            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);

            var visitor = new ImageAssemblyScanVisitor(fileInfo.FullName, null);
            styleSheetNode = styleSheetNode.Accept(visitor) as StyleSheetNode;
            Assert.IsNotNull(styleSheetNode);

            Trace.WriteLine(visitor.ImageAssemblyAnalysisLog.ToString());
            var imageReferencesToAssemble = visitor.DefaultImageAssemblyScanOutput.ImageReferencesToAssemble;
            Assert.IsNotNull(imageReferencesToAssemble);
            Assert.IsTrue(imageReferencesToAssemble.Count == 95);
        }

        /// <summary>A test for background selectors which should be sprited with ignore.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void SpritingCandidatesWithIgnoreTest()
        {
            const string FileName = @"spritingcandidateswithignore.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);

            var visitor = new ImageAssemblyScanVisitor(fileInfo.FullName, new[] { "/i/1.gif", "/i/2.gif" });
            styleSheetNode = styleSheetNode.Accept(visitor) as StyleSheetNode;
            Assert.IsNotNull(styleSheetNode);

            var imageReferencesToAssemble = visitor.DefaultImageAssemblyScanOutput.ImageReferencesToAssemble;
            Assert.IsNotNull(imageReferencesToAssemble);
            Assert.IsTrue(imageReferencesToAssemble.Count == 3);
            Assert.IsTrue(imageReferencesToAssemble[0].AbsoluteImagePath.Contains(@"\i\3.gif"));
            Assert.IsTrue(imageReferencesToAssemble[1].AbsoluteImagePath.Contains(@"\i\4.gif"));
            Assert.IsTrue(imageReferencesToAssemble[2].AbsoluteImagePath.Contains(@"\i\5.gif"));
        }

        /// <summary>A test for background selectors with duplicate declaration.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void RepeatedPropertyNameExceptionTest()
        {
            const string FileName = @"repeatedpropertynameexception.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);

            try
            {
                styleSheetNode.Accept(new ImageAssemblyScanVisitor(fileInfo.FullName, null));
            }
            catch (ImageAssembleException imageAssembleException)
            {
                Assert.IsTrue(imageAssembleException.ToString().Contains(string.Format(CultureInfo.InvariantCulture, CssStrings.RepeatedPropertyNameError, "background-image")));
            }
        }

        /// <summary>A test for duplicate background format exception.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void DuplicateBackgroundFormatExceptionTest()
        {
            const string FileName = @"duplicatebackgroundformatexception.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);
            
            try
            {
                styleSheetNode.Accept(new ImageAssemblyScanVisitor(fileInfo.FullName, null));
            }
            catch (ImageAssembleException imageAssembleException)
            {
                Assert.IsTrue(imageAssembleException.ToString().Contains(CssStrings.DuplicateBackgroundFormatError));
            }
        }

        /// <summary>A test for duplicate image references with different rules.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void DuplicateImageReferenceWithDifferentRulesExceptionTest()
        {
            const string FileName = @"duplicateimagereferencewithdifferentrulesexception.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);

            try
            {
                styleSheetNode.Accept(new ImageAssemblyScanVisitor(fileInfo.FullName, null));
            }
            catch (ImageAssembleException imageAssembleException)
            {
                Assert.IsTrue(imageAssembleException.ToString().Contains(string.Format(CultureInfo.InvariantCulture, CssStrings.DuplicateImageReferenceWithDifferentRulesError, Path.Combine(ActualDirectory, "foo.gif").ToLowerInvariant())));
            }
        }

        /// <summary>A test for too many lengths on background node.</summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void TooManyLengthsExceptionTest()
        {
            const string FileName = @"toomanylengthsexception.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);

            try
            {
                styleSheetNode.Accept(new ImageAssemblyScanVisitor(fileInfo.FullName, null));
            }
            catch (ImageAssembleException imageAssembleException)
            {
                Assert.IsTrue(imageAssembleException.ToString().Contains(string.Format(CultureInfo.InvariantCulture, CssStrings.TooManyLengthsError, string.Empty).TrimEnd(new[] { '.', '\'' })));
            }
        }

        /// <summary>
        /// A test for the image url is token.
        /// tokenized background images should be ignored.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.CssParser)]
        [TestCategory(TestCategories.ImageAssemblyScanVisitor)]
        public void TokenImageUrlTest()
        {
            const string FileName = @"tokenimageurl.css";
            var fileInfo = new FileInfo(Path.Combine(ActualDirectory, FileName));

            var styleSheetNode = CssParser.Parse(fileInfo);
            Assert.IsNotNull(styleSheetNode);
            var visitor = new ImageAssemblyScanVisitor(fileInfo.FullName, null);
            styleSheetNode.Accept(visitor);
            var imageReferencesToAssemble = visitor.DefaultImageAssemblyScanOutput.ImageReferencesToAssemble;
            Assert.IsNotNull(imageReferencesToAssemble);
            Assert.AreEqual(0, imageReferencesToAssemble.Count);
        }
    }
}