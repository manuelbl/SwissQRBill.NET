//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    /// <summary>
    /// Compares a generated file with an reference file
    /// </summary>
    public static class FileComparison
    {
        /// <summary>
        /// Asserts that the specified file content matches the content of a reference file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the actual content differs from the expected file content, an assertion
        /// exception is thrown and the actual content is saved to a file starting with
        /// the name "actual_".
        /// </para>
        /// <para>
        /// If the actual content matches the expected file content, the file system is
        /// checked for a file starting with the name "actual_". If it exists, it is
        /// deleted.
        /// </para>
        /// </remarks>
        /// <param name="actualContent">content of actual file</param>
        /// <param name="expectedFileName">file name of expected file (reference file)</param>
        public static void AssertFileContentsEqual(byte[] actualContent, string expectedFileName)
        {

            try
            {
                byte[] expectedContent = LoadReferenceFile(expectedFileName);
                Assert.Equal(expectedContent, actualContent);

            }
            catch (XunitException e)
            {
                SaveActualFile(actualContent, expectedFileName);
                throw e;
            }
            catch (IOException e)
            {
                SaveActualFile(actualContent, expectedFileName);
                throw new Exception("Failed to read reference file", e);
            }

            DeleteActualFile(expectedFileName);
        }

        /*
        static void AssertGrayscaleImageContentsEqual(byte[] actualContent, string expectedFileName)
        {

            try
            {
                byte[] expectedContent = loadReferenceFile(expectedFileName);
                ImageComparison.assertGrayscaleImageContentEquals(expectedContent, actualContent);

            }
            catch (AssertionError e)
            {
                saveActualFile(actualContent, expectedFileName);
                throw e;
            }
            catch (IOException e)
            {
                saveActualFile(actualContent, expectedFileName);
                throw new RuntimeException(e);
            }

            deleteActualFile(expectedFileName);
        }
        */

        private static byte[] LoadReferenceFile(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream(typeof(FileComparison), $"ReferenceFiles.{filename}");
            if (resourceStream == null)
            {
                throw new FileNotFoundException($"Resource not found: ReferenceFiles.{filename}");
            }

            using (Stream rs = resourceStream)
            {
                MemoryStream ms = new MemoryStream();
                rs.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static void SaveActualFile(byte[] data, string expectedFileName)
        {
            string actualFileName = $"actual_{expectedFileName}";
            File.WriteAllBytes(actualFileName, data);
        }

        private static void DeleteActualFile(string expectedFileName)
        {
            string actualFileName = $"actual_{expectedFileName}";
            if (File.Exists(actualFileName))
            {
                File.Delete(actualFileName);
            }
        }
    }
}
