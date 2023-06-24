using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float maxSpeed = 20;

    [Header("Sprites")]
    public SpriteRenderer carSprite;
    public SpriteRenderer shadowSprite;

    [Header("Jumping")]
    public AnimationCurve jumpCurve;

    public ParticleSystem landingParticles;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    public float velocityVsUp = 0;
    public bool isJumping = false;
    public bool canMove = true;

    //Component
    Rigidbody2D carRb2D;
    public Collider2D carCollider;
    CarSfxHandler SfxHandler;
    private void OnEnable() {
        carRb2D = GetComponent<Rigidbody2D>();
        carCollider = GetComponentInChildren<Collider2D>();
        SfxHandler = GetComponent<CarSfxHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        if(GameManager.instance.GetGameState() == GameManager.GameState.COUNTDOWN){
            return;
        }
        
        if(!canMove)
            return;
        
        ApplyEngineForce();      

        KillOrthogonalVelocity();

        ApplySteering(); 

    }    

    void ApplyEngineForce(){

        //Don't let brake emmiting when we are jumping
        if(isJumping && accelerationInput < 0){
            accelerationInput = 0;
        }
        //Calculate how much "foward" we are going in term of the velocity direction
        velocityVsUp = Vector2.Dot(transform.up, carRb2D.velocity);

        //Limit the speed so we can no break throught the max speed with "forward" direction
        if(velocityVsUp > maxSpeed && accelerationInput > 0){
            return;
        }

        //Limit the speed so we can no break throught the max speed with "backward" direction
        if(velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0){
            return;
        }

        //Limit speed while acceleration in any direction
        if(carRb2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping){
            return;
        }
        //Apply drag if there is no accelerationInput so the car will stop when player let go the acellarator
        if(accelerationInput == 0){
            carRb2D.drag = Mathf.Lerp(carRb2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else {
            carRb2D.drag = 0;
        }

        //tạo lực đẩy cho xe
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Áp dụng lực đẩy
        carRb2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering(){

        //Limit car ability to turn when moving slowly
        float minSpeedToTurnFactor = (carRb2D.velocity.magnitude / 8);
        minSpeedToTurnFactor = Mathf.Clamp01(minSpeedToTurnFactor);

        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedToTurnFactor;
        

        //Apply steering by rotating car object
        carRb2D.MoveRotation(rotationAngle);
    }

    public void SetInputVector(Vector2 inputVector){
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    void KillOrthogonalVelocity(){
        //DOT -> dot product -> trả về tích vô hướng của 2 vector: cho ta biết tương quan giữa hướng và độ lớn của 2 vector đó
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRb2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRb2D.velocity, transform.right);

        carRb2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking){
        lateralVelocity = getLateralVelocity();
        isBraking = false;

        //check if user moving forward and if he/she is hitting the brake. In that case the tires should screech
        if(accelerationInput < 0 && velocityVsUp > 0){
            isBraking = true;
            return true;
        }

        //if we have a lot of side movement then the tires shouble be screeching
        if(Mathf.Abs(getLateralVelocity()) > 2.0f){
            return true;
        }

        return false;
    }

    float getLateralVelocity(){

        //return how fast the car is moving side way
        return Vector2.Dot(transform.right, carRb2D.velocity);
    }

    public float GetVelocityMagnitude()
    {
        return carRb2D.velocity.magnitude;
    }

    public void Jump(float jumpHeightScale, float jumpPushScale, int carColliderLayerBeforeJump){
        if(!isJumping){
            StartCoroutine(JumpCO(jumpHeightScale, jumpPushScale, carColliderLayerBeforeJump));
        }
    }
    private IEnumerator JumpCO(float jumpHeightScale, float jumpPushScale, int carColliderLayerBeforeJump){
        isJumping = true;
        
        float jumpStartTime = Time.time;
        float jumpDuration = carRb2D.velocity.magnitude * 0.05f;

        jumpHeightScale = jumpHeightScale * carRb2D.velocity.magnitude * 0.05f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);
        
        //Change the layer of collider
        carCollider.gameObject.layer = LayerMask.NameToLayer("FlyingObject");
        

         //Change sorting layer to flying
        carSprite.sortingLayerName = "Flying";
        shadowSprite.sortingLayerName = "Flying";
        
        SfxHandler.PlayJumpSFX();

        

        //Push the car foward as a jump
        carRb2D.AddForce(carRb2D.velocity.normalized * jumpPushScale * 10, ForceMode2D.Impulse);

        while(isJumping){
            
            //Percentage 0 - 1.0 of where we are in the jumping progress
            float jumpCompletePercentage = (Time.time -  jumpStartTime) / jumpDuration;
            jumpCompletePercentage = Mathf.Clamp01(jumpCompletePercentage);

            //Take the base scale of 1 and add how much we should increse it
            carSprite.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpCompletePercentage) * jumpHeightScale;
            
            //Change the shadow scale also but a bit smaller. Althought this is opposite with reality, the higher object has bigger shadow, but we need it look good in the game so...
            shadowSprite.transform.localPosition = carSprite.transform.localScale * 0.75f;

            //Offset the shadow a bit, to make it look good
            shadowSprite.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpCompletePercentage) * jumpHeightScale;

            //When we reach 100% percent, we are done jumping
            if(jumpCompletePercentage  == 1.0f){
                break;
            }

            yield return null;
        }

        //Disable the car collider so we can perform an overlapped check 
        carCollider.enabled = false;

         //Do not check for collisions with triggers
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;

        Collider2D[] hitResults = new Collider2D[2];

        int numberOfHitObjects = Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter2D, hitResults);

        carCollider.enabled = true;

        //Check if landing is ok or not, if we hit zero objects then it is ok
        if (numberOfHitObjects != 0)
        {
            //Something is below the car so we need to jump again
            isJumping = false;

            //add a small jump and push the car forward a bit. 
            Jump(0.2f, 0.6f, carColliderLayerBeforeJump);
        }
        else{
            //Handle landing, scale back the object
            carSprite.transform.localScale = Vector3.one;

            //reset the shadows position and scale
            shadowSprite.transform.localPosition = Vector3.zero;
            shadowSprite.transform.localScale = carSprite.transform.localScale;

            //We are safe to land, so enable change the collision layer back to what it was before we jumped
            carCollider.gameObject.layer = carColliderLayerBeforeJump;

            //Change sorting layer to regular layer
            carSprite.sortingLayerName = "Default";
            shadowSprite.sortingLayerName = "Default";

            //Play the landing particle system if it is a bigger jump
            if (jumpHeightScale > 0.2f)
            {
                landingParticles.Play();

                SfxHandler.PlayLandingSFX();
            }

             //Change state
            isJumping = false;
            }
    }
   
    void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Jump")){
            //Get the jump data from the jump
            JumpData jumpData = col.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale, carCollider.gameObject.layer);
        }
    }

}
