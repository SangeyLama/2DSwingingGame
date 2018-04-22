using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skyhook : MonoBehaviour {

    public GameObject anchor;
    private Vector2 anchorPosition;
    private Vector2 targetPosition;
    private DistanceJoint2D distanceJoint2D;
    private bool tethered;
    private bool midSwing;
    private PlayerPlatformerController ppc;
    private Rigidbody2D rb2d;

    private float lengthOfRope;

    public LineRenderer lineRenderer;
    public LayerMask ropeLayerMask;
    private List<Vector2> ropePositions = new List<Vector2>();
    // Use this for initialization
    void Awake () {
        anchor = GameObject.Find("Anchor");
        ppc = GetComponent<PlayerPlatformerController>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

      
        // If clicked left mouse button
        if (Input.GetMouseButtonDown(0) && !tethered)
        {
            var mousePosition = Input.mousePosition;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero, Mathf.Infinity);
            if (hit && hit.collider.gameObject.layer == 9)
            {
                targetPosition = hit.point;
            }
            else
                return;

            distanceJoint2D = gameObject.AddComponent<DistanceJoint2D>() as DistanceJoint2D;
            distanceJoint2D.enableCollision = true;
            distanceJoint2D.autoConfigureDistance = true;

            ppc.enabled = false;
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.velocity = ppc.targetVelocity;
            lengthOfRope = Vector3.Distance(anchorPosition, targetPosition);
            distanceJoint2D.anchor = anchor.transform.localPosition;
            distanceJoint2D.connectedAnchor = targetPosition;
            tethered = true;
            midSwing = true;
            distanceJoint2D.enabled = true;

          
        }

        if (tethered)
        {
            StartCoroutine("ShortenRope");
        }

        if (Input.GetMouseButtonDown(1) && tethered)
        {
            Destroy(distanceJoint2D);
            distanceJoint2D = null; 
            tethered = false;
        }

        if(midSwing && IsGrounded())
        {
            ppc.enabled = true;
            midSwing = false;
            rb2d.velocity = new Vector2(0f, 0f);
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            Destroy(distanceJoint2D);
            distanceJoint2D = null;
            tethered = false;
        }

        

        anchorPosition = anchor.transform.position;
        UpdateRope();

    }

    IEnumerator ShortenRope()
    {
        while (distanceJoint2D.distance >= lengthOfRope * 0.75f)
        {
            distanceJoint2D.distance = distanceJoint2D.distance * 0.999f;
            yield return null;
        }
    }

    private void UpdateRope()
    {
        if (tethered)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, anchorPosition);
            lineRenderer.SetPosition(1, targetPosition);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected const float shellRadius = 0.01f;

    private bool IsGrounded()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

        int count = rb2d.Cast(Vector2.down, contactFilter, hitBuffer, 0.05f + shellRadius);
        hitBufferList.Clear();
        for (int i = 0; i < count; i++)
        {
            hitBufferList.Add(hitBuffer[i]);
        }

        for (int i = 0; i < hitBufferList.Count; i++)
        {
            Vector2 currentNormal = hitBufferList[i].normal;
            if (currentNormal.y > 0.65f)
            {
                return true;
            }
        }

        return false;

    }
 
}
