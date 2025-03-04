using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CollisionController : MonoBehaviour
{
    // Let's you change the color of an object upon collision
    public float pushPower = 2.0f;
    
    public bool changeColor;
    public Color myColor;

    // States of GameObjects to destroy them upon collision
    public bool destroyEnemy;
    public bool destroyCollectibles;

    // Allows you to add an audio file that's played on collision
    public AudioClip collisionAudio;
    private AudioSource audioSource;
    
    public TMPro.TMP_Text scoreUI;
    
    public int increaseScore = 1;
    public int decreaseScore = 1;
    
    private int score = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (scoreUI != null)
        {
            scoreUI.text = score.ToString();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Change the color of the object on collision if changeColor is true
        if (changeColor)
        {
            gameObject.GetComponent<Renderer>().material.color = myColor;
        }
        
        // Play collision sound if not already playing
        PlayCollisionSound();

        // Destroy the collided object if it's an enemy or collectible
        bool v = other.gameObject.CompareTag("Collectable");
        if ((destroyEnemy && other.gameObject.CompareTag("Enemy")) || (destroyCollectibles && v))
        {
            Destroy(other.gameObject);
        }
        
        if (scoreUI != null && other.gameObject.tag == "Collectible")
        {
            score += increaseScore;
        }
        
        if (scoreUI != null && other.gameObject.tag == "Enemy")
        {
            score -= decreaseScore;
        }
        
    }
    
    // only for GameObjects with a character controller applied
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        
        // If no Rigidbody or if it's Kinematic, do nothing
        if (body == null || body.isKinematic)
        {
            return;
        }
       
        // Don't push ground or platform GameObjects below character
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }
        
        // Calculate push direction (only along X and Z axes)
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        
        // Apply push force to the object (if it has the tag "Object")
        if (hit.gameObject.CompareTag("Object"))
        {
            body.velocity = pushDir * pushPower;
        }
        
        // Play collision sound if not already playing
        PlayCollisionSound();

        // Destroy the object if it's an enemy or collectible
        if (destroyEnemy && hit.gameObject.CompareTag("Enemy") || destroyCollectibles && hit.gameObject.CompareTag("Collectible"))
        {
            Destroy(hit.gameObject);
        }
        
        if (scoreUI != null && hit.gameObject.tag == "Collectible")
        {
            score += increaseScore;
        }
        
        if (scoreUI != null && hit.gameObject.tag == "Enemy")
        {
            score -= decreaseScore;
        }
    }

    // Helper method to handle audio playback
    private void PlayCollisionSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(collisionAudio, 0.5f);
        }
    }
}