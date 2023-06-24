using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Pathfinding;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode {
        FollowPlayer,
        FollowWaypoint
    }
    
    //Local variables
    Vector3 targetPos = Vector3.zero;
    Transform targetTransform = null;
    float orginalMaxSpeed = 0;
    float angleToTarget;

    [Header("AI Settings")]
    public AIMode aiMode = AIMode.FollowPlayer;
    public float maxSpeed = 16;
    public bool isAvoidingCars = true;
    [Range(0.0f, 1.0f)]
    public float skillLevel = 1.0f;

    //Avoidance 
    Vector2 avoidanceVectorLerped = Vector3.zero;

    //Waypoints
    public WaypointNode currentWaypoint = null;
    WaypointNode previousWaypoint = null;
    WaypointNode[] allWaypoints;

    //Stuck handling
    bool isRunningStuckCheck = false;
    int stuckCheckCounter = 0;
    List<Vector2> temporaryWaypoints = new List<Vector2>();

    
    //Components
    TopDownCarController carController;
    PolygonCollider2D polygon2d;
    public AIDestinationSetter setter;
    public AIPath aiPath;
    public Seeker seeker;
    
    private void Awake() {
        carController = GetComponent<TopDownCarController>();
        allWaypoints = FindObjectsOfType<WaypointNode>();
        polygon2d = GetComponentInChildren<PolygonCollider2D>();
        orginalMaxSpeed = maxSpeed;
        setter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        seeker =  GetComponent<Seeker>();
        aiPath.maxSpeed = maxSpeed;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        aiPath.maxSpeed = maxSpeed;
        aiPath.canMove = false;
        SetMaxSpeedBasedOnSkillLevel(maxSpeed);
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        if(GameManager.instance.GetGameState() == GameManager.GameState.COUNTDOWN){
            return;
        }

        if(carController.canMove == false){
            return;
        }
        Vector2 inputVector = Vector2.zero;

        switch(aiMode){
            case AIMode.FollowPlayer:
                FollowPlayer();
                break;
            case AIMode.FollowWaypoint:
                FollowWaypoint();
                break;
            default:
                break;
        }

        inputVector.x = TurnTowardTarget();
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);
        
        //If AI is applying throttle but not insrease any speed
        if(carController.GetVelocityMagnitude() < 0.5f && Mathf.Abs(inputVector.y) > 0.01f && !isRunningStuckCheck){
            StuckCheckAsync();
        }

        if(stuckCheckCounter >= 3 && !isRunningStuckCheck){
            StuckCheckAsync();
        }
        //Send the input  to the car Controller
        carController.SetInputVector(inputVector);
    }

    void FollowPlayer(){
        if(targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if(targetTransform != null){
            targetPos = targetTransform.position;
        }
    
    }

    float TurnTowardTarget(){
        Vector2 vectorToTarget = targetPos - transform.position;
        vectorToTarget.Normalize();

        //Apply advoidance to steering
        if(isAvoidingCars)
            AvoidCar(vectorToTarget, out vectorToTarget);
        
        //Calculate the angle toward player
        angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;
        
        //For a smoothing rotation, the car will turn as much as possible if angle > 45 degrees, and smaller turn for smaller angle
        float steerAmount = angleToTarget / 45.0f;
        //Clamp Steering between -1.0f, 1.0f
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
       
        return steerAmount;
    }

    void FollowWaypoint(){
        //Pick the closest waypoint if we don't have a waypoint yet
        if(currentWaypoint == null){
            currentWaypoint = FindClosestWaypoint();
            previousWaypoint = currentWaypoint;
        }

        //Set the target on waypoint position
        if(currentWaypoint != null){
            targetPos = currentWaypoint.transform.position;

            //Store how close we are to the target
            float distanceToWaypoint = (targetPos - transform.position).magnitude;

            if(distanceToWaypoint > 20){
                //Update the nearest point on the AI path we draw and set it to the target position
                Vector3 nearestPointOnTheWaypointLine = FindNearestPointOnWaypath(previousWaypoint.transform.position, currentWaypoint.transform.position, transform.position);
                
                float segments = distanceToWaypoint / 20.0f;

                targetPos = (targetPos + nearestPointOnTheWaypointLine * segments) / (segments + 1);

                Debug.DrawLine(transform.position, targetPos, Color.cyan);
            }

            //Check if we are close enough to consider that we reached the waypoint
            if(distanceToWaypoint <= currentWaypoint.minDistanceToNextWaypoint){

                if(currentWaypoint.maxSpeed > 0){
                    SetMaxSpeedBasedOnSkillLevel(currentWaypoint.maxSpeed);
                }
                else{
                    SetMaxSpeedBasedOnSkillLevel(1000);
                }

                // if we are close enough
                //Store the current as previous
                aiPath.canMove = false;
            
                previousWaypoint = currentWaypoint;
                
                //Update the current waypoint as one of its next waypoint
                // currentWaypoint = currentWaypoint.nextWaypointNode[Random.Range(0, currentWaypoint.nextWaypointNode.Length)]; 
                int test = Random.Range(0, currentWaypoint.nextWaypointNode.Length);
                currentWaypoint = currentWaypoint.nextWaypointNode[test];
                
            }         
        }
    }

    WaypointNode FindClosestWaypoint(){
        return allWaypoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float ApplyThrottleOrBrake(float inputX){
        
        // If we are going too  fast then do not accelerate
        if(carController.GetVelocityMagnitude() > maxSpeed){
            return 0;
        }

        float reduceSpeedDueToCornering = Mathf.Abs(inputX) / 1.0f;

        //Apply throttle based on cornering and skill
        float throttle = 1.05f - reduceSpeedDueToCornering * skillLevel;

        if(stuckCheckCounter >= 3){
            //if the angle is larger  to reach so better to reverse
            if(angleToTarget > 70){
                throttle = throttle * -1;
                
            }
            else {
                throttle = throttle * -1;
                
            }
            FindPath();
        }
        //Apply throttle foward based on how much the car want to turn
        return throttle;
    }

    void SetMaxSpeedBasedOnSkillLevel(float newSpeed){
        maxSpeed = Mathf.Clamp(newSpeed, 0.3f, orginalMaxSpeed);

        float skillBasedMaxSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed *= skillBasedMaxSpeed;
    }

    //Find the nearest point on a line
    Vector2 FindNearestPointOnWaypath(Vector2 lineStartPos, Vector2 lineEndPos, Vector2 point){
        //Get heading as a vector
        Vector2 lineHeadingVector = (lineEndPos - lineStartPos);

        //Store the max distance
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        //Do projection from the start to the point
        Vector2 lineFromStartToPoint = point - lineStartPos;
        float dotProduct = Vector2.Dot(lineFromStartToPoint, lineHeadingVector);

        //Clamp the dot product to max Distance
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);
        return lineStartPos + lineHeadingVector * dotProduct;
    }


    //Check is there any car ahead
    bool isAnyCarAhead(out Vector3 position, out Vector3 otherCarRbVector){

        //Disable the collider show the raycast won't detect itself
        polygon2d.enabled = false;

        //Perform the circle cast in front of the car with a slight offset forward and only in car layer
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 1.2f, transform.up, 12, 1 << LayerMask.NameToLayer("Car"));

        //Enable the collider we turn off before
        polygon2d.enabled = true;

        if(raycastHit2D.collider != null){
            //Draw a white line showing how long the detection 
            Debug.DrawRay(transform.position, transform.up * 12, Color.white);

            position = raycastHit2D.collider.transform.position;
            otherCarRbVector = raycastHit2D.collider.transform.right;

            return true; 
        }
        else{
            // We didn't find any car ahead
            Debug.DrawRay(transform.position, transform.up * 12, Color.black);
        }

            position = Vector3.zero;
            otherCarRbVector = Vector3.zero;
            return false;
    }

    void AvoidCar(Vector2 vectorToTarget, out Vector2 newVectorToTarget){
        if(isAnyCarAhead(out Vector3 othercarPos, out Vector3 otherCarRightVector)){
            Vector2 avoidanceVector = Vector2.zero;

            //calculate the vector to advoid
            avoidanceVector = Vector2.Reflect((othercarPos - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPos - transform.position).magnitude;

            //We want the AI to able to move toward waypoint or avoiding car is better decision
            // As closer to waypoint, the AI will choose to move toward it instead of avoid
            float driveToTargetInfluence = 6.0f/distanceToTarget;

            //Ensure that limit of the value between 30 - 100% as we always want the AI to reach the waypoint
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.3f, 1.0f);

            //The desire to avoid the car is simply inverse to reach waypoint
            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            //Reduce jittering a little by using lerp
            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);

            // advoidance vector
            newVectorToTarget = vectorToTarget * driveToTargetInfluence  +  avoidanceVectorLerped * avoidanceInfluence;
            newVectorToTarget.Normalize();

            //Draw the vector which indicates the advoidance in yellow
            Debug.DrawRay(transform.position, avoidanceVector*10, Color.yellow);

            //The vector theh car will go
            Debug.DrawRay(transform.position, newVectorToTarget*10, Color.green);

            return;
        }

        // We need to assign a default value if we didn't hit any cars before exit the function
        newVectorToTarget = vectorToTarget;
    }

    public async void StuckCheckAsync(){
        Vector3 initialStuckPosition = transform.position;

        isRunningStuckCheck = true;

        await Task.Delay(700);

        // If we not move for a second so we stuck
        if((transform.position - initialStuckPosition).sqrMagnitude < 3){            
            stuckCheckCounter++;
        }
        else{
            stuckCheckCounter = 0;
        }

        isRunningStuckCheck = false;
    }

    async void FindPath(){
        await Task.Delay(500);
        aiPath.canMove = true;
        setter.target = currentWaypoint.transform;
        
        await Task.Delay(700);
        aiPath.canMove = false;
    }
    
}
