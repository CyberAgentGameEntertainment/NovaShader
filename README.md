<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/144776407-7ea24e22-2fe0-437e-b7e3-787963fd6f19.png#gh-dark-mode-only" alt="NOVA Shader">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/142821815-7d838ac4-ff18-4025-b60f-0d22ad538f50.png#gh-light-mode-only" alt="NOVA Shader">
</p>

# NOVA Shader: Uber shader for Particle System

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/CyberAgentGameEntertainment/NovaShader/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#Requirements)

**Docs** ([English](README.md), [日本語](README_JA.md))
| **Samples** ([English](Assets/Samples/README.md), [日本語](Assets/Samples/README_JA.md))
| **Demo** ([English](Assets/Demo/README.md), [日本語](Assets/Demo/README_JA.md))

NOVA Shader is a multi-functional shader for the Particle System that supports Universal Render Pipeline (URP).
General-purpose functions commonly used in visual effects are implemented so you can create high-quality effects efficiently.

<p align="center">
  <img width="70%" src="https://user-images.githubusercontent.com/47441314/144193003-53bcaa8a-b9a2-4b79-a1de-aa7b001abdaa.gif" alt="Sample1">
</p>
<p align="center">
  <img width="70%" src="https://user-images.githubusercontent.com/47441314/144192957-64e63c4a-3644-4a08-8134-dcbeb85d5493.gif" alt="Sample2"><br>
  <font color="grey">Author: </font><a href="https://twitter.com/Ugokashiya">@Ugokashiya</a>
</p>

It implements some distinctive features like Flow Map, Flip-Book (sequential texture animation), Dissolve, Fade, Rotation, Animated Tint Map, Emission, Distortion, and so on.

<p align="center">
  <img width="70%" src="https://user-images.githubusercontent.com/47441314/143531706-7f0230bb-4e4f-41de-9dbf-1586f295225c.gif" alt="Features"><br>
  <font color="grey">Features</a>
</p>

For more information, please refer to the following documents, [Samples](Assets/Samples/README.md) and [Demo](Assets/Demo/README.md).

## Table of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<details>
<summary>Details</summary>

