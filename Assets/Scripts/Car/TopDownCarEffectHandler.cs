using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Lean.Pool;
using System.Linq;
using DG.Tweening;

public class TopDownCarEffectHandler : MonoBehaviour
{
    public GameObject carEcho;
    public GameObject item;
    //Components
    TopDownCarController carController;
    Rigidbody2D carRb2d;

    public ParticleSystem metalBurstSystem;
    public GameObject stunEffect;
    public GameObject crosshairPrefab;

  
    //Stats
    public CarItem currentItem = CarItem.NONE;

    //Local variables
    float echoCD = 0.2f;
    public float itemCD = 2f;
    float stopMoveCD = 0;
    public GameObject target;
    GameObject currentCrosshair = null;
    public List<Collider2D> enemiesInRange  = new List<Collider2D>(); 

    CameraShake cameraShake;
    CarSfxHandler SfxHandler;
    
    //Effect 
    Transform crosshairTransform;
    [SerializeField] bool isOilSlicking = false;
    [SerializeField] bool isCollising = false;
    [SerializeField] bool isNitroBoost = false;
    [SerializeField] bool isShieding = false;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        carController = GetComponent<TopDownCarController>();    
        carRb2d = GetComponent<Rigidbody2D>();
        carEcho.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        SfxHandler = GetComponent<CarSfxHandler>();
    }

    private void Update() {

        if(CompareTag("Player") && Input.GetKey(KeyCode.Space) && carController.velocityVsUp > 0 && currentItem != CarItem.NONE){
            ActiveItem();
        }

        if(CompareTag("AI") && itemCD < 0 && carController.velocityVsUp > 0 && currentItem != CarItem.NONE ){
            ActiveItem();
        }

       if(echoCD < 0 && carController.velocityVsUp > 16){
            SpawnEcho();
            echoCD = 0.2f;
       }
       echoCD -= Time.deltaTime;
       itemCD -= Time.deltaTime;

       if(currentItem == CarItem.SNOW){
            DetectEnemy();       
       }else{
            if(currentCrosshair != null){
                Destroy(currentCrosshair);
            }
       }
       
    }

    void DetectEnemy(){
        // Perform enemy detection
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20f);

        // Iterate through the colliders and handle enemy detection
        foreach (Collider2D collider in colliders)
        {
            if(CompareTag("Player")){
                if (collider.CompareTag("AI"))
                {
                    if (!enemiesInRange.Contains(collider))
                    {
                        // New enemy detected
                        enemiesInRange.Add(collider);
                    }
                }
            }
            else{
                if(collider.CompareTag("Player"))
                {
                    if (!enemiesInRange.Contains(collider))
                    {
                        // New enemy detected
                        enemiesInRange.Add(collider);
                    }
                }
            }

             // Check for enemies that moved out of range
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                Collider2D enemyCollider = enemiesInRange[i];
                if (!colliders.Any(collider => collider == enemyCollider))
                {
                    // Enemy moved out of range
                    enemiesInRange.Remove(enemyCollider);
                    Debug.Log("Enemy out of range!");
                }
            }

            if(enemiesInRange.Count > 0){
                // Sort enemies by distance
                    enemiesInRange = enemiesInRange.OrderBy(enemy => Vector2.Distance(transform.position, enemy.transform.position)).ToList();
                  
                if(currentCrosshair != null ){
                    currentCrosshair.transform.position = enemiesInRange[0].transform.position;
                    currentCrosshair.transform.rotation = enemiesInRange[0].transform.rotation;
                }
                if (currentCrosshair == null || currentCrosshair.transform.parent != enemiesInRange[0].transform)
                {
                    // Remove previous crosshair, if any
                    if (currentCrosshair != null)
                    {
                        Destroy(currentCrosshair);
                    }

                    // Spawn crosshair on the closest enemy
                    SpawnCrosshair(enemiesInRange[0]);
                }   
            }
            else{
                if(currentCrosshair != null){
                    Destroy(currentCrosshair);
                }
            }
        }

    }
    public async void NitroBooster(){
        isNitroBoost = true;
        carRb2d.AddForce(transform.up * Mathf.Floor(carController.velocityVsUp * 0.5f), ForceMode2D.Impulse);
        await Task.Delay(500);
        isNitroBoost = false;
    }

    public async void SpawnOil(){
        GameObject oilSpawned = LeanPool.Spawn(item, transform.position, Quaternion.identity);
        oilSpawned.GetComponent<Collider2D>().enabled = false;
        LeanPool.Despawn(oilSpawned, 10f);
        await Task.Delay(500);
        oilSpawned.GetComponent<Collider2D>().enabled = true;
    }

    public async void SpawnMine(){
        GameObject mineSpawned = LeanPool.Spawn(item, transform.position, Quaternion.identity);
        mineSpawned.GetComponent<Collider2D>().enabled = false;
        LeanPool.Despawn(mineSpawned, 10f);
        await Task.Delay(500);
        mineSpawned.GetComponent<Collider2D>().enabled = true;
    }

    public async void SpawnShield(){
        if(isShieding){
            await Task.Delay(5000);
            isShieding = false;
        }
        else{
            isShieding = true;
            await Task.Delay(5000);
            isShieding = false;
        }
    }

    public async void OilSlickEffect(){
        if(isOilSlicking){
            return;
        }
        else{
            isOilSlicking = true;
            carRb2d.AddForce(carRb2d.velocity.normalized * 15, ForceMode2D.Impulse);
            
            await Task.Delay(300);

            isOilSlicking = false;
        }
    }


    public async void SpawnMetalParticle(){
        if(isCollising)
            return;
        
        else{
            isCollising = true;
            var particles = LeanPool.Spawn(metalBurstSystem, transform.position, transform.rotation);
            carController.velocityVsUp /= 2;
            LeanPool.Despawn(particles, 5f);
            TriggerShakeCam();
            await Task.Delay(500);
            isCollising = false;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col) {
        SpawnMetalParticle();
        
    }

    private void OnTriggerStay2D(Collider2D col) {
        if(col.CompareTag("Oil")){
            OilSlickEffect();
        }
    }

   
    public void SpawnEcho(){
        GameObject echo = LeanPool.Spawn(carEcho, transform.position, transform.rotation);
        echo.transform.rotation = transform.rotation;
        LeanPool.Despawn(echo, 0.5f);
    }

    void TriggerShakeCam(){
        if(cameraShake != null)
            cameraShake.ShakeCamera();
    }

    public async void StunEffect(float duration){
        if(!carController.canMove)
            return;
        else{
            carController.canMove = false;
            carRb2d.velocity = Vector2.zero;
            var stun = LeanPool.Spawn(stunEffect, transform.position, transform.rotation, transform);
            LeanPool.Despawn(stun, duration);
            await Task.Delay((int) duration * 1000);
            carController.canMove = true;
        }
    }

    public void AddItem(int itemID){
        if(currentItem == CarItem.NONE){
            switch(itemID){
                case 1:
                    currentItem = (CarItem) itemID;
                    break;
                case 2:
                    currentItem = (CarItem) itemID;
                    break;
                case 3:
                    currentItem = (CarItem) itemID;
                    break;
                case 5:
                    currentItem = (CarItem) itemID;
                    break;
                default:
                    break;
            }
            SfxHandler.PlayGetItemSFX();
        }
    }

    public void ActiveItem(){
        if(currentItem != CarItem.NONE && itemCD < 0){
            itemCD = 2f;
            switch(currentItem){
                case CarItem.NITROTANK:
                    NitroBooster();
                    break;
                case CarItem.OIL:
                    SpawnOil();
                    break;
                case CarItem.MINE:
                    SpawnMine();
                    break;
                case CarItem.SHIELD:
                    SpawnShield();
                    break;
                case CarItem.SNOW:
                    SpawnSnow();
                    break;
                default:
                    break;
            }
            currentItem = CarItem.NONE;
            item  = null;
        }

        else{
            currentItem = CarItem.NONE;
            item  = null;
        }
    }

    public void SpawnSnow(){
        if(enemiesInRange[0] == null)
            return;
        else
        {
            target = enemiesInRange[0].gameObject;
            GameObject snow = LeanPool.Spawn(item, transform.position, transform.rotation);
            SnowBullet bullet = snow.GetComponent<SnowBullet>();
            Vector2 moveDirection = (target.transform.position - transform.position).normalized * 20;
            bullet.rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
            bullet.transform.Rotate(0, 0, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg);
            SfxHandler.PlaySnowBallSFX();
        }
    }

    public async void CarDestroyed(){
            carController.canMove = false;
            carController.carSprite.enabled = false;
            carController.shadowSprite.enabled = false;
            carController.carCollider.enabled = false;
            carRb2d.velocity = Vector2.zero;
            gameObject.transform.position = GetComponent<CarLapCounter>().lastCPPosition;
            await Task.Delay(3000);
            carController.carSprite.enabled = true;
            carController.shadowSprite.enabled = true;
            carController.carCollider.enabled = true;
            carController.canMove = true;
    }

    void SpawnCrosshair(Collider2D enemy){
        Vector3 spawnPosition = enemy.transform.position;
        Quaternion spawnRotation = enemy.transform.parent.rotation;

        currentCrosshair = Instantiate(crosshairPrefab, spawnPosition, spawnRotation);
        currentCrosshair.transform.SetParent(enemy.transform, true);
        
    }

    public enum CarItem{
        NITROTANK = 1,
        MINE= 2, 
        OIL = 3,
        SHIELD = 4,
        SNOW = 5,
        NONE = 0
    }

}
