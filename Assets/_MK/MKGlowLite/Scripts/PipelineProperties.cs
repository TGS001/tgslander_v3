﻿//////////////////////////////////////////////////////
// MK Glow Pipeline Properties                      //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.           
 //
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.Glow
{
    #if UNITY_2018_3_OR_NEWER
    using XRSettings = UnityEngine.XR.XRSettings;
    #endif

    /// <summary>
    /// Contains all PipelineProperties used in MK Glow
    /// </summary>
    internal static class PipelineProperties
    {
        //For even super large displays preserve some extra memory to prevent erros and gc.
        internal static readonly int renderBufferSize = 15;
        internal static bool scriptableRenderPipelineActive{ get { return UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null; } }
        #if UNITY_2018_3_OR_NEWER
        internal static bool xrEnabled { get{ return XRSettings.enabled; } }
        internal static bool singlePassStereoDoubleWideEnabled { get{ return XRSettings.enabled && XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass; } }
        internal static bool singlePassStereoInstancedEnabled { get{ return XRSettings.enabled && (XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePassInstanced || XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePassMultiview); } }
        #else
        //No proper way of detecting stereo rendering mode so just return false
        internal static bool xrEnabled { get{ return false; } }
        internal static bool singlePassStereoDoubleWideEnabled { get{ return false; } }
        internal static bool singlePassStereoInstancedEnabled { get{ return false; } }
        #endif

        /// <summary>
        /// Shader PipelineProperties as IDs
        /// </summary>
        internal static class ShaderProperties
        {
            /// <summary>
            /// Representation of a render property based on unity version
            /// The id of the given name will be autogenerated
            /// </summary>
            internal class DefaultProperty
            {
                protected string _name;
                internal string name
                {
                    get{return _name;}
                }
                #if UNITY_2017_3_OR_NEWER
                protected int _id;
                internal int id
                {
                    get{return _id;}
                }
                #else
                internal string id
                {
                    get{return _name;}
                }
                #endif

                internal DefaultProperty(string name)
                {
                    this._name = name;
                    #if UNITY_2017_3_OR_NEWER
                    this._id = Shader.PropertyToID(name);
                    #endif
                }
            }

            //Main, Bloom
            internal static readonly DefaultProperty singlePassStereoScale           = new DefaultProperty("_SinglePassStereoScale");
            internal static readonly DefaultProperty viewMatrix                      = new DefaultProperty("_ViewMatrix");
            internal static readonly DefaultProperty sourceTex                       = new DefaultProperty("_SourceTex");
            internal static readonly DefaultProperty targetTex                       = new DefaultProperty("_TargetTex");
            internal static readonly DefaultProperty copyTargetTex                   = new DefaultProperty("_CopyTargetTex");
            internal static readonly DefaultProperty bloomTex                        = new DefaultProperty("_BloomTex");
            internal static readonly DefaultProperty bloomTargetTex                  = new DefaultProperty("_BloomTargetTex");
            internal static readonly DefaultProperty bloomSpread                     = new DefaultProperty("_BloomSpread");
            internal static readonly DefaultProperty bloomThreshold                  = new DefaultProperty("_BloomThreshold");
            internal static readonly DefaultProperty bloomIntensity                  = new DefaultProperty("_BloomIntensity");
            internal static readonly DefaultProperty higherMipBloomTex               = new DefaultProperty("_HigherMipBloomTex");

            //Lens Surface
            internal static readonly DefaultProperty lensSurfaceDirtTex              = new DefaultProperty("_LensSurfaceDirtTex");
            internal static readonly DefaultProperty lensSurfaceDiffractionTex       = new DefaultProperty("_LensSurfaceDiffractionTex");
            internal static readonly DefaultProperty lensSurfaceDirtIntensity        = new DefaultProperty("_LensSurfaceDirtIntensity");
            internal static readonly DefaultProperty lensSurfaceDiffractionIntensity = new DefaultProperty("_LensSurfaceDiffractionIntensity");
            internal static readonly DefaultProperty lensSurfaceDirtTexST            = new DefaultProperty("_LensSurfaceDirtTex_ST");
        }

        /// <summary>
        /// CommandBuffer PipelineProperties as strings
        /// </summary>
        internal static class CommandBufferProperties
        {
            //Main
            internal static readonly string commandBufferName         = "MK Glow";
            internal static readonly string selectiveRenderBuffer     = "_SelectiveRenderBuffer";
            internal static readonly string bloomDownsampleBuffer     = "_BloomDownsampleBuffer";
            internal static readonly string bloomUpsampleBuffer       = "_BloomUpsampleBuffer";
            internal static readonly string sourceBuffer              = "_SourceBuffer";

            //Buffer Samples
            internal static readonly string sampleDownsample          = "Downsample";
            internal static readonly string samplePreSample           = "Presample";
            internal static readonly string sampleUpsample            = "Upsample";
            internal static readonly string sampleComposite           = "Composite";
            internal static readonly string sampleCreateBuffers       = "Create Mip Buffers";
            internal static readonly string sampleClearBuffers        = "Clear Mip Buffers";
            internal static readonly string sampleSetup               = "Setup Constant Buffer";
            internal static readonly string sampleCopySource          = "Copy Source";
            internal static readonly string sampleReplacement         = "Render Replacement";
            internal static readonly string samplePrepare             = "Prepare";
        }
    }
}
