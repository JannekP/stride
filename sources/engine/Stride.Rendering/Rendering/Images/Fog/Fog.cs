// Copyright (c) Stride contributors (https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Graphics;

namespace Stride.Rendering.Images
{
    [DataContract("Fog")]
    public class Fog : ImageEffect
    {
        private readonly ImageEffectShader FogPass;

        private float nearClipPlane, farClipPlane;

        public Fog()
    : this("FogShader")
        {
            density = 0.035f;
            opacity = 0.6f;
            FogColor = new Vector3(1.0f);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Fog"/> class.
        /// </summary>
        /// <param name="FogPassShaderName">Name of the bright pass shader.</param>
        public Fog(string FogPassShaderName) : base(FogPassShaderName)
        {
            if (FogPassShaderName == null) throw new ArgumentNullException("FogPassShaderName");
            FogPass = new ImageEffectShader(FogPassShaderName);
        }

        /// <summary>
        /// Modulate the fog by a certain density value.
        /// </summary>
        /// <value>The density.</value>
        /// <userdoc>The value of the intensity threshold used to identify bright areas</userdoc>
        [DataMember(10)]
        [DefaultValue(0.035f)]
        public float density { get; set; }

        /// <summary>
        /// Modulate the fog by a certain opacity value.
        /// </summary>
        /// <value>The opacity.</value>
        /// <userdoc>The visibility threshold</userdoc>
        [DataMember(15)]
        [DefaultValue(0.6f)]
        public float opacity { get; set; }

        /// <summary>
        /// Modulate the fog by a certain color.
        /// </summary>
        /// <value>The color.</value>
        /// <userdoc>It affects the color of the fog.</userdoc>
        [DataMember(20)]
        public Vector3 FogColor { get; set; }


        protected override void InitializeCore()
        {
            base.InitializeCore();
            ToLoadAndUnload(FogPass);
        }

        protected override void SetDefaultParameters()
        {
            base.SetDefaultParameters();
            density = 0.35f;
            opacity = 0.6f;
            FogColor = new Vector3(1.0f);
        }

        protected override void DrawCore(RenderDrawContext context)
        {
            var originalColorBuffer = GetSafeInput(0);
            var originalDepthBuffer = GetSafeInput(1);

            var outputTexture = GetSafeOutput(0);

            if (originalColorBuffer == null || originalDepthBuffer == null || outputTexture == null)
            {
                return;
            }

            FogPass.SetInput(0, originalColorBuffer);
            FogPass.SetInput(1, originalDepthBuffer);
            FogPass.Parameters.Set(FogShaderKeys.FogColor, FogColor);
            FogPass.Parameters.Set(FogShaderKeys.b, density);
            FogPass.Parameters.Set(FogShaderKeys.opacity, opacity);
            FogPass.Parameters.Set(FogShaderKeys.nearClipPlane, nearClipPlane);
            FogPass.Parameters.Set(FogShaderKeys.farClipPlane, farClipPlane);

            FogPass.SetOutput(outputTexture);
            ((RendererBase)FogPass).Draw(context);
        }

        /// <summary>
        /// Provides a color buffer and a depth buffer to apply the fog to.
        /// </summary>
        /// <param name="colorBuffer">A color buffer to process.</param>
        /// <param name="depthBuffer">The depth buffer corresponding to the color buffer provided.</param>
        public void SetColorDepthInput(Texture colorBuffer, Texture depthBuffer, float nearClipPlane, float farClipPlane)
        {
            SetInput(0, colorBuffer);
            SetInput(1, depthBuffer);
            this.nearClipPlane = nearClipPlane;
            this.farClipPlane = farClipPlane;
        }
    }
}
