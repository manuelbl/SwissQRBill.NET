//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace libQrCodeGenerator.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Creates <c>ICanvas</c> instances using the registered <c>ICanvasFactory</c> instances.
    /// </summary>
    /// <seealso cref="ICanvas"/>
    /// <seealso cref="ICanvasFactory"/>
    public static class CanvasCreator
    {
        static CanvasCreator()
        {
            // Add default factories
            Register(new SVGCanvasFactory());
            Register(new PDFCanvasFactory());
        }

        /// <summary>
        /// Register an additional canvas factory.
        /// <para>
        /// Factories added later take precedence over earlier added factories.</para>
        /// </summary>
        /// <param name="factory">factory to add</param>
        public static void Register(ICanvasFactory factory)
        {
            factories.Insert(0, factory);
        }

        /// <summary>
        /// Creates a new <c>ICanvas</c> instance for the specified bill format.
        /// </summary>
        /// <param name="format">bill format</param>
        /// <param name="width">canvas width, in mm</param>
        /// <param name="height">canvas height, in mm</param>
        /// <returns></returns>
        public static ICanvas Create(BillFormat format, double width, double height)
        {
            foreach (ICanvasFactory factory in factories)
                if (factory.CanCreate(format))
                    return factory.Create(format, width, height);

            return null;
        }

        /// <summary>
        /// Registers the canvas factory for PNG output.
        /// <para>
        /// The method checks if the libQrCodeGenerator.SwissQRBill.Generator assembly is present.
        /// </para>
        /// </summary>
        public static void RegisterPixelCanvasFactory()
        {
            if (checkedForPixelCanvas)
                return;

            checkedForPixelCanvas = true;

            // Locate the PixelCanvas assembly and the PNGCanvasFactory class and register it
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "libQrCodeGenerator.SwissQRBill.Generator")
                {
                    Type factoryType = assembly.GetType("libQrCodeGenerator.SwissQRBill.PixelCanvas.PNGCanvasFactory");
                    if (factoryType != null)
                    {
                        ICanvasFactory factory = (ICanvasFactory)Activator.CreateInstance(factoryType);
                        Register(factory);
                        return;
                    }
                }
            }
        }

        private static readonly List<ICanvasFactory> factories = new List<ICanvasFactory>();

        private static bool checkedForPixelCanvas = false;
    }
}
