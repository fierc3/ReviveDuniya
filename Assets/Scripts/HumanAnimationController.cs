using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    public enum DieDirection {
        Forwad,
        Back
    }

    #region Actions
    public void DoLightAttack()
    {
        animator.SetTrigger("LightAttack");
    }

    public void DoHeavyAttack()
    {
        animator.SetTrigger("HeavyAttack");
    }

    public void DoDrawWeapon()
    {
        animator.SetTrigger("Weapon");
    }

    public void TakeDrink()
    {
        animator.SetTrigger("Drink");
    }

    public void PickUp()
    {
        animator.SetTrigger("PickingUp");
    }

    public void DoFinish() {
        animator.SetTrigger("LightBonfire");
    }


    public void DoDodge()
    {
        animator.SetTrigger("Dodge");
    }
    public void DoDie(DieDirection direction)
    {
        animator.SetTrigger(direction == DieDirection.Forwad ? "DieForward" : "DieBack"); // animacao de morte
        animator.SetBool("Dead", true);
    }

    public void DoFallDamage() => animator.SetTrigger("FallDamage");
    public void DoFallForward() => animator.SetTrigger("FallForward");
    public void DoTakeDamage() => animator.SetTrigger("TakeDamage");
    public void DoTakeDamageLeft() => animator.SetTrigger("TakeDamageLeft");
    public void DoTakeDamageRight() => animator.SetTrigger("TakeDamageRight");

    #endregion


    public bool IsEquipped() => animator.GetBool("Equipped");
    public bool IsDrinking() => animator.GetBool("Drinking");
    public bool IsAttacking() => animator.GetBool("Attacking");
    public bool CanAttack() => animator.GetBool("CanAttack");
    public bool CanMove() => animator.GetBool("CanMove");
    public bool IsDodging() => animator.GetBool("Dodging");
    public bool IsStunned() => animator.GetBool("Intangible");
    public bool IsDriving() => animator.GetBool("IsDriving");
    public bool IsDead() => animator.GetBool("Dead");
    public bool IsSweepFalling() => animator.GetCurrentAnimatorStateInfo(2).IsName("Sweep Fall");
    public bool IsGettingThrown() => animator.GetCurrentAnimatorStateInfo(2).IsName("Getting Thrown");


    public void StopMovement()
    {
        animator.SetFloat("Speed", 0);
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);
    }


    public float GetSpeed() => animator.GetFloat("Speed");
    public void SetSpeed(float speed) => animator.SetFloat("Speed", speed);

    public void SetSpeedWithDirection(Vector3 stickDirection, float clampValue) => animator.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, clampValue).magnitude, 0.02f, Time.deltaTime); // clamp para limitar a 1, visto que a diagonal seria de 1.4

    public void SetHorizontalInput(float x) => animator.SetFloat("Horizontal", x);

    public void SetVerticalInput(float z) => animator.SetFloat("Vertical", z);

    public float GetVertical() => animator.GetFloat("Vertical");

    public void SetCanMove(bool value) => animator.SetBool("CanMove", value);
    public void SetDriving(bool value) {
        SetSpeed(0);
        animator.SetBool("IsDriving", value);
    }



}
