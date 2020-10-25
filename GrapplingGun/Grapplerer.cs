using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Grapplerer : MonoBehaviour
{

    #region Fields

    private CharacterController character;
    private ContinuousMovement continuousMovement;
    
    #endregion

    #region Public Fields

    public static XRController grapplingHand;

    #endregion

    #region Unity Methods

    void Awake()
    {
        character = GetComponent<CharacterController>();
        continuousMovement = GetComponent<ContinuousMovement>();
    }

    void FixedUpdate()
    {
        if (grapplingHand)
        {
            continuousMovement.enabled = false;
            GrapMove();
        }
        else if(!continuousMovement.enabled)
        {
            continuousMovement.enabled = true;
        }
    }

    #endregion


    #region Private Methods

    void GrapMove()
    {
        //Hand force while gripping
        InputDevices.GetDeviceAtXRNode(grapplingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);

        character.Move(transform.rotation * - velocity * Time.fixedDeltaTime);
    }

    #endregion

}
