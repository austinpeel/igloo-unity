mergeInto(LibraryManager.library, {
  // Lens parameters
  SetLensParams: function (thetaE, axisRatio, positionAngle, x0, y0) {
    window.dispatchReactUnityEvent(
      'SetLensParams',
      thetaE,
      axisRatio,
      positionAngle,
      x0,
      y0
    );
  },
  // Source parameters
  SetSourceParams: function (
    rSersic,
    nSersic,
    axisRatio,
    positionAngle,
    x0,
    y0
  ) {
    window.dispatchReactUnityEvent(
      'SetSourceParams',
      rSersic,
      nSersic,
      axisRatio,
      positionAngle,
      x0,
      y0
    );
  },
});
