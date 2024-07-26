# Photo Editor

## Description

This is a simple online photo editor designed to apply a bloom effect to an imported image.
It provides a simple way to customize bloom settings, apply it to a photo, and compare the original image with the edited one.

## Tools
1. Based on ASP.NET MVC Core.
2. UI: HTML 5, CSS 3, JS, Bootstrap.
3. Image manipulation: SixLabors.ImageSharp external library.
4. CI/CD support with Azure DevOps.
5. Deployed using Azure Web Apps.

## Bloom algorithm
1. Bilinear interpolation, downscaling.
2. Gaussian threshold.
3. Gaussian blur.
4. Bilinear interpolation, upscaling.
5. Additive blending.

>[!IMPORTANT]
>## [Try the app out](https://bloom-effect.azurewebsites.net/)

## How to use
<ul>
  <li>
    <h3>Import an image by clicking on it or dragging it to the dotted area:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/browse-a-photo.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>Customize bloom parameters to make them optimal for the selected image:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/bloom-customization.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>Observe the edited photo and compare it with the original one:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/before-after.PNG" width="100%" />
    </p>
  </li>
</ul>

## More examples
<img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/before-after-2.PNG" width="100%" />
<img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/before-after-3.PNG" width="100%" />
<img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/before-after-4.PNG" width="100%" />

## Step-by-step results of the algorithm
<ul>
  <li>
    <h3>Imported image downscaling using bilinear interpolation:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/downscaling.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>The reduced image thresholding using Gaussian threshold:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/threshold.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>The thresholded image Gaussian blur:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/blur.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>The blurred image upscaling using bilinear interpolation:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/upscaling.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>Additive blending of the original image with the upscaled one:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/additive-blending.PNG" width="100%" />
    </p>
  </li>
</ul>

## Gaussian threshold of the image with different lighting
<p>
  <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/gaussian-threshold.PNG" width="100%" />
</p>

## Optimization results
### The images below illustarte the optimization results for a 1917 × 1278 image with the default bloom setup: downscaling ratio – 16, Gaussian kernel radius – 10.
### Each stage duration result, as well as a total response time, is presented on each image.
<ul>
  <li>
    <h3>Before optimizations:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/before-optimizations.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>With separable Gaussian kernel only:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/gaussian-kernel-separability.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>With image scaling only:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/image-scaling.PNG" width="100%" />
    </p>
  </li>
  <li>
    <h3>With all optimizations, meaning image scaling and a separable kernel:</h3>
    <p>
      <img src="https://github.com/beingful/photo-editor/blob/attachments/attachments/all-optimizations.PNG" width="100%" />
    </p>
  </li>
</ul>
