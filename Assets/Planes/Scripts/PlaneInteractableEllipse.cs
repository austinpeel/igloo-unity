using UnityEngine;

public class PlaneInteractableEllipse : Plane
{
    [SerializeField] protected InteractableEllipseUI interactEllipseUI;

    protected void Start() 
    {
        interactEllipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
        interactEllipseUI.OnEllipsePositionEndDrag += OnEllipsePositionEndDragHandler;
        interactEllipseUI.OnEllipseEinsteinChanged += OnEllipseEinsteinChangedHandler;
        interactEllipseUI.OnEllipseQChanged += OnEllipseQChangedHandler;
        interactEllipseUI.OnEllipseAngleChanged += OnEllipseAngleChangedHandler;
    }

    protected void OnDestroy() 
    {
        interactEllipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
        interactEllipseUI.OnEllipsePositionEndDrag -= OnEllipsePositionEndDragHandler;
        interactEllipseUI.OnEllipseEinsteinChanged -= OnEllipseEinsteinChangedHandler;
        interactEllipseUI.OnEllipseQChanged -= OnEllipseQChangedHandler;
        interactEllipseUI.OnEllipseAngleChanged -= OnEllipseAngleChangedHandler;
    }

    protected new void Update() 
    {
        if (!interactEllipseUI) return;

        base.Update();

        if (GetHasModeChanged())
        {
            interactEllipseUI.SetIsInSnapMode(GetIsInSnapMode());
        }
    }

