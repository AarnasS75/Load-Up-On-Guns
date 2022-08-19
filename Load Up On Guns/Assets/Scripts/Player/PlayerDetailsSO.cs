using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    [Header("PLAYER BASE DETAILS")]
    public string playerCharacterName;

    public GameObject playerPrefab;

    public RuntimeAnimatorController runtimeAnimatorController;

    [Header("PLAYER HELTH")]
    public int playerHealth;

    [Header("PLAYER OTHER DETAILS")]
    public Sprite playerMiniMapIcon;

    public Sprite playerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValues(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckNullValues(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValues(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValues(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckPositiveValues(this, nameof(playerHealth), playerHealth, false);

    }
#endif
    #endregion

}
