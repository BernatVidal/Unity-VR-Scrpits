using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(XRGrabInteractable))]
public class GrapplingGun : MonoBehaviour
{

    #region Private Fields

    private bool isGrappling = false;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private SpringJoint sJoint;
    private Transform player;

    #endregion

    #region Public Fields

    public float maxDistance = 100;
    public LayerMask whatIsGrappable;
    public Transform grappleStartPoint, barrel;

    [Header("Spring Properties")]
    public float springForce = 10f;
    public float springDamp = 2f;
    public float springMassScale = 5.5f;
    public float springMaxDistance = 0.8f;
    public float springMinDistance = 0.15f;

    #endregion

    #region Unity Methods

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Start()
    {
        lr.positionCount = 0;
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            SetAnchorToGun();
        }
    }

    //Using late for drawing
    void LateUpdate()
    {
        if (isGrappling)
        {
            DrawRope();
        }
    }

    #endregion

    #region Public Methods

    public void StartGrapple()
    {
       
        RaycastHit hit;

        if(Physics.Raycast(grappleStartPoint.position, - barrel.forward, out hit, maxDistance))
        {
            isGrappling = true;

            grapplePoint = hit.point;

            sJoint = player.gameObject.AddComponent<SpringJoint>();
            sJoint.autoConfigureConnectedAnchor = false;
            sJoint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position + player.GetComponent<CharacterController>().center, grapplePoint);

            //The distance grapple will try to keep from grapple point;
            sJoint.maxDistance = distanceFromPoint * springMaxDistance;
            sJoint.minDistance = distanceFromPoint * springMinDistance;

            //To calibrate
            sJoint.spring = springForce;
            sJoint.damper = springDamp;
            sJoint.massScale = springMassScale;

            //Better with Game Events
            player.GetComponent<CharControllerToColliderManager>().EnableCollider();
        }
    }

    public void StopGrapple()
    {
        isGrappling = false;

        lr.positionCount = 0;
        Destroy(sJoint);

        player.GetComponent<CharControllerToColliderManager>().EnableContinuousMovement();

    }

    #endregion


    #region Private Methods

    void DrawRope()
    {
        lr.positionCount = 2;
        lr.SetPosition(0, grappleStartPoint.position);
        lr.SetPosition(1, grapplePoint);

        DrawDebugRope();
    }

    void DrawDebugRope()
    {
        lr.SetPosition(0, player.position + player.InverseTransformPoint(grappleStartPoint.position)); 
    }

    void SetAnchorToGun()
    {
        sJoint.anchor = player.InverseTransformPoint(grappleStartPoint.position);
    }

    #endregion
}
