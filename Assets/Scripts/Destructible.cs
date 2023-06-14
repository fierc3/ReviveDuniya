﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public bool playerCanDestroyIt = true;
    public GameObject destroyedObj;
    public AudioClip destructionSound;
    public GameObject sandImpactEffect;

    private HumanAnimationController animController;

    public void Start()
    {
        animController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<HumanAnimationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Animator otherAnim = other.transform.root.GetComponentInChildren<Animator>();
        
        if ((otherAnim != null && otherAnim.GetBool("Attacking")) && (other.gameObject.tag == "GreatSword" || (other.gameObject.tag == "Sword" && playerCanDestroyIt))) // atingido por espadas
        {
            Destroy();
        }
        else if(other.gameObject.name.Contains("Magic") || other.gameObject.tag == "Magic") // atingido por magica
        {
            Destroy();
        }
    }

    private void OnCollisionEnter(Collision collision) // colisao porque eh o corpo do boss
    {
        if (collision.gameObject.name.Contains("Boss") || collision.gameObject.tag.Contains("Car")) // atingido pelo boss se movendo
        {
            Destroy();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Destroy();
    }

    private void Destroy()
    {
        GameObject poeira = Instantiate(sandImpactEffect, this.transform.position, Quaternion.identity);
        Destroy(poeira, 2);
        Vector3 scale = this.transform.localScale;
        GameObject obj = Instantiate(destroyedObj, transform.position, transform.rotation, transform.parent);
        obj.transform.localScale = scale;
        Vector3 pos = obj.transform.position; pos.y = 0;
        obj.transform.position = pos;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject,2);
    }

    public void SwingTrailDetected() // Tracked during swing trail
    {
        if (animController.IsAttacking())
        {
            print("I was destroyed by FillTrail");
            Destroy();
        }
    }

}