- [Setup](#setup)
  - [Requirements](#requirements)
    - [Install](#install)
- [Usage](#usage)
    - [Add Renderer Feature](#add-renderer-feature)
    - [Activate Depth Texture](#activate-depth-texture)
    - [Create and assign the Material](#create-and-assign-the-material)
- [Uber Unlit Shader](#uber-unlit-shader)
    - [Render Settings](#render-settings)
    - [Vertex Deformation](#vertex-deformation)
    - [Base Map](#base-map)
    - [Tint Color](#tint-color)
    - [Flow Map](#flow-map)
    - [Parallax Map](#parallax-map)
    - [Color Correction](#color-correction)
    - [Alpha Transition](#alpha-transition)
    - [Emission](#emission)
    - [Transparency](#transparency)
- [Uber Lit Shader](#uber-lit-shader)
    - [Render Settings](#render-settings-1)
    - [Surface Maps](#surface-maps)
- [Distortion Shader](#distortion-shader)
    - [Render Settings](#render-settings-2)
    - [Distortion](#distortion)
    - [Flow Mapping](#flow-mapping)
    - [Alpha Transition](#alpha-transition-1)
    - [Transparency](#transparency-1)
- [Uber Unlit/Lit shaders( for uGUI )](#uber-unlitlit-shaders-for-ugui-)
- [Abort Shadow Caster](#abort-shadow-caster)
- [Use with the Custom Vertex Streams](#use-with-the-custom-vertex-streams)
    - [Set up the Custom Data](#set-up-the-custom-data)
    - [Set up the Custom Vertex Streams](#set-up-the-custom-vertex-streams)
    - [Set up the Material Property](#set-up-the-material-property)
- [Use Mesh GPU Instancing](#use-mesh-gpu-instancing)
    - [Enable Mesh GPU Instancing](#enable-mesh-gpu-instancing)
    - [Set up the Custom Vertex Streams](#set-up-the-custom-vertex-streams-1)
- [Automatic set up the Custom Vertex Streams.](#automatic-set-up-the-custom-vertex-streams)
    - [Fix Now](#fix-now)
- [Remove Unused Parameter References](#remove-unused-parameter-references)
- [Reducing Memory Usage with Optimized Shaders](#reducing-memory-usage-with-optimized-shaders)
  - [OptimizedShaderGenerator.Generate()](#optimizedshadergeneratorgenerate)
  - [OptimizedShaderGenerator.Replace()](#optimizedshadergeneratorreplace)
  - [Sample Code](#sample-code)
- [Editor APIs Reference](#editor-apis-reference)
- [Licenses](#licenses)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Setup

### Requirements
This library is compatible with the following environments.

* Unity 2021.3 or higher
* Universal Render Pipeline
* Shader Model 3.5

Note that Shader Model 4.5 is required to use [Mesh GPU Instancing](https://docs.unity3d.com/Manual/PartSysInstancing.html). And also, if you want to use `Mirror Sampling`, your hardware needs to support [Inline Sampler States](https://docs.unity3d.com/Manual/SL-SamplerStates.html). And if you are using 3D Texture or 2D Texture Array, the compression format must support them.

And this document assumes that you have already set up the Universal Render Pipeline. For more information about the Universal Render Pipeline, please refer to the [Unity Manual](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/)

#### Install
To install the software, follow the steps below.

1. Open the Package Manager from `Window > Package Manager`
2. `"+" button > Add package from git URL`
3. Enter the following
   * https://github.com/CyberAgentGameEntertainment/NovaShader.git?path=/Assets/Nova

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143533003-177a51fc-3d11-4784-b9d2-d343cc622841.png" alt="Package Manager">
</p>

Or, open `Packages/manifest.json` and add the following to the dependencies block.

```json
{
    "dependencies": {
        "jp.co.cyberagent.nova": "https://github.com/CyberAgentGameEntertainment/NovaShader.git?path=/Assets/Nova"
    }
}
```

If you want to set the target version, write as follows.

* https://github.com/CyberAgentGameEntertainment/NovaShader.git?path=/Assets/Nova#1.0.0

Note that if you get a message like `No 'git' executable was found. Please install Git on your system and restart Unity`, you will need to set up Git on your machine.

To update the version, rewrite the version as described above.  
If you don't want to specify a version, you can also update the version by editing the hash of this library in the package-lock.json file.

```json
{
  "dependencies": {
      "jp.co.cyberagent.nova": {
      "version": "https://github.com/CyberAgentGameEntertainment/NovaShader.git?path=/Assets/Nova",
      "depth": 0,
      "source": "git",
      "dependencies": {},
      "hash": "..."
    }
  }
}
```

## Usage

#### Add Renderer Feature
First, set up the **Renderer Feature** to apply Distortion.
Click **Add Renderer Feature > Screen Space Distortion** from the bottom of the Inspector of the **ForwardRendererData** asset.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/195064231-02e0798d-bc3a-4bb2-b2fb-d9d28f65cd1a.png" alt="Add Screen Space Distortion"><br>
  <font color="grey">Add Screen Space Distortion</font>
</p>

Confirm that **Screen Space Distortion** has been added as shown below.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/195064556-c9192bbc-7a82-4726-98dc-ef3a878d7b63.png" alt="Screen Space Distortion"><br>
  <font color="grey">Screen Space Distortion</font>
</p>

> **Note**  
> This setting is not necessary if you do not use the Distortion Shader.

#### Activate Depth Texture
Next, activate Depth Texture to use the **Soft Particles** or **Depth Fade** features.
Check Depth Texture in the **UniversalRenderPipelineAsset** Inspector.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/195065590-29935b9a-5088-46c3-9cd9-50f496aa1c6e.png" alt="Depth Texture"><br>
  <font color="grey">Depth Texture</font>
</p>

Depth Texture setting is also found in each Camera, so set it as necessary.

> **Note**  
> This setting is not necessary if neither **Soft Particles** nor **Depth Fade** is used.

#### Create and assign the Material
Next, create a material with the Nova shader.
Create a material and set the shader to **Nova/Particles/UberUnlit** and assign a texture to the Base Map.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/195066541-45cc854f-86ed-4b9d-b1db-7ebf3b9c6306.png" alt="Material"><br>
  <font color="grey">Material</font>
</p>

After creating a Particle System and assigning this material, you will see particles with the specified texture.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/195067160-2235ee34-3fcd-47a0-b6e2-8b1595aeb994.png" alt="Particle"><br>
  <font color="grey">Particle</font>
</p>

For more information on each shader and each function, see the sections below.

## Uber Unlit Shader
The Uber Unlit shader is a multifunctional shader with no lighting applied.
Use this shader for particles that do not need lighting, such as glowing effects.

To use this shader, assign the `Nova/Particles/UberUnlit` shader to your material. The following is the description of each property that can be set from the Inspector.

#### Render Settings
The Render Settings control how the Material is rendered.

<p align="center">
  <img width="60%" src="Documentation~/Images/unlit_rendersettings_01.png" alt="Render Settings"><br>
  <font color="grey">Render Settings</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Render Type</b></td><td>
<p>
You can specify the type of rendering from the following options.
</p>
<p>
<ul>
<li>Opaque</li>
<li>Cutout</li>
<li>Transparent (Default)</li>
</ul>
</p>
<p>
If you select Cutout, the CutOff property will be displayed.
When Transparent is selected, the Blend Mode property will be displayed.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>CutOff</b></td><td>
<p>
<b>This property is visible only when the Render Type is set to Cutout.</b>
</p>
<p>
Controls the threshold to clip semi-transparent areas.
The higher this value is, the more alpha will be cropped.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Blend Mode</b></td><td>
<p>
<b>This property is visible only when the Render Type is set to Transparent.</b>
</p>
<p>
You can specify the color blend method from the following options.
</p>
<p>
<ul>
<li>Alpha (Alpha Blending / Default)</li>
<li>Average</li>
<li>Multiply</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>Render Face</b></td><td>
<p>
You can specify the rendering face from the following options.
</p>
<p>
<ul>
<li>Front (Default)</li>
<li>Back</li>
<li>Both</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>Render Priority</b></td><td>
<p>
If the Render Type is the same, the one with the lower Render Priority will be drawn first.
</p>
</td></tr>
<tr><td colspan="3"><b>Vertex Alpha Mode</b></td><td>
<p>
You can specify how to use the alpha value of the vertex color from the following options.
</p>
<p>
<ul>
<li>Alpha: Use as alpha value (Default).</li>
<li>Transition Progress: Use as Progress in Alpha Transition function (see below).</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>ZWrite</b></td><td>
<p>
You can set ZWrite value.
</p>
<p>
<ul>
<li>Auto: ZWrite is disabled when the Render Type is set to Transparent, and it is enabled when the Render Type is set to any other values.
</li>
<li>Off: ZWrite is always disabled.</li>
<li>On: ZWrite is always enabled.</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>ZTest</b></td><td>
<p>
You can set ZTest value.
</p>
<p>
<ul>
<li>Disabled</li>
<li>Never</li>
<li>Less</li>
<li>Equal</li>
<li>Less Equal (Default)</li>
<li>Greater</li>
<li>Not Equal</li>
<li>Greater Equal</li>
<li>Always</li>
</ul>
</p>
</td></tr>
</tbody>
</table>

#### Vertex Deformation
With Vertex Deformation, you can control the deformation of vertices.

<p align="center">
  <img width="60%" src="Documentation~/Images/vertex_deformation.png" alt="Vertex Deformation"><br>
  <font color="grey">Vertex Deformation</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Vertex Deformation Map. The specifications are as follows.
</p>
<p>
<ul>
    <li>Push vertices in the normal direction of the object space based on the value of a single specified texture channel.</li>
    <li>You can select a channel by Channels property.</li>
    <li>Areas of the texture with a value of zero will not be pushed out, and areas with larger values will be pushed out more towards the exterior.</li>
</ul>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan=3><b>Intensity</b></td><td>
<p>
Sets the Vertex Deformation intensity.
</p>
</td></tr>
<tr><td colspan=3><b>Base Value</b></td><td>
<p>
It is an offset applied to the values of the texture.<br>
Values lower than this will be pushed inward, while larger values will be pushed outward.
</p>
</tbody>
</table>

#### Base Map
Base Map controls the base color texture and its settings.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143205498-78b66ab9-3ea6-44ed-9a97-f5a00bda153e.png" alt="Base Map"><br>
  <font color="grey">Base Map</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify the type of the Base Map from the following options.
</p>
<p>
<ul>
<li>Single Texture: Use 2D texture (Default).</li>
<li>Flip Book: Use Flip-Book animation.</li>
<li>Flip Book Blending: Use Flip-Book animation with interpolated frames.</li>
</ul>
</p>
<p>
The required texture type will change depending on the mode you selected.<br>
Note: In NOVA Shader, please do not use the Particle System's Texture Sheet Animation. Instead, use the Flip Book feature.
</p>
</td></tr>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Set the Base Map.
</p>
<p>
If you specify Flip Book as the Mode, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture2DArray.html">Texture2DArray</a>.
And if you specify Flip Book Blending, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture3D.html">Texture3D</a>.
</p>
</td></tr>
</td></tr>
<tr><td colspan="3"><b>Rotation</b></td><td>
<p>
Set the amount of rotation of the Base Map.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Offset</b></td><td>
<p>
Shifts the center coordinate of rotation.
</p>
</td></tr>
<tr><td colspan="3"><b>Mirror Sampling</b></td><td>
<p>
If true, mirrors the texture to create a repeating pattern.
</p>
</td></tr>
<tr><td colspan="3"><b>Flip-Book Progress</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Flip Book or Flip Book Blending.</b>
</p>
<p>
The progress of the Flip-Book or Flip-Book Blending.
</p>
</td></tr>
</tbody>
</table>

#### Tint Color
Tint Color controls the color to be multiplied.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143205648-b669f20a-cc21-4a07-9d5c-3a18cd5cb085.png" alt="Tint Color"><br>
  <font color="grey">Tint Color</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify the area to apply the Tint Color from the following options.
</p>
<p>
<ul>
<li>None: Disable the Tint Color (Default).</li>
<li>All: Apply to the entire surface.</li>
<li>Rim: Apply to the rim.</li>
</ul>
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Progress</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Rim.</b>
</p>
<p>
The larger this value is, the more only the areas near the edges will be colored.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Sharpness</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Rim.</b>
</p>
<p>
The larger this value is, the sharper the edge of the rim will be.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Inverse</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Rim.</b>
</p>
<p>
If checked, reverse the range of the rim.
</p>
</td></tr>
<tr><td colspan="3"><b>Color Mode</b></td><td>
<p>
You can specify how you set the Tint Color.
</p>
<p>
<ul>
<li>Single Color: Single Color (Default).</li>
<li>Texture 2D: Specified by the texture.</li>
<li>Texture 3D: Specified by the 3D texture (Animatable).</li>
</ul>
</p>
</td></tr>
<tr><td colspan=3><b>Color</b></td><td>
<p>
<b>This property is visible only when the Color Mode is set to Single Color.</b>
</p>
<p>
The color to be multiplied.
</p>
</td></tr>
<tr><td colspan=3><b>Texture</b></td><td>
<p>
<b>This property is visible only when the Color Mode is set to Texture 2D or Texture 3D.</b>
</p>
<p>
The texture whose color will be multiplied.
</p>
</td></tr>
<tr><td colspan=3><b>Progress</b></td><td>
<p>
<b>This property is visible only when the Color Mode is set to Texture 3D.</b>
</p>
<p>
The progress of the 3D Texture.
</p>
</td></tr>
<tr><td colspan=3><b>Blend Rate</b></td><td>
<p>
The color multiplication factor.
If this value is zero, the color will not be applied; if it is one, the color will be multiplied as is.
</p>
</td></tr>
</tbody>
</table>

#### Flow Map
You can use the Flow Map to distort the Base Map to the specified directions.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/106138524/173483428-e6027ef4-a61e-4308-a90a-542bf75b0eaf.png" alt="Flow Map"><br>
  <font color="grey">Flow Map</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Flow Map. The specifications are as follows.
</p>
<p>
<ul>
    <li>Shifts the UV values of the Base Map based on the texture color channels.</li>
    <li>The color channels used is determined by the X value and Y value of the Channels properties.</li>
    <li>The smaller the value from 0.5, the more the UV value shifts in the negative direction, and vice versa.</li>
</ul>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan=3><b>Intensity</b></td><td>
<p>
Sets the Flow Map intensity.
</p>
</td></tr>
<tr><td colspan=3><b>Targets</b></td><td>
<p>
Set the targets to which the flow map will be applied (multiple selections are possible).

* Base Map
* Tint Map
* Alpha Transition Map
* Emission Map
</p>
</td></tr>
</tbody>
</table>

#### Parallax Map
Parallax Map can create a parallax effect.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/119645979/232398368-619f9c27-aa20-41d7-ad7f-04bcbd66ead1.png" alt="Parallax Map"><br>
  <font color="grey">Parallax Map</font>
</p>
<table width="100%">
<thead>
<tr><td colspan="3"><b>Property</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
Specifies the mode of the parallax map from the following options:
</p>
<p>
<ul>
<li>Single Texture: Regular 2D texture (default)</li>
<li>Flip Book: Flip-Book animation</li>
<li>Flip Book Blending: Flip-Book animation with blending</li>
</ul>
</p>
<p>
The texture type changes depending on the selected mode.
</p>
</td></tr>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the parallax map.<br>
</p>
<p>
If Flip Book is selected in Mode, a <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture2DArray.html">Texture2DArray</a> needs to be set.<br>
If Flip Book Blending is selected in Mode, a <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture3D.html">Texture3D</a> needs to be set.<br>
</p>
The specifications of the parallax map are as follows:
<p>
<ul>
<li>Changes the surface's concavity based on the color value of the specified channel</li>
<li>The value of 0 is the original state, and the closer it gets to 1, the more concave it becomes</li>
</ul>
Since a texture is used as a value, not a color, be sure to uncheck the sRGB Color checkbox in the texture settings.
</p>
</td></tr>
<tr><td colspan=3><b>Strength</b></td><td>
<p>
Sets the strength of the parallax map when applied.
</p>
</td></tr>
<tr><td colspan=3><b>Targets</b></td><td>
<p>
Sets the targets to apply the parallax map (multiple selection possible).
Base Map
Tint Map
Emission Map
</p>
</td></tr>
</tbody>
</table>

#### Color Correction
Color Correction correct the colors up to this point.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143205890-207e62d5-174c-4f81-a1ea-a26bbc606769.png" alt="Color Correction"><br>
  <font color="grey">Color Correction</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify how to correct the colors.
</p>
<p>
<ul>
<li>None: Disable the Color Correction (Default).</li>
<li>Greyscale: Chage the colors to greyscale.</li>
<li>Gradient Map: Use the gradient map.</li>
</ul>
</p>
</td></tr>
<tr><td><td colspan=2><b>Texture</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Gradient.</b>
</p>
<p>
Sets the Gradient Map. The specifications are as follows.
<ul>
<li>Replace the luminance with the colors of the gradient map.</li>
<li>Change the U value for sampling the gradient map according to luminance.</li>
<li>Therefore, we need to use a texture with a horizontal gradient.</li>
</ul>
</p>
</td></tr>
</tbody>
</table>

#### Alpha Transition
Alpha Transition controls the alpha value using a texture.

<p align="center">
  <img width="60%" src="Documentation~/Images/unlit_alpha_transition.png" alt="Alpha Transition"><br>
  <font color="grey">Alpha Transition</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify Alpha Transition Mode from the following options.
</p>
<p>
<ul>
<li>None: Disable the Alpha Transition (Default).</li>
<li>Fade: Transition using the fade texture.</li>
<li>Dissolve: Transition using the dissolve texture.</li>
</ul>
</p>
</td></tr>
<td colspan=3><b>Map Mode</b></td><td>
<p>
You can specify the type of the Alpha Transition Map from the following options.
</p>
<p>
<ul>
<li>Single Texture: Use 2D texture (Default).</li>
<li>Flip Book: Use Flip-Book animation.</li>
<li>Flip Book Blending: Use Flip-Book animation with interpolated frames.</li>
</ul>
</p>
<p>
The required texture type will change depending on the mode you selected.
</p>
</td></tr>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Alpha Transition Map. The specifications are as follows.
<ul>
    <li>Changes the alpha value based on the texture color channel.</li> 
    <li>The color channels used is determined by the X value of the Channels properties.</li>
    <li>The smaller the R value, the easier it is to disappear, and vice versa.</li>
</ul>
</p>
<p>
If you specify Flip Book as the Mode, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture2DArray.html">Texture2DArray</a>.
And if you specify Flip Book Blending, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture3D.html">Texture3D</a>.
</p>
<p>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan="3"><b>Flip-Book Progress</b></td><td>
<p>
<b>This property is visible only when the Map Mode is set to Flip Book or Flip Book Blending.</b>
</p>
<p>
Sets the progress of Flip-Book or Flip-Book Blending.
</p>
</td></tr>
<tr><td colspan="3"><b>Transition Progress</b></td><td>
<p>
Sets the progress of Transition.
</p>
</td></tr>
<tr><td colspan="3"><b>2nd Texture Blend Mode</b></td><td>
<p>
You can specify how to composite the second texture from the following options.
</p>
<p>
<ul>
<li>None: Don't use The second texture (Default).</li>
<li>Additive: Referring to the average of two textures.</li>
<li>Multiply</li>
</ul>
</p>
<tr><td colspan="3"><b>Edge Sharpness</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Dissolve.</b>
</p>
<p>
Sets the sharpness of the edge.
</p>
</td></tr>
</tbody>
</table>

#### Emission
The Emission control how the particles are glowing.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/106138524/173484557-4f2504af-c93a-4f5a-85b0-ac07b8e0d6fe.png" alt="Emission"><br>
  <font color="grey">Emission</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify the area to be glowed.
</p>
<p>
<ul>
<li>None: Disable the Emission (Default).</li>
<li>All: The entire surface will glow.</li>
<li>By Texture: The area specified by the texture will glow.</li>
<li>Edge: Edges (areas where alpha is greater than 0 and less than 1) will glow.</li>
</ul>
</p>
</td></tr>
<td colspan=3><b>Map Mode</b></td><td>
<p>
<b>This property is visible only when the Mode is set to By Texture.</b>
</p>
<p>
You can specify the Mode of the Emission Map from the following options.
</p>
<p>
<ul>
<li>Single Texture: Use 2D texture (Default).</li>
<li>Flip Book: Use Flip-Book animation.</li>
<li>Flip Book Blending: Use Flip-Book animation with interpolated frames.</li>
</ul>
</p>
<p>
The required texture type will change depending on the mode you selected.
</p>
</td></tr>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
<b>This property is visible only when the Mode is set to By Texture.</b>
</p>
<p>
Sets the Emission Map. The specifications are as follows.
<ul>
    <li>The larger the texture color channles value, the more likely it is to glow.</li>
    <li>The color channels used is determined by the X value of the Channels property.</li>
</ul>
</p>
<p>
If you specify Flip Book as the Mode, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture2DArray.html">Texture2DArray</a>.
And if you specify Flip Book Blending, you need to set <a href="https://docs.unity3d.com/2020.3/Documentation/Manual/class-Texture3D.html">Texture3D</a>.
</p>
<p>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan="3"><b>Flip-Book Progress</b></td><td>
<p>
<b>This property is visible only when the Mode is set to By Texture and the Map Mode is set to Flip Book or Flip Book Blending.</b>
</p>
<p>
Sets the progress of Flip-Book or Flip-Book Blending.
</p>
</td></tr>
<tr><td colspan="3"><b>Color Type</b></td><td>
<p>
You can specify the type of the emission color from the following options.
<ul>
<li>Color: Use the specified single color as the emission color.</li>
<li>Base Color: Use the RGB values up to this point as the emission color.</li>
<li>Gradient Map: Use a gradient map to specify the emission color.</li>
</ul>
Gradient Map can only be selected when Mode is set to By Texture or Edge.
</p>
</td></tr>
<tr><td colspan="3"><b>Color</b></td><td>
<p>
<b>This property is visible only when the Color Type is set to Color.</b>
</p>
<p>
Sets the emission color as HDR color.
</p>
</td></tr>
<tr><td colspan="3"><b>Keep Edge Transparency</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Edge.</b>
</p>
<p>
If checked, the edges will not be transparent.
</p>
</td></tr>
<tr><td colspan="3"><b>Gradient Map</b></td><td>
<p>
<b>This property is visible only when the Color Type is set to Gradient Map.</b>
</p>
<p>
Sets the Gradient Map. The specifications are as follows.
<ul>
<li>When Mode is By Texture: Sampling the R value of the Emission Map as the U value of the Gradient Map.</li>
<li>When Mode is Edge: Sample alpha values as U values of the Gradient Map.</li>
<li>You need to use a texture with a horizontal gradient as the Gradient Map.</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>Intensity</b></td><td>
<p>
Sets the intensity of the emission.
</p>
</td></tr>
</tbody>
</table>

#### Transparency
Control the transparency in various ways.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143206260-12b123fe-858a-4770-96d1-a47cbab4079f.png" alt="Transparency"><br>
  <font color="grey">Transparency</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Rim</b></td><td>
<p>
If checked, the rim will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Progress</b></td><td>
<p>
Sets the progress of transparency.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Sharpness</b></td><td>
<p>
The larger this value is, the sharper the edge will be.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Inverse</b></td><td>
<p>
Inverse the transparency area.
</p>
</td></tr>
<tr><td colspan="3"><b>Luminance</b></td><td>
<p>
If checked, areas with low brightness will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Progress</b></td><td>
<p>
Sets the progress of transparency.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Sharpness</b></td><td>
<p>
The larger this value is, the sharper the edge will be.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Inverse</b></td><td>
<p>
Inverse the transparency area.
</p>
</td></tr>
<tr><td colspan="3"><b>Sort Particles</b></td><td>
<p>
If checked, soft particles will be enabled.
To use this feature, the Depth Texture in URP settings must be enabled.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Intensity</b></td><td>
<p>
The larger this value, the larger the transparent area.
</p>
</td></tr>
<tr><td colspan="3"><b>Depth Fade</b></td><td>
<p>
Makes the areas near and far from the camera transparent.
To use this feature, the Depth Texture in URP settings must be enabled.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Distance</b></td><td>
<p>
Set the transparency range.
The area closer than Near and farther than Far from the camera will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Width</b></td><td>
<p>
The distance from the beginning of transparency to the end of complete transparency.
</p>
</td></tr>
</tbody>
</table>

## Uber Lit Shader
The Uber Lit shader is a multifunctional shader with lighting applied. Use this shader for particles that need lighting, such as mesh effects.
This shader reflects lighting compliant with Unity's PBR lighting specification. 

To use this shader, assign the `Nova/Particles/UberLit` shader to your material. 
This shader is based on the Uber Unlit shader with additional processing and properties.

The description of the added properties is as follows.



#### Render Settings
The following red-framed properties have been added to Render Settings.

<p align="center">
  <img width="60%" src="Documentation~/Images/lit_rendersettings_01.png" alt="Render Settings"><br>
  <font color="grey">Render Settings</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Work Flow Mode</b></td><td>
<p>
You can specify the mode of PBR workflow from the following options.<br>
</p>
<p>
<ul>
<li>Specular</li>
<li>Metallic</li>
</ul>
</p>
For more information on workflow, please refer to the following URL
https://docs.unity3d.com/2018.4/Documentation/Manual/StandardShaderMetallicVsSpecular.html
https://docs.unity3d.com/2018.4/Documentation/Manual/StandardShaderMaterialParameterSpecular.html
https://docs.unity3d.com/2018.4/Documentation/Manual/StandardShaderMaterialParameterMetallic.html
<tr><td colspan="3"><b>Receive Shadows</b></td><td>
<p>
If checking the box, it will be received shadows from directional lights.<br>
</p>
<tr><td colspan="3"><b>Specular Highlights</b></td><td>
<p>
If checking the box, it will be enabled specular highlighting.<br>
</p>
<tr><td colspan="3"><b>Environment Reflections</b></td><td>
<p>
If checking the box, it will be affected ambient light from the Reflection Probe and Skybox.<br>
For more information on the effects of ambient light, please refer to "Environment Reflection" at the following URL.<br>
https://docs.unity3d.com/2018.4/Documentation/Manual/GlobalIllumination.html
</p>
</tbody>
</table>

#### Surface Maps
Several surface properties have been added for lighting.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/106138524/187354400-aedd2347-cc5d-4b39-bf87-ef5318177bba.png" alt="Surface Maps"><br>
  <font color="grey">Surface Maps</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Normal Map</b></td><td>
<p>
Set up the normal map.The specifications of the normal map are as follows.<br>
<ul>
<li>The only normal maps that can be set up are those for tangent space.</li>
<li>If normal map isn't specified, vertex normals are used</li>
<li>The normal scale value can be specified. The default scale is 1.0.</li>
</ul>
For more infomation of the normal map
Please refer to the following URL for details on the normal map.<br>
https://docs.unity3d.com/2021.3/Documentation/Manual/StandardShaderMaterialParameterNormalMap.html
</p>
<tr><td colspan="3"><b>Metallic Map</b></td><td>
<b>It is displayed when the Work Flow Mode is set to Metallic.</b>

Set up the metallic map.The specifications of the metallic map are as follows.<br>
<ul>
<li>If the metallic map isn't set, the value of the metallic property will be used as the uniform metallicity.
<li>If the metallic map is set、the value of the metallic property will be used as multiplier value.</li>
<li>The Channels property allows specifying the channel where the metallic value is stored. Default is R channel.</li>
</ul>
Please refer to the following URL for details on the metallic map.<br>
https://docs.unity3d.com/2018.4/Documentation/Manual/StandardShaderMaterialParameterMetallic.html
</ul>

<tr><td colspan="3"><b>Specular Map</b></td><td>
<b>It is displayed when the Work Flow Mode is set to Specular.</b>

Set up the specular map. The specifications of the specular map are as follows.<br>
<ul>
<li>If the specular map isn't set, the color of specular property will be used as the uniform specular color.</li>
<li>If the specular map is set, the color of specular property will be used as the multiplier color.</li>
</ul>
Please refer to the following URL for details on the specular map.<br>
https://docs.unity3d.com/2018.4/Documentation/Manual/StandardShaderMaterialParameterSpecular.html
</ul>

<tr><td colspan="3"><b>Smoothness Map</b></td><td>
Set up the smoothness map. The specifications of the smoothness map are as follows.<br>
<br>
<ul>
<li>If the smoothness map isn't set, the value of the smoothness property will be used as the uniform smoothess.</li>
<li>If the smoothness map is set, the value of the smoothness property will be used as the multiplier value.</li>
<li>The Channels property allows specifying the channel where the smoothness value is stored. Default is α channel.</li>
</ul>
</tbody>
</table>

## Distortion Shader
Using the Distortion shader, you can apply distortion effects to the screen, such as heat waves.

To use this shader, assign the `Nova/Particles/Distortion` shader to your material. The following is the descriptions of each property that can be set from the Inspector.

#### Render Settings
The Render Settings control how the Material is rendered.

<p align="center">
  <img width="60%" src="Documentation~/Images/distortion_rendersettings_01.png" alt="Render Settings"><br>
  <font color="grey">Render Settings</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Render Face</b></td><td>
<p>
You can specify the rendering face from the following options.
</p>
<p>
<ul>
<li>Front (Default)</li>
<li>Back</li>
<li>Both</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>ZTest</b></td><td>
<p>
You can set ZTest value.
</p>
<p>
<ul>
<li>Disabled</li>
<li>Never</li>
<li>Less</li>
<li>Equal</li>
<li>Less Equal (Default)</li>
<li>Greater</li>
<li>Not Equal</li>
<li>Greater Equal</li>
<li>Always</li>
</ul>
</p>
</td></tr>
</tbody>
</table>

#### Distortion
Distortion controls how it is distorted.

<p align="center">
  <img width="60%" src="Documentation~/Images/distortion.png" alt="Distortion"><br>
  <font color="grey">Distortion</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Distortion Map. The specifications are as follows.
</p>
<p>
<ul>
    <li>Distorts the screen based on the texture color channels.</li>
    <li>The color channels used is determined by the X value of the Channels Property.</li>
    <li>0.5 is the reference value, and the further away from the reference value, the stronger the distortion.</li>
</ul>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan=3><b>Intensity</b></td><td>
<p>
Sets the distortion intensity.
</p>
</td></tr><tr><td colspan="3"><b>Rotation</b></td><td>
<p>
Set the amount of rotation of the Distortion Map.
</p>
</td></tr>
<tr><td></td><td colspan=2><b>Offset</b></td><td>
<p>
Shifts the center coordinate of rotation.
</p>
</td></tr>
<tr><td colspan="3"><b>Mirror Sampling</b></td><td>
<p>
If true, mirrors the texture to create a repeating pattern.
</p>
</td></tr>
<tr><td colspan="3"><b>Use As Normal Map</b></td><td>
<p>
If true, the Texture Type is treated as a Normal Map and will be unpacked for sampling.<br>
Additionally, please specify a normalized Normal Map for the Texture. Otherwise, the rendering results may vary depending on the platform.
</p>
</td></tr>
</tbody>
</table>

#### Flow Mapping
You can use the Flow Map to distort the Distortion Map to the specified directions.

<p align="center">
  <img width="60%" src="Documentation~/Images/flow_mapping.png" alt="Flow Map"><br>
  <font color="grey">Flow Map</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Flow Map. The specifications are as follows.
</p>
<p>
<ul>
    <li>Shifts the UV values of the Distortion Map based on the texture color channles.</li>
    <li>The texture color channels used is determined by the X value of the Channels property.</li>
    <li>The smaller the value from 0.5, the more the UV value shifts in the negative direction, and vice versa.</li>
</ul>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan=3><b>Intensity</b></td><td>
<p>
Sets the Flow Map intensity.
</p>
</td></tr>
</tbody>
</table>

#### Alpha Transition
Alpha Transition controls the alpha value using a texture.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/106138524/173484691-f2ec4b11-cf2f-404d-890f-3331a45bbf5a.png" alt="Alpha Transition"><br>
  <font color="grey">Alpha Transition</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Mode</b></td><td>
<p>
You can specify Alpha Transition Mode from the following options.
</p>
<p>
<ul>
<li>None: Don't use the Alpha Transition (Default).</li>
<li>Fade: Transition using the fade texture.</li>
<li>Dissolve: Transition using the dissolve texture.</li>
</ul>
</p>
</td></tr>
<tr><td colspan="3"><b>Texture</b></td><td>
<p>
Sets the Alpha Transition Map. The specifications are as follows.
<ul>
    <li>Changes the alpha value based on the texture color channel.</li>
    <li>The color channles used is determined by the X value of the Channels property.</li>
    <li>The smaller the R value, the easier it is to disappear, and vice versa.</li>
</ul>
</p>
<p>
Note that you need to uncheck sRGB Color in the texture import setting because the pixels will be used as values, not colors.
</p>
</td></tr>
<tr><td colspan="3"><b>Progress</b></td><td>
<p>
Sets the progress of Transition.
</p>
</td></tr>
<tr><td colspan="3"><b>Edge Sharpness</b></td><td>
<p>
<b>This property is visible only when the Mode is set to Dissolve.</b>
</p>
<p>
Sets the sharpness of the edge.
</p>
</td></tr>
</tbody>
</table>

#### Transparency
Control the transparency in various ways.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143213193-3405c0c0-5812-4c41-be52-18b8c48ebd1c.png" alt="Transparency"><br>
  <font color="grey">Transparency</font>
</p>

<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Description</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Rim</b></td><td>
<p>
If checked, the rim will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Progress</b></td><td>
<p>
Sets the progress of transparency.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Sharpness</b></td><td>
<p>
The larger this value is, the sharper the edge will be.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Inverse</b></td><td>
<p>
Inverse the transparency area.
</p>
</td></tr>
<tr><td colspan="3"><b>Luminance</b></td><td>
<p>
If checked, areas with low brightness will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Progress</b></td><td>
<p>
Sets the progress of transparency.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Sharpness</b></td><td>
<p>
The larger this value is, the sharper the edge will be.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Inverse</b></td><td>
<p>
Inverse the transparency area.
</p>
</td></tr>
<tr><td colspan="3"><b>Sort Particles</b></td><td>
<p>
If checked, soft particles will be enabled.
To use this feature, the Depth Texture in URP settings must be enabled.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Intensity</b></td><td>
<p>
The larger this value, the larger the transparent area.
</p>
</td></tr>
<tr><td colspan="3"><b>Depth Fade</b></td><td>
<p>
Makes the areas near and far from the camera transparent.
To use this feature, the Depth Texture in URP settings must be enabled.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Distance</b></td><td>
<p>
Set the transparency range.
The area closer than Near and farther than Far from the camera will be transparent.
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Width</b></td><td>
<p>
The distance from the beginning of transparency to the end of complete transparency.
</p>
</td></tr>
</tbody>
</table>


## Uber Unlit/Lit shaders( for uGUI )
Uber Unlit/Lit shaders are available for uGUI. if you want to play the effect on uGUI, use `Nova/UIParticles/UberUnlit` or `Nova/UIParticles/UberLit`.

The items that can be set in the material inspector are basically the same as for the regular `UberUnlit` and `UberLit`, but note that the z and w elements are not available when working with Custom Vertex Streams.
This is because z and w data are discarded inside uGUI.

If z and w are used, an error will be displayed as shown in the following figure.

<p align="center">
  <img width="60%" src="Documentation~/Images/custom_vertex_error.png" alt="Custom Vertex Error"><br>
  <font color="grey">Custom Vertex Error</font>
</p>

Also, if you want to [use with the Custom Vertex Streams](#use-with-the-custom-vertex-streams), please add TexCoord1 and TexCoord2 to `Additional Shader Channels` in Canvas.

<p align="center">
  <img width="60%" src="Documentation~/Images/additional_shader_channels.png" alt="Additional Shader Channels"><br>
  <font color="grey">Additional Shader Channels</font>
</p>


## Abort Shadow Caster
Enabling the Shadow Caster feature will allow you to cast shadows from NovaShader.
<p align="center">
  <img width="60%" src="Documentation~/Images/shadow_caster_01.png" alt="Shadow Caster"><br>
  <font color="grey">Shadow Caster</font>
</p>
<table width="100%">
<thead>
<tr><td colspan="3"><b>Property Name</b></td><td><b>Discription</b></td></tr>
</thead>
<tbody>
<tr><td colspan="3"><b>Enable</b></td><td>
<p>
Check this to enable ShadowCasterPass
</p>
</td></tr>
<tr><td colspan="3"><b>Apply Vertex Deformation</b></td><td>
<p>
If checked, Vertex Deformation will be applied to shadow casting calculations
</p>
</td></tr>
<tr><td colspan="3"><b>Alpha Test Enable</b></td><td>
<p>
If checked, Alpha Test will be enabled for calculating shadow casting, and shadows will not be cast in areas that do not pass.<br>
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Cutoff</b></td><td>
<p>
Areas where the Alpha value is less than the Cutoff value will not cast a shadow (this will be a different value from the Cutoff of the drawing process)
</p>
</td></tr>
<tr><td colspan="3"><b>Alpha Affected By</b></td><td>
<p>
Items that affect Alpha value during shadow casting calculation
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Tint Color</b></td><td>
<p>
If checked, Tint Color will affect Alpha value
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Flow Map</b></td><td>
<p>
If checked, Flow Map will affect Alpha value
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Alpha Transition Map</b></td><td>
<p>
If checked, Alpha Transition Map will affect Alpha value
</p>
</td></tr>
<tr><td></td><td colspan="2"><b>Transparency Luminance</b></td><td>
<p>
If checked, Transparency Luminance will affect Alpha value
</p>
</td></tr>
</tbody>
</table>

<p align="center">
  <img width="60%" src="Documentation~/Images/shadow_caster_demo01.gif" alt="Shadow Caster Demo"><br>
  <font color="grey">Shadow Caster Demo</font>
</p>


## Use with the Custom Vertex Streams
Using the Particle System's Custom Vertex Streams, you can animate the properties of the Material.
In the following example, we will use the Custom Vertex Streams to rotate the texture.

#### Set up the Custom Data
First, set up the [Particle System's Custom Data](https://docs.unity3d.com/2019.4/Documentation/Manual/PartSysCustomDataModule.html).
In this case, we have set a value that changes from 0 to 1 over time to `Custom1.X`.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143419403-d5f33c24-6875-4e0e-bf05-c6ebdd94bb94.png" alt="Custom Data"><br>
  <font color="grey">Custom Data</font>
</p>

#### Set up the Custom Vertex Streams
Next, set the Custom Vertex Streams as shown below to pass `Custom1.x` to `TEXCOORD1.x`.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143446418-0daf3b2e-7f21-4b0a-a78e-aac50770a186.png" alt="Custom Vertex Streams"><br>
  <font color="grey">Custom Vertex Streams</font>
</p>

#### Set up the Material Property
Next, select `COORD 1X` from the `Rotation` dropdown to use the value passed to `TEXCOORD1.x` as shown below.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143424542-61dc2d6b-402f-45d1-85bb-a2170e05643c.png" alt="Coord"><br>
  <font color="grey">Coord</font>
</p>

The texture will now rotate.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143531888-b49c55e9-3df1-4dae-a0fe-d3e4a1638af2.gif" alt="Rotation"><br>
  <font color="grey">Rotation</font>
</p>

## Use Mesh GPU Instancing
You can use the [Particle System Mesh GPU Instancing](https://docs.unity3d.com/Manual/PartSysInstancing.html) to draw particles efficiently.
The following section describes how to use `Mesh GPU Instancing` for materials using this shader.

> **Note**  
> When displaying particles on the preview screen using **PreviewRenderUtility**, we have confirmed a bug on the Unity side that does not render correctly when **Enable Mesh GPU Instancing** is enabled.

#### Enable Mesh GPU Instancing
To use `Mesh GPU Instancing`, you need to set the `Render Mode` of the `Renderer` module to Mesh.
Then, check the `Enable Mesh GPU Instancing` checkbox.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143447533-0e03627f-9af0-43cd-bab1-254c78ea7f93.png" alt="Enable Mesh GPU Instancing"><br>
  <font color="grey">Enable Mesh GPU Instancing</font>
</p>

#### Set up the Custom Vertex Streams
Next, set up the Custom Vertex Streams as shown below.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/47441314/143448020-45beb08a-6795-4372-894a-c04e33a8029d.png" alt="Custom Vertex Streams"><br>
  <font color="grey">Custom Vertex Streams</font>
</p>

The `Custom Data` section can be filled with values from other modules, such as Noise, but make sure that all of `INSTANCED1.xyzw` and `INSTANCED2.xyzw` are filled without excess or deficiency.

Now you can use `Mesh GPU Instancing`.


## Automatic set up the Custom Vertex Streams.
We have seen several cases where Custom Vertex Streams are set up manually.<br/>
In addition, the vertex streams required by the GPUs vary depending on their settings.<br/>

It has a feature that automatically set up the Custom vertex streams as `Fix Now`.

#### Fix Now
If vertex streams are different from what the CPUs require, <br/>
`Fix Now` button and error will appear at bottom of the inspector.<br/>

At this time, pressing this button will automatically set the typical vertex stream needed.

<p align="center">
  <img width="60%" src="https://user-images.githubusercontent.com/106138524/191191870-7b22351b-e826-4ccb-92c9-693009133909.png" alt="Fix Now"><br>
  <font color="grey">Fix Now</font>
</p>

If you are not familiar with setting up Custom Vertex Streams,<br/>
It is recommended that you use `Fix Now` to correct errors to avoid unwanted errors.

## Remove Unused Parameter References
During the trial-and-error process of effect creation, references to parameters that are not actually used may remain.<br/>
(Example: The Base Map Mode is set to Single Texture, but a Texture 2D Array for Flip Book is also assigned.)<br/>
If unused texture references remain, issues such as an increase in Asset Bundle size may occur, so it is recommended to remove them.<br/>
To address this, there is a feature called `RemoveUnusedReferences`.<br/>
You can execute this feature by selecting one or more materials in the Project view and navigating to `Tools > NOVA Shader > RemoveUnusedReferences`.<br/>
If any unused references are removed, a log will be output to the Console.<br/>

## Reducing Memory Usage with Optimized Shaders
The Uber Unlit/Lit shaders are versatile general-purpose shaders with many shader keywords defined. This can potentially lead to an explosion of variants due to keyword combinations.

Additionally, shader passes such as `Depth Only Pass`, `Depth Normals Pass`, and `Shadow Caster Pass` may be included even when unnecessary for certain projects.

These factors can cause increased memory usage in Uber shaders.

To address this issue, Nova Shader provides the following editor APIs for generating and applying optimized shaders:

|API|Description|
|---|---|
|OptimizedShaderGenerator.Generate()|Generate optimized shaders|
|OptimizedShaderGenerator.Replace()|Replace with optimized shaders|

By utilizing these APIs to optimize shaders, we have confirmed that memory usage can be reduced by up to 50%.


### OptimizedShaderGenerator.Generate()
Generates optimized shaders from the Uber shader. The generated shaders are created based on combinations of `Rendering Type` and `Used Shader Passes` as shown below.<br/>
For detailed information on how to use the API, please refer to the API reference [OptimizedShaderGenerator](Documentation~/OptimizedShaderGenerator.md).
<p align="center">
  <img width="60%" src="Documentation~/Images/optimized_shader.png" alt="Optimized Shader"><br>
  <font color="grey">Optimized Shader</font>
</p>

### OptimizedShaderGenerator.Replace()
Replaces Uber shaders assigned to materials with appropriate optimized shaders based on their rendering type and shader pass settings. To use this API, you must first generate optimized shaders using `OptimizedShaderGenerator.Generate()`.<br/>
For detailed information on how to use the API, please refer to the API reference [OptimizedShaderReplacer](Documentation~/OptimizedShaderReplacer.md).

### Sample Code
Please refer to [ShaderOptimizeSample.cs](https://github.com/CyberAgentGameEntertainment/NovaShader/blob/main/Assets/Samples/Editor/ShaderOptimizeSample.cs) for a sample implementation of using these APIs.

## Editor APIs Reference
- [RenderErrorHandler](Documentation~/RenderErrorHandler.md)
- [OptimizedShaderGenerator](Documentation~/OptimizedShaderGenerator.md)
- [OptimizedShaderReplacer](Documentation~/OptimizedShaderReplacer.md)
## Licenses
This software is released under the MIT license.
You are free to use it within the scope of the license, but the following copyright and license notices are required.

* [LICENSE.md](LICENSE.md)

In addition, the table of contents for this document has been created using the following software

* [toc-generator](https://github.com/technote-space/toc-generator)

See [Third Party Notices.md](Third%20Party%20Notices.md) for more information about the license of toc-generator.
