using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetals")]
public class MovementDetailsSO : ScriptableObject
{
    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;

    [Tooltip("If there is a roll movement - this is the roll speed")]
    public float rollSpeed; // For player
    public float rollDistance;
    public float rollCooldownTime;

    public float GetMoveSpeed()
    {
        if(minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if(rollSpeed != 0 || rollDistance != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValues(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValues(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValues(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
    #endregion
}
