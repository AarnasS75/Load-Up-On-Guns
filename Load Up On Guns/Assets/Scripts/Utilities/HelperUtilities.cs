using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    /// <summary>
    /// Empty string debug check
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }
    /// <summary>
    /// List empty or contains null value check - returns true if there is an error
    /// </summary>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        foreach (var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null values in object" + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log(fieldName + " has no values in object" + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Null value check
    /// </summary>
    public static bool ValidateCheckNullValues(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck = null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// Positive value check, if zero is allowed, set zeroIsAllowed to true
    /// </summary>
    public static bool ValidateCheckPositiveValues(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if(valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if(valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }
    public static bool ValidateCheckPositiveValues(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if(valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if(valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum,
        float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if(valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValues(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValues(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }
    /// <summary>
    /// Get nearest spawn position to the player
    /// </summary>
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(1000f, 1000f, 0f);

        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }

    /// <summary>
    /// Get mouse world position
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    /// <summary>
    /// Get the angle in degrees from a direction vector
    /// </summary>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;
        
        return degrees;
    }

    public static AimDirection GetAimDirection(float angleDegree)
    {
        AimDirection aimDirection;

        if (angleDegree >= 22f && angleDegree <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (angleDegree > 67f && angleDegree <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if(angleDegree > 112f && angleDegree <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if((angleDegree <= 180f && angleDegree > 158f) || (angleDegree > -180f && angleDegree <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if (angleDegree > -135f && angleDegree <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if ((angleDegree > -45 && angleDegree <= 0f) || (angleDegree > 0 && angleDegree < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }
}
