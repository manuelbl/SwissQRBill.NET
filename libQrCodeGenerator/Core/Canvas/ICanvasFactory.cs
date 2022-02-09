//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace libQrCodeGenerator.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Factory for creating <c>ICanvas</c> instances
    /// </summary>
    public interface ICanvasFactory
    {
        /// <summary>
        /// Indicates if this factory can create a canvas for the specified bill format.
        /// <para>
        /// Factories must register themselves with the global <c>CanvasCreator</c> instance.
        /// </para>
        /// </summary>
        /// <param name="format">bill format</param>
        /// <returns><c>true</c> if it is able to create, <c>false</c> otherwise</returns>
        bool CanCreate(BillFormat format);

        /// <summary>
        /// Creates a canvas for the specified bill format.
        /// </summary>
        /// <param name="format">bill format</param>
        /// <param name="width">canvas width, in mm</param>
        /// <param name="height">canvas height, in mm</param>
        /// <returns>new <c>ICanvas</c> instance</returns>
        ICanvas Create(BillFormat format, double width, double height);
    }
}