    public void ResetEllipseParameters()
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.ResetParameters();
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public void SetEllipseQParameter(float newQ)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetQ(newQ, true);
    }

    // Get the q ratio of the ellipse
    public float GetEllipseQParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetQParameter();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public void SetEllipsePhiAngleParameter(float newAngle)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetAngle(newAngle, true);
    }

    // Get the phi angle of the ellipse in degree
    public float GetEllipseAngleParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetAngleParameter();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetEinsteinRadius(newEinsteinRadius, true);
    }

    // Get the einstein radius of the ellipse in coordinate
    public float GetEllipseEinsteinRadiusParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetEinsteinRadiusParameter();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetCenterPosition(newCenterCoord, true);
    }

    // Get the position of the center of the ellipse in coordinate
    public Vector2 GetEllipseCenterPositionParameter()
    {
        if (!interactEllipseUI) return Vector2.zero;

        return interactEllipseUI.GetCenterPositionParameter();
    }

    protected void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // if angleNewPosition.x > 0 => turn clockwise (decrease angle)
        // if angleNewPosition.x < 0 => turn anti-clockwise (increase angle)
        float sign = (angleNewPosition.x > 0) ? -1.0f : 1.0f;

        float deltaAngle = sign * Vector2.Angle(Vector2.up, angleNewPosition.normalized);
        float angle = GetEllipseAngleParameter();

        interactEllipseUI.SetAngle(angle + deltaAngle, true);
        

        if (!interactEllipseUI.GetIsInRotationMode())
        {
            interactEllipseUI.SetIsInRotationMode(true);
            interactEllipseUI.DisplayRotationLines(true);
        }

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetAnglePoint();
        }
    }

    protected void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        interactEllipseUI.SetQWithYAxis(qNewPosition.y);

        // Update the positions of the points parameter
        interactEllipseUI.UpdatePointsParametersPositions();

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetQPoint();
        }
    }

    protected void OnEllipseEinsteinChangedHandler(Vector2 einsteinNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Convert position in LensPlane Rect
        Vector2 einsteinLensRect = ConvertEllipseRectToLensPlaneRect(einsteinNewPosition);
        // Check if it's outside the boundaries
        if (!CheckPositionInBoundaries(einsteinLensRect))
        {
            // Convert to the limit position
            Vector2 convertedPosition = ConvertToLimitPosition(einsteinLensRect);
            float convertedWidthX = (convertedPosition - interactEllipseUI.GetCenterPositionInRect()).x;
            // Get the RectTransform Einstein radius that corresponds to the position X
            float convertedEinsteinR = interactEllipseUI.ComputeEinsteinRadius(convertedWidthX, interactEllipseUI.ComputeMajorAxis(convertedWidthX, GetEllipseQParameter()));

            Vector2 convertedCoord = ConvertRectPositionToCoordinate(Vector2.right * convertedEinsteinR);
            // Move the ellipse to this limit position
            interactEllipseUI.SetEinsteinRadius(convertedCoord.x, true);

            return;
        }

        // Get the RectTransform Einstein radius that corresponds to the position X
        float convertedR = interactEllipseUI.ComputeEinsteinRadius(einsteinNewPosition.x, interactEllipseUI.ComputeMajorAxis(einsteinNewPosition.x, GetEllipseQParameter()));

        float einsteinInCoord = ConvertRectPositionToCoordinate(Vector2.right * convertedR).x;

        interactEllipseUI.SetEinsteinRadius(einsteinInCoord, true);
    }

    protected void OnEllipsePositionChangedHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Check if it's outside the boundaries
        if (!CheckPositionInBoundaries(ellipseNewPosition))
        {
            // Convert to the limit position
            Vector2 convertedPosition = ConvertToLimitPosition(ellipseNewPosition);
            Vector2 convertedCoord = ConvertRectPositionToCoordinate(convertedPosition);
            // Move the ellipse to this limit position
            interactEllipseUI.SetCenterPosition(convertedCoord, true);
        }
        // Else if it remains inside the boundaries simply, then simply moves the ellipse
        else 
        {
            Vector2 convertedCoord = ConvertRectPositionToCoordinate(ellipseNewPosition);
            // Move the ellipse to the ellipseNewPosition
            interactEllipseUI.SetCenterPosition(convertedCoord, true);
        }

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetCenterPoint();
        }

        // Display the grid
        GridUI gridUI = GetGridUI();
        if (gridUI)
        {
            gridUI.SetGridVisibility(true);
        }
    }

    protected void OnEllipsePositionEndDragHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Don't display the grid
        GridUI gridUI = GetGridUI();
        if (gridUI)
        {
            gridUI.SetGridVisibility(false);
        }

        // Put the ellipse back at the old position if it hides any point
        if(!CheckAllEllipsePointsVisibility())
        {
            // Convert the old position
            Vector2 oldConvertedPosition = ConversionUtils.ConvertScreenPositionToRect(rectTransform, GetComponentInParent<Canvas>().worldCamera, ellipseOldCursorPosition);
            Vector2 oldConvertedCoord = ConvertRectPositionToCoordinate(oldConvertedPosition);
            // Move the ellipse to the old position
            interactEllipseUI.SetCenterPosition(oldConvertedCoord, true);
        }
    }

    private bool CheckPositionInBoundaries(Vector2 position)
    {
        float limitPositionX = width / 2f - GetBoundaryX();
        float limitPositionY = height / 2f - GetBoundaryY();

        // Check the x coordinate
        if (-limitPositionX <= position.x && position.x <= limitPositionX)
        {
            // Check the y coordinate
            if (-limitPositionY <= position.y && position.y <= limitPositionY)
            {
                return true;
            }
        }

        return false;
    }

    // Change the position so that it is in the boundaries
    private Vector2 ConvertToLimitPosition(Vector2 position)
    {
        Vector2 convertedPosition = position;

        float limitPositionX = width / 2f - GetBoundaryX();
        float limitPositionY = height / 2f - GetBoundaryY();

        if (Mathf.Abs(position.x) > limitPositionX)
        {
            convertedPosition.x = Mathf.Sign(position.x) * limitPositionX;
        }

        if (Mathf.Abs(position.y) > limitPositionY)
        {
            convertedPosition.y = Mathf.Sign(position.y) * limitPositionY;
        }
        
        return convertedPosition;
    }

    private Vector2 ConvertEllipseRectToLensPlaneRect(Vector2 ellipseRect)
    {
        return interactEllipseUI.GetCenterPositionInRect() + ellipseRect;
    }

    // Check that all of the button on the ellipse are displayed on the screen
    private bool CheckAllEllipsePointsVisibility()
    {
        Vector2 centerEllipsePosition = interactEllipseUI.GetCenterPositionInRect();
        float widthLimit = width / 2f;
        float heightLimit = height / 2f;

        // Check for the Q Point
        float qRectDistance = interactEllipseUI.GetPositionRectQPoint().y;
        float qRotationAngle = interactEllipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedQPointPosition = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * qRotationAngle) * qRectDistance, 
                                                    Mathf.Cos(Mathf.Deg2Rad * qRotationAngle) * qRectDistance);
        Vector2 qPointPosition = centerEllipsePosition + rotatedQPointPosition;

        if (!CheckPositionInBoundaries(qPointPosition))
        {
            return false;
        }

        // Check for the Angle Point
        Vector2 anglePointPosition = centerEllipsePosition + rotatedQPointPosition.normalized * (rotatedQPointPosition.magnitude + interactEllipseUI.GetAnglePointParameterLineLength());

        if (!CheckPositionInBoundaries(anglePointPosition))
        {
            return false;
        }

        // Check for the Einstein Point
        float einsteinRectDistance = interactEllipseUI.GetPositionRectEinsteinPoint().x;
        float einsteinRotationAngle = interactEllipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedEinsteinPointPosition = new Vector2(Mathf.Cos(Mathf.Deg2Rad * einsteinRotationAngle) * einsteinRectDistance, 
                                                    Mathf.Sin(Mathf.Deg2Rad * einsteinRotationAngle) * einsteinRectDistance);
        Vector2 einsteinPointPosition = centerEllipsePosition + rotatedEinsteinPointPosition;

        if (!CheckPositionInBoundaries(einsteinPointPosition))
        {
            return false;
        }

        return true;
    }

    // --------------------- USED IN PLANE EDITOR ---------------------

    public void SetInteractEllipseUI(InteractableEllipseUI newInteractEllipseUI)
    {
        interactEllipseUI = newInteractEllipseUI;
    }

    public InteractableEllipseUI GetInteractEllipseUI()
    {
        return interactEllipseUI;
    }
}
