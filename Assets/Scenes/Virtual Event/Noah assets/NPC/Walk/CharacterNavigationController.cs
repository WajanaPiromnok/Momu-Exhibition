using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CharacterNavigationController : MonoBehaviour
{

    public float movementSpeed;
    public float rotationSpeed;
    public float stopDistance;

    public Animator animator;

    public Vector3 destination;
    Vector3 lastPosition;    
    Vector3 velocity;

    public bool reachedDestination;  

    CapsuleCollider capsuleCollider;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        movementSpeed = Random.Range(1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != destination)
        {

            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }


            velocity = (transform.position - lastPosition) / Time.deltaTime;

            velocity.y = 0;
            var velocityMagnitude = velocity.magnitude;
            velocity = velocity.normalized;
            var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            var rightDotProduct = Vector3.Dot(transform.right, velocity);



            animator.SetFloat("horizonatal", rightDotProduct);
            animator.SetFloat("vertical", fwdDotProduct);
        }
        else 
        {
            reachedDestination = true;
        }

    }


    
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}